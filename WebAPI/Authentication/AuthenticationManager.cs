using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace WebAPI.Authentication
{
    public class AuthenticationManager
    {
        private OAuthAuthorizationServerOptions _oAuthServerOptions;

        public void ConfigureAuth(IAppBuilder app, HttpConfiguration configuration)
        {
            // Setup Authorization Server
            _oAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                AuthorizeEndpointPath = new PathString(Paths.AuthorizePath),
                TokenEndpointPath = new PathString(Paths.TokenPath),
                ApplicationCanDisplayErrors = true,
#if DEBUG
                AllowInsecureHttp = true,
#endif
                // Authorization server provider which controls the lifecycle of Authorization Server
                Provider = new OAuthAuthorizationServerProvider
                {
                    OnValidateClientAuthentication = ValidateClientAuthentication,
                    OnGrantCustomExtension = GrantCustomExtension
                }, 
                
            };

            app.UseOAuthAuthorizationServer(_oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            // Setup Configuration
            configuration.SuppressDefaultHostAuthentication();
            configuration.Filters.Add(new HostAuthenticationFilter(_oAuthServerOptions.AuthenticationType));
        }

        private static Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            var registeredUsers = new List<User>
            {
                new User {Login = "123", Password = "456"},
                new User {Login = "aaa", Password = "bbb"}
            };

            var login = context.Parameters["login"];
            var password = context.Parameters["password"];
            string userId;

            var user = registeredUsers.SingleOrDefault(x => x.Login == login && x.Password == password);
            if (user != null)
            {
                userId = user.Login;
            }
            else
            {
                context.Rejected();
                return Task.FromResult<object>(null);
            }

            var oAuthIdentity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId)); // ?
            var ticket = new AuthenticationTicket(oAuthIdentity, null);

            context.Validated(ticket);

            return Task.FromResult<object>(null);
        }

        private static Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (IsClientAuthentication(context))
                return Task.FromResult<object>(null);

            context.Validated();
            return Task.FromResult<object>(null);
        }

        private static bool IsClientAuthentication(BaseValidatingClientContext context)
        {
            return context.ClientId != null;
        }

    }
}