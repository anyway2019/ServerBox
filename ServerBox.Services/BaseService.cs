
using SqlSugar;
using System.Linq.Expressions;
using ServerBox.Core;
using ServerBox.Data;

namespace ServerBox.Services;

public class BaseService<T> where T : BaseEntity, new()
{
    private readonly EfRepository<T> _efRepository;

    public BaseService(SqlSugarClient db)
    {
        _efRepository = new EfRepository<T>(db);

    }

    public long InsertReturnBigId(T info)
    {
        return _efRepository.Db.Insertable(info).ExecuteReturnBigIdentity();
    }

    public virtual void Add(T info)
    {
        _efRepository.Insert(info);
    }

    public virtual long AddReturnId(T info)
    {
        return _efRepository.InsertReturnId(info);
    }

    public virtual void Update(T info)
    {
        _efRepository.Update(info);
    }

    public virtual T GetById(long id)
    {
        return _efRepository.GetById(id);
    }

    public List<T> GetList(Expression<Func<T, bool>> where)
    {
        return _efRepository.SimpleClient.GetList(where);
    }

    public int GetCount(Expression<Func<T, bool>> where)
    {
        return _efRepository.Db.Queryable<T>().Where(where).Count();
    }

    public bool Contains(Expression<Func<T, bool>> where)
    {
        var count = _efRepository.Db.Queryable<T>().Where(where).Count();
        if (count > 0) return true;
        else return false;
    }

    public virtual void Delete(T info)
    {
        _efRepository.Delete(info);
    }

    public void BeginTran()
    {
        _efRepository.Db.Ado.BeginTran();
    }

    public void CommitTran()
    {
        _efRepository.Db.Ado.CommitTran();
    }

    public void RollbackTran()
    {
        _efRepository.Db.Ado.RollbackTran();
    }
}

