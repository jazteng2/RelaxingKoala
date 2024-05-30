using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;
using Xunit.Abstractions;

namespace TestRelaxingKoala
{
    public class TestOrder
    {
        private readonly ITestOutputHelper output;
        private readonly OrderRepository orderRepo;
        private readonly TableRepository tableRepo;
        private readonly MenuItemRepository menuItemRepo;
        private readonly CustomerRepository customerRepo;
        public TestOrder(ITestOutputHelper output)
        {
            var conn = new MySqlDataSource("Server=127.0.0.1;Port=3306;User ID=root;Password=admin;Database=rkdb");
            this.output = output;
            orderRepo = new OrderRepository(conn);
            tableRepo = new TableRepository(conn);
            menuItemRepo = new MenuItemRepository(conn);
            customerRepo = new CustomerRepository(conn);
        }

        [Fact]
        public void TestInsertOrder()
        {
            Customer c = customerRepo.GetByFirstName("Emily");
            DeliveryOrder order = new DeliveryOrder()
            {
                Id = Guid.NewGuid(),
                Cost = 0,
                UserId = c.Id,
                State = OrderState.Payed,
                Type = OrderType.Delivery,
            };

            var tables = tableRepo.GetAll();
            var menuItems = menuItemRepo.GetAll();

            order.Tables.Add(tables[1]);
            order.Tables.Add(tables[2]);

            order.MenuItems.Add(menuItems[0]);
            order.MenuItems.Add(menuItems[1]);

            order.RecalculateCost();

            DisplayOrder(order);
            orderRepo.Insert(order);
        }

        [Fact]
        public void TestGetOrderById()
        {
            Guid id = new Guid("03bd73e9-f53c-4c33-9e90-1993e794f683");
            var order = orderRepo.GetById(id);
            DisplayOrder(order);
        }

        [Fact]
        public void TestUpdateOrder()
        {
            Guid id = new Guid("03bd73e9-f53c-4c33-9e90-1993e794f683");
            var order = orderRepo.GetById(id);
            var tables = tableRepo.GetAll();
            var menuitems = menuItemRepo.GetAll();
            DisplayOrder(order);

            order.State = OrderState.Ready;
            order.Tables.Clear();
            order.Tables.Add(tables[3]);
            order.Tables.Add(tables[5]);


            order.MenuItems.Clear();
            order.MenuItems.Add(menuitems[0]);
            order.MenuItems.Add(menuitems[1]);
            order.RecalculateCost();

            orderRepo.Update(order);
            DisplayOrder(orderRepo.GetById(id));
        }

        [Fact]
        public void TestDeleteOrder()
        {
            Guid id = new Guid("2d880532-b728-4a9d-91db-07608472e15d");
            var prevOrder = orderRepo.GetById(id);
            orderRepo.Delete(id);
            var getOrder = orderRepo.GetById(id);
            Assert.NotEqual(prevOrder, getOrder);
        }

        private void DisplayOrder(IOrder order)
        {
            output.WriteLine("{0} {1} {2} {3} {4}", order.Id, order.Cost, order.UserId, order.State, order.Type);
            foreach (var table in order.Tables)
            {
                output.WriteLine("{0} {1}", table.Id, table.Availability);
            }
            foreach (var item in order.MenuItems)
            {
                output.WriteLine("{0} {1} {2}", item.Id, item.Name, item.Cost);
            }
        }
    }
}
