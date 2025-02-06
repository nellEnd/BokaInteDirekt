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

        public BookingServiceTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open(); // Viktigt! Annars raderas databasen direkt

            var options = new DbContextOptionsBuilder<BokaInteDirektContext>()
                .UseSqlite(connection) // Använd SQLite in-memory
                .Options;

            _context = new BokaInteDirektContext(options);
            _context.Database.EnsureCreated(); // Skapa tabeller

            _bookingService = new BookingService(_context);
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

    }
}

