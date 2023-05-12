using Darshit_Practical_3_Web.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Practical_3.DataAccess.Repository.IRepository;
using Practical_3.Model.Model.VM;
using System.Text.Json.Serialization;
using System.Text.Json;
using static Darshit_Practical_3_Web.Model.Order;
using System.Security.Claims;
using NuGet.Protocol.Plugins;

namespace Darshit_Practical_3_Web.Controllers
{
    public class OrderController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public OrderController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        #region Get Product
        [HttpGet]
        [Route("ProductList")]
        public IEnumerable<Product> Product()
        {
            return _unitOfWork.Product.GetAll();
        }
        #endregion


        #region Add Order
        [HttpPost]
        [Route("Add-Order")]
        public async Task<ActionResult> Post([FromBody] OrdersVM orders)
        {
            try
            {
                //// Get the currently authenticated user
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var user = claimsIdentity.FindFirst(ClaimTypes.Name);

                var order = new Order
                {
                    CustomerName = string.IsNullOrEmpty(orders.CustomerName) ? (user.Value) : orders.CustomerName,
                    CustomerEmail = orders.CustomerEmail,
                    OrderDate = DateTime.Now,
                    Note = orders.Note,
                    Status = StatusType.Open,
                    DiscountAmount = 10,
                    CustomerContactNo = orders.CustomerContactNo,
                    IsActive = true,
                    CreatedOn = DateTime.Now,

                };

                double Total_price = 0;

                //Add orderItems
                var orderItems = new List<OrderItems>();

                foreach (OrderItemVM item in orders.OrderItems)
                {
                    var product = _unitOfWork.Product.GetFirstOrDefault(x => x.ProductId == item.Productid);
                    if (product == null)
                    {
                        return BadRequest($"Product with id not found");
                    }
                    if (product.Quantity < item.Quantity)
                    {
                        return BadRequest($"Product with id {item.Productid} does not have enough quantity");
                    }
                    var orderItem = new OrderItems
                    {
                        ProductId = item.Productid,
                        Quantity = item.Quantity,
                        IsActive = true,
                        Price = product.Price * item.Quantity,
                    };

                    Total_price += orderItem.Price;
                    product.Quantity -= item.Quantity;
                    _unitOfWork.Product.Update(product);
                    orderItems.Add(orderItem);
                }

                if (orders.OrderItems.Count == 0)
                {
                    return BadRequest("No valid items in the order");
                }
                // If no customer name is provided, and no user is logged in,
                if (string.IsNullOrEmpty(orders.CustomerName) && user.Value == null)
                {
                    return BadRequest("Customer name is required");
                }

                if (string.IsNullOrEmpty(orders.CustomerEmail))
                {
                    return BadRequest("Customer email is required");
                }

                order.orderItems = orderItems;
                double discountAmount = Total_price * order.DiscountAmount / 100;
                double totalAmount = Total_price - discountAmount;

                order.TotalAmount = totalAmount; // Set the total amount for the order
                _unitOfWork.Order.Add(order);
                _unitOfWork.Save();

                return Ok($"Order with id {order.OrderId} added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }
        #endregion


        #region Get All Order
        [Authorize]
        [HttpGet]
        [Route("auth/GetAllOrder")]
        public IActionResult GetAllOrders(DateTime? fromDate, DateTime? toDate, StatusType? statusType, string? customerSearch)
        {
            try
            {
                var orders = _unitOfWork.Order.GetAll();

                if (fromDate != null)
                {
                    orders = orders.Where(o => o.CreatedOn >= fromDate.Value);
                }
                if (toDate != null)
                {
                    orders = orders.Where(o => o.CreatedOn <= toDate.Value);
                }
                if (statusType != null)
                {
                    orders = orders.Where(o => o.Status == statusType.Value);
                }
                if (!string.IsNullOrEmpty(customerSearch))
                {
                    orders = orders.Where(o => o.CustomerName.ToLower().Contains(customerSearch.ToLower())
                                         || o.CustomerEmail.ToLower().Contains(customerSearch.ToLower()));
                }

                var orderListVm = orders.Select(o => new
                {
                    OrderId = o.OrderId,
                    Note = o.Note,
                    CreatedOn = o.CreatedOn,
                    Status = o.Status,
                    IsActive = o.IsActive

                }).ToList();

                if(orderListVm.Count == 0)
                {
                    return StatusCode(404, "No Data");
                }

                return Ok(orderListVm);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex);
            }

        }
        #endregion


        #region UpdateOrderStatus
        [Authorize]
        [HttpPut]
        [Route("auth/UpdateOrderStatus/{orderId}/{statusType}")]
        public IActionResult UpdateOrderStatus(int orderId, string statusType)
        {
            try
            {
                var order = _unitOfWork.Order.GetFirstOrDefault(o => o.OrderId == orderId);

                if (order == null)
                {
                    return NotFound();
                }

                if (order.Status == StatusType.Shipped && statusType.ToLower() != "paid")
                {
                    return BadRequest("Order is shipped so status cannot be changed to any other status except for Paid.");
                }
                if (order.Status == StatusType.Paid)
                {
                    return BadRequest("Order is paid so status cannot be changed.");
                }

                switch (statusType.ToLower())
                {
                    case "open":
                        order.Status = StatusType.Open;
                        break;
                    case "draft":
                        order.Status = StatusType.Draft;
                        break;
                    case "shipped":
                        order.Status = StatusType.Shipped;
                        break;
                    case "paid":
                        order.Status = StatusType.Paid;
                        break;
                    default:
                        return BadRequest("Invalid status type");
                }

                order.ModifiedOn = DateTime.Now;
                _unitOfWork.Order.Update(order);
                _unitOfWork.Save();

                return Ok($"Order status of id {order.OrderId} status changed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        #endregion


        #region InActiveOrder
        [Authorize]
        [HttpPut]
        [Route("auth/InActiveOrder/{orderId}")]
        public IActionResult InActiveOrder(int orderId)
        {
            try
            {
                var order = _unitOfWork.Order.GetFirstOrDefault(o => o.OrderId == orderId);

                if (order == null)
                {
                    return NotFound();
                }

                if (order.Status == StatusType.Shipped)
                {
                    return BadRequest($"Order with id {order.OrderId} already shipped. Cannot inactive the order");
                }

                if (order.Status == StatusType.Paid)
                {
                    return BadRequest($"Order with id {order.OrderId} already paid. Cannot inactive the order");
                }

                order.IsActive = !order.IsActive;
                order.ModifiedOn = DateTime.Now;

                _unitOfWork.Order.Update(order);
                _unitOfWork.Save();

                return Ok($"Order with id {order.OrderId} inactivated successfully");
            }
            catch(Exception ex) 
            { 
                return StatusCode(StatusCodes.Status500InternalServerError, ex); 
            }
        }
        #endregion


        #region Update-OrderItem-Quantity
        [Authorize]
        [HttpPut]
        [Route("auth/Update-OrderItem-Quantity")]
        public async Task<ActionResult> UpdateOrderItemQuantity(int orderItemId, int newQuantity)
        {
            try
            {
                var orderItem = _unitOfWork.OrderItems.GetFirstOrDefault(u => u.OrderItemId == orderItemId);

                if (orderItem == null)
                {
                    return BadRequest($"Order item with id {orderItemId} not found");
                }

                if (!orderItem.IsActive)
                {
                    return BadRequest($"Order with id {orderItemId} is not active. Cannot update order item quantity");
                }

                

                var product = _unitOfWork.Product.GetFirstOrDefault(u => u.ProductId == orderItem.ProductId);

                if (product == null)
                {
                    return BadRequest($"Product with id {orderItem.ProductId} not found");
                }

                var order = _unitOfWork.Order.GetFirstOrDefault(u => u.OrderId == orderItem.OrderId);

                if (!order.IsActive)
                {
                    return BadRequest($"Order with id {orderItem.OrderId} is not active. Cannot update order item quantity");
                }

                if(order.Status == StatusType.Paid)
                {
                    return BadRequest($"Order with id {orderItem.OrderId} is alredy paid. Cannot update order item quantity");
                }

                if (order.Status == StatusType.Shipped)
                {
                    return BadRequest($"Order with id {orderItem.OrderId} is alredy Shipped. Cannot update order item quantity");
                }

                if (newQuantity <= 0)
                {
                    // Remove the order item if new quantity is 0
                    orderItem.Price -= product.Price;
                    order.TotalAmount -= orderItem.Quantity * orderItem.Price;
                    product.Quantity += orderItem.Quantity;
                    _unitOfWork.OrderItems.Remove(orderItem);
                }
                else
                {
                    // Check if the product has enough quantity
                    if (product.Quantity < newQuantity)
                    {
                        return BadRequest($"Product with id {product.ProductId} does not have enough quantity");
                    }

                    //When Quntity decresed
                    if (orderItem.Quantity > newQuantity)
                    {
                        var quantity = orderItem.Quantity - newQuantity;
                        if (product.Quantity < newQuantity)
                        {
                            return BadRequest($"Product with id {product.ProductId} does not have enough quantity");
                        }
                        product.Quantity += quantity;
                        orderItem.Quantity = newQuantity;
                        orderItem.Price = product.Price * newQuantity;

                        var price_diff = product.Price * quantity;
                        var newPrice = product.Price - (price_diff * (order.DiscountAmount / 100));

                        order.TotalAmount -= newPrice;
                    }

                    //When Quntity Incresed
                    if (orderItem.Quantity < newQuantity)
                    {
                        var quantity = newQuantity - orderItem.Quantity;
                        if (product.Quantity < newQuantity)
                        {
                            return BadRequest($"Product with id {product.ProductId} does not have enough quantity");
                        }
                        product.Quantity -= quantity;
                        orderItem.Quantity = newQuantity;
                        orderItem.Price = product.Price * newQuantity;

                        var price_diff = product.Price * quantity;
                        var newPrice = product.Price - (price_diff * (order.DiscountAmount / 100));

                        order.TotalAmount += newPrice;
                    }
                }

                order.ModifiedOn = DateTime.Now;
                _unitOfWork.Save();

                return Ok($"Order item with id {orderItemId} quantity updated to {newQuantity}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        #endregion


        #region Remove-Order-Item
        [Authorize]
        [HttpDelete]
        [Route("auth/Remove-Order-Item")]
        public async Task<ActionResult> RemoveOrderItem(int orderItemId)
        {
            try
            {
                var orderItem = _unitOfWork.OrderItems.GetFirstOrDefault(u => u.OrderItemId == orderItemId);

                if (orderItem == null)
                {
                    return BadRequest($"Order item with id {orderItemId} not found");
                }

                if (!orderItem.IsActive)
                {
                    return BadRequest($"Order with id {orderItemId} is not active. Cannot remove order item");
                }

                var product = _unitOfWork.Product.GetFirstOrDefault(u => u.ProductId == orderItem.ProductId);

                if (product == null)
                {
                    return BadRequest($"Product with id {orderItem.ProductId} not found");
                }

                // Retrieve the item's subtotal from the order
                var order = _unitOfWork.Order.GetFirstOrDefault(u => u.OrderId == orderItem.OrderId);
                var subtotal = orderItem.Quantity * orderItem.Price;

                // Remove the item from the order
                _unitOfWork.OrderItems.Remove(orderItem);

                // Subtract the item's subtotal from the order total
                order.TotalAmount -= subtotal;
                product.Quantity += orderItem.Quantity;

                // Save the changes to the database
                _unitOfWork.Save();

                return Ok($"Order item with id {orderItemId} removed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        #endregion


        #region GetOrderByID
        [Authorize]
        [HttpGet]
        [Route("auth/GetOrderByID")]
        public ActionResult<Order> GetOrderById(int orderId)
        {
            try
            {
                var order = _unitOfWork.Order.GetAll(includeProperties: "orderItems,orderItems.Product")
                                  .FirstOrDefault(o => o.OrderId == orderId);

                if (order == null)
                {
                    return BadRequest($"Order with id {orderId} not found");
                }

                // Convert OrderItems into JSON
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    MaxDepth = 64
                };
                var json = JsonSerializer.Serialize(order, options);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        #endregion

    }
}


