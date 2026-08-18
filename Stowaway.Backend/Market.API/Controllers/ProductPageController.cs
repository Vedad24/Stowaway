using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stowaway.Application.Modules.Sales.ProductPage.ListWarhouses;
using Stowaway.Application.Modules.Sales.ProductPage.ListContainerTypes;


namespace Market.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductPageController(ISender sender) : ControllerBase
    {
        [HttpGet("get-user-warehouses")]
        public async Task<PageResult<ListWarehousesQueryDto>> GetUserWarehouses([FromQuery] ListWarehousesQuery query, CancellationToken ct)
        {
            return await sender.Send(query, ct);
        }

        [HttpGet("get-container-types")]
        [AllowAnonymous] //for server-side rendering
        public async Task<PageResult<ListContainerTypeQueryDto>> GetContainerTypes([FromQuery] ListContainerTypeQuery query, CancellationToken ct)
        {
            return await sender.Send(query, ct);
        }
    }
}