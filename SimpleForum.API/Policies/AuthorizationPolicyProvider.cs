using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SimpleForum.API.Policies
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }

        // TODO - Add method of adding and choosing requirements
        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return base.GetPolicyAsync(policyName);
        }
    }
}