using System.ComponentModel.DataAnnotations;

namespace RelaxingKoala.Models.Users
{
    public enum UserRole
    {
        [Display(Name = "Customer")]
        Customer,
        [Display(Name = "Staff")]
        Staff,
        [Display(Name = "Admin")]
        Admin,
        [Display(Name = "Driver")]
        Driver
    }
}
