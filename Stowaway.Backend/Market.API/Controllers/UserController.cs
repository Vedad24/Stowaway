using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Stowaway.Application.Modules.Identity.Commands.Create;
using Stowaway.Application.Modules.Identity.Queries.GetById;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<int>> CreateUser(CreateUserCommand command, CancellationToken ct)
        {
            int id = await sender.Send(command, ct);
            return id;
        }
        [HttpGet]
        public async Task<GetByIdQueryDto> GetById([FromQuery]GetByIdQuery command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }
        
    }
}
