using LoginApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        private readonly ILogger<ProductsController> _logger;

        public ProductsController(MongoDbService mongoDbService, ILogger<ProductsController> logger)
        {

            _mongoDbService = mongoDbService ?? throw new ArgumentNullException(nameof(mongoDbService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Product>>> GetAsyncProduct(string id)
        {
            try
            {
                var login = await _mongoDbService.GetAsyncProduct(id);

                if (login == null)
                {
                    return NotFound(new ApiResponse<Product>
                    {
                        Success = false,
                        Message = $"User with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<Product>
                {
                    Success = true,
                    Message = "Product found successfully",
                    Data = login
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting login by ID: {id}");
                return StatusCode(500, new ApiResponse<Product>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                });
            }
        }


        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(ProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CreatedAt = DateTime.UtcNow
            };

            await _mongoDbService.createProduct(product);

            return CreatedAtAction(nameof(GetAsyncProduct), new { id = product.Id },
                    new ApiResponse<Product>
                    {
                        Success = true,
                        Message = "Product Added to the Cart",
                        Data = product
                    });
        }

    }
}