namespace RelaxingKoala.Models.ViewModels
{
    public class SalesReportViewModel
    {
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        public List<MenuItem> MenuItem { get; set; } = new List<MenuItem>();
    }
}
