using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public DateOnly CreatedDate { get; set; }
        public DateOnly ReservedDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int NumberOfPeople { get; set; }
        public Guid UserId { get; set; }
        public List<int> Tables { get; set; } = new List<int>();
    }
}
