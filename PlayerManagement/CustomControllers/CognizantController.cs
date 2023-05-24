using Microsoft.AspNetCore.Mvc;

namespace PlayerManagement.CustomControllers
{
    /// <summary>
    /// "self aware" controller - to know controller's name and action's name
    /// custom controller to add commonly needed methods
    /// </summary>
    public class CognizantController : Controller
    {
        internal string ControllerName()
        {
            return ControllerContext.RouteData.Values["controller"].ToString();
        }
        internal string ActionName()
        {
            return ControllerContext.RouteData.Values["action"].ToString();
        }
    }
}
