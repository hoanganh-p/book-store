using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Domain.Entities
{
    [Table("Book")]
    public class Book
    {
        [Key]
        public int BookID { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Author { get; set; }

        public required string Publisher { get; set; }

        [RegularExpression(@"^\d{13}$", ErrorMessage = "Invalid ISBN number.")]
        public required string ISBN { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public int CategoryID { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string? ImageURL { get; set; }



        //public Category? Category { get; set; }
    }
}
