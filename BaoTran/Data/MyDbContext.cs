using Microsoft.EntityFrameworkCore;

namespace BaoTran.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }

        #region DbSet
        public DbSet<DayOfTheWeek> DayOfTheWeeks { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<DateRange> DateRanges { get; set; }
        public DbSet<TimeRange> TimeRanges { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // DateRange
            modelBuilder.Entity<DateRange>()
                .HasOne(dr => dr.MediaFile)
                .WithMany(mf => mf.DateRanges)
                .HasForeignKey(dr => dr.MediaFileId)
                .OnDelete(DeleteBehavior.Cascade);

            //DayOfTheWeek
            modelBuilder.Entity<DayOfTheWeek>()
                .HasOne(d => d.DateRanges)
                .WithMany(dr => dr.DayOfTheWeeks)
                .HasForeignKey(d => d.DateRangeId)
                .OnDelete(DeleteBehavior.Cascade);

            //TimeRange
            modelBuilder.Entity<TimeRange>()
                .HasOne(tr => tr.DayOfTheWeek)
                .WithMany(d => d.TimeRanges)
                .HasForeignKey(tr => tr.DayOfWeekId)
                .OnDelete(DeleteBehavior.Cascade);



        }


    }
}
