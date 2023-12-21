using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        //CategoryId, FOREIGN KEY
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [Column]
        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public String? Note { get; set; }
        [Column]
        public DateTime Date { get; set; } = DateTime.Now;
     }
}
