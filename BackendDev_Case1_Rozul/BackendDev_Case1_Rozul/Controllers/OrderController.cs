using System.Threading.Tasks;
using BackendDev_Case1_Rozul.Database;
using BackendDev_Case1_Rozul.Entities;
using BackendDev_Case1_Rozul.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BackendDev_Case1_Rozul.Controllers
{

    [Route("api/")]
    public class OrderController : Controller
    {
        #region  GetOrder
        [HttpGet("orders")]
        public IActionResult GetOrders()
        {
            using(var context = new ApplicationDbContext()){
                try{
                    List<OrderDTO> orders = context.Orders
                                            .AsNoTracking()
                                            .Include(p => p.OrderItems)
                                                .ThenInclude(c => c.Product)
                                                    .ThenInclude(p => p!.Categories)
                                                        .ThenInclude(p => p.CategoryType)
                                            .Select(p => new OrderDTO(p)).ToList();

                    return Ok(
                        new {
                            status = 200,
                            message = "Retrieved orders successfully.",
                            body = new 
                            {
                                orders
                            }
                        }
                    );
                }catch(Exception ex){
                    return ErrorStatusCode(ex);
                }
            }
        }
        #endregion

        #region  GetOrdersLastMonth
        [HttpGet("orders/lastmonth")]
        public IActionResult GetOrdersLastMonth()
        {
            using(var context = new ApplicationDbContext()){
                try{
                    DateTime pastMonth = DateTime.Now.AddMonths(-1);
                    
                    List<OrderDTO> orders = context.Orders
                                            .AsNoTracking()
                                            .Where(o => o.OrderDate < pastMonth)
                                            .Include(p => p.OrderItems)
                                                .ThenInclude(c => c.Product)
                                                    .ThenInclude(p => p!.Categories)
                                                        .ThenInclude(p => p.CategoryType)
                                            .Select(p => new OrderDTO(p)).ToList();

                    return Ok(
                        new {
                            status = 200,
                            message = "Retrieved orders last month successfully.",
                            body = new 
                            {
                                orders
                            }
                        }
                    );
                }catch(Exception ex){
                    return ErrorStatusCode(ex);
                }
            }
        }
        #endregion

        #region  GetOrderByID
        [HttpGet("orders/{id}")]
        public IActionResult GetOrderByID(int id)
        {
            using(var context = new ApplicationDbContext()){
                try{
                    Order? order = context.Orders
                                            .AsNoTracking()
                                            .Where(p => p.OrderID.Equals(id))
                                            .Include(p => p.OrderItems)
                                                .ThenInclude(c => c.Product)
                                                    .ThenInclude(p => p!.Categories)
                                                        .ThenInclude(p => p.CategoryType)
                                            .FirstOrDefault();

                    if(order != null){
                            
                        return Ok(
                            new {
                                status = 200,
                                message = "Retrieved order successfully.",
                                body = new 
                                {
                                    order = new OrderDTO(order)
                                }
                            }
                        );
                    }

                    return NotFound(new 
                    { 
                        status = 404, 
                        message = "Order does not exist!"
                    });

                }catch(Exception ex){
                    return ErrorStatusCode(ex);
                }
            }
        }
        #endregion

        #region CreateOrder
        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderPostRequest request)
        {
            if (request == null || !ModelState.IsValid || request.OrderItems.IsNullOrEmpty())
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequestCode("Invalid client request", errors);
            }

            using var context = new ApplicationDbContext();

            try
            {
                var validProductIds = context.Products
                                            .AsNoTracking()
                                            .Select(p => p.ProductID)
                                            .ToHashSet();

                // Validate product IDs before creating the order
                foreach (var item in request.OrderItems!)
                {
                    if (!validProductIds.Contains(item.ProductID ?? -1))
                    {
                        return BadRequestCode($"Product ID {item.ProductID} does not exist.", []);
                    }
                }

                var order = new Order
                {
                    CustomerName = request.CustomerName!,
                };
                context.Orders.Add(order);
                await context.SaveChangesAsync();

                foreach (var item in request.OrderItems)
                {
                    Product? product = DBUtility.GetProduct(item.ProductID!.Value);
                    if(product!.StockQuantity < item.Quantity!.Value) return BadRequestCode($"Product ID {item.ProductID} currently has {product.StockQuantity}. You requested {item.Quantity.Value}", []);
                    context.OrderItems.Add(new OrderItem
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID!.Value,
                        Quantity = item.Quantity!.Value
                    });
                    context.Products.FirstOrDefault(p => p.ProductID == item.ProductID!.Value)!.StockQuantity -= item.Quantity!.Value;
                }

                await context.SaveChangesAsync();

                return Ok(new
                {
                    status = 200,
                    message = "Order added successfully.",
                    body = new
                    {
                        order = DBUtility.GetUpdatedOrder(order.OrderID)
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return ErrorStatusCode(ex);
            }
        }

        #endregion

        #region UpdateOrder
        [HttpPut("orders/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderPostRequest request)
        {
            if (request == null || !ModelState.IsValid || request.OrderItems.IsNullOrEmpty())
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequestCode("Invalid client request", errors);
            }

            using var context = new ApplicationDbContext();

            try
            {
                var order = context.Orders
                                .Include(o => o.OrderItems)
                                .FirstOrDefault(o => o.OrderID == id);

                if (order == null)
                {
                    return NotFound(new { status = 404, message = "Order not found" });
                }

                order.CustomerName = request.CustomerName!;

                var validProductIds = context.Products
                                            .AsNoTracking()
                                            .Select(p => p.ProductID)
                                            .ToHashSet();

                foreach (var item in request.OrderItems!)
                {
                    if (!validProductIds.Contains(item.ProductID ?? -1))
                    {
                        return BadRequestCode($"Product ID {item.ProductID} does not exist.", []);
                    }
                }

                // Remove existing order items
                context.OrderItems.RemoveRange(order.OrderItems);

                foreach (var item in request.OrderItems)
                {
                    Product? product = DBUtility.GetProduct(item.ProductID!.Value);
                    if (product!.StockQuantity < item.Quantity!.Value)
                        return BadRequestCode($"Product ID {item.ProductID} currently has {product.StockQuantity}. You requested {item.Quantity.Value}", []);

                    context.OrderItems.Add(new OrderItem
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID!.Value,
                        Quantity = item.Quantity!.Value
                    });

                    context.Products.FirstOrDefault(p => p.ProductID == item.ProductID!.Value)!.StockQuantity -= item.Quantity!.Value;
                }

                await context.SaveChangesAsync();

                return Ok(new
                {
                    status = 200,
                    message = "Order updated successfully.",
                    body = new
                    {
                        order = DBUtility.GetUpdatedOrder(order.OrderID)
                    }
                });
            }
            catch (Exception ex)
            {
                return ErrorStatusCode(ex);
            }
        }
        #endregion


        #region DeleteOrder
        [HttpDelete("orders/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            using var context = new ApplicationDbContext();

            Order? order = context.Orders.AsNoTracking().FirstOrDefault(p => p.OrderID == id);
            if (order == null)
                return NotFound(new { status = 404, message = "Order not found" });
            order.IsDeleted = true;
            await context.SaveChangesAsync();

            return Ok(
                new {
                    status = 200,
                    message = "Order deleted.",
                    body = new 
                        {
                            product = DBUtility.GetUpdatedOrder(order.OrderID)
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