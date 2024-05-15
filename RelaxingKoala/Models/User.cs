using System.ComponentModel.DataAnnotations;

namespace RelaxingKoala.Models
{
    public abstract class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
}
