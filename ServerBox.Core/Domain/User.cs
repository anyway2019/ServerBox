using SqlSugar;

namespace ServerBox.Core;

[SugarTable("user")]
public class User : BaseEntity
{
    /// <summary>
    /// 
    /// </summary>
    [SugarColumn(ColumnName = "Name")]
    public string Name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [SugarColumn(ColumnName = "Password")]
    public string Password { get; set; }
    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(ColumnName = "Email")]
    public string Email { get; set; }
    
    public string Remark { get; set; }
    
    public int CustomerId { get; set; }
    
    public string OpenId { get; set; }
 
    
    [SugarColumn(ColumnName = "CreateTime")]
    public DateTime CreateTime { get; set; }
    
    [SugarColumn(ColumnName = "Status")]
    public int Status { get; set; }

}