namespace BanSach.Helpers
{
    public class AnhBiaHelper
    {
        private readonly IWebHostEnvironment _env;

        public AnhBiaHelper(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> LuuAnh(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var hopLe = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            if (!hopLe.Contains(ext))
            {
                throw new InvalidOperationException("Chỉ chấp nhận ảnh JPG, PNG, WEBP hoặc GIF.");
            }

            var folder = Path.Combine(_env.WebRootPath, "uploads", "bia");
            Directory.CreateDirectory(folder);

            var tenFile = $"{Guid.NewGuid():N}{ext}";
            var duongDan = Path.Combine(folder, tenFile);

            await using var stream = new FileStream(duongDan, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/bia/{tenFile}";
        }

        public void XoaAnh(string? duongDan)
        {
            if (string.IsNullOrWhiteSpace(duongDan) || duongDan.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var full = Path.Combine(_env.WebRootPath, duongDan.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(full))
            {
                File.Delete(full);
            }
        }
    }
}
