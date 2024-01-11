using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Expense_Tracker.Filters
{
    public class RemoveUserFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.ModelState.Remove("UserId");
            context.ModelState.Remove("User");
        }
    }
}