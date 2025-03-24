using AutoMapper;
using BaoTran.Data;
using BaoTran.Models;
using BaoTran.Repository;
using BaoTran.Validators;

namespace BaoTran.Services
{
    public interface IPlaySchedualService
    {
        Task<IEnumerable<PlaySchedualResponse>> GetAllPlaySchedual();
        Task<(bool Success, string ErrorMessage)> PostPlaySchedual(PlaySchedualRequest playSchedualNew);
        Task<(bool Success, string ErrorMessage)> PutPlaySchedual(int id, PlaySchedualRequest playSchedualUpdate);
        Task<(bool Success, string ErrorMessage)> DeletePlaySchedual(int id);

    }

    public class PlaySchedualService : IPlaySchedualService
    {
        private readonly IMapper mapper;
        private readonly PlaySchedualValidator playSchedualValidator;
        private readonly IRepositoryWrapper repository;

        public PlaySchedualService(IMapper mapper, PlaySchedualValidator playSchedualValidator, IRepositoryWrapper repository)
        {
            this.mapper = mapper;
            this.playSchedualValidator = playSchedualValidator;
            this.repository = repository;
        }

        public async Task<IEnumerable<PlaySchedualResponse>> GetAllPlaySchedual()
        {
            IEnumerable<Data.DateRange> dateRanges = await repository.DateRanges.GetAllAsync();
            IEnumerable<Data.TimeRange> timeRanges = await repository.TimeRanges.GetAllAsync();
            IEnumerable<Data.DayOfTheWeek> dayOfTheWeeks = await repository.DayOfTheWeeks.GetAllAsync();

            List<PlaySchedualResponse> playSchedualResponses = new();

            foreach (Data.DayOfTheWeek day in dayOfTheWeeks)
            {

                IEnumerable<Data.TimeRange> timeRangesForDay = timeRanges.Where(tr => tr.DayOfWeekId == day.DayOfWeekId);

                Data.DateRange? dateRangeForDay = dateRanges.FirstOrDefault(dr => dr.DateRangeId == day.DateRangeId);

                foreach (Data.TimeRange? timeRange in timeRangesForDay)
                {
                    PlaySchedualResponse playSchedualResponse = new()
                    {
                        DaysOfWeek = day.DayOfWeek,
                        StartTime = timeRange.StartTime,
                        EndTime = timeRange.EndTime,
                        StartDate = dateRangeForDay?.StartDate ?? DateTime.MinValue,
                        EndDate = dateRangeForDay?.EndDate ?? DateTime.MaxValue,
                        IdMediaFile = dateRangeForDay.MediaFileId,
                    };

                    playSchedualResponses.Add(playSchedualResponse);
                }
            }

            return playSchedualResponses;
        }

        //THÊM 1 LỊCH PHÁT
        public async Task<(bool Success, string ErrorMessage)> PostPlaySchedual(PlaySchedualRequest playSchedualNew)
        {
            (bool isConflict, string errorMessage) result = await playSchedualValidator.IsTimeValid(playSchedualNew, await GetAllPlaySchedual());
            if (!result.isConflict)
            {
                return (false, result.errorMessage);
            }

            FluentValidation.Results.ValidationResult validationresult = await playSchedualValidator.ValidateAsync(playSchedualNew);
            if (!validationresult.IsValid)
                return (false, validationresult.Errors.First().ErrorMessage);

            await SaveSchedule(playSchedualNew);

            return (true, null);
        }


        private async Task SaveSchedule(PlaySchedualRequest playSchedualNew)
        {
            DateRange dateRange = new()
            {
                StartDate = DateTime.Parse(playSchedualNew.StartDate).Date,
                EndDate = DateTime.Parse(playSchedualNew.EndDate).Date,
                MediaFileId = playSchedualNew.IdMediaFile
            };
            repository.DateRanges.Create(dateRange);
            await repository.SaveChangeAsync();

            DayOfTheWeek dayOfTheWeek = new()
            {
                DateRangeId = dateRange.DateRangeId,
                DayOfWeek = Enum.Parse<DayOfWeek>(playSchedualNew.DaysOfWeek)
            };
            repository.DayOfTheWeeks.Create(dayOfTheWeek);
            await repository.SaveChangeAsync();

            TimeRange timeRange = new()
            {
                DayOfWeekId = dayOfTheWeek.DayOfWeekId,
                StartTime = TimeSpan.Parse(playSchedualNew.StartTime),
                EndTime = TimeSpan.Parse(playSchedualNew.EndTime)
            };
            repository.TimeRanges.Create(timeRange);
            await repository.SaveChangeAsync();
        }


        //LẤY RA 1 LỊCH PHÁT
        public async Task<PlaySchedualResponse> GetPlayScheduleById(int idDateRange)
        {
            DateRange existingDateRange = await repository.DateRanges.GetSingleAsync(g => g.DateRangeId == idDateRange);

            if (existingDateRange != null)
            {
                DayOfTheWeek existingDayOfTheWeek = await repository.DayOfTheWeeks.GetSingleAsync(g => g.DateRangeId == existingDateRange.DateRangeId);

                if (existingDayOfTheWeek != null)
                {
                    TimeRange existingTimeRange = await repository.TimeRanges.GetSingleAsync(g => g.DayOfWeekId == existingDayOfTheWeek.DayOfWeekId);

                    if (existingTimeRange != null)
                    {
                        PlaySchedualResponse playSchedual = new()
                        {
                            StartDate = existingDateRange.StartDate,
                            EndDate = existingDateRange.EndDate,
                            StartTime = existingTimeRange.StartTime,
                            EndTime = existingTimeRange.EndTime,
                            IdMediaFile = existingDateRange.MediaFileId,
                            DaysOfWeek = existingDayOfTheWeek.DayOfWeek
                        };

                        return playSchedual;
                    }
                }
            }

            return null;
        }


