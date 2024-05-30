﻿namespace RelaxingKoala.Models.Users
{
    public abstract class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int UserRoleId { get; set; } // Add this property

    }
}
