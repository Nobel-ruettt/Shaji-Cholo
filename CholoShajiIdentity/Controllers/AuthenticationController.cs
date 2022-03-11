using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CholoShajiIdentity.Models;
using CholoShajiIdentity.Models.CredentialModels;
using CholoShajiIdentity.Services;
using Microsoft.Extensions.Configuration;

namespace CholoShajiIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly IUserAuthenticationService _userAuthenticationService;

        public AuthenticationController(IConfiguration configuration, IUserAuthenticationService userAuthenticationService)
        {
            this._configuration = configuration;
            this._userAuthenticationService = userAuthenticationService;
        }

        [HttpPost("SignUpUser")]
        public async Task<IActionResult> SignUpUser(UserSignUpCredential command)
        {
            var result = await _userAuthenticationService.RegisterUser(command);

            if (!result.IsSuccessful)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser(LoginCredential command)
        {
            var result = await _userAuthenticationService.LogInUser(command);
            if (!result.IsSuccessful)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpPost("SignOutUser")]
        public async Task<IActionResult> LoginUser(SignOutCredential command)
        {
            var result = await _userAuthenticationService.SignOutUser(command);
            if (!result.IsSuccessful)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }



        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailCredential command)
        {
            var result = await _userAuthenticationService.ConfirmEmail(command);
            if (!result.IsSuccessful)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
