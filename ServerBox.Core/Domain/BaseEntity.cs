namespace ServerBox.Core;

public abstract class BaseEntity
{
    [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public virtual long Id { get; set; }
}