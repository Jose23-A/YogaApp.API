using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using YogaApp.API.Entities;


namespace YogaApp.API
{
    // Esta clase es el puente entre tu código C# y SQL Server.
    public class YogaDbContext : DbContext
    {
        public YogaDbContext(DbContextOptions<YogaDbContext> options) : base(options)
        {
        }

        // Aquí declaras qué clases se convierten en tablas
        public DbSet<Student> Students { get; set; }
        public DbSet<ClassDefinition> ClassDefinitions { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<UserSubscription> Subscriptions { get; set; }
    }
}
