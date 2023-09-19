using DicomSaver.Entities;

using Microsoft.EntityFrameworkCore;

namespace DicomSaver.Services
{
    public class DicomDbContext : DbContext
    {
        public DicomDbContext(DbContextOptions<DicomDbContext> options) : base(options) { }

        public DbSet<DicomSnapshot> Snapshots { get; set; }
    }
}
