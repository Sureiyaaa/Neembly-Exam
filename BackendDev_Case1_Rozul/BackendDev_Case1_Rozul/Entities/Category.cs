
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendDev_Case1_Rozul.Entities{
    [Table("tblCategory")]
    public class Category {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        [Required]
        public required int CategoryTypeID { get; set; }

        [ForeignKey("CategoryTypeID")] 
        public CategoryType? CategoryType { get; set; }

        [Required]
        public required int ProductID { get; set;}
        
        [ForeignKey("ProductID")]
        public Product? Product { get ; set; }

    }

    public class CategoryDTO {
        public int CategoryID { get; set; }
        public ProductDTO? Product { get; set; }
        public CategoryTypeDTO? Category { get; set; }
        
        public CategoryDTO(Category category){
            CategoryID = category.CategoryID;

            // Null safety checks for nullable data
            Product = category.Product != null ? new ProductDTO(category.Product) : null;
            Category = category.CategoryType != null ? new CategoryTypeDTO(category.CategoryType) : null;
        }
    }
}