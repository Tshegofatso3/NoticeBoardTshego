using NoticeBoardApi.Models;

namespace NoticeBoardApi.Services
{
    public interface INoticeService
    {
        IEnumerable<Notice> GetAll();
        Notice? GetById(int id);
        Notice Create(Notice notice);
        Notice? Update(int id, Notice notice);
        bool Delete(int id);
        IEnumerable<Notice> GetArchived();
    }
}
