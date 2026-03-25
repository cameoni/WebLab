using Microsoft.EntityFrameworkCore;

namespace WebLab.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            if (Database.CanConnect()) { Database.EnsureCreated(); }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>().HasData(
                    new Teacher { Id = 1, FullName = "Иван Иванов Иванович", Department = "Институт Цифровых Систем", Email = "ivanovii@mail.ru", ExperienceYears = 10, Active = true },
                    new Teacher { Id = 2, FullName = "Федоров Федор Федорович", Department = "Институт Архитектуры и Дизайна", Email = "fedorovff@mail.ru", ExperienceYears = 7, Active = true },
                    new Teacher { Id = 3, FullName = "Александрова Александра Александровна", Department = "Институт Химии и Химических Технологий", Email = "alexandrovaaa@mail.ru", ExperienceYears = 14, Active = false }
                );
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Login = "Admin", Password = "3IvWjUTQh1MWCr7YPXGENQ==:0VSOfu2pa1qTmk41V2XM+couSPsgjtKxvb4m3KxdopE=", EditAccess = true },
                new User { Id = 2, Login = "User", Password = "T8lhLTQIXSlfJYL+H3cNjA==:JpCRIVDubT4jqHnYSTZzdKB94VCpGJMWkkYlZFxh7u4=", EditAccess = false }
                );
        }
    }
}
