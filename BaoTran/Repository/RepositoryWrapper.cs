using BaoTran.Data;
using BaoTran.ReRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace BaoTran.Repository
{
    public interface IRepositoryWrapper
    {
        IRepositoryBase<MediaFile> MediaFiles { get; }
        IRepositoryBase<DateRange> DateRanges { get; }
        IRepositoryBase<TimeRange> TimeRanges { get; }
        IRepositoryBase<DayOfTheWeek> DayOfTheWeeks { get; }

        Task SaveChangeAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }

    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly MyDbContext db;

        public RepositoryWrapper(MyDbContext db)
        {
            this.db = db;
        }

        private IRepositoryBase<MediaFile> MediaFilesRepositoryBase;
        public IRepositoryBase<MediaFile> MediaFiles => MediaFilesRepositoryBase ??= new RepositoryBase<MediaFile>(db);


        private IRepositoryBase<DateRange> DateRangesRepositoryBase;
        public IRepositoryBase<DateRange> DateRanges => DateRangesRepositoryBase ??= new RepositoryBase<DateRange>(db);


        private IRepositoryBase<TimeRange> TimeRangesRepositoryBase;
        public IRepositoryBase<TimeRange> TimeRanges => TimeRangesRepositoryBase ??= new RepositoryBase<TimeRange>(db);


        private IRepositoryBase<DayOfTheWeek> DayOfTheWeeksRepositoryBase;
        public IRepositoryBase<DayOfTheWeek> DayOfTheWeeks => DayOfTheWeeksRepositoryBase ??= new RepositoryBase<DayOfTheWeek>(db);


        public async Task SaveChangeAsync()
        {
            await db.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await db.Database.BeginTransactionAsync();
        }
    }
}
