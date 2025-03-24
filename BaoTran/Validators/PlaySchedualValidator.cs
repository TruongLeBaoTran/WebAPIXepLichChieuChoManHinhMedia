using BaoTran.Models;
using BaoTran.Repository;
using FluentValidation;

namespace BaoTran.Validators
{
    public class PlaySchedualValidator : AbstractValidator<PlaySchedualRequest>
    {
        private readonly IRepositoryWrapper repository;
        public PlaySchedualValidator(IRepositoryWrapper repositoryWrapper)
        {
            repository = repositoryWrapper;

            RuleFor(x => x.DaysOfWeek)
                .NotEmpty().WithMessage("Ngày trong tuần không được để trống.")
                .Must(BeAValidDayOfWeek).WithMessage("Ngày trong tuần không hợp lệ.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Thời gian bắt đầu không được để trống.")
                .Must(BeAValidTime).WithMessage("Thời gian bắt đầu không hợp lệ.");


            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("Thời gian kết thúc không được để trống.")
                .Must(BeAValidTime).WithMessage("Thời gian kết thúc không hợp lệ.")
                .Must((model, endTime) => TimeSpan.Parse(endTime) > TimeSpan.Parse(model.StartTime))
                .WithMessage("Thời gian kết thúc phải sau thời gian bắt đầu.");



            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Ngày bắt đầu không được để trống.")
                .Must(BeAValidDate).WithMessage("Ngày bắt đầu không hợp lệ.");


            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("Ngày kết thúc không được để trống.")
                .Must(BeAValidDate).WithMessage("Ngày kết thúc không hợp lệ.")
                .Must((model, endDate) => DateTime.Parse(endDate) >= DateTime.Parse(model.StartDate))
                .WithMessage("Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.");



            RuleFor(x => x.IdMediaFile)
                .GreaterThan(0).WithMessage("IdMediaFile phải lớn hơn 0.");
        }

        private bool BeAValidDayOfWeek(string dayOfWeek)
        {
            return Enum.TryParse<DayOfWeek>(dayOfWeek, true, out _);
        }

        private bool BeAValidTime(string time)
        {
            return TimeSpan.TryParse(time, out _);
        }

        private bool BeAValidDate(string date)
        {
            return DateTime.TryParse(date, out _);
        }


        public async Task<(bool Success, string ErrorMessage)> IsTimeValid(PlaySchedualRequest playSchedualNew, IEnumerable<PlaySchedualResponse> allSchedules)
        {
            DateTime startDate = DateTime.Parse(playSchedualNew.StartDate).Date;
            DateTime endDate = DateTime.Parse(playSchedualNew.EndDate).Date;
            TimeSpan startTime = TimeSpan.Parse(playSchedualNew.StartTime);
            TimeSpan endTime = TimeSpan.Parse(playSchedualNew.EndTime);
            DayOfWeek newDayOfWeek = Enum.Parse<DayOfWeek>(playSchedualNew.DaysOfWeek);

            foreach (PlaySchedualResponse existingSchedule in allSchedules)
            {
                // Chỉ kiểm tra nếu có cùng ngày trong tuần
                if (existingSchedule.DaysOfWeek == newDayOfWeek)
                {
                    // Kiểm tra sự giao thoa giữa các khoảng thời gian ngày
                    bool isDateOverlap = !(endDate < existingSchedule.StartDate || startDate > existingSchedule.EndDate);

                    if (isDateOverlap)
                    {
                        // Nếu trùng ngày thì kiểm tra thời gian
                        if (existingSchedule.StartTime < endTime && existingSchedule.EndTime > startTime)
                        {
                            return (false, "Lịch trình đã tồn tại trong khoảng thời gian này.");
                        }
                    }
                }
            }

            return (true, string.Empty);
        }

        public async Task<bool> IsSameSchedule(PlaySchedualResponse existingSchedual, PlaySchedualRequest playSchedualUpdate)
        {
            bool isSameSchedule = existingSchedual.StartDate.ToString("yyyy-MM-dd") == playSchedualUpdate.StartDate &&
                     existingSchedual.EndDate.ToString("yyyy-MM-dd") == playSchedualUpdate.EndDate &&
                     existingSchedual.IdMediaFile == playSchedualUpdate.IdMediaFile &&
                     existingSchedual.StartTime == TimeSpan.Parse(playSchedualUpdate.StartTime) &&
                     existingSchedual.EndTime == TimeSpan.Parse(playSchedualUpdate.EndTime);
            if (isSameSchedule)
                return true;
            return false;
        }

    }
}