using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CholoShajiIdentity.Models;
using CholoShajiIdentity.Models.CredentialModels;
using CholoShajiIdentity.Models.IdentityModels;
using CholoShajiIdentity.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CholoShajiIdentity.Services
{
    public interface IUserAuthenticationService
    {
        Task<Response> RegisterUser(UserSignUpCredential userSignUpCredentials);
        Task<Response> LogInUser(LoginCredential loginCredentials);
        Task<Response> SignOutUser(SignOutCredential signOutCredential);
        Task<Response> ConfirmEmail(ConfirmEmailCredential confirmEmailCredential);
    }

    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserAuthenticationService(IConfiguration configuration, UserManager<User> userManager, SignInManager<User>signInManager)
        {
            this._configuration = configuration;
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        #region public Exposed Methods

        public async Task<Response> RegisterUser(UserSignUpCredential userSignUpCredentials)
        {
            var user = GetUserToSaveInIdentity(userSignUpCredentials);
            var result = await _userManager.CreateAsync(user, userSignUpCredentials.Password);
            if (result.Succeeded)
            {
                await SaveClaimsInUserIdentity(userSignUpCredentials, user);
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"choloshaji.com?username={userSignUpCredentials.UserName},confirmationToken={confirmationToken}";

                var message = new MailMessage("nobelNurulIslam@gmail.com",user.Email,"please confirm your mail",$"please click on this link to confirm your email address: {confirmationLink}");

                using (var emailClient = new SmtpClient("smtp-relay.sendinblue.com", 587))
                {
                    emailClient.Credentials = new NetworkCredential("nobelnurulislam@gmail.com", "HgI95YT1U8EKOJrs");
                    await emailClient.SendMailAsync(message);
                }


                return new Response
                {
                    Messages = new List<string> { $"{userSignUpCredentials.UserName} is successfully created " },
                    IsSuccessful = true
                };
            }
            return new Response
            {
                Messages = result.Errors.Select(error => error.Description).ToList(),
                IsSuccessful = false
            };
        }

        public async Task<Response> LogInUser(LoginCredential loginCredentials)
        {
            var user = new User
            {
                UserName = loginCredentials.UserName,
            };

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginCredentials.Password, loginCredentials.RememberMe, false);
            if (result.Succeeded)
            {
                var identityUser = await _userManager.FindByNameAsync(loginCredentials.UserName);
                var claims = await _userManager.GetClaimsAsync(identityUser);
                var token = CreateToken(claims);
                return  new Response
                {
                    Messages = new List<string> { $"{user.UserName} is successfully Logged In " },
                    IsSuccessful = true,
                    AdditionalCredentialInformation = new Dictionary<string, string> { { "AccessToken", token } }
                };
            }

            return new Response
            {
                Messages = GetErrorMessage(user.UserName, result),
                IsSuccessful = false
            };
        }

        public async Task<Response> SignOutUser(SignOutCredential signOutCredential)
        {
            try
            {
                await _signInManager.SignOutAsync();
                return new Response
                {
                    Messages = new List<string> { "bro you are successfully signed Out" },
                    IsSuccessful = true,
                };
            }
            catch (Exception e)
            {
                return new Response
                {
                    Messages = new List<string> { "bro you aren't signed Out" },
                    IsSuccessful = false
                };
            }
        }

        public async Task<Response> ConfirmEmail(ConfirmEmailCredential confirmEmailCredential)
        {
            var user = await GetUserByUserName(confirmEmailCredential.UserName);
            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailCredential.EmailConfirmationToken);
            if (result.Succeeded)
            {
                return new Response
                {
                    Messages = new List<string> { $"{user.UserName}'s Email is Successfully Confirmed. You Can Login Now" },
                    IsSuccessful = true
                };
            }
            return new Response
            {
                Messages = new List<string> { $"{user.UserName}'s Email Confirmation Is Failed." },
                IsSuccessful = false
            };
        }

        #endregion


        #region PrivateHelperMethods

        private async Task SaveClaimsInUserIdentity(UserSignUpCredential userSignUpCredentials, User user)
        {
            var claims = GetUserClaims(userSignUpCredentials, user);
            var identityUser = await GetUserByUserName(user.UserName);
            await _userManager.AddClaimsAsync(identityUser, claims);
        }

        private async Task<User> GetUserByUserName(string userName)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);
            return identityUser;
        }

        private User GetUserToSaveInIdentity(UserSignUpCredential userSignUpCredentials)
        {
            var user = new User
            {
                FirstName = userSignUpCredentials.FirstName,
                LastName = userSignUpCredentials.LastName,
                Email = userSignUpCredentials.Email,
                UserName = userSignUpCredentials.UserName,
                UserType = "User"
            };
            return user;
        }

        private List<Claim> GetUserClaims(UserSignUpCredential userSignUpCredentials, User user)
        {
            var claims = new List<Claim>
            {
                new Claim("UserName", userSignUpCredentials.UserName),
                new Claim(ClaimTypes.Email, userSignUpCredentials.Email),
                new Claim("FirstName", userSignUpCredentials.FirstName),
                new Claim("LastName", userSignUpCredentials.LastName),
                new Claim("UserType", user.UserType),
            };
            return claims;
        }

        private List<string> GetErrorMessage(string userUserName, SignInResult result)
        {
            var messages = new List<string>();
            if (result.IsLockedOut)
            {
                messages.Add($"{userUserName} is locked out");
            }
            else
            {
                messages.Add("Failed To Login");
            }

            return messages;
        }

        private string CreateToken(IList<Claim> claims)
        {
            var secretKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey"));
            var expiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("ValidationTime"));


            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        #endregion

    }
}
