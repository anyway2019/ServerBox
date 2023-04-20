using ServerBox.Core;
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

    public User GetUserByName(string name)
    {
        var info = _repository.Db.Queryable<User>().Single(u => u.Name == name);
        return info;
    }

    public PagedList<User> GetPagedList(int pageIndex, int pageSize)
    {
        var totalNum = 0;
        var list = _repository.Db.Queryable<User>().ToPageList(pageIndex, pageSize, ref totalNum);
        return new PagedList<User>(list, pageSize, totalNum);
    }

    public List<User> GetAll()
    {
        return _repository.Db.Queryable<User>().ToList();
    }
}