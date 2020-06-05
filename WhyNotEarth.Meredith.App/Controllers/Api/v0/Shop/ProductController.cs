using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Shop;
using WhyNotEarth.Meredith.App.Results.Api.v0.Shop;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Shop
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
        [HttpPost("")]
        public async Task<ActionResult<ShopProductResult>> Create(ProductModel model)
        {
            var variations = model.Variations.Select(item => new Variation {Name = item.Name}).ToList();
            
            var productLocationInventories = model.ProductLocationInventories.Select(item => new ProductLocationInventory
            {
                LocationId = item.LocationId,
                Count = item.Count
            }).ToList();

            var product = await _productService.CreateAsync(model.PageId, model.PriceId, model.CategoryId, 
                variations, productLocationInventories);

            return Ok(new ShopProductResult(product));
        }

        [Returns204]
        [Returns404]
        [HttpPut("{productId}")]
        public async Task<IActionResult> Edit(int productId, ProductModel model)
        {
            var variations = model.Variations.Select(item =>
                new Variation
                {
                    Id = item.Id,
                    Name = item.Name
                }).ToList();
            
            var productLocationInventories = model.ProductLocationInventories.Select(item =>
                new ProductLocationInventory
                {
                    Id = item.Id,
                    LocationId = item.LocationId,
                    Count = item.Count
                }).ToList();

            await _productService.EditAsync(productId, model.PageId, model.PriceId, model.CategoryId, 
                variations, productLocationInventories);

            return NoContent();
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
        public async Task<ActionResult<ShopProductResult>> Get(int productId)
        {
            var product = await _productService.GetAsync(productId);

            return Ok(new ShopProductResult(product));
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<ShopProductResult>>> List()
        {
            var products = await _productService.ListAsync();

            return Ok(products.Select(item => new ShopProductResult(item)).ToList());
        }
    }
}
