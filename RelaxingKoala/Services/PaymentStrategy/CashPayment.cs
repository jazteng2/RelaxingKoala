﻿using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;
using System.Linq.Expressions;
using System.Xml.Schema;

namespace RelaxingKoala.Services.PaymentStrategy
{
    public class CashPayment : IPaymentMethod
    {
        private readonly InvoiceRepository invoiceRepo;
        public CashPayment(MySqlDataSource dataSource)
        {
            invoiceRepo = new InvoiceRepository(dataSource);
        }
        public bool ProcessPayment(IOrder order, int givenPay)
        {
            if (order.Cost > givenPay) return false;
            Invoice i = new Invoice()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateOnly.FromDateTime(DateTime.Today),
                TotalPay = order.Cost,
                GivenPay = givenPay,
                ExcessPay = order.Cost - givenPay,
                PaymentMethod = PaymentMethod.Cash,
                OrderId = order.Id,
                UserId = order.UserId,
            };
            invoiceRepo.Insert(i);
            return true;
        }
    }
}
