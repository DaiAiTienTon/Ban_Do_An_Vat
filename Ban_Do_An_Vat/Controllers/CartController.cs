using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Ban_Do_An_Vat.Data;
using Ban_Do_An_Vat.Models;
using Ban_Do_An_Vat.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Ban_Do_An_Vat.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Ban_Do_An_Vat.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMomoService _momoService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string CartSessionKey = "Cart";

        public CartController(ApplicationDbContext context, IMomoService momoService, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _momoService = momoService;
            _configuration = configuration;
            _userManager = userManager;
        }

        private List<CartItem> GetCart()
        {
            try
            {
                var cartJson = HttpContext.Session.GetString(CartSessionKey);
                return string.IsNullOrEmpty(cartJson) 
                    ? new List<CartItem>() 
                    : JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
            }
            catch (Exception)
            {
                HttpContext.Session.Remove(CartSessionKey);
                return new List<CartItem>();
            }
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        // GET: Cart
        public IActionResult Index()
        {
            var cart = GetCart();
            var cartTotal = cart.Sum(item => item.TotalPrice);
            
            decimal discount = 0;
            var couponCode = HttpContext.Session.GetString("AppliedCoupon");
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = _context.Coupons.FirstOrDefault(c => c.Code == couponCode && c.IsActive && c.ExpiryDate >= DateTime.Now);
                if (coupon != null)
                {
                    if (cartTotal >= coupon.MinOrderAmount)
                    {
                        if (coupon.DiscountType == "Percentage")
                        {
                            discount = cartTotal * (coupon.DiscountAmount / 100);
                        }
                        else
                        {
                            discount = coupon.DiscountAmount;
                        }
                        ViewBag.AppliedCouponCode = couponCode;
                    }
                    else
                    {
                        HttpContext.Session.Remove("AppliedCoupon");
                    }
                }
            }

            var shippingFee = cartTotal > 150000 ? 0 : 20000;
            
            ViewBag.CartTotal = cartTotal;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Discount = discount;
            ViewBag.GrandTotal = cartTotal + shippingFee - discount;
            return View(cart);
        }

        // POST: Cart/Add
        [HttpPost]
        public IActionResult Add(int snackId, int quantity)
        {
            var snack = _context.Snacks.FirstOrDefault(s => s.Id == snackId);
            if (snack == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            if (snack.StockQuantity < quantity)
            {
                return Json(new { success = false, message = "Số lượng tồn kho không đủ." });
            }

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(i => i.SnackId == snackId);

            if (cartItem == null)
            {
                cart.Add(new CartItem
                {
                    SnackId = snack.Id,
                    Name = snack.Name,
                    Price = snack.Price,
                    ImageUrl = snack.ImageUrl,
                    Weight = snack.Weight,
                    Quantity = quantity
                });
            }
            else
            {
                if (snack.StockQuantity < (cartItem.Quantity + quantity))
                {
                    return Json(new { success = false, message = "Số lượng trong giỏ cộng thêm vượt quá tồn kho." });
                }
                cartItem.Quantity += quantity;
            }

            SaveCart(cart);
            return Json(new { success = true });
        }

        // POST: Cart/AddCombo
        [HttpPost]
        public IActionResult AddCombo(int comboId, int quantity)
        {
            var combo = _context.Combos.Include(c => c.ComboItems).FirstOrDefault(c => c.Id == comboId && c.IsAvailable);
            if (combo == null)
            {
                return Json(new { success = false, message = "Combo không tồn tại hoặc đã ngừng bán." });
            }

            foreach (var ci in combo.ComboItems)
            {
                var snack = _context.Snacks.FirstOrDefault(s => s.Id == ci.SnackId);
                if (snack == null || snack.StockQuantity < (ci.Quantity * quantity))
                {
                    return Json(new { success = false, message = $"Sản phẩm '{snack?.Name}' trong combo không đủ tồn kho." });
                }
            }

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(i => i.IsCombo && i.ComboId == comboId);

            if (cartItem == null)
            {
                cart.Add(new CartItem
                {
                    SnackId = 0,
                    ComboId = combo.Id,
                    IsCombo = true,
                    Name = combo.Name,
                    Price = combo.SalePrice,
                    ImageUrl = combo.ImageUrl ?? "",
                    Weight = "",
                    ComboLabel = $"{combo.ComboItems.Count} món • Tiết kiệm {combo.DiscountPercent}%",
                    Quantity = quantity
                });
            }
            else
            {
                foreach (var ci in combo.ComboItems)
                {
                    var snack = _context.Snacks.FirstOrDefault(s => s.Id == ci.SnackId);
                    if (snack != null && snack.StockQuantity < (ci.Quantity * (cartItem.Quantity + quantity)))
                    {
                        return Json(new { success = false, message = $"Số lượng sản phẩm '{snack.Name}' trong giỏ cộng thêm vượt quá tồn kho." });
                    }
                }
                cartItem.Quantity += quantity;
            }

            SaveCart(cart);
            return Json(new { success = true, cartCount = cart.Sum(i => i.Quantity) });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int snackId, int quantity, int comboId = 0)
        {
            var cart = GetCart();
            CartItem? cartItem;

            if (comboId > 0)
            {
                var combo = _context.Combos.Include(c => c.ComboItems).FirstOrDefault(c => c.Id == comboId && c.IsAvailable);
                if (combo == null || quantity <= 0)
                {
                    return Json(new { success = false, message = "Combo không hợp lệ." });
                }

                // Kiểm tra tồn kho của tất cả sản phẩm trong combo
                foreach (var ci in combo.ComboItems)
                {
                    var snack = _context.Snacks.FirstOrDefault(s => s.Id == ci.SnackId);
                    if (snack == null || snack.StockQuantity < (ci.Quantity * quantity))
                    {
                        return Json(new { success = false, message = $"Sản phẩm '{snack?.Name}' trong combo không đủ tồn kho." });
                    }
                }

                cartItem = cart.FirstOrDefault(i => i.IsCombo && i.ComboId == comboId);
            }
            else
            {
                var snack = _context.Snacks.FirstOrDefault(s => s.Id == snackId);
                if (snack == null || quantity <= 0)
                {
                    return Json(new { success = false, message = "Sản phẩm không hợp lệ." });
                }

                if (snack.StockQuantity < quantity)
                {
                    return Json(new { success = false, message = "Tồn kho không đủ." });
                }

                cartItem = cart.FirstOrDefault(i => !i.IsCombo && i.SnackId == snackId);
            }

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                SaveCart(cart);
                
                var cartTotal = cart.Sum(i => i.TotalPrice);
                var shippingFee = cartTotal > 150000 ? 0 : 20000;
                
                decimal discount = 0;
                var couponCode = HttpContext.Session.GetString("AppliedCoupon");
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var coupon = _context.Coupons.FirstOrDefault(c => c.Code == couponCode && c.IsActive && c.ExpiryDate >= DateTime.Now);
                    if (coupon != null)
                    {
                        if (cartTotal >= coupon.MinOrderAmount)
                        {
                            if (coupon.DiscountType == "Percentage")
                            {
                                discount = cartTotal * (coupon.DiscountAmount / 100);
                            }
                            else
                            {
                                discount = coupon.DiscountAmount;
                            }
                        }
                        else
                        {
                            HttpContext.Session.Remove("AppliedCoupon");
                            couponCode = null;
                        }
                    }
                }

                var grandTotal = cartTotal + shippingFee - discount;
                var cartCount = cart.Sum(i => i.Quantity);

                return Json(new { 
                    success = true, 
                    itemSubtotal = cartItem.TotalPrice, 
                    cartTotal = cartTotal,
                    shippingFee = shippingFee,
                    discount = discount,
                    couponCode = couponCode,
                    grandTotal = grandTotal,
                    cartCount = cartCount
                });
            }

            return Json(new { success = false, message = "Không tìm thấy món ăn trong giỏ." });
        }

        // POST: Cart/Remove
        [HttpPost]
        public IActionResult Remove(int snackId, int comboId = 0)
        {
            var cart = GetCart();
            CartItem? cartItem;

            if (comboId > 0)
            {
                cartItem = cart.FirstOrDefault(i => i.IsCombo && i.ComboId == comboId);
            }
            else
            {
                cartItem = cart.FirstOrDefault(i => !i.IsCombo && i.SnackId == snackId);
            }

            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Cart/GetTotalItems
        [HttpGet]
        public IActionResult GetTotalItems()
        {
            var cart = GetCart();
            var total = cart.Sum(item => item.Quantity);
            return Content(total.ToString());
        }

        // GET: Cart/Checkout
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                return RedirectToAction("Index", "Snacks");
            }

            var user = await _userManager.GetUserAsync(User);
            var order = new Order();
            if (user != null)
            {
                order.CustomerName = user.FullName;
                order.CustomerEmail = user.Email ?? "";
                order.CustomerPhone = user.PhoneNumber ?? "";
                order.DeliveryAddress = user.Address ?? "";
            }

            var cartTotal = cart.Sum(item => item.TotalPrice);
            decimal discount = 0;
            var couponCode = HttpContext.Session.GetString("AppliedCoupon");
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = _context.Coupons.FirstOrDefault(c => c.Code == couponCode && c.IsActive && c.ExpiryDate >= DateTime.Now);
                if (coupon != null && cartTotal >= coupon.MinOrderAmount)
                {
                    if (coupon.DiscountType == "Percentage")
                    {
                        discount = cartTotal * (coupon.DiscountAmount / 100);
                    }
                    else
                    {
                        discount = coupon.DiscountAmount;
                    }
                    ViewBag.CouponCode = coupon.Code;
                }
            }

            var shippingFee = cartTotal > 150000 ? 0 : 20000;

            ViewBag.Cart = cart;
            ViewBag.CartTotal = cartTotal;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Discount = discount;
            ViewBag.GrandTotal = cartTotal + shippingFee - discount;
            return View(order);
        }

        // POST: Cart/Checkout
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống.");
                ViewBag.Cart = cart;
                ViewBag.CartTotal = 0;
                ViewBag.ShippingFee = 0;
                ViewBag.Discount = 0;
                ViewBag.GrandTotal = 0;
                return View(order);
            }

            var cartTotal = cart.Sum(i => i.TotalPrice);
            var shippingFee = cartTotal > 150000 ? 0 : 20000;
            
            decimal discount = 0;
            var couponCode = HttpContext.Session.GetString("AppliedCoupon");
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = _context.Coupons.FirstOrDefault(c => c.Code == couponCode && c.IsActive && c.ExpiryDate >= DateTime.Now);
                if (coupon != null && cartTotal >= coupon.MinOrderAmount)
                {
                    if (coupon.DiscountType == "Percentage")
                    {
                        discount = cartTotal * (coupon.DiscountAmount / 100);
                    }
                    else
                    {
                        discount = coupon.DiscountAmount;
                    }
                    order.CouponCode = coupon.Code;
                }
            }

            var grandTotal = cartTotal + shippingFee - discount;

            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                order.TotalAmount = grandTotal;
                order.DiscountAmount = discount;
                order.Status = "Pending";
                order.PaymentStatus = "Unpaid";
                
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                _context.Orders.Add(order);
                _context.SaveChanges(); // Saves order and generates order.Id

                foreach (var item in cart)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        SnackId = item.IsCombo ? null : (int?)item.SnackId,
                        ComboId = item.IsCombo ? (int?)item.ComboId : null,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    _context.OrderItems.Add(orderItem);

                    // Deduct stock quantity
                    if (item.IsCombo)
                    {
                        var combo = _context.Combos.Include(c => c.ComboItems).FirstOrDefault(c => c.Id == item.ComboId);
                        if (combo != null)
                        {
                            foreach (var ci in combo.ComboItems)
                            {
                                var dbSnack = _context.Snacks.FirstOrDefault(s => s.Id == ci.SnackId);
                                if (dbSnack != null)
                                {
                                    dbSnack.StockQuantity -= ci.Quantity * item.Quantity;
                                    if (dbSnack.StockQuantity < 0) dbSnack.StockQuantity = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        var dbSnack = _context.Snacks.FirstOrDefault(s => s.Id == item.SnackId);
                        if (dbSnack != null)
                        {
                            dbSnack.StockQuantity -= item.Quantity;
                            if (dbSnack.StockQuantity < 0) dbSnack.StockQuantity = 0;
                        }
                    }
                }

                _context.SaveChanges();
                HttpContext.Session.Remove("AppliedCoupon"); // Clear coupon from session after checkout success

                if (order.PaymentMethod == "Momo")
                {
                    var momoResponse = await _momoService.CreatePaymentAsync(order, grandTotal);
                    if (momoResponse != null && momoResponse.ResultCode == 0 && !string.IsNullOrEmpty(momoResponse.PayUrl))
                    {
                        HttpContext.Session.Remove(CartSessionKey);
                        return Redirect(momoResponse.PayUrl);
                    }
                    else
                    {
                        // Rollback database changes if Momo fails
                        _context.OrderItems.RemoveRange(_context.OrderItems.Where(oi => oi.OrderId == order.Id));
                        _context.Orders.Remove(order);
                        
                        // Re-add stock
                        foreach (var item in cart)
                        {
                            if (item.IsCombo)
                            {
                                var combo = _context.Combos.Include(c => c.ComboItems).FirstOrDefault(c => c.Id == item.ComboId);
                                if (combo != null)
                                {
                                    foreach (var ci in combo.ComboItems)
                                    {
                                        var dbSnack = _context.Snacks.FirstOrDefault(s => s.Id == ci.SnackId);
                                        if (dbSnack != null)
                                        {
                                            dbSnack.StockQuantity += ci.Quantity * item.Quantity;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var dbSnack = _context.Snacks.FirstOrDefault(s => s.Id == item.SnackId);
                                if (dbSnack != null)
                                {
                                    dbSnack.StockQuantity += item.Quantity;
                                }
                            }
                        }
                        _context.SaveChanges();

                        ModelState.AddModelError("", "Khởi tạo thanh toán MoMo thất bại: " + (momoResponse?.Message ?? "Không thể kết nối đến máy chủ MoMo."));
                        ViewBag.Cart = cart;
                        ViewBag.CartTotal = cartTotal;
                        ViewBag.ShippingFee = shippingFee;
                        ViewBag.Discount = discount;
                        ViewBag.GrandTotal = grandTotal;
                        return View(order);
                    }
                }

                // For COD and VietQR, clear session cart and redirect
                HttpContext.Session.Remove(CartSessionKey);
                return RedirectToAction(nameof(Success), new { id = order.Id });
            }

            ViewBag.Cart = cart;
            ViewBag.CartTotal = cartTotal;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Discount = discount;
            ViewBag.GrandTotal = grandTotal;
            return View(order);
        }

        // GET: Cart/MomoCallback
        public IActionResult MomoCallback(
            string partnerCode, string orderId, string requestId, string amount, 
            string orderInfo, string orderType, string transId, string resultCode, 
            string message, string payType, string responseTime, string extraData, 
            string signature)
        {
            var momoSettings = _configuration.GetSection("Momo");
            var accessKey = momoSettings["AccessKey"] ?? "";
            var secretKey = momoSettings["SecretKey"] ?? "";

            bool isValidSignature = _momoService.ValidateSignature(
                accessKey, secretKey, amount, extraData, message, orderId, orderInfo, 
                orderType, partnerCode, payType, requestId, responseTime, resultCode, 
                transId, signature);

            if (!isValidSignature)
            {
                TempData["PaymentError"] = "Chữ ký xác thực giao dịch MoMo không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            // Extract original order ID
            var parts = orderId.Split('_');
            if (parts.Length > 0 && int.TryParse(parts[0], out int originalOrderId))
            {
                var order = _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Snack)
                    .FirstOrDefault(o => o.Id == originalOrderId);

                if (order != null)
                {
                    if (resultCode == "0")
                    {
                        order.PaymentStatus = "Paid";
                        order.Status = "Processing";
                        _context.SaveChanges();
                        TempData["PaymentSuccessMsg"] = "Thanh toán qua ví MoMo thành công!";
                    }
                    else
                    {
                        order.PaymentStatus = "Failed";
                        _context.SaveChanges();
                        TempData["PaymentError"] = $"Thanh toán MoMo không thành công: {message}. Bạn có thể chọn quét mã VietQR để hoàn tất chuyển khoản.";
                    }

                    return RedirectToAction(nameof(Success), new { id = order.Id });
                }
            }

            TempData["PaymentError"] = "Không tìm thấy đơn hàng tương ứng.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/MomoIpn
        [HttpPost]
        public IActionResult MomoIpn([FromBody] MomoIpnRequest request)
        {
            var momoSettings = _configuration.GetSection("Momo");
            var accessKey = momoSettings["AccessKey"] ?? "";
            var secretKey = momoSettings["SecretKey"] ?? "";

            bool isValidSignature = _momoService.ValidateSignature(
                accessKey, secretKey, request.Amount.ToString(), request.ExtraData, request.Message, 
                request.OrderId, request.OrderInfo, request.OrderType, request.PartnerCode, 
                request.PayType, request.RequestId, request.ResponseTime.ToString(), 
                request.ResultCode.ToString(), request.TransId.ToString(), request.Signature);

            if (isValidSignature)
            {
                var parts = request.OrderId.Split('_');
                if (parts.Length > 0 && int.TryParse(parts[0], out int originalOrderId))
                {
                    var order = _context.Orders.FirstOrDefault(o => o.Id == originalOrderId);
                    if (order != null && order.PaymentStatus != "Paid")
                    {
                        if (request.ResultCode == 0)
                        {
                            order.PaymentStatus = "Paid";
                            order.Status = "Processing";
                        }
                        else
                        {
                            order.PaymentStatus = "Failed";
                        }
                        _context.SaveChanges();
                    }
                }
                return NoContent(); // MoMo expects HTTP 204
            }

            return BadRequest("Invalid Signature");
        }

        // POST: Cart/ApplyCoupon
        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode)
        {
            if (string.IsNullOrEmpty(couponCode))
            {
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá." });
            }

            couponCode = couponCode.ToUpper().Trim();
            var coupon = _context.Coupons.FirstOrDefault(c => c.Code == couponCode);
            if (coupon == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không tồn tại." });
            }

            if (!coupon.IsActive || coupon.ExpiryDate < DateTime.Now)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn hoặc không khả dụng." });
            }

            var cart = GetCart();
            var cartTotal = cart.Sum(i => i.TotalPrice);
            if (cartTotal < coupon.MinOrderAmount)
            {
                return Json(new { success = false, message = $"Mã này yêu cầu đơn hàng tối thiểu từ {coupon.MinOrderAmount.ToString("N0")}đ." });
            }

            HttpContext.Session.SetString("AppliedCoupon", coupon.Code);

            decimal discount = 0;
            if (coupon.DiscountType == "Percentage")
            {
                discount = cartTotal * (coupon.DiscountAmount / 100);
            }
            else
            {
                discount = coupon.DiscountAmount;
            }

            var shippingFee = cartTotal > 150000 ? 0 : 20000;
            var grandTotal = cartTotal + shippingFee - discount;

            return Json(new {
                success = true,
                message = "Áp dụng mã giảm giá thành công!",
                couponCode = coupon.Code,
                discount = discount,
                grandTotal = grandTotal,
                cartTotal = cartTotal,
                shippingFee = shippingFee
            });
        }

        // POST: Cart/RemoveCoupon
        [HttpPost]
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.Remove("AppliedCoupon");
            
            var cart = GetCart();
            var cartTotal = cart.Sum(i => i.TotalPrice);
            var shippingFee = cartTotal > 150000 ? 0 : 20000;
            var grandTotal = cartTotal + shippingFee;

            return Json(new {
                success = true,
                message = "Đã hủy bỏ mã giảm giá.",
                grandTotal = grandTotal,
                cartTotal = cartTotal,
                shippingFee = shippingFee
            });
        }

        // GET: Cart/Success/5
        public IActionResult Success(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Snack)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Combo)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Cart/ConfirmVietQRPayment/5
        [Authorize]
        public IActionResult ConfirmVietQRPayment(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order != null && order.PaymentMethod == "VietQR" && order.PaymentStatus == "Unpaid")
            {
                order.PaymentStatus = "Paid";
                order.Status = "Processing";
                _context.SaveChanges();
                TempData["PaymentSuccessMsg"] = "Xác nhận chuyển khoản thành công! Cảm ơn bạn.";
            }
            return RedirectToAction(nameof(Success), new { id = id });
        }

        // GET: Cart/ReinitMomoPayment/5
        [Authorize]
        public async Task<IActionResult> ReinitMomoPayment(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order != null && order.PaymentMethod == "Momo" && order.PaymentStatus != "Paid")
            {
                var momoResponse = await _momoService.CreatePaymentAsync(order, order.TotalAmount);
                if (momoResponse != null && momoResponse.ResultCode == 0 && !string.IsNullOrEmpty(momoResponse.PayUrl))
                {
                    return Redirect(momoResponse.PayUrl);
                }
                TempData["PaymentError"] = "Không thể khởi tạo cổng thanh toán MoMo lúc này. Vui lòng thử lại sau.";
            }
            return RedirectToAction(nameof(Success), new { id = id });
        }
    }

    public class MomoIpnRequest
    {
        public string PartnerCode { get; set; } = "";
        public string OrderId { get; set; } = "";
        public string RequestId { get; set; } = "";
        public long Amount { get; set; }
        public string OrderInfo { get; set; } = "";
        public string OrderType { get; set; } = "";
        public long TransId { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; } = "";
        public string PayType { get; set; } = "";
        public long ResponseTime { get; set; }
        public string ExtraData { get; set; } = "";
        public string Signature { get; set; } = "";
    }
}
