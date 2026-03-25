using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebLab.Models;
using static WebLab.Program;

namespace WebLab.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        public ApplicationContext datacontext;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, ApplicationContext context)
        {
            _logger = logger;
            _config = config;
            datacontext = context;
        }

        [Route("")]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpDelete]
        [HttpPatch]
        [HttpPut]
        [HttpGet]
        [Route("/auth")]
        [Route("/auth/login")]
        [Route("/auth/logout")]
        public IActionResult UnsupportedAuth()
        {
            return StatusCode(405, new {message = "Данный запрос не поддерживается."});
        }

        [HttpPost]
        [Route("/auth")]
        [Route("/auth/login")]
        public IActionResult Login(LoginModel loginInfo)
        {
            if (loginInfo.Login == null || loginInfo.Password == null) return Unauthorized(new { message = "Не введён логин или пароль!" });
            
            var User = datacontext.Users.FirstOrDefault(u => u.Login == loginInfo.Login);
            if (User is null) return Unauthorized(new { message = "Нет такого пользователя!" });

            if (!PasswordHasher.VerifyPassword(loginInfo.Password, User.Password)) return Unauthorized(new { message = "Неверный пароль." });

            var Claims = new List<Claim> {
                new Claim(ClaimTypes.Name, loginInfo.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, User.EditAccess.ToString())};

            var jwt = new JwtSecurityToken(
                issuer: _config["JWT:ISSUER"], 
                audience: _config["JWT:AUDIENCE"],
                claims: Claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["JWT:DURATION"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:KEY"])), SecurityAlgorithms.HmacSha256)
            );
            var response = new { access_token = new JwtSecurityTokenHandler().WriteToken(jwt) };
            return Json(response);
        }

        [HttpPost]
        [Authorize]
        [Route("/auth/logout")]
        public IActionResult Logout()
        {
            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("/entities")]
        public IActionResult Entities()
        {
            var teachers = datacontext.Teachers.ToArray();
            return Ok(new { teacher_list = teachers });
        }

        [HttpPost]
        [Authorize(Roles = "True")]
        [Route("/entities")]
        public IActionResult Entities(Teacher teacherData)
        {
            var teacherdata = teacherData;
            if (!ModelState.IsValid) return UnprocessableEntity(new { message = "Ошибка валидации данных." });
            datacontext.Teachers.Add(teacherdata);
            datacontext.SaveChanges();
            return StatusCode(201);
        }

        [HttpPut]
        [HttpPatch]
        [HttpDelete]
        [Authorize]
        [Route("/entities")]
        public IActionResult EntitiesUnsupported()
        {
            return StatusCode(405, new { message = "Данный запрос не поддерживается." });
        }

        [HttpPost]
        [Authorize]
        [Route("/entities/{id}")]
        public IActionResult EntitiesIdUnsupported()
        {
            return StatusCode(405, new { message = "Данный запрос не поддерживается." });
        }

        [HttpGet]
        [Authorize]
        [Route("/entities/{id}")]
        public IActionResult Entities(int id)
        {
            var Teacher = datacontext.Teachers.FirstOrDefault(t => t.Id == id);
            if (Teacher == null) return NotFound(new {message = "Преподаватель не найден."});
            return Ok(new { teacher_data = Teacher });
        }        

        [HttpPut]
        [Authorize(Roles = "True")]
        [Route("/entities/{id}")]
        public IActionResult PutEntities(int id, Teacher TeacherNew)
        {
            var TeacherCurrent = datacontext.Teachers.FirstOrDefault(t => t.Id == id);
            if (TeacherCurrent == null) return NotFound(new { message = "Преподаватель не найден." });

            if (id != TeacherNew.Id) return UnprocessableEntity(new {message = "Ошибка валидации данных."});

            TeacherCurrent.Email = TeacherNew.Email;
            TeacherCurrent.FullName = TeacherNew.FullName;
            TeacherCurrent.Department = TeacherNew.Department;
            TeacherCurrent.ExperienceYears = TeacherNew.ExperienceYears;
            TeacherCurrent.Active = TeacherNew.Active;

            if (!ModelState.IsValid) return UnprocessableEntity(new { message = "Ошибка валидации данных." });

            datacontext.Teachers.Update(TeacherCurrent);
            datacontext.SaveChanges();
            return Ok();
        }

        [HttpPatch("/entities/{id}")]
        [Consumes("application/json-patch+json")]
        [Authorize(Roles = "True")]
        public IActionResult PatchEntities([FromRoute] int id, [FromBody] JsonPatchDocument<Teacher> patch)
        {
            var TeacherCurrent = datacontext.Teachers.FirstOrDefault(t => t.Id == id);
            if (TeacherCurrent == null) return NotFound(new { message = "Преподаватель не найден." });
            
            if (patch == null) return BadRequest(new { message = "Неправильный запрос." });
            patch.ApplyTo(TeacherCurrent, ModelState);
            if (!TryValidateModel(TeacherCurrent)) return UnprocessableEntity(new { message = "Ошибка валидации данных." });

            datacontext.Update(TeacherCurrent);
            datacontext.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "True")]
        [Route("/entities/{id}")]
        public IActionResult DeleteEntities(int id)
        {
            var TeacherCurrent = datacontext.Teachers.FirstOrDefault(t => t.Id == id);
            if (TeacherCurrent == null) return NotFound(new { message = "Преподаватель не найден." });
            datacontext.Teachers.Remove(TeacherCurrent);
            datacontext.SaveChanges();
            return NoContent();
        }
    }
}
