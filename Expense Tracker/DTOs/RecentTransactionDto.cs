namespace Expense_Tracker.DTOs
{
    public class RecentTransactionDto
    {
        public int CategoryId { get; set; }
        public string CategoryTitleWithIcon { get; set; }

        public DateTime Date { get; set; }
        public string FormattedAmount { get; set; }

    }
}
