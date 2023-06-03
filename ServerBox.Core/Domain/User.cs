using SqlSugar;

namespace ServerBox.Core.Domain
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("user")]
    public class User :BaseEntity
    {
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="username")]
           public string Username {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="password")]
           public string Password {get;set;}

    }
}
