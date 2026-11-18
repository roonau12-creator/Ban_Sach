using BanSach.Data;
using BanSach.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BanSach.Tests.Models
{
    public class ThongBaoTests
    {
        [Fact]
        public void Model_ThongBaoRangBuocTheoTaiKhoanVaSuKien()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql("Host=localhost;Database=BanSach")
                .Options;
            using var context = new ApplicationDbContext(options);

            var entity = context.Model.FindEntityType(typeof(ThongBao))!;
            var eventIndex = entity.GetIndexes().Single(x =>
                x.Properties.Select(property => property.Name).SequenceEqual(new[] { "MaTK", "MaSuKien" }));
            var accountRelationship = entity.GetForeignKeys().Single(x => x.Properties.Single().Name == "MaTK");
            var orderRelationship = entity.GetForeignKeys().Single(x => x.Properties.Single().Name == "MaDH");

            Assert.True(eventIndex.IsUnique);
            Assert.Equal(DeleteBehavior.Cascade, accountRelationship.DeleteBehavior);
            Assert.Equal(DeleteBehavior.SetNull, orderRelationship.DeleteBehavior);
        }
    }
}
