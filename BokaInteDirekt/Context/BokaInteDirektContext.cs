using BokaInteDirekt.Models;
using Microsoft.EntityFrameworkCore;

namespace BokaInteDirekt.Context
{
    public class BokaInteDirektContext:DbContext
    {
        public BokaInteDirektContext(DbContextOptions<BokaInteDirektContext> options) : base(options) { }

        public DbSet<Booking> Bookings { get; set; }
    }
}
