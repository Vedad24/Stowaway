using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Stowaway.Application.Modules.Storage.Items.Commands.Create;
using Stowaway.Application.Modules.Storage.Items.Commands.Delete;
using Stowaway.Application.Modules.Storage.Items.Commands.Move;
using Stowaway.Application.Modules.Storage.Items.Commands.Update;
using Stowaway.Application.Modules.Storage.Items.Commands.UpdateCanvasPosition;
using Stowaway.Application.Modules.Storage.Items.Queries.GetById;
using Stowaway.Application.Modules.Storage.Items.Queries.List;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<PageResult<ListItemQueryDto>> List([FromQuery] ListItemQuery query, CancellationToken cancellationToken)
        {
            var result = await sender.Send(query, cancellationToken);
            return result;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<GetItemByIdQueryDto> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetItemByIdQuery { Id = id },cancellationToken);
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<int>> Create(CreateItemCommand payload, CancellationToken cancellationToken)
        {
            int id = await sender.Send(payload, cancellationToken);
            return CreatedAtAction(nameof(GetById), new {id}, new {id});
        }

        [HttpDelete("{id:int}")]
        [AllowAnonymous]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteItemCommand { Id = id},cancellationToken);
        }

        [HttpPut("{id:int}")]
        [AllowAnonymous]
        public async Task Update(int id, UpdateItemCommand payload, CancellationToken cancellationToken)
        {
            payload.Id = id;
            await sender.Send(payload, cancellationToken);
        }

        [HttpPut("{id:int}/canvas-position")]
        [AllowAnonymous]
        public async Task UpdateCanvasPosition(int id, UpdateItemCanvasPositionCommand payload, CancellationToken cancellationToken)
        {
            payload.Id = id;
            await sender.Send(payload, cancellationToken);
        }

        [HttpPut("{id:int}/container")]
        [AllowAnonymous]
        public async Task Move(int id, MoveItemCommand payload, CancellationToken cancellationToken)
        {
            payload.Id = id;
            await sender.Send(payload, cancellationToken);
        }
    }
}
