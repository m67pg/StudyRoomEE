using Microsoft.EntityFrameworkCore;

namespace StudyRoomEE
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using StudyRoomEE.Models;

    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<AttendanceLog> Attendances => Set<AttendanceLog>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // EF Core 9 の「モデルが動的に変わる」警告を無視する設定
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // パスワードハッシュの準備
            var hasher = new PasswordHasher<ApplicationUser>();

            // 固定ユーザー1：管理者
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = "admin-uuid", // 固定のID
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                StudentId = 0,
                PasswordHash = hasher.HashPassword(null!, "Admin123!")
            });

            // 固定ユーザー2：生徒A
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = "student-a-uuid",
                UserName = "student-a@example.com",
                NormalizedUserName = "STUDENT-A@EXAMPLE.COM",
                StudentId = 1,
                PasswordHash = hasher.HashPassword(null!, "StudentA123!")
            });

            // 固定ユーザー3：生徒B
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = "student-b-uuid",
                UserName = "student-b@example.com",
                NormalizedUserName = "STUDENT-B@EXAMPLE.COM",
                StudentId = 2,
                PasswordHash = hasher.HashPassword(null!, "StudentB123!")
            });
        }
    }
}
