using AutoMapper;
using BaoTran.Data;
using BaoTran.Models;
using BaoTran.Repository;
using BaoTran.Validators;

namespace BaoTran.Services
{
    public interface IMediaFileService
    {
        Task<IEnumerable<MediaFileResponse>> GetAllMediaFiles();
        Task<MediaFileResponse> GetSingleMediaFile(int idMediaFile);
        Task<(bool Success, string ErrorMessage)> PostMediaFile(MediaFileRequest mediaFileNew);
        Task<(bool Success, string ErrorMessage)> PutMediaFile(int id, MediaFileRequest mediaFileUpdate);
        Task<(bool Success, string ErrorMessage)> DeleteMediaFile(int id);

    }

    public class MediaFileService : IMediaFileService
    {
        private readonly IMapper mapper;
        private readonly MediaFileValidator mediaFileValidator;
        private readonly IRepositoryWrapper repository;

        public MediaFileService(IMapper mapper, MediaFileValidator mediaFileValidator, IRepositoryWrapper repository)
        {
            this.mapper = mapper;
            this.mediaFileValidator = mediaFileValidator;
            this.repository = repository;
        }

        public async Task<IEnumerable<MediaFileResponse>> GetAllMediaFiles()
        {
            IEnumerable<MediaFile> mediaFiles = await repository.MediaFiles.GetAllAsync();
            return mapper.Map<IEnumerable<MediaFileResponse>>(mediaFiles);
        }

        public async Task<MediaFileResponse> GetSingleMediaFile(int idMediaFile)
        {
            MediaFile mediaFile = await repository.MediaFiles.GetSingleAsync(x => x.MediaFileId == idMediaFile);
            if (mediaFile == null)
            {
                return null;
            }
            return mapper.Map<MediaFileResponse>(mediaFile);
        }

        public async Task<string> SaveFile(IFormFile imageFile)
        {

            if (imageFile != null && imageFile.Length > 0)
            {

                if (await mediaFileValidator.IsFileValid(imageFile)) return "Invalid file type";

                string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "File");

                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string extension = Path.GetExtension(imageFile.FileName);

                string newFileName = $"{fileNameWithoutExtension}-{DateTime.UtcNow.Ticks}{extension}";
                string dest = Path.Combine(uploads, newFileName);

                using (FileStream stream = new(dest, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                return $"/Files/{newFileName}";
            }
            else
            {
                return "";
            }
        }

        public async Task<(bool Success, string ErrorMessage)> PostMediaFile(MediaFileRequest mediaFileNew)
        {

            if (await repository.MediaFiles.AnyAsync(g => g.Title == mediaFileNew.Title))
                return (false, "Title is already taken");

            FluentValidation.Results.ValidationResult validationResult = await mediaFileValidator.ValidateAsync(mediaFileNew);
            if (!validationResult.IsValid)
                return (false, validationResult.Errors.First().ErrorMessage);

            MediaFile mediaFile = mapper.Map<MediaFile>(mediaFileNew);

            mediaFile.File = await SaveFile(mediaFileNew.File);

            repository.MediaFiles.Create(mediaFile);
            await repository.SaveChangeAsync();

            return (true, null);

        }

        public async Task<(bool Success, string ErrorMessage)> PutMediaFile(int id, MediaFileRequest mediaFileUpdate)
        {
            MediaFile existingMediaFile = await repository.MediaFiles.GetSingleAsync(g => g.MediaFileId == id);
            if (existingMediaFile == null)
                return (false, "Media File not found.");

            FluentValidation.Results.ValidationResult validationResult = await mediaFileValidator.ValidateAsync(mediaFileUpdate);
            if (!validationResult.IsValid)
                return (false, validationResult.Errors.First().ErrorMessage);

            mapper.Map(mediaFileUpdate, existingMediaFile);

            if (mediaFileUpdate.File != null && mediaFileUpdate.File.Length > 0)
                existingMediaFile.File = await SaveFile(mediaFileUpdate.File) ?? existingMediaFile.File;

            repository.MediaFiles.Update(existingMediaFile);
            await repository.SaveChangeAsync();

            return (true, null);
        }



        public async Task<(bool Success, string ErrorMessage)> DeleteMediaFile(int id)
        {
            MediaFile? mediaFile = await repository.MediaFiles.FirstOrDefaultAsync(u => u.MediaFileId == id);
            if (mediaFile == null)
                return (false, "Media File not found.");

            repository.MediaFiles.Delete(mediaFile);
            await repository.SaveChangeAsync();

            return (true, null);
        }


    }
}
