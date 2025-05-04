using System.Threading.Tasks;
using BackendDev_Case1_Rozul.Database;
using BackendDev_Case1_Rozul.Entities;
using BackendDev_Case1_Rozul.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendDev_Case1_Rozul.Controllers
{

    [Route("api/")]
    public class CategoryController : Controller
    {
        #region  GetCategories
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            using(var context = new ApplicationDbContext()){
                try{
                    List<CategoryTypeDTO> categories = context.CategoryTypes.Select(c => new CategoryTypeDTO(c)).ToList();

                    return Ok(
                        new {
                            status = 200,
                            message = "Retrieved categories successfully.",
                            body = new 
                            {
                                categories
                            }
                        }
                    );
                }catch(Exception ex){
                    return ErrorStatusCode(ex);
                }
            }
        }
        #endregion

        #region  GetCategory
        [HttpGet("categories/{id}")]
        public IActionResult GetCategory(int id)
        {
            using(var context = new ApplicationDbContext()){
                try{
                    CategoryType? category = context.CategoryTypes.FirstOrDefault(p => p.CategoryTypeID.Equals(id));

                    if(category != null){
                            
                        return Ok(
                            new {
                                status = 200,
                                message = "Retrieved Category successfully.",
                                body = new 
                                {
                                    category = new CategoryTypeDTO(category)
                                }
                            }
                        );
                    }

                    return NotFound(new 
                    { 
                        status = 404, 
                        message = "Category does not exist!"
                    });

                }catch(Exception ex){
                    return ErrorStatusCode(ex);
                }
            }
        }
        #endregion

        #region CreateCategory
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryTypePostRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequestCode("Invalid client request", errors);
            }

            using(var context = new ApplicationDbContext()){
                try{
                    // Instantiate Category
                    CategoryType category = new CategoryType{
                        Name = request.Name!,
                        Description = request.Description
                    };
                    
                    // Save Changes
                    context.CategoryTypes.Add(category);
                    await context.SaveChangesAsync();

                    return Ok(
                        new {
                            status = 200,
                            message = "Category added successfully.",
                            body = new 
                            {
                                category = DBUtility.GetUpdatedCategory(category.CategoryTypeID)
                            }
                        }
                    );
                }catch(Exception ex){
                    return ErrorStatusCode(ex);
                }
            }
        }
        #endregion

        #region  UpdateCategory
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryTypePostRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequestCode("Invalid client request", errors);
            }
            using(var context = new ApplicationDbContext()){
                try{
                    CategoryType? category = context.CategoryTypes.FirstOrDefault(p => p.CategoryTypeID.Equals(id));

                    if(category != null){
                        category.Name = request.Name!;
                        category.Description = request.Description;
                        await context.SaveChangesAsync();


                        return Ok(
                            new {
                                status = 200,
                                message = "Category updated successfully.",
                                body = new 
                                {
                                    category = DBUtility.GetUpdatedCategory(category.CategoryTypeID)
                                }
                            }
                        );
                    }

                    return NotFound(new 
                    { 
                        status = 404, 
                        message = "Category does not exist!"
                    });

                }catch(Exception ex){
                    return ErrorStatusCode(ex);
                }
            }
        }
        #endregion

        #region DeleteCategory
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            using var context = new ApplicationDbContext();

            CategoryType? category = context.CategoryTypes.FirstOrDefault(p => p.CategoryTypeID == id);
            if (category == null)    
                return NotFound(new { status = 404, message = "Category not found" });
            foreach(Category c in context.Category.Where(c => c.CategoryTypeID == category.CategoryTypeID).ToList()){
                context.Category.Remove(c);
            }
            context.CategoryTypes.Remove(category);
            await context.SaveChangesAsync();

            return Ok(
                new {
                    status = 200,
                    message = "Category deleted.",
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