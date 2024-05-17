namespace RelaxingKoala.Models.Users
{
    public abstract class User
    {
        private int _id;
        private string _name;
        private string _email;
        public User(int id, string name, string email)
        {
            _id = id;
            _name = name;
            _email = email;
        }
    }
}
