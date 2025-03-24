using BaoTran.Models;
using BaoTran.Repository;
using FluentValidation;
using System.Text.RegularExpressions;

namespace BaoTran.Validators
{
    public class MediaFileValidator : AbstractValidator<MediaFileRequest>
    {
        private readonly IRepositoryWrapper _repository;
        public MediaFileValidator(IRepositoryWrapper repositoryWrapper)
        {
            _repository = repositoryWrapper;


            RuleFor(m => m.Singer)
                .NotEmpty().WithMessage("FileFormat is required.")
                .Must(fileFormat =>
                             fileFormat.Any(char.IsLetter) ||
                             Regex.IsMatch(fileFormat, @"^[a-zA-Z0-9 ]*$")
                    ).WithMessage("FileFormat must contain at least one letter or only letters and numbers.");

            RuleFor(m => m.Musician)
                .NotEmpty().WithMessage("FileFormat is required.")
                .Must(fileFormat =>
                             fileFormat.Any(char.IsLetter) ||
                             Regex.IsMatch(fileFormat, @"^[a-zA-Z0-9 ]*$")
                    ).WithMessage("FileFormat must contain at least one letter or only letters and numbers.");

            RuleFor(m => m.Title)
                .NotEmpty().WithMessage("FileFormat is required.")
                .Must(fileFormat =>
                             fileFormat.Any(char.IsLetter) ||
                             Regex.IsMatch(fileFormat, @"^[a-zA-Z0-9 ]*$")
                    ).WithMessage("FileFormat must contain at least one letter or only letters and numbers.");

            RuleFor(m => m.FileFormat)
                .NotEmpty().WithMessage("FileFormat is required.")
                .Must(fileFormat =>
                             fileFormat.Any(char.IsLetter) ||
                             Regex.IsMatch(fileFormat, @"^[a-zA-Z0-9 ]*$")
                    ).WithMessage("FileFormat must contain at least one letter or only letters and numbers.");
        }


        public async Task<bool> IsFileValid(IFormFile file)
        {
            string[] allowedMimeTypes = new[]
            {
                // Các loại tệp hình ảnh
                "image/png",
                "image/jpeg",
                "image/gif",  

                // Các loại tệp video
                "video/mp4",
                "video/x-ms-wmv",
                "video/x-flv",
                "video/quicktime",
                "video/webm", 

                // Các loại tệp âm thanh
                "audio/mpeg",
                "audio/wav",
                "audio/x-ms-wma",
                "audio/ogg",
                "audio/mp4"
            };

            if (!allowedMimeTypes.Contains(file.ContentType))
                return false;
            return true;
        }
    }

}