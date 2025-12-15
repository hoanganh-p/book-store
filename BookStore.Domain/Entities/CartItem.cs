using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Domain.Entities
{
    [Table("CartItem")]
    public class CartItem
    {
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int BookID { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;

        public Cart Cart { get; set; }
        public Book Book { get; set; }  
    }
}
