using Microsoft.EntityFrameworkCore;

namespace StudyRoomEE
{
    using StudyRoomEE.Models;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<AttendanceLog> Attendances => Set<AttendanceLog>();
    }
}
