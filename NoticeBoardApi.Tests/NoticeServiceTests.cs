using Microsoft.EntityFrameworkCore;
using NoticeBoardApi.Data;
using NoticeBoardApi.Models;
using NoticeBoardApi.Services;
using Xunit;

namespace NoticeBoardApi.Tests
{
    public class NoticeServiceTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }
        [Fact]
        public void GetAll_ReturnsEmptyList_WhenNoNoticesExist()
        {
            // Arrange 
            var context = CreateContext();
            var service = new NoticeService(context);

            // Act 
            var result = service.GetAll();

            // Assert 
            Assert.Empty(result);
        }
        [Fact]
        public void Create_AddsNoticeAndReturnsItWithId()
        {
            // Arrange 
            var context = CreateContext();
            var service = new NoticeService(context);
            var notice = new Notice
            {
                Title = "Test Notice",
                Body = "Body text",
                CreatedBy = "tester"
            };

            // Act 
            var created = service.Create(notice);

            // Assert 
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal("Test Notice", created.Title);
            Assert.Single(service.GetAll());
        }

        [Fact]
        public void GetById_ReturnsNull_WhenNoticeDoesNotExist()
        {
            // Arrange 
            var context = CreateContext();
            var service = new NoticeService(context);

            // Act 
            var result = service.GetById(999);

            // Assert 
            Assert.Null(result);
        }

        [Fact]
        public void Delete_ReturnsFalse_WhenNoticeDoesNotExist()
        {
            // Arrange 
            var context = CreateContext();
            var service = new NoticeService(context);

            // Act 
            var result = service.Delete(999);

            // Assert 
            Assert.False(result);
        }

        [Fact]
        public void Delete_RemovesNotice_WhenItExists()
        {
            // Arrange 
            var context = CreateContext();
            var service = new NoticeService(context);
            var notice = service.Create(new Notice
            {
                Title = "To Delete",
                Body = "Body"
            });

            // Act 
            var result = service.Delete(notice.Id);

            // Assert 
            Assert.True(result);
            Assert.Empty(service.GetAll());
        }

        [Fact]
        public void GetArchived_ReturnsOnlyArchivedNotices()
        {
            // Arrange 
            var context = CreateContext();
            var service = new NoticeService(context);
            service.Create(new Notice
            {
                Title = "Active",
                Body = "Body",
                IsArchived =false
            });
            service.Create(new Notice
            {
                Title = "Archived",
                Body = "Body",
                IsArchived =true
            });

            // Act 
            var result = service.GetArchived().ToList();

            // Assert 
            Assert.Single(result);
            Assert.Equal("Archived", result[0].Title);
        }
    }
}