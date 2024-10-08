using ITValetFrontEnd.HelperClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ITValetFrontEnd.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class RoleBasedValidationAttribute : Attribute, IActionFilter
    {
        private readonly EnumRoles[] _allowedRoles;

        public RoleBasedValidationAttribute(params EnumRoles[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var helper = (GeneralHelper)context.HttpContext.RequestServices.GetService(typeof(GeneralHelper));

            var loggedInUser = helper.ValidateUserAsync().GetAwaiter().GetResult();
            if (loggedInUser == null)
            {
                // User is not logged in, store the return URL and redirect to the login page
                var returnUrl = context.HttpContext.Request.Path;
                context.Result = new RedirectToRouteResult(new
                {
                    controller = "Auth",
                    action = "Login",
                    returnUrl = returnUrl
                });
            }
            else if (loggedInUser.IsCompleteValetAccount == "0")
            {
                // User is not logged in, store the return URL and redirect to the login page
                var returnUrl = context.HttpContext.Request.Path;
                context.Result = new RedirectToRouteResult(new
                {
                    controller = "User",
                    action = "Account",
                    returnUrl = returnUrl
                });
            }
            else
            {
                var userRole = (EnumRoles)Enum.Parse(typeof(EnumRoles), loggedInUser.Role);
                if (!_allowedRoles.Contains(userRole))
                {
                    // User is logged in but does not have any of the required roles, redirect to unauthorized page
                    context.Result = new RedirectToRouteResult(new { controller = "Error", action = "NotFound" });
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // This method is not implemented as per your code
        }
    }
}
