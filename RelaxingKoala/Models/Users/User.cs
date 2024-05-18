namespace RelaxingKoala.Models.Users
{
    public abstract class User
    {
        private Guid _id;
        private string _name;
        private string _email;
        public User(Guid id, string name, string email)
        {
            _id = id;
            _name = name;
            _email = email;
        }

        public Guid Id { get { return _id; } }
        public string Name { get { return _name; } }
        public string Email { get { return _email; } }
    }
}
