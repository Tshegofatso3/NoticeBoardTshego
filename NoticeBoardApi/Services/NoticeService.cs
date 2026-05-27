using Microsoft.EntityFrameworkCore;
using NoticeBoardApi.Data;
using NoticeBoardApi.Models;

namespace NoticeBoardApi.Services
{
    public class NoticeService : INoticeService
    {
        private readonly AppDbContext _db;

        public NoticeService(AppDbContext db)
        {
            _db = db;
        }
        public IEnumerable<Notice> GetArchived()=> _db.Notices.Where(n => n.IsArchived).ToList();

        public IEnumerable<Notice> GetAll()
            => _db.Notices.ToList();

        public Notice? GetById(int id)
            => _db.Notices.FirstOrDefault(n => n.Id == id);

        public Notice Create(Notice notice)
        {
            notice.CreatedAt = DateTime.UtcNow;
            _db.Notices.Add(notice);
            _db.SaveChanges();
            return notice;
        }

        public Notice? Update(int id, Notice updated)
        {
            var existing = GetById(id);
            if (existing is null) return null;
            existing.Title = updated.Title;
            existing.Body = updated.Body;
            _db.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            var notice = GetById(id);
            if (notice is null) return false;
            _db.Notices.Remove(notice);
            _db.SaveChanges();
            return true;
        }
    }
}
