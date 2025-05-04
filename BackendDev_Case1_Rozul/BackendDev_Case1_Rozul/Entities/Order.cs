
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendDev_Case1_Rozul.Entities{
    [Table("tblOrder")]
    public class Order {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        [Required]
        public required string? CustomerName { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderDTO {

        public int OrderID { get; set;}
        public string? CustomerName {get; set;}
        public DateTime OrderDate {get; set;}
        public List<OrderItemDTO> Items { get; set; } = new();
        public OrderDTO(Order order){
            OrderID = order.OrderID;
            CustomerName = order.CustomerName;
            OrderDate = order.OrderDate;

            // Convert all order item to OrderDTO
            foreach(OrderItem orderItem in order.OrderItems){
                Items.Add(new OrderItemDTO(orderItem));
            }
        }
    }
}