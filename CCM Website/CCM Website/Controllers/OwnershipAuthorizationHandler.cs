using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using CCM_Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CCM_Website.Controllers
{
    public class OwnershipAuthorizationHandler
        : AuthorizationHandler<OwnershipRequirement, Workbook>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OwnershipRequirement requirement,
            Workbook resource
        )
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (resource.OwnerId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
