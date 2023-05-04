using SqlSugar;
using System.Linq.Expressions;
using ServerBox.Core;
using ServerBox.Core.Utils;

namespace ServerBox.Data;

public sealed class EfRepository<T> where T : BaseEntity, new()
{
    public EfRepository(SqlSugarClient db)
    {
        Db = db;
        SimpleClient = new SimpleClient<T>(db);
    }

    public readonly SimpleClient<T> SimpleClient;

    public SqlSugarClient Db { get; }


    /// <summary>
    /// Get entity by identifier
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns>Entity</returns>
    public T GetById(long id)
    {
        ////see some suggested performance optimization (not tested)
        ////http://stackoverflow.com/questions/11686225/dbset-find-method-ridiculously-slow-compared-to-singleordefault-on-id/11688189#comment34876113_11688189

        return Db.Queryable<T>().Single(s => s.Id == id);
    }

    /// <summary>
    /// 插入实体，自动更新id字段
    /// </summary>
    /// <param name="entity">Entity</param>
    public void Insert(T entity)
    {
        var id = Db.Insertable(entity).ExecuteReturnBigIdentity();
        entity.Id = id;
    }

    public long InsertReturnId(T entity)
    {
        var id = Db.Insertable(entity).ExecuteReturnBigIdentity();
        return id;
    }

    public List<T> GetList(Expression<Func<T, bool>> where) => SimpleClient.GetList(where);

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="entity">Entity</param>
    public void Update(T entity)
    {
        Db.Updateable(entity).ExecuteCommand();
    }

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="entity">Entity</param>
    public void Delete(T entity)
    {
        Db.Deleteable(entity).ExecuteCommand();
    }

    public void Delete(System.Linq.Expressions.Expression<Func<T, bool>> expression)
    {
        Db.Deleteable(expression).ExecuteCommand();
    }

    public List<T> GetSqlQuery(string sql, object parameters = null)
    {
        return Db.Ado.SqlQuery<T>(sql, parameters);
    }

    public int ExecuteSqlCommand(string sql, object parameters)
    {
        return Db.Ado.ExecuteCommand(sql, parameters);
    }

    public PagedList<T> Paging(string sql, int pageIndex, int pageSize)
    {
        var totalCount = 0;
        var list = Db.Queryable<T>().ToPageList(pageIndex, pageSize, ref totalCount);
        var result = new PagedList<T>(list, pageSize, totalCount);
        return result;
    }
}

