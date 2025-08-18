using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        public async Task AddAsync(Payment payment)
        {
            await _paymentRepository.AddAsync(payment);
        }
        public async Task UpdateAsync(Payment payment)
        {
             await _paymentRepository.UpdateAsync(payment);
        }
    }
}
