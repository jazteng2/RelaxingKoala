namespace RelaxingKoala.Models.ViewModels
{
    public class SalesReportViewModel
    {
        public MenuItem MenuItem { get; set; } = new MenuItem();
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        public int TotalSalesCost { get; set; } = 0;
        public int NumberOfSelection { get; set; } = 0;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
