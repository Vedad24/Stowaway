using Market.API.Authorization;
using Market.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Stowaway.Application.Modules.Identity.Users.Commands.Create;
using Stowaway.Application.Modules.Identity.Users.Commands.Delete;
using Stowaway.Application.Modules.Identity.Users.Commands.DeleteSelf;
using Stowaway.Application.Modules.Identity.Users.Commands.Update;
using Stowaway.Application.Modules.Identity.Users.Commands.UpdateSelf;
using Stowaway.Application.Modules.Identity.Users.Queries.GetById;
using Stowaway.Application.Modules.Identity.Users.Queries.GetByMail;
using Stowaway.Application.Modules.Identity.Users.Queries.GetSelf;
using Stowaway.Application.Modules.Identity.Users.Queries.List;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
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
        [HasPermission(Permissions.UsersRead)]
        public async Task<GetByIdQueryDto> GetById(int id, CancellationToken ct)
        {
            return await sender.Send(new GetByIdQuery { Id = id }, ct);
        }

        [HttpPut]
        [HasPermission(Permissions.UsersUpdate)]
        public async Task<UpdateUserCommandDto> UpdateUser([FromBody] UpdateUserCommand command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }

        [HttpGet]
        [HasPermission(Permissions.UsersRead)]
        public async Task<PageResult<ListUserQueryDto>> ListUsers([FromQuery] ListUserQuery command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.UsersDelete)]
        public async Task DeleteUser(int id, CancellationToken ct)
        {
            await sender.Send(new DeleteUserCommand { Id = id }, ct);
        }

        //userByMail
        [HttpGet("mail/{mail}")]
        [HasPermission(Permissions.UsersRead)]
        public async Task<GetByMailQueryDto> GetByMail(string mail, CancellationToken ct)
        {
            return await sender.Send(new GetByMailQuery { Mail = mail }, ct);
        }


        //User-Self commands
        [HttpGet("me")]
        [HasPermission(Permissions.UsersSelfRead)]
        public async Task<GetSelfQueryDto> GetSelf(CancellationToken ct)
        {
            return await sender.Send(new GetSelfQuery(), ct);
        }

        [HttpPut("me")]
        [HasPermission(Permissions.UsersSelfUpdate)]
        public async Task<UpdateSelfCommandDto> UpdateSelf([FromBody] UpdateSelfCommand command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }

        [HttpDelete("me")]
        [HasPermission(Permissions.UsersSelfDelete)]
        public async Task DeleteSelf(CancellationToken ct)
        {
            await sender.Send(new DeleteSelfCommand(), ct);
        }
    }
}
