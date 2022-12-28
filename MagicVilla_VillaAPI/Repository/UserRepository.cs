using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private string secretKey;
        public UserRepository(ApplicationDbContext db, IConfiguration configuration, 
            UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            this.userManager = userManager;
            this.mapper = mapper;
            this.roleManager = roleManager;
        }

        public bool IsUniqueUser(string userName)
        {
            var user = db.ApplicationUsers.FirstOrDefault(x => x.UserName == userName);
            if(user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = db.ApplicationUsers
                .FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            bool isValidPassword = await userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if(user == null || isValidPassword == false)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }
            //if user was found generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var roles = await userManager.GetRolesAsync(user);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO response = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = mapper.Map<UserDTO>(user)
            };
            return response;

        }

        public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
             ApplicationUser user = new()
            {
                UserName = registrationRequestDTO.UserName,
                Email = registrationRequestDTO.UserName, 
                NormalizedEmail = registrationRequestDTO.UserName.ToUpper(),
                Name = registrationRequestDTO.Name
            };
            try
            {
                var result = await userManager.CreateAsync(user, registrationRequestDTO.Password);
                if(result.Succeeded)
                {
                    if (!roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await roleManager.CreateAsync(new IdentityRole("admin"));
                        await roleManager.CreateAsync(new IdentityRole("customer"));
                    }
                    await userManager.AddToRoleAsync(user, "admin");
                    var userToReturn = db.ApplicationUsers
                        .FirstOrDefault(u => u.UserName == registrationRequestDTO.UserName);
                    return mapper.Map<UserDTO>(userToReturn);
                }
            }
            catch(Exception ex)
            {

            }
            return new UserDTO();
        }
    }
}
