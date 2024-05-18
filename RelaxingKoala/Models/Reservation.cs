using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required List<Table> Tables { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
