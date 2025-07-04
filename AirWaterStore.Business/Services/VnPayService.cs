using AirWaterStore.Business.Interfaces;
using AirWaterStore.Business.Library;
using AirWaterStore.Business.LibraryS;
using AirWaterStore.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AirWaterStore.Business.Services;

public class VnPayService : IVnPayService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly VnPayConfig _vnPayConfig;

    private string currencyCode = "VND";

    public VnPayService(IHttpContextAccessor contextAccessor, IOptions<VnPayConfig> vnpayConfig)
    {
        _contextAccessor = contextAccessor;
        _vnPayConfig = vnpayConfig.Value;
    }

    public string CreatePayment(Order paymentRequest)
    {

        if (paymentRequest == null)
        {
            return "Order is null";
        }

        VnPayLibrary vnpay = new VnPayLibrary();

        vnpay.AddRequestData(VnPayRequestParam.vnp_Version, VnPayLibrary.VERSION);
        vnpay.AddRequestData(VnPayRequestParam.vnp_Command, "pay");
        vnpay.AddRequestData(VnPayRequestParam.vnp_TmnCode, _vnPayConfig.TmnCode);
        vnpay.AddRequestData(VnPayRequestParam.vnp_Amount, ((int)paymentRequest.TotalPrice * 100).ToString());
        //skip bank code
        vnpay.AddRequestData(VnPayRequestParam.vnp_CreateDate, DateTime.Now.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData(VnPayRequestParam.vnp_CurrCode, currencyCode);
        vnpay.AddRequestData(VnPayRequestParam.vnp_IpAddr, Utils.GetIpAddress(_contextAccessor.HttpContext));
        vnpay.AddRequestData(VnPayRequestParam.vnp_Locale, "vn"); //hard code vietnamese for testing purposes
        vnpay.AddRequestData(VnPayRequestParam.vnp_OrderInfo, "Thanh toan don hang: " + paymentRequest.OrderId);
        vnpay.AddRequestData(VnPayRequestParam.vnp_OrderType, "other");
        vnpay.AddRequestData(VnPayRequestParam.vnp_ReturnUrl, _vnPayConfig.ReturnUrl);
        vnpay.AddRequestData(VnPayRequestParam.vnp_TxnRef, VnPayHelper.GenerateUniqueId(paymentRequest));

        string paymentUrl = vnpay.CreateRequestUrl(_vnPayConfig.PaymentUrl, _vnPayConfig.HashSecret);

        return paymentUrl;
    }
}
