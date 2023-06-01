using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Final_Exam.Data;
using Darshit_Practical_3_Web.Model;
using Practical_3.Model.Model.Exam_4_Model;
using Practical_3.Model.Model.VM;
using System.Linq;
using System.Collections.Generic;
using Practical_3.Model.Model;
using static Practical_3.Model.Model.Order;
using Newtonsoft.Json.Serialization;

namespace Final_Exam
{
    public class Task
    {

        #region Database Dependency
        private readonly ApplicationDBcontext _db;
        public Task(ApplicationDBcontext db)
        {
            _db = db;
        }
        #endregion

        #region ProductList
        [FunctionName("GetProductList")]
        public async Task<IActionResult> ProductList(
     [HttpTrigger(nameof(AuthorizationLevel.Function), "get", Route = "ProductList")]
 HttpRequest req,
     ILogger log)
        {
            try
            {
                var product = await _db.Products.ToListAsync();
                return new OkObjectResult(product);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error getting address list");
                return new StatusCodeResult(500);
            }
        }
        #endregion

     

        #region Add Address
        [FunctionName("AddAddress")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var Address = JsonConvert.DeserializeObject<AddressModel>(requestBody);

                //Validate input
                if (
                    string.IsNullOrEmpty(Address.Address) ||
                    string.IsNullOrEmpty(Address.Country) ||
                    string.IsNullOrEmpty(Address.State) ||
                    string.IsNullOrEmpty(Address.City) ||
                    string.IsNullOrEmpty(Address.ZipCode) ||
                    string.IsNullOrEmpty(Address.ContactPerson) ||
                    string.IsNullOrEmpty(Address.ContactNo))
                {
                    return new BadRequestObjectResult(new Response { Status = "Error", Message = "Please Enter All Values" });
                }

                var address = new AddressModel
                {
                    AddressType = Address.AddressType,
                    Address = Address.Address,
                    Country = Address.Country,
                    State = Address.State,
                    City = Address.City,
                    ZipCode = Address.ZipCode,
                    ContactPerson = Address.ContactPerson,
                    ContactNo = Address.ContactNo
                };

                _db.Add(address);
                await _db.SaveChangesAsync();

                return new OkObjectResult(address);

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error adding address");
                return new StatusCodeResult(500);
            }
        }
        #endregion



        #region GetAddressList
        [FunctionName("GetAddressList")]
        public async Task<IActionResult> GetAddress(
     [HttpTrigger(nameof(AuthorizationLevel.Function), "get", Route = "GetAddress")]
 HttpRequest req,
     ILogger log)
        {
            try
            {
                var addresses = await _db.Address.ToListAsync();
                return new OkObjectResult(addresses);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error getting address list");
                return new StatusCodeResult(500);
            }
        }
        #endregion



        #region UpdateOrderStatus
        [FunctionName("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus(
    [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateOrderStatus/{OrderId}")] HttpRequest req,
    int OrderId,
    ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var orderStatus = JsonConvert.DeserializeObject<Order>(requestBody);


                var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == OrderId);

                if (order == null)
                {
                    return new BadRequestObjectResult(new Response { Status = "Error", Message = $"Order {OrderId} not found" });
                }

                //shipped order change only in paid
                if (order.Status == StatusType.Shipped && order.Status == StatusType.Paid)
                {
                    return new BadRequestObjectResult(new Response { Status = "Error", Message = "Order is shipped so status cannot be changed to any other status except for Paid" });
                }

                //paid orders can't change
                if (order.Status == StatusType.Paid)
                {
                    return new BadRequestObjectResult(new Response { Status = "Error", Message = "Order is paid so status cannot be changed." });
                }

                // Fetch OrderAddress based on OrderId
                var orderAddress = await _db.OrderAddress
                    .Include(oa => oa.Address)
                    .FirstOrDefaultAsync(oa => oa.OrderId == OrderId);

                // Check if OrderAddress or Address is null
                if (orderStatus.Status == StatusType.Paid && orderAddress.Address.AddressType == AddressTypeEnum.Billing)
                {
                    return new BadRequestObjectResult(new Response { Status = "Error", Message = "Order must have a shipping address before the status can be changed to Paid." });
                }



                order.Status = orderStatus.Status;
                await _db.SaveChangesAsync();

                return new OkResult();

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error updating order status");
                return new StatusCodeResult(500);
            }
        }
        #endregion



