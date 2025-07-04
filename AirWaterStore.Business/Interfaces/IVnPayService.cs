using AirWaterStore.Data.Models;

namespace AirWaterStore.Business.Interfaces;

public interface IVnPayService
{
    string CreatePayment(Order paymentRequest);
}
