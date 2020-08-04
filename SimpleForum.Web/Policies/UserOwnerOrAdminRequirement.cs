using Microsoft.AspNetCore.Authorization;

namespace SimpleForum.Web.Policies
{
    public class UserOwnerOrAdminRequirement : IAuthorizationRequirement
    {
        
    }
}