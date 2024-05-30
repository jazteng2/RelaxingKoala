using RelaxingKoala.Models.Users;
namespace RelaxingKoala.Models.ViewModels
{
    public class MenuViewModel
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public User? User { get; set; }
    }
}
