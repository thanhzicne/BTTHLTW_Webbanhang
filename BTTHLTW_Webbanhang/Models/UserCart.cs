using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTTHLTW_Webbanhang.Models
{
    public class UserCart
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
