using ServerBox.Web.Extensions;
using SqlSugar;

namespace ServerBox.Web.Utils;

//https://www.donet5.com/home/Doc?typeId=1207

public class EntityGenerator
{
    /// <summary>
    /// generate entity class file to target folder with namespace.
    /// </summary>
    /// <param name="con">database connection string</param>
    /// <param name="nameSpace">target namespace</param>
    public static void Build(string con,string nameSpace)
    {
        var db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = con,
            DbType = DbType.MySql,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });
        
        foreach (var item in db.DbMaintenance.GetTableInfoList())
        {
            var entityName = TableName2ClassName(item.Name);
            db.MappingTables.Add(entityName , item.Name);
            
            foreach (var col in db.DbMaintenance.GetColumnInfosByTableName(item.Name))
            {
                db.MappingColumns.Add(ColumnName2PropertyName(col.DbColumnName), col.DbColumnName, entityName);
            }
        }
      
        db.DbFirst.IsCreateAttribute().CreateClassFile(GetOutputDir(), nameSpace);
    }

    /// <summary>
    /// set output dir
    /// </summary>
    /// <returns></returns>
    private static string GetOutputDir()
    {
        var baseDir = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory())?.FullName;
        return $"{baseDir}\\ServerBox.Core\\Domain";
    }

    /// <summary>
    /// table name to class name. e.g. create_time to CreateTime
    /// </summary>
    /// <param name="name">table name</param>
    /// <returns></returns>
    private static string TableName2ClassName(string name)
    {
        return name.Split("_").Select(x => x.CapitalizeFirst()).Aggregate((x, y) => x + y);
    }
    
    /// <summary>
    /// column name to property name. e.g. create_time to CreateTime
    /// </summary>
    /// <param name="name">column name</param>
    /// <returns></returns>
    private static string ColumnName2PropertyName(string name)
    {
        return name.Split("_").Select(x => x.CapitalizeFirst()).Aggregate((x, y) => x + y);
    }
}