using Microsoft.AspNetCore.Mvc.Filters;
using PlayerManagement.Utilities;

namespace PlayerManagement.CustomControllers
{
    /// <summary>
    /// Elephant Controller
    /// persist the Index Sort, Filter and Paging parameters
    /// into a URL stored in ViewData
    /// WARNING: Depends on the following Utilities
    ///  - CookieHelper
    ///  - MaintainURL
    /// </summary>
    public class ElephantController : CognizantController
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (ActionName() != "Index")//Not needed on Index
            {
                ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
            }
            base.OnActionExecuting(context);
        }

        public override Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (ActionName() != "Index")//Not needed on Index
            {
                ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
