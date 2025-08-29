using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymBro.API.Data;

namespace GymBro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController:ControllerBase
    {
        private readonly ApplicationDbContext _context; 

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
    }
}
