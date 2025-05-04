using System.ComponentModel.DataAnnotations;

namespace BackendDev_Case1_Rozul.Models 
{
    public class ProductPostRequest{
        [Required(ErrorMessage = "Product Name is Required")]
        public string? Name { get; set; }
             
        [Required(ErrorMessage = "Product Price is Required")]
        public decimal? Price { get; set;}
        public string? Description { get; set; } = "No description provided for this product";
        public List<int> CategoryIDs { get; set;} = new();
        public int StockQuantity { get; set; } = 0;
    }
    public class CategoryTypePostRequest{
        [Required(ErrorMessage = "Category Name is required")]
        public string? Name { get; set; }        
        public string? Description { get; set; } = "No description provided for this product";
    }

    public class OrderPostRequest{
        [Required(ErrorMessage = "Customer Name is required")]
        public string? CustomerName { get; set; } 
        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<OrderItemPostRequest>? OrderItems { get; set; }
    }

    public class OrderItemPostRequest{
        [Required(ErrorMessage = "Product ID is required")]
        public int? ProductID { get; set; }
        [Required(ErrorMessage = "Product Quantity is required")]
        public int? Quantity { get; set; }
    }
    public class ProductPutrequest{
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set;} = 0;
        public int StockQuantity { get; set; } = 0;
    }
}