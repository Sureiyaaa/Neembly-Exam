using System.Threading.Tasks;
using BackendDev_Case1_Rozul.Database;
using BackendDev_Case1_Rozul.Entities;
using BackendDev_Case1_Rozul.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendDev_Case1_Rozul.Controllers
{

    [Route("api/")]
    public class ProductController : Controller
    {
        #region  GetProducts
        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            using(var context = new ApplicationDbContext()){
                try{
                    List<ProductDTO> products = context.Products
                                            .AsNoTracking()
                                            .Include(p => p.Categories)
                                                .ThenInclude(c => c.CategoryType)
                                            .Select(p => new ProductDTO(p)).ToList();

                    return Ok(
                        new {
                            status = 200,
                            message = "Retrieved products successfully.",
                            body = new 
                            {
                                products
                            }
                        }
                    );
                }catch(Exception ex){
                    return ErrorStatusCode(ex);
                }
            }
        }
        #endregion

        #region  GetProductByID
        [HttpGet("products/{id}")]
        public IActionResult GetProductByID(int id)
        {
            using(var context = new ApplicationDbContext()){
                try{
                    Product? product = context.Products
                                            .AsNoTracking()
                                            .Where(p => p.ProductID.Equals(id))
                                            .Include(p => p.Categories)
                                                .ThenInclude(c => c.CategoryType)
                                            .FirstOrDefault();

                    if(product != null){
                            
                        return Ok(
                            new {
                                status = 200,
                                message = "Retrieved product successfully.",
                                body = new 
                                {
                                    product = new ProductDTO(product)
                                }
                            }
                        );
                    }

                    return NotFound(new 
                    { 
                        status = 404, 
                        message = "Product does not exist!"
                    });

                }catch(Exception ex){
                    return ErrorStatusCode(ex);
                }
            }
        }
        #endregion

        #region CreateProduct
        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductPostRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequestCode("Invalid client request", errors);
            }

            using(var context = new ApplicationDbContext()){
                try{
                    // Instantiate Product
                    Product product = new Product{
                        Name = request.Name,
                        Description = request.Description,
                        Price = request.Price ?? 0,
                        StockQuantity = request.StockQuantity
                    };
                    
                    // Save Changes
                    context.Products.Add(product);
                    await context.SaveChangesAsync();
                      // Validate List of categories
                    var validCategoryTypeIds = context.CategoryTypes
                                                        .AsNoTracking()
                                                        .Select(ct => ct.CategoryTypeID)
                                                        .ToHashSet();
                                                            
                    foreach(int categoryID in request.CategoryIDs){
                        if (!validCategoryTypeIds.Contains(categoryID))
                        {
                                return BadRequestCode($"CategoryTypeID {categoryID} does not exist.", []);
                        }
                        context.Category.Add(new Category{
                            CategoryTypeID = categoryID,
                            ProductID = product.ProductID,
                        });
                    }

                    await context.SaveChangesAsync();
                    return Ok(
                        new {
                            status = 200,
                            message = "Product added successfully.",
                            body = new 
                            {
                                product = DBUtility.GetUpdatedProduct(product.ProductID)
                            }
                        }
                    );
                }catch(Exception ex){
                    return ErrorStatusCode(ex);
                }
            }
        }
        #endregion

        #region  UpdateProduct
        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductPostRequest request)
        {
            using(var context = new ApplicationDbContext()){
                try{
                    Product? product = context.Products
                                            .Where(p => p.ProductID.Equals(id))
                                            .Include(p => p.Categories)
                                                .ThenInclude(c => c.CategoryType)
                                            .FirstOrDefault();

                    if(product != null){
                        // Check changes from the request
                            product.Name = request.Name;
                            product.Description = request.Description;
                            product.StockQuantity = request.StockQuantity;
                            product.Price = request.Price ?? 0;

                        // Validate List of categories
                        var validCategoryTypeIds = context.CategoryTypes
                                                            .AsNoTracking()
                                                            .Select(ct => ct.CategoryTypeID)
                                                            .ToHashSet();

                        foreach(Category category in product.Categories){
                            context.Category.Remove(category);
                        }
                        foreach(int categoryID in request.CategoryIDs){
                            if (!validCategoryTypeIds.Contains(categoryID))
                            {
                                return BadRequestCode($"CategoryTypeID {categoryID} does not exist.", []);
                            }
                            context.Category.Add(new Category{
                                    ProductID = product.ProductID,
                                    CategoryTypeID = categoryID,
                            });
                        }
                        await context.SaveChangesAsync();
                        ProductDTO? productDTO = DBUtility.GetUpdatedProduct(product.ProductID);
                        return Ok(
                            new {
                                status = 200,
                                message = "Product updated successfully.",
                                body = new 
                                {
                                    product = productDTO
                                }
                            }
                        );
                    }

                    return NotFound(new 
                    { 
                        status = 404, 
                        message = "Product does not exist!"
                    });

                }catch(Exception ex){
                    Console.WriteLine(ex);
                    return ErrorStatusCode(ex);
                }
            }
        }
        #endregion

        #region DeleteProduct
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            using var context = new ApplicationDbContext();

            Product? product = context.Products.AsNoTracking().FirstOrDefault(p => p.ProductID == id);
            if (product == null)
                return NotFound(new { status = 404, message = "Product not found" });
            product.IsDeleted = true;
            await context.SaveChangesAsync();

            return Ok(
                new {
                    status = 200,
                    message = "Product deleted.",
                    body = new 
                        {
                            product = DBUtility.GetUpdatedProduct(product.ProductID)
                        }
                    }
                );
        }
        #endregion
        
        ObjectResult BadRequestCode(string message, List<string> errors){
            return BadRequest(new { status = 400, message, errors });
        }
        ObjectResult ErrorStatusCode(Exception ex){
            return StatusCode(500, new
                    {
                        status = 500,
                        message = "An error occurred.",
                        error = ex.Message
                    });
        }
    }
}