
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendDev_Case1_Rozul.Entities{
    [Table("tblProduct")]
    public class Product {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        [Required]
        public string? Name { get; set; } = "Unnamed Product";

        [Required]
        public string? Description { get; set; } = "No description provided for this product";
        
        [Required]
        public decimal Price { get; set;} = 0;

        public int StockQuantity {get;set;} = 0;
 
        public ICollection<Category> Categories {get;set;} = new List<Category>();
    }

    public class ProductDTO {

        public int ProductID { get; set;}
        public string? Name {get; set;} = "Unnamed Product";
        public string? Description {get; set;} = "";
        public decimal Price { get; set; } = 0;
        public int StockQuantity { get; set; } = 0;

        public ProductDTO(Product product){
            ProductID = product.ProductID;
            Name = product.Name;
            Description = product.Description;
            Price = product.Price;
            StockQuantity = product.StockQuantity;
        }
    }
}