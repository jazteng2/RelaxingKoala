using Microsoft.EntityFrameworkCore.Diagnostics;
using RelaxingKoala.Services.PaymentStrategy;
using System.Diagnostics;
namespace RelaxingKoala.Models.Orders
{
    public class DineInOrder : Order
    {
        public override bool Pay(IPaymentMethod method, int givenPay)
        {
            PaymentContext context = new PaymentContext();
            context.SetPaymentStrategy(method);
            bool payed = context.ProcessPayment(this, givenPay);
            if (!payed) return false;
            State = OrderState.Payed;
            return true;
        }
    }
}
