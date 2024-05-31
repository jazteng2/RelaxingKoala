using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;
using Xunit.Abstractions;

namespace TestRelaxingKoala
{
    public class TestUser
    {
        private readonly CustomerRepository customerRepo;
        private readonly UserRepository userRepo;
        private readonly OrderRepository orderRepo;
        private readonly ITestOutputHelper output;
        public TestUser(ITestOutputHelper output)
        {
            var conn = new MySqlDataSource("Server=127.0.0.1;Port=3306;User ID=root;Password=admin;Database=rkdb");
            customerRepo = new CustomerRepository(conn);
            userRepo = new UserRepository(conn);
            orderRepo = new OrderRepository(conn);
            this.output = output;
        }

        [Fact]
        public void TestFetchCustomers()
        {
            List<Customer> customers = customerRepo.GetAll();
            foreach(Customer customer in customers)
            {
                output.WriteLine(@"
                    {0} {1} {2} {3} {4}
                ", customer.Id, customer.FirstName, customer.LastName, customer.Email, customer.Password);
            }
        }

        [Fact]
        public void TestFetchCustomerById()
        {
            Customer c = customerRepo.GetById(new Guid("d840d48c-91b8-4a69-97b9-1b90a8ab3acf"));
            Assert.NotNull(c);
        }

        [Fact]
        public void TestFetchCustomerByName()
        {
            Customer c = customerRepo.GetByFirstName("Emily");
            Assert.Equal("Emily", c.FirstName);
        }

        [Fact]
        public void TestFetchCustomerByEmail()
        {
            Customer c = customerRepo.GetByEmail("emily.davis@example.com");
            Assert.NotNull(c);

        }

        [Fact]
        public void TestFetchUserByEmail()
        {
            User u = userRepo.GetByEmail("emily.davis@example.com");
            output.WriteLine(u.Role.ToString());
        }

        [Fact]
        public void TestFetchUserByEmailFail()
        {
            User u = userRepo.GetByEmail("");
            Assert.True(string.IsNullOrEmpty(u.FirstName));
        }

        [Fact]
        public void TestInsert()
        {
            Admin u = new Admin()
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "Boss",
                Email = "admin@gmail.com",
                Password = "admin",
                Role = UserRole.Admin
            };

            Driver d = new Driver()
            {
                Id = Guid.NewGuid(),
                FirstName = "Uber",
                LastName = "Driver",
                Email = "uber@gmail.com",
                Password = "uber",
                Role = UserRole.Driver
            };
            userRepo.Insert(u);
            userRepo.Insert(d);
        }
    }
}