        #region UpadateOrderAddress
        [FunctionName("UpdateOrderAddress")]
        public async Task<IActionResult> UpdateOrderAddress(
    [HttpTrigger(AuthorizationLevel.Function, "put", Route = "orders/{orderId}/address")] HttpRequest req,
    ILogger log, int orderId)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var updatedAddress = JsonConvert.DeserializeObject<OrderAddress>(requestBody);

                var orderAddress = await _db.OrderAddress
                   .Include(oa => oa.Address)
                   .FirstOrDefaultAsync(oa => oa.OrderId == orderId);

                if (orderAddress == null)
                {
                    return new BadRequestObjectResult(new Response { Status = "Error", Message = "Order  is Not found" });
                }

                var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

                // Check if OrderAddress or Address is null
                if (order.Status == StatusType.Paid)
                {
                    return new BadRequestObjectResult(new Response { Status = "Error", Message = "Order is paid  so Address cannot be changed." });
                }



                // Update the address ID
                orderAddress.AddressId = updatedAddress.AddressId;

                await _db.SaveChangesAsync();

                return new OkObjectResult(orderAddress);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error updating order address");
                return new StatusCodeResult(500);
            }
        }
        #endregion



        #region Get Order
        [FunctionName("GetOrders")]
        public IActionResult GetOrders(
   [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetOrders")] HttpRequest req,
   ILogger log)
        {
            try
            {
                //All this comes from Params
                string startDateStr = req.Query["startDate"];
                string endDateStr = req.Query["endDate"];
                string customerSerarch = req.Query["customerSerarch"];
                bool.TryParse(req.Query["isActive"], out bool isActive);
                List<string> statusList = req.Query["status"] != "null" ? req.Query["status"].ToString().Split(',').ToList() : new List<string>();
                List<string> productList = req.Query["product"] != "null" ? req.Query["product"].ToString().Split(',').ToList() : new List<string>();


                DateTime startDate, endDate;

                if (string.IsNullOrEmpty(startDateStr) || !DateTime.TryParse(startDateStr, out startDate))
                {
                    // Set default start date logic if Date is less than 5 than start date will be from previus month 
                    startDate = DateTime.Today.Date.Day < 5 ? DateTime.Now.AddMonths(-1) : DateTime.Now;
                    startDate = new DateTime(startDate.Year, startDate.Month, 1);
                }

                if (string.IsNullOrEmpty(endDateStr) || !DateTime.TryParse(endDateStr, out endDate))
                {
                    endDate = DateTime.Now;
                    endDate = new DateTime(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));
                }


                var query = _db.OrderAddress
                    .Include(oa => oa.Order)
                        .ThenInclude(o => o.OrderItems)
                            .ThenInclude(oi => oi.Product)
                    .Include(oa => oa.Address)
                    .Where(oa => oa.Order.OrderDate >= startDate && oa.Order.OrderDate <= endDate)
                    .ToList();


                if (!string.IsNullOrEmpty(customerSerarch))
                {
                    query = _db.OrderAddress.Where(u => u.Order.CustomerName.ToLower() == customerSerarch.ToLower() || u.Order.CustomerEmail.ToLower().Contains(customerSerarch.ToLower())).ToList();
                }

                if (isActive == true)
                {
                    query = query.Where(u => u.Order.IsActive == true).ToList();
                }
                else
                {
                    query = query.Where(u => u.Order.IsActive == false).ToList();
                }

                //check status multiple
                if (statusList.Count > 0)
                {
                    query = query.Where(u => statusList.Contains(u.Order.Status.ToString())).ToList();
                }


                //Check product multiple
                if (productList.Count > 0)
                {
                    query = query.Where(a => a.Order.OrderItems.Any(b => productList.Contains(b.ProductId.ToString()))).ToList();
                }


                //Response 
                var response = query.Select(o => new
                {
                    OrderId = o.Order.OrderId,
                    Description = o.Order.Note,
                    CustomerName = o.Order.CustomerName,
                    CustomerEmail = o.Order.CustomerEmail,
                    Status = o.Order.Status.ToString(),
                    TotalItems = o.Order.OrderItems.Sum(oi => oi.Quantity),
                    TotalAmount = o.Order.TotalAmount,
                    ShippingAddress = $"Address:{o.Address.Address},City: {o.Address.City}, State: {o.Address.State},Country: {o.Address.Country}, Zipcode: {o.Address.ZipCode}"
                });

                return new OkObjectResult(response);

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error creating draft order");
                return new StatusCodeResult(500);
            }
        }
        #endregion



        #region Draft Order
        [FunctionName("CreateDraftOrder")]
        public async Task<IActionResult> CreateDraftOrder(
                [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
                ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var draftOrder = JsonConvert.DeserializeObject<OrderVm>(requestBody);

                if (draftOrder == null)
                {
                    return new BadRequestObjectResult(new Response { Status = "Error", Message = "Please Enter correct Value" });
                }
                if (draftOrder.orderItems == null)
                {
                    return new BadRequestObjectResult(new Response { Status = "Error", Message = "Please Enter correct Order Item" });
                }

                //this is for error 
                var contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };

                //this is for error 
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = contractResolver
                };


                //all are Input Field
                var order = new Order
                {
                    CustomerName = draftOrder.CustomerName,
                    CustomerEmail = draftOrder.CustomerEmail,
                    CustomerContactNo = draftOrder.CustomerContactNo,
                    DiscountAmount = 10,
                    Note = draftOrder.Note,
                    Status = StatusType.Draft,
                    IsActive = true,
                    CreatedOn = DateTime.Now,

                };

                double totalPrice = 0;
                double discount = 0;
                double totalAmount = 0;

                var orderItems = new List<OrderItems>();

                foreach (var item in draftOrder.orderItems)
                {
                    var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

                    if (product == null)
                    {
                        return new BadRequestObjectResult(new Response { Status = "Error", Message = "No product found" });
                    }


                    //check Quantity
                    if (product.Quantity < item.Quantity)
                    {
                        return new BadRequestObjectResult(new Response { Status = "Error", Message = ($"Product {item.ProductId} does not have enough quantity") });
                    }

                    var orderItem = new OrderItems
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = product.Price,
                        IsActive = true
                    };

                    totalPrice += orderItem.Price * orderItem.Quantity;

                    if(order.DiscountAmount >= 0)
                    {
                        //it calculate Discount
                        discount = totalPrice * order.DiscountAmount / 100;
                    }
                   else
                    {
                        return new BadRequestObjectResult(new Response { Status = "Error", Message = "Discount value is not less than zero" });
                    }


                    //its give final amount
                    totalAmount = totalPrice - discount;


                    //decrease product quantity
                    product.Quantity -= item.Quantity;
                    _db.Products.Update(product);

                    //add order item
                    orderItems.Add(orderItem);

                }

                order.OrderItems = orderItems;
                order.TotalAmount = totalAmount;

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();


                var address = await _db.Address.FirstOrDefaultAsync(a => a.AddressId == draftOrder.AddressId);

                if (address == null)
                {
                    return new BadRequestObjectResult(new Response { Status = "Error", Message = $"Address {draftOrder.AddressId} not found" });
                }

                var orderAddress = new OrderAddress
                {
                    Order = order,
                    Address = address,
                };


                _db.OrderAddress.Add(orderAddress);
                await _db.SaveChangesAsync();

                string structuredOrder = JsonConvert.SerializeObject(order, settings);

                return new OkObjectResult(structuredOrder);

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error creating draft order");
                return new StatusCodeResult(500);
            }
        }
        #endregion


    }
}



//http://localhost:7047/api/GetOrders?startDate=&endDate=&customerSerarch=Darshit&isActive=true&status=Open&product=1