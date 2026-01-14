using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Stowaway.Application.Modules.Identity.Commands.Create;
using Stowaway.Application.Modules.Identity.Commands.Delete;
using Stowaway.Application.Modules.Identity.Commands.Update;
using Stowaway.Application.Modules.Identity.Queries.GetById;
using Stowaway.Application.Modules.Identity.Queries.List;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous] //delete later
    public class UserController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<int>> CreateUser(CreateUserCommand command, CancellationToken ct)
        {
            int id = await sender.Send(command, ct);
            return id;
        }
        [HttpGet("{id:int}")]
        public async Task<GetByIdQueryDto> GetById(int id, CancellationToken ct)
        {
            return await sender.Send( new GetByIdQuery {Id = id }, ct);
        }
        [HttpPut]
        public async Task<UpdateUserCommandDto> UpdateUser([FromBody] UpdateUserCommand command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }

        [HttpGet]
        public async Task<PageResult<ListUserQueryDto>> ListUsers([FromQuery] ListUserQuery command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }
        [HttpDelete("{id:int}")]
        public async Task DeleteUser(int id, CancellationToken ct)
        {
            await sender.Send(new DeleteUserCommand { Id = id }, ct);
        }
    }
}
