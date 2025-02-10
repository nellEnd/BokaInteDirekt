using BokaInteDirekt.Context;
using BokaInteDirekt.DTO;
using BokaInteDirekt.Models;
using BokaInteDirekt.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BokaInteDirekt.Tests
{
    public class BookingServiceTests
    {
        private readonly BokaInteDirektContext _context;
        private readonly BookingService _bookingService;
        private readonly SqliteConnection _connection;

        public BookingServiceTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open(); // Viktigt! Annars raderas databasen direkt

            var options = new DbContextOptionsBuilder<BokaInteDirektContext>()
                .UseSqlite(_connection) // Använd SQLite in-memory
                .Options;

            _context = new BokaInteDirektContext(options);
            _context.Database.EnsureDeleted(); // Nollställ databas
            _context.Database.EnsureCreated(); // Skapa tabeller

            SeedDatabase(); // Lägg till testdata

            _bookingService = new BookingService(_context);
        }

        private void SeedDatabase()
        {
            _context.Bookings.AddRange(new List<Booking>
            {
                new Booking { Id = 1, Day = "2025-02-11", StartTime = "10:00", EndTime = "10:20", IsAvailable = true, BookingType = "Behandling" },
                new Booking { Id = 2, Day = "2025-02-11", StartTime = "10:20", EndTime = "10:40", IsAvailable = true, BookingType = "Behandling" }
            });

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        [Fact]
        public async Task CreateAppointment_Should_Add_Booking_And_SaveChanges()
        {
            // Arrange
            var request = new BookingRequest
            {
                Day = "2025-02-06",
                StartTime = "10:00",
                EndTime = "11:00",
                BookingType = "Behandling"
            };

            // Act
            var result = await _bookingService.CreateAppointment(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Day, result.Day);
            Assert.Equal(DateTime.ParseExact(request.Day, "yyyy-MM-dd", null), result.Date);
            Assert.Equal(request.StartTime, result.StartTime);
            Assert.Equal(request.EndTime, result.EndTime);
            Assert.True(result.IsAvailable);

            // Kontrollera att bokningen faktiskt sparades
            var bookingInDb = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == result.Id);
            Assert.NotNull(bookingInDb);
        }

        [Fact]
        public async Task BookAppointment_ShouldBook_WhenValidRequest()
        {
            // Arrange
            var request = new BookAppointmentRequest
            {
                User = new User { Email = "test@mail.se" }
            };
            var bookingType = "Nybesök";

            // Act
            var result = await _bookingService.BookAppointment(1, bookingType, request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsAvailable);
            Assert.Equal("test@mail.se", result.CustomerEmail);
            Assert.Equal("10:40", result.EndTime); // 10:00 + 40 min

            // Kontrollera att nästa tidslucka också bokas
            var nextSlot = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == 2);
            Assert.NotNull(nextSlot);
            Assert.False(nextSlot.IsAvailable);
            Assert.Equal("test@mail.se", nextSlot.CustomerEmail);
        }
        
        [Fact]
        public async Task BookAppointment_ShouldReturnNull_WhenAppointmentIsNotAvailable()
        {
            // Arrange - Lägg till en bokning som redan är upptagen
            var existingBooking = await _context.Bookings.FindAsync(1);
            if (existingBooking != null)
            {
                existingBooking.IsAvailable = false;
                existingBooking.CustomerEmail = "test@mail.se";
            }
            else
            {
                _context.Bookings.Add(new Booking
                {
                    Id = 1,
                    Day = "2025-02-11",
                    StartTime = "10:00",
                    EndTime = "10:20",
                    IsAvailable = false,
                    CustomerEmail = "test@mail.se",
                    BookingType = "Behandling"
                });
            }

            await _context.SaveChangesAsync();

            // Act - Försök boka en upptagen tid
            var request = new BookAppointmentRequest
            {
                User = new User { Email = "test@mail.se" }
            };
            var bookingType = "Återbesök";

            var result = await _bookingService.BookAppointment(1, bookingType, request);

            // Assert - Ska returnera null
            Assert.Null(result); 
        }
    }
}

