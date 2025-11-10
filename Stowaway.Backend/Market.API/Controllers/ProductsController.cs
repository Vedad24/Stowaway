using Market.Application.Common;
using Market.Application.Modules.Catalog.Products.Commands.Create;
using Market.Application.Modules.Catalog.Products.Queries.GetProductById;
using Market.Application.Modules.Catalog.Products.Queries.GetProductsByCategory;
using Market.Application.Modules.Catalog.Products.Queries.GetProductsByName;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Market.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(ISender sender) : ControllerBase
    {
        

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<ActionResult<int>> CreateProduct([FromBody] CreateProductCommand request, CancellationToken ct)
        {
            int id = await sender.Send(request, ct);
            //vraća 201, kako i treba
            return CreatedAtAction(nameof(CreateProduct), new { id }, new { id });
        }

        [HttpGet("/ByCategory")]
        public async Task<PageResult<GetProductsByCategoryQueryDto>> GetProductsByCategory([FromQuery]GetProductsByCategoryQuery request, CancellationToken ct)
        {
            var result = await sender.Send(request,ct);
            return result;
        }

        [HttpGet("/ByName")]
        public async Task<PageResult<GetProductsByNameQueryDto>> GetProductsByName([FromQuery] GetProductsByNameQuery request, CancellationToken ct)
        {
            var result = await sender.Send(request, ct);
            return result;
        }

        [HttpGet("/ById")]
        public async Task<GetProductByIdQueryDto> GetProductsByName([FromQuery] GetProductByIdQuery request, CancellationToken ct)
        {
            var result = await sender.Send(request, ct);
            return result;
        }
    }
}
