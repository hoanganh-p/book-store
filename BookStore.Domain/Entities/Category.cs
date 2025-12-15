using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BookStore.Domain.Entities
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }      
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } 
        [StringLength(255)]
        public string Description { get; set; }  

        //public ICollection<Book>? Books { get; set; } 
    }
}
