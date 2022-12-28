using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/UsersAuth")]
    [ApiController]
    [ApiVersionNeutral]
    public class UsersController : Controller
    {
        private readonly IUserRepository userRepo;
        protected APIResponse response;
        public UsersController(IUserRepository userRepo)
        {
            this.userRepo = userRepo;
            response = new();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await userRepo.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.Errors.Add("Username or password is incorrect");
                return BadRequest(response);
            }
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Result = loginResponse;
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            bool isUserNameUnique = userRepo.IsUniqueUser(model.UserName);
            if (!isUserNameUnique)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.Errors.Add("Username already exist!");
                return BadRequest(response);
            }
            var user = await userRepo.Register(model);
            if (user == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.Errors.Add("Error while registering");
                return BadRequest(response);
            }
            response.StatusCode = System.Net.HttpStatusCode.OK;
            return Ok(response);
        }
    }
}
