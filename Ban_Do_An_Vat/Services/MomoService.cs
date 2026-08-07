using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ban_Do_An_Vat.Models;
using Microsoft.Extensions.Configuration;

namespace Ban_Do_An_Vat.Services
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponse?> CreatePaymentAsync(Order order, decimal amount);
        bool ValidateSignature(string accessKey, string secretKey, string amount, string extraData, string message, string orderId, string orderInfo, string orderType, string partnerCode, string payType, string requestId, string responseTime, string resultCode, string transId, string signatureReceived);
    }

    public class MomoService : IMomoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MomoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<MomoCreatePaymentResponse?> CreatePaymentAsync(Order order, decimal amount)
        {
            var momoSettings = _configuration.GetSection("Momo");
            var partnerCode = momoSettings["PartnerCode"] ?? "";
            var accessKey = momoSettings["AccessKey"] ?? "";
            var secretKey = momoSettings["SecretKey"] ?? "";
            var baseUrl = momoSettings["BaseUrl"] ?? "";
            var redirectUrl = momoSettings["RedirectUrl"] ?? "";
            var ipnUrl = momoSettings["IpnUrl"] ?? "";

            // Unique request IDs using order ID and timestamp
            var requestId = Guid.NewGuid().ToString();
            var momoOrderId = order.Id.ToString() + "_" + DateTime.UtcNow.Ticks;
            var orderInfo = "Thanh toan don hang #" + order.Id;
            var requestType = "captureWallet";
            var extraData = ""; // Empty string

            // Raw string signature formula:
            // accessKey=$accessKey&amount=$amount&extraData=$extraData&ipnUrl=$ipnUrl&orderId=$orderId&orderInfo=$orderInfo&partnerCode=$partnerCode&redirectUrl=$redirectUrl&requestId=$requestId&requestType=$requestType
            var rawHash = $"accessKey={accessKey}&amount={(long)amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={momoOrderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";
            
            var signature = ComputeHmacSha256(rawHash, secretKey);

            var requestBody = new MomoCreatePaymentRequest
            {
                PartnerCode = partnerCode,
                RequestId = requestId,
                Amount = (long)amount,
                OrderId = momoOrderId,
                OrderInfo = orderInfo,
                RedirectUrl = redirectUrl,
                IpnUrl = ipnUrl,
                RequestType = requestType,
                ExtraData = extraData,
                Signature = signature
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync(baseUrl, requestBody);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<MomoCreatePaymentResponse>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ValidateSignature(
            string accessKey, string secretKey, string amount, string extraData, string message, 
            string orderId, string orderInfo, string orderType, string partnerCode, string payType, 
            string requestId, string responseTime, string resultCode, string transId, string signatureReceived)
        {
            // Raw string callback formula:
            // accessKey=$accessKey&amount=$amount&extraData=$extraData&message=$message&orderId=$orderId&orderInfo=$orderInfo&orderType=$orderType&partnerCode=$partnerCode&payType=$payType&requestId=$requestId&responseTime=$responseTime&resultCode=$resultCode&transId=$transId
            var rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&message={message}&orderId={orderId}&orderInfo={orderInfo}&orderType={orderType}&partnerCode={partnerCode}&payType={payType}&requestId={requestId}&responseTime={responseTime}&resultCode={resultCode}&transId={transId}";
            
            var calculatedSignature = ComputeHmacSha256(rawHash, secretKey);

            return calculatedSignature.Equals(signatureReceived, StringComparison.OrdinalIgnoreCase);
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }

    public class MomoCreatePaymentRequest
    {
        public string PartnerCode { get; set; } = "";
        public string RequestId { get; set; } = "";
        public long Amount { get; set; }
        public string OrderId { get; set; } = "";
        public string OrderInfo { get; set; } = "";
        public string RedirectUrl { get; set; } = "";
        public string IpnUrl { get; set; } = "";
        public string RequestType { get; set; } = "";
        public string ExtraData { get; set; } = "";
        public string Signature { get; set; } = "";
        public string Lang { get; set; } = "vi";
    }

    public class MomoCreatePaymentResponse
    {
        public string PartnerCode { get; set; } = "";
        public string OrderId { get; set; } = "";
        public string RequestId { get; set; } = "";
        public long Amount { get; set; }
        public long ResponseTime { get; set; }
        public string Message { get; set; } = "";
        public int ResultCode { get; set; }
        public string PayUrl { get; set; } = "";
        public string Deeplink { get; set; } = "";
        public string QrCodeUrl { get; set; } = "";
    }
}
