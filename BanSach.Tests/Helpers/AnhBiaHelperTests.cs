using BanSach.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace BanSach.Tests.Helpers
{
    public class AnhBiaHelperTests
    {
        private static AnhBiaHelper TaoHelper(string webRoot)
        {
            var env = new FakeWebHostEnvironment { WebRootPath = webRoot };
            return new AnhBiaHelper(env);
        }

        private static FormFile TaoFile(string tenFile, int doDai = 4)
        {
            var stream = new MemoryStream(new byte[doDai]);
            return new FormFile(stream, 0, doDai, "file", tenFile);
        }

        [Fact]
        public async Task LuuAnh_KhongChonFile_TraVeNull()
        {
            var helper = TaoHelper(Path.GetTempPath());

            Assert.Null(await helper.LuuAnh(null));
        }

        [Fact]
        public async Task LuuAnh_FileRong_TraVeNull()
        {
            var helper = TaoHelper(Path.GetTempPath());

            Assert.Null(await helper.LuuAnh(TaoFile("anh.jpg", doDai: 0)));
        }

        [Theory]
        [InlineData("doc.pdf")]
        [InlineData("phan-mem.exe")]
        [InlineData("script.php")]
        public async Task LuuAnh_DinhDangKhongHopLe_NemException(string tenFile)
        {
            var helper = TaoHelper(Path.GetTempPath());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => helper.LuuAnh(TaoFile(tenFile)));
        }

        [Theory]
        [InlineData("a.jpg")]
        [InlineData("a.jpeg")]
        [InlineData("a.png")]
        [InlineData("a.webp")]
        [InlineData("a.gif")]
        public async Task LuuAnh_DinhDangHopLe_TraVeDuongDanUpload(string tenFile)
        {
            var webRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var helper = TaoHelper(webRoot);

            try
            {
                var duongDan = await helper.LuuAnh(TaoFile(tenFile));

                Assert.NotNull(duongDan);
                Assert.StartsWith("/uploads/bia/", duongDan);
                Assert.EndsWith(Path.GetExtension(tenFile), duongDan);
                Assert.True(File.Exists(Path.Combine(webRoot, duongDan.TrimStart('/'))));
            }
            finally
            {
                Directory.Delete(webRoot, recursive: true);
            }
        }

        [Fact]
        public void XoaAnh_DuongDanRong_KhongLamGi()
        {
            var helper = TaoHelper(Path.GetTempPath());

            helper.XoaAnh(null);
            helper.XoaAnh("");
            helper.XoaAnh("   ");
        }

        [Fact]
        public void XoaAnh_DuongDanHttp_KhongXoa()
        {
            var webRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(webRoot, "uploads", "bia"));
            var helper = TaoHelper(webRoot);
            var file = Path.Combine(webRoot, "uploads", "bia", "anh.jpg");
            File.WriteAllBytes(file, new byte[1]);

            helper.XoaAnh("https://example.com/uploads/bia/anh.jpg");

            Assert.True(File.Exists(file));
            Directory.Delete(webRoot, recursive: true);
        }

        [Fact]
        public void XoaAnh_DuongDanTonTai_XoaFile()
        {
            var webRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(webRoot, "uploads", "bia"));
            var helper = TaoHelper(webRoot);
            var file = Path.Combine(webRoot, "uploads", "bia", "anh.jpg");
            File.WriteAllBytes(file, new byte[1]);

            helper.XoaAnh("/uploads/bia/anh.jpg");

            Assert.False(File.Exists(file));
            Directory.Delete(webRoot, recursive: true);
        }

        [Fact]
        public void XoaAnh_KhongTonTai_KhongNemException()
        {
            var webRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(webRoot);
            var helper = TaoHelper(webRoot);

            helper.XoaAnh("/uploads/bia/khong-co.jpg");
        }

        private sealed class FakeWebHostEnvironment : IWebHostEnvironment
        {
            public string ApplicationName { get; set; } = "BanSach.Tests";
            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
            public string ContentRootPath { get; set; } = Path.GetTempPath();
            public string EnvironmentName { get; set; } = "Test";
            public string WebRootPath { get; set; } = Path.GetTempPath();
            public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        }
    }
}
