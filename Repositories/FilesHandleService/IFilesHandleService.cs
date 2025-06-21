namespace RentAppBE.Repositories.FilesHandleService
{
	public interface IFilesHandleService
	{
		bool IsValidImageExtension(string fileName);
		Task<bool> IsValidImageContent(IFormFile file);
		bool IsValidImageSize(long fileSize);
		Task<string> SaveImage(IFormFile image, string folderPath);
	}
}