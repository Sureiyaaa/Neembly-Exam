
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendDev_Case1_Rozul.Entities{
    [Table("tblCategoryType")]
    public class CategoryType {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryTypeID { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public string? Description { get; set; } = "No description provided for this Category"; 

        public ICollection<Category> Categories { get; set;} = new List<Category>();
    }

    public class CategoryTypeDTO{
        public int CategoryTypeID { get; set;}
        public string? Name { get; set; }
        public string? Description { get; set; }

        public CategoryTypeDTO (CategoryType categoryType){
            CategoryTypeID = categoryType.CategoryTypeID;
            Name = categoryType.Name;
            Description = categoryType.Description;
        }
    }
}