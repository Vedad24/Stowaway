using Stowaway.Application.Modules.Storage.Items.Queries.GetById;
using Stowaway.Application.Modules.Storage.Items.Queries.List;
using Stowaway.Application.Modules.Storage.Supplier.Commands.Create;
using Stowaway.Application.Modules.Storage.Supplier.Commands.Delete;
using Stowaway.Application.Modules.Storage.Supplier.Commands.Update;
using Stowaway.Application.Modules.Storage.Supplier.Queries.GetById;
using Stowaway.Application.Modules.Storage.Supplier.Queries.List;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SupplierController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<PageResult<ListSupplierQueryDto>> List([FromQuery] ListSupplierQuery query, CancellationToken cancellationToken)
        {
            var result = await sender.Send(query, cancellationToken);
            return result;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<GetSupplierByIdQueryDto> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetSupplierByIdQuery { Id = id }, cancellationToken);
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<int>> Create(CreateSupplierCommand command, CancellationToken cancellationToken)
        {
            int id = await sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpDelete("{id:int}")]
        [AllowAnonymous]
        public async Task<Unit> Delete(int id, CancellationToken cancellationToken)
        {
            return await sender.Send(new DeleteSupplierCommand { Id = id }, cancellationToken);

        }

        [HttpPut("{id:int}")]
        [AllowAnonymous]
        public async Task Update(int id, UpdateSupplierCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            await sender.Send(command, cancellationToken);
        }
    }
}
