using Microsoft.AspNetCore.Mvc.Filters;

namespace ServerBox.Web.Filters;

public class RequestFilter: IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
       
    }
}