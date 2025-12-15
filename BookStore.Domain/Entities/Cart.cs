using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Domain.Entities
{
    [Table("Cart")]
    public class Cart
    {
        public int CartID { get; set; }
        public string UserID { get; set; }
        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    }
}
