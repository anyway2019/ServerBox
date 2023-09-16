using Newtonsoft.Json.Linq;
using SqlSugar;

namespace ServerBox.Core.Utils;

[Serializable]
public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> list, int pageSize, int totalCount)
        : base(list)
    {
        if(pageSize == 0)
        {
            pageSize = 1;
        }
        TotalCount = totalCount;
        TotalPage = TotalCount / pageSize;
        if (TotalCount % pageSize > 0)
        {
            TotalPage += 1;
        }
    }
    
    public PagedList(ISugarQueryable<T> list, int pageIndex, int pageSize)
    {
        TotalPage = TotalCount / pageSize;
        if (TotalCount % pageSize > 0)
        {
            TotalPage += 1;
        }
    }

    public int TotalCount { get; set; }

    public int TotalPage { get; set; }

}
