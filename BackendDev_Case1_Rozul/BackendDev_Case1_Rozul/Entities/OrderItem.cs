
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendDev_Case1_Rozul.Entities{
    [Table("tblOrderItem")]
    public class OrderItem {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemID { get; set; }

        [Required]
        public required int OrderID { get; set;} 

        [ForeignKey("OrderID")] 
        public Order? Order { get; set; }

        [Required]
        public required int ProductID { get; set;} 

        [ForeignKey("ProductID")] 
        public Product? Product { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

    public class OrderItemDTO {

        public int OrderItemID { get; set;}
        public ProductDTO? Product { get; set;}
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public OrderItemDTO(OrderItem order){
            OrderItemID = order.OrderItemID;
            Quantity = order.Quantity;

            // Null safety checks for nullable data
            Product = order.Product != null ? new ProductDTO(order.Product) : null;
            // Calculate UnitPrice
            UnitPrice = order.Product != null ? Product!.Price * Quantity : 0;
        }
    }
}