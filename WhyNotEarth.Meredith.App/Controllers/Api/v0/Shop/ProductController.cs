using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Shop
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/shop/products")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.Developer)]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [Returns200]
        [Returns404]
        [HttpPost("")]
        public async Task<IActionResult> Create(ProductModel model)
        {
            await _productService.CreateAsync(model.PageId, model.PriceId,
                model
                .Variations
                .Select(item =>
                    new Variation
                    {
                        Id = item.Id,
                        Name = item.Name
                    })
                .ToList(),
                model
                .ProductLocationInventories
                .Select(item =>
                    new ProductLocationInventory
                    {
                        Id = item.Id,
                        LocationId = item.LocationId,
                        Count = item.Count
                    })
                .ToList());

            return Ok();
        }

        [Returns204]
        [Returns404]
        [HttpPut("{productId}")]
        public async Task<IActionResult> Edit(int productId, ProductModel model)
        {
            await _productService.EditAsync(
                productId,
                model.PageId,
                model.PriceId,
                model
                .Variations
                .Select(item =>
                    new Variation
                    {
                        Id = item.Id,
                        Name = item.Name
                    })
                .ToList(),
                model
                .ProductLocationInventories
                .Select(item =>
                    new ProductLocationInventory
                    {
                        Id = item.Id,
                        LocationId = item.LocationId,
                        Count = item.Count
                    })
                .ToList());

            return Ok();
        }

        [Returns204]
        [Returns404]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            await _productService.DeleteAsync(productId);

            return NoContent();
        }

        [Returns200]
        [Returns404]
        [HttpGet("{productId}")]
        public async Task<IActionResult> Get(int productId)
        {
            var product = await _productService.GetAsync(productId);

            return Ok(new ProductModel(product));
        }
    }
}
