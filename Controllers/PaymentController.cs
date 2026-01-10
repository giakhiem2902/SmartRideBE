using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace SmartRideBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("vnpay-create")]
        public IActionResult CreateVNPayPayment([FromBody] PaymentRequest request)
        {
            // request chứa: TicketId, Amount, OrderInfo

            var vnpayConfig = _configuration.GetSection("VNPay");
            var tmnCode = vnpayConfig["TmnCode"];
            var hashSecret = vnpayConfig["HashSecret"];
            var returnUrl = vnpayConfig["ReturnUrl"];
            var vnpayUrl = vnpayConfig["Url"];

            var timeZoneId = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var timeZoneInfo = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneId);
            var createDate = timeZoneInfo.ToString("yyyyMMddHHmmss");

            var orderId = request.TicketId.ToString();
            var amount = (long)(request.Amount * 100); // VNPay tính bằng đồng x100

            var data = new Dictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", amount.ToString() },
                { "vnp_CreateDate", createDate },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", GetIpAddress() },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", request.OrderInfo ?? $"Ticket {orderId}" },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_TxnRef", orderId },
            };

            // Sắp xếp theo key
            var sortedData = data.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            // Tạo query string
            var queryString = string.Join("&", sortedData.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

            // Tính hash
            var hash = ComputeHash(queryString, hashSecret);
            var paymentUrl = $"{vnpayUrl}?{queryString}&vnp_SecureHash={hash}";

            return Ok(new { paymentUrl });
        }

        [HttpGet("vnpay-callback")]
        public IActionResult VNPayCallback()
        {
            var vnpayConfig = _configuration.GetSection("VNPay");
            var hashSecret = vnpayConfig["HashSecret"];

            var vnpTxnRef = Request.Query["vnp_TxnRef"];
            var vnpTransactionNo = Request.Query["vnp_TransactionNo"];
            var vnpResponseCode = Request.Query["vnp_ResponseCode"];
            var vnpSecureHash = Request.Query["vnp_SecureHash"];

            // Xóa vnp_SecureHash khỏi data để tính lại hash
            var hashInput = new Dictionary<string, string>();
            foreach (var (key, value) in Request.Query)
            {
                if (key != "vnp_SecureHash" && key.StartsWith("vnp_"))
                {
                    hashInput[key] = value.ToString();
                }
            }

            var sortedHashInput = hashInput.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            var queryString = string.Join("&", sortedHashInput.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            var computedHash = ComputeHash(queryString, hashSecret);

            if (!computedHash.Equals(vnpSecureHash.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid hash");
            }

            if (vnpResponseCode == "00")
            {
                // ✅ Thanh toán thành công - cập nhật ticket status
                // TODO: Update ticket status to "Paid"
                return Ok(new { message = "Payment successful", transactionNo = vnpTransactionNo });
            }
            else
            {
                // ❌ Thanh toán thất bại
                return Ok(new { message = "Payment failed", responseCode = vnpResponseCode });
            }
        }

        private string ComputeHash(string input, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private string GetIpAddress()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = HttpContext.Request.Headers["X-Forwarded-For"];

            return ipAddress ?? "127.0.0.1";
        }
    }

    public class PaymentRequest
    {
        public int TicketId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
    }
}