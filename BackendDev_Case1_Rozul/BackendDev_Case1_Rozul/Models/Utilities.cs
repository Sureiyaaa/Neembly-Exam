using BackendDev_Case1_Rozul.Database;
using BackendDev_Case1_Rozul.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendDev_Case1_Rozul.Models
{
    public class DBUtility
    {
        #region GetUpdatedProduct
        public static ProductDTO? GetUpdatedProduct(int productID){
            using(var context = new ApplicationDbContext()){
                return context.Products
                                .AsNoTracking()
                                .Include(p => p.Categories)
                                    .ThenInclude(c => c.CategoryType)
                                .Select(p => new ProductDTO(p))
                                .ToList()
                                .FirstOrDefault(p => p.ProductID == productID);
            }
        }

        public static Product? GetProduct(int productID){
            using(var context = new ApplicationDbContext()){
                return context.Products
                                .AsNoTracking()
                                .Where(p => p.ProductID == productID)
                                .Include(p => p.Categories)
                                    .ThenInclude(c => c.CategoryType)
                                .ToList()
                                .FirstOrDefault();
            }
        }
        #endregion
        #region GetUpdatedCategory
        public static CategoryTypeDTO? GetUpdatedCategory(int categoryID){
            using(var context = new ApplicationDbContext()){
                return context.CategoryTypes
                                .AsNoTracking()
                                .Select(p => new CategoryTypeDTO(p))
                                .ToList()
                                .FirstOrDefault(p => p.CategoryTypeID == categoryID);
            }
        }
        #endregion

        #region GetUpdatedOrder
        public static OrderDTO? GetUpdatedOrder(int orderID){
            using(var context = new ApplicationDbContext()){
                return context.Orders
                                .AsNoTracking()
                                .Where(p => p.OrderID == orderID)
                                .Include(o => o.OrderItems)
                                    .ThenInclude(o => o.Product)
                                .Select(o => new OrderDTO(o))
                                .ToList()
                                .FirstOrDefault();
            }
        }
        #endregion
    }
    
}