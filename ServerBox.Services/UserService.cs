using ServerBox.Core.Domain;
using ServerBox.Core.Dto;
using ServerBox.Core.Utils;
using ServerBox.Data;
using SqlSugar;

namespace ServerBox.Services;

public class UserService :BaseService<User>
{
    private readonly EfRepository<User> _repository;
    
    public UserService(SqlSugarClient db) : base(db)
    {
        _repository = new EfRepository<User>(db);
    }

    public User? GetUserByName(string name)
    {
        var info = _repository.Db.Queryable<User>().Single(u => u.Username == name);
        return info;
    }

    public PagedList<UserModel> GetPagedList(int pageIndex, int pageSize)
    {
        var totalNum = 0;
        var list = _repository.Db.Queryable<User>().Select(c=>new UserModel()
        {
            NickName = c.Username,
        }).ToPageList(pageIndex, pageSize, ref totalNum);
        return new PagedList<UserModel>(list, pageSize, totalNum);
    }

    public List<User> GetAll()
    {
        return _repository.Db.Queryable<User>().ToList();
    }
    
    public void MockAsyncTask()
    {
        //mock async task
        Thread.Sleep(3000);
    }
}