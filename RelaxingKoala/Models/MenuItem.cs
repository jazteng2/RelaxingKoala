namespace RelaxingKoala.Models
{
    public class MenuItem
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Cost { get; set; }
    }
}