        //SỬA LỊCH PHÁT
        public async Task<(bool Success, string ErrorMessage)> PutPlaySchedual(int idDateRange, PlaySchedualRequest playSchedualUpdate)
        {
            DateRange existingDateRange = await repository.DateRanges.GetSingleAsync(g => g.DateRangeId == idDateRange);
            if (existingDateRange == null) return (false, "Lịch trình này không tồn tại");

            FluentValidation.Results.ValidationResult validationResult = await playSchedualValidator.ValidateAsync(playSchedualUpdate);
            if (!validationResult.IsValid)
                return (false, validationResult.Errors.First().ErrorMessage);

            // Kiểm tra nếu dữ liệu đang được cập nhật lại giống như chính nó ban đầu
            PlaySchedualResponse existingSchedual = await GetPlayScheduleById(idDateRange);
            bool isSameSchedule = await playSchedualValidator.IsSameSchedule(existingSchedual, playSchedualUpdate);

            if (!isSameSchedule)
            {
                IEnumerable<PlaySchedualResponse> otherSchedules = await GetAllPlaySchedual();
                (bool isConflict, string errorMessage) = await playSchedualValidator.IsTimeValid(playSchedualUpdate, otherSchedules);
                if (!isConflict)
                {
                    return (false, errorMessage);
                }
            }

            await UpdateDateRange(playSchedualUpdate);

            return (true, null);
        }



        private async Task UpdateDateRange(PlaySchedualRequest playSchedualUpdate)
        {
            DateRange existingDateRange = new()
            {
                StartDate = DateTime.Parse(playSchedualUpdate.StartDate).Date,
                EndDate = DateTime.Parse(playSchedualUpdate.EndDate).Date,
                MediaFileId = playSchedualUpdate.IdMediaFile
            };

            repository.DateRanges.Update(existingDateRange);
            await repository.SaveChangeAsync();

            await UpsertDayOfTheWeek(existingDateRange, playSchedualUpdate);
        }



        private async Task UpsertDayOfTheWeek(DateRange existingDateRange, PlaySchedualRequest playSchedualUpdate)
        {
            DayOfTheWeek existingDayOfTheWeek = await repository.DayOfTheWeeks.GetSingleAsync(g => g.DateRangeId == existingDateRange.DateRangeId);

            if (existingDayOfTheWeek != null)
            {
                existingDayOfTheWeek.DayOfWeek = Enum.Parse<DayOfWeek>(playSchedualUpdate.DaysOfWeek);
                repository.DayOfTheWeeks.Update(existingDayOfTheWeek);
            }
            else
            {
                existingDayOfTheWeek = new DayOfTheWeek
                {
                    DateRangeId = existingDateRange.DateRangeId,
                    DayOfWeek = Enum.Parse<DayOfWeek>(playSchedualUpdate.DaysOfWeek)
                };
                repository.DayOfTheWeeks.Create(existingDayOfTheWeek);
            }

            await repository.SaveChangeAsync();

            await UpsertTimeRange(existingDayOfTheWeek, playSchedualUpdate);
        }


        private async Task UpsertTimeRange(DayOfTheWeek existingDayOfTheWeek, PlaySchedualRequest playSchedualUpdate)
        {
            TimeRange existingTimeRange = await repository.TimeRanges.GetSingleAsync(g => g.DayOfWeekId == existingDayOfTheWeek.DayOfWeekId);

            if (existingTimeRange != null)
            {
                existingTimeRange.StartTime = TimeSpan.Parse(playSchedualUpdate.StartTime);
                existingTimeRange.EndTime = TimeSpan.Parse(playSchedualUpdate.EndTime);
                repository.TimeRanges.Update(existingTimeRange);
            }
            else
            {
                existingTimeRange = new TimeRange
                {
                    DayOfWeekId = existingDayOfTheWeek.DayOfWeekId,
                    StartTime = TimeSpan.Parse(playSchedualUpdate.StartTime),
                    EndTime = TimeSpan.Parse(playSchedualUpdate.EndTime)
                };
                repository.TimeRanges.Create(existingTimeRange);
            }

            await repository.SaveChangeAsync();
        }

        //XÓA 1 LỊCH PHÁT
        public async Task<(bool Success, string ErrorMessage)> DeletePlaySchedual(int id)
        {
            DateRange? playSchedual = await repository.DateRanges.FirstOrDefaultAsync(u => u.DateRangeId == id);
            if (playSchedual == null)
                return (false, "Play Schedule not found.");

            DayOfTheWeek? dayOfTheWeek = await repository.DayOfTheWeeks.GetSingleAsync(d => d.DateRangeId == playSchedual.DateRangeId);
            if (dayOfTheWeek != null)
            {
                TimeRange? timeRange = await repository.TimeRanges.GetSingleAsync(t => t.DayOfWeekId == dayOfTheWeek.DayOfWeekId);
                if (timeRange != null)
                {
                    repository.TimeRanges.Delete(timeRange);
                }
                repository.DayOfTheWeeks.Delete(dayOfTheWeek);
            }
            repository.DateRanges.Delete(playSchedual);
            await repository.SaveChangeAsync();

            return (true, null);
        }



    }
}
