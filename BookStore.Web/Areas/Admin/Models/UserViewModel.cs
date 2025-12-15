namespace BookStore.Web.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; } 
        public string LockoutStatus { get; set; }  
    }
}