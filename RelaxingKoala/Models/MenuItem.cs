namespace RelaxingKoala.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public int Cost { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Availability { get; set; }
    }
}
