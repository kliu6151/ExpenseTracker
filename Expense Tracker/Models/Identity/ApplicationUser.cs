using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {

        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "First Name is required")]
        public String FirstName { get; set; }
        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        public String LastName { get; set; } = String.Empty;

        public virtual ICollection<Category>? Categories { get; set; }
        public virtual ICollection<Transaction>? Transactions { get; set; }



    }
}
