using System.Text;
using System.Drawing.Imaging;
using System.Drawing;


namespace RentAppBE.Repositories.FilesHandleService
{
	public class FilesHandleService(IConfiguration configuration) : IFilesHandleService
	{
		private readonly IConfiguration _configuration = configuration;
		private readonly Dictionary<string, List<byte[]>> ImageSignature = new()
		{
			{ ".jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 } } },
			{ ".jpg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 } } },
			{ ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47 } } },
			{ ".gif", new List<byte[]> { "GIF8"u8.ToArray() } },
			{ ".bmp", new List<byte[]> { "BM"u8.ToArray() } }
		};


		public bool IsValidImageExtension(string fileName)
		{
			var extension = Path.GetExtension(fileName).ToLowerInvariant();
			return _configuration.GetSection("AllowedImageExtensions").Get<string[]>()!.Contains(extension);
		}

		public async Task<bool> IsValidImageContent(IFormFile file)
		{
			using var stream = file.OpenReadStream();
			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

			if (!ImageSignature.ContainsKey(extension))
			{
				return false;
			}

			var signatures = ImageSignature[extension];
			var headerBytes = new byte[signatures.Max(m => m.Length)];

			await stream.ReadAsync(headerBytes, 0, headerBytes.Length);

			return signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
		}

		public bool IsValidImageSize(long fileSize) => fileSize <= _configuration.GetSection("MaxImageSizeInBytes").Get<double>()!;

		public async Task<string> SaveImage(IFormFile image, string folderPath)
		{
			if (image == null || image.Length == 0)
				return string.Empty;

			var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
			var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			var fullFolderPath = Path.Combine(webRootPath, folderPath);

			if (!Directory.Exists(fullFolderPath))
				Directory.CreateDirectory(fullFolderPath);

			var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
			var sanitizedFileName = Path.GetFileNameWithoutExtension(image.FileName);
			var uniqueFileName = $"{sanitizedFileName}_{timestamp}{extension}";

			var fullFilePath = Path.Combine(fullFolderPath, uniqueFileName);

			using (var memoryStream = new MemoryStream())
			{
				await image.CopyToAsync(memoryStream);

				// image compression 
				using (var originalImage = Image.FromStream(memoryStream))
				{
					var jpegEncoder = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
					var encoderParameters = new EncoderParameters(1);
					encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L); // Compression quality (0-100)

					originalImage.Save(fullFilePath, jpegEncoder, encoderParameters);
				}
			}

			return Path.Combine(folderPath, uniqueFileName).Replace("\\", "/");
		}
	}
}