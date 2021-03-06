using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleForum.Common;
using SimpleForum.Common.Server;
using SimpleForum.Models;
using SimpleForum.Web.Models;
using SimpleForum.Web.Policies;

namespace SimpleForum.Web.Controllers
{
    public class LoginController : WebController
    {
        private readonly SimpleForumRepository _repository;
        private readonly CrossConnectionManager _crossConnectionManager;
        private readonly SimpleForumConfig _config;

        public LoginController(SimpleForumRepository repository, CrossConnectionManager crossConnectionManager,
            IOptionsSnapshot<SimpleForumConfig> config)
        {
            _repository = repository;
            _crossConnectionManager = crossConnectionManager;
            _config = config.Value;
        }
        
        // Returns the login page
        [AnonymousOnly]
        public IActionResult Index(int? error, string ReturnUrl)
        {
            LoginViewModel model = new LoginViewModel()
            {
                Error = error,
                ReturnUrl = ReturnUrl
            };

            return View("Login", model);
        }

        // Logs in a user
        [AnonymousOnly]
        public async Task<IActionResult> SendLogin(string username, string password, string ReturnUrl)
        {
            // Returns error if username or password are null
            if (username == null || password == null) return Redirect("/Login?error=0");
            
            // Retrieves user, returning error if none are found
            User user = await _repository.GetUserAsync(username);
            if (user == null) return Redirect("/Login?error=1");
            
            // Returns error if password is incorrect
            if (!user.CheckPassword(password) || user.Deleted) return Redirect("/Login?error=1");
            IActionResult returnResult = Url.IsLocalUrl(ReturnUrl) ? Redirect(ReturnUrl) : Redirect("/");
            return await SignInUser(user, returnResult);
        }

        // Signs out a user
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        // Returns the page for submitting a password reset request
        [AnonymousOnly]
        public IActionResult ForgotPassword(int? error)
        {
            FormViewModel model = new FormViewModel()
            {
                Error = error
            };
            return View(model);
        }

        // Requests a password reset
        [AnonymousOnly]
        public async Task<IActionResult> SendForgotPassword(string email)
        {
            // Checks the entered email is valid
            if (email == null) return RedirectToAction("ForgotPassword", new {error = 0});
            
            // Retrieves user and returns error if none found
            User user = await _repository.GetUserAsync(email);
            if (user == null) return RedirectToAction("ForgotPassword", new {error = 1});

            // Requests the password reset
            await _repository.RequestPasswordResetAsync(user);
            await _repository.SaveChangesAsync();
            
            // Returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Email sent",
                MessageTitle = "Password reset email sent"
            };
            return View("Message", model);
        }

        // The page for submitting password resets
        [AnonymousOnly]
        public async Task<IActionResult> ResetPassword(string code, int? error)
        {
            // Retrieves email code and redirects if null
            EmailCode emailCode = await _repository.GetEmailCodeAsync(code);
            if (emailCode == null) return Redirect("/");

            ResetPasswordViewModel model = new ResetPasswordViewModel()
            {
                Error = error,
                Code = code,
                UserID = emailCode.UserID
            };
            
            return View(model);
        }
        
        // Resets a user's password
        [AnonymousOnly]
        public async Task<IActionResult> SendResetPassword(string password, string confirmPassword,
            string code, int userID)
        {
            // Checks the parameters are valid and handles errors accordingly
            if (code == null) return Redirect("/");
            if (password == null || confirmPassword == null) return RedirectToAction("ResetPassword", new {code, error = 0}); 
            if (password != confirmPassword) return RedirectToAction("ResetPassword", new {code, error = 1});

            // Changes the password, returning an error if permission is denied
            Result result = await _repository.ResetPasswordAsync(password, code, userID);
            if (result.Failure) return new StatusCodeResult(result.Code);
            await _repository.SaveChangesAsync();
            

            // Returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Password changed",
                MessageTitle = "Password changed successfully"
            };
            return View("Message", model);
        }

        // Returns the RemoteLogin view
        public IActionResult RemoteLogin()
        {
            return View();
        }

        // Redirects the user to the remote CrossLogin page
        public IActionResult RemoteLoginRedirect(string address, string type)
        {
            string url = (address.StartsWith("http://") || address.StartsWith("https://")) switch
            {
                true => address,
                false => $"http://{address}"
            };
            
            return Redirect($"{url}/Login/CrossLogin?address={_config.InstanceURL}&type={type}");
        }

        // Authenticates an API user with the given token for a cross instance login
        public async Task<IActionResult> ApiCrossLogin(string address, string token)
        {
            // Returns if token or address is null
            if (address == null || token == null) return Redirect("/");
            
            // Retrieves token, returning if invalid
            TempApiToken tempApiToken = await _repository.GetTempApiToken(token);
            if (tempApiToken == null) return Unauthorized();
            
            // Signs in user
            IActionResult returnResult = RedirectToAction("CrossLogin", new
            {
                address,
                type = "api"
            });
            return await SignInUser(tempApiToken.User, returnResult);
        }
        
        // Displays the page asking the user if they want to login to another instance
        [Authorize]
        public async Task<IActionResult> CrossLogin(string address, string type)
        {
            // Returns error if logged in with remote account
            User user = await _repository.GetUserAsync(User);
            if (user.ServerID != null) return StatusCode(400, "Cannot remotely sign in with remote account");
            
            // Redirects if address empty 
            if (String.IsNullOrEmpty(address)) return Redirect("/");

            CrossLoginViewModel model = new CrossLoginViewModel() { ReturnUrl = address, Type = type };
            return View(model);
        }
        
        // Logs into the other instance
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendCrossLogin(string address, string type)
        {
            if (String.IsNullOrEmpty(address)) return Redirect("/");
            await _crossConnectionManager.SetupContact(address);

            RemoteAuthToken token = new RemoteAuthToken()
            {
                Token = Guid.NewGuid().ToString(),
                UserID = Tools.GetUserID(User),
                ValidUntil = DateTime.Now.AddMonths(1)
            };
            await _repository.AddRemoteAuthTokenAsync(token);
            await _repository.SaveChangesAsync();
            return Redirect($"{address}/Login/Callback?token={token.Token}&address={_config.InstanceURL}&type={type}");
        }

        // Signs in a remote user
        [AnonymousOnly]
        public async Task<IActionResult> Callback(string token, string address, string type)
        {
            // Authenticates the token
            Result<User> result = await _crossConnectionManager.AuthenticateUser(address, token);
            if (result.Failure) return StatusCode(result.Code, result.Error);
            
            // Signs in with cookie if web, otherwise returns JWT
            if (type == "web") return await SignInUser(result.Value, Redirect("/"));
            if (type == "api") return Ok(JwtToken.CreateToken(result.Value.Username, result.Value.UserID.ToString(), _config.PrivateKey));
            return BadRequest();
        }

        // Signs any user in
        private async Task<IActionResult> SignInUser(User user, IActionResult returnResult)
        {
            // Returns error if user is banned
            if (user.Banned)
            {
                MessageViewModel model = new MessageViewModel()
                {
                    Title = "Account banned",
                    MessageTitle = "Your account is banned",
                    MessageContent =  "Ban reason: " + user.BanReason
                };
                return View("Message", model);
            }

            // Creates a ClaimPrincipal for the user
            ClaimsPrincipal principal = Auth.CreateClaims(user);
            
            // Signs in the user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMonths(1),
                    IsPersistent = true,
                    AllowRefresh = false
                });

            // Redirects the user to the previously requested page if needed, otherwise returns to index
            return returnResult;
        }
    }
}