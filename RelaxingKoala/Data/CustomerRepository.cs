using MySqlConnector;

namespace RelaxingKoala.Data
{
    public class CustomerRepository
    {
        private readonly MySqlDataSource _dataSource;
        public CustomerRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        
    }
}
