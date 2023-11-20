using System.ComponentModel.DataAnnotations;
using LarekLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.Client;
using Orders.Context;

namespace Orders.Controllers
{
    /// <summary>
    /// Контроллер заказов.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersContext _context;
        private readonly ICatalogApi catalogApi;
        private readonly IDeliveryApi deliveryApi;

        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        public OrdersController(OrdersContext context, ICatalogApi catalogApi, IDeliveryApi deliveryApi)
        {
            _context = context;
            this.catalogApi = catalogApi;
            this.deliveryApi = deliveryApi;
        }

        /// <summary>
        /// Список всех позиций таблицы заказов.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersModel>>> GetOrders()
        {
            if (_context.Orders == null)
            {
                return NotFound("Контекст пуст.");
            }
            return await _context.Orders.ToListAsync();
        }

        /// <summary>
        /// Получить заказ по Id.
        /// </summary>
        /// <param name="id">Id заказа.</param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<OrdersModel>> GetOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound("Контекст пуст.");
            }

            var ordersModel = await _context.Orders.FindAsync(id);
            if (ordersModel == null)
            {
                return NotFound("Запись не найдена.");
            }

            return ordersModel;
        }

        /// <summary>
        /// Изменить запись в таблице заказов.
        /// </summary>
        /// <param name="orderId">Id заказа.</param>
        /// <param name="name">Имя заказчика.</param>
        /// <param name="isDelivery">Производить доставку?</param>
        /// <param name="phone">Телефон заказчика.</param>
        /// <param name="address">Адрес доставки.</param>
        /// <param name="dateTime">Дата доставки.</param>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<IActionResult> PutOrder(int orderId ,string name, bool isDelivery, 
            string? phone, string? address,
            [DataType(DataType.Date)] DateTime? dateTime)
        {
            OrdersModel? ordersModel = await _context.Orders.FindAsync(orderId);
            if (ordersModel == null)
            {
                return BadRequest("Запись по ИД не найдена.");
            }

            bool oldIsDelivery = ordersModel.Delivery;

            ordersModel.CustomerName = name;
            ordersModel.Delivery = isDelivery;

            _context.Entry(ordersModel).State = EntityState.Modified;
           
            try
            {
                if (isDelivery && !oldIsDelivery)
                {
                    if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(address) && dateTime > DateTime.Now)
                    {
                        _ = await deliveryApi.CreateDelivery(new DeliveryModel
                        {
                            OrderId = orderId,
                            DeliveryLocation = address,
                            DateOrder = dateTime,
                            IsCollected = false,
                            IsDelivered = false,
                            Phone = phone
                        });
                    }
                    else
                    {
                        return NotFound("Контекст для доставки заполнен не верно " +
                            "или указаны не все обязательные поля.");
                    }
                }
                else if (!isDelivery && oldIsDelivery)
                {
                   var delivery = await deliveryApi.GetDeliveryByOrderId(orderId);
                   await deliveryApi.DeleteDelivery(delivery.Id);
                }
                else if (isDelivery && oldIsDelivery)
                {
                    if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(address) && dateTime > DateTime.Now)
                    {
                        var delivery = await deliveryApi.GetDeliveryByOrderId(orderId);
                        delivery.DeliveryLocation = address;
                        delivery.DateOrder = dateTime;
                        delivery.Phone = phone;
                        await deliveryApi.PutDelivery(delivery);
                    }
                    else
                    {
                        return NotFound("Контекст для доставки заполнен не верно " +
                            "или указаны не все обязательные поля.");
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Ой-ёй, что-то пошло не так!");
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var delivery = await deliveryApi.GetDeliveryByOrderId(orderId);
                await deliveryApi.DeleteDelivery(delivery.Id);
                throw new Exception("Не удалось обновить объект.");
            }

            return NoContent();
        }

        /// <summary>
        /// Выдать заказ.
        /// </summary>
        /// /// <param name="orderId">Id заказа.</param>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<IActionResult> ToIssue(int orderId)
        {
            if (!OrdersModelExists(orderId))
            {
                return BadRequest("Запись но ИД не найдена.");
            }

            OrdersModel ordersModel = await _context.Orders.FirstAsync(x => x.Id == orderId);
            ordersModel.Payment = true;

            _context.Entry(ordersModel).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Запись о выдаче не прошла.");
            }
            return Ok();
        }

        /// <summary>
        /// Оформление заказа.
        /// </summary>
        /// <param name="productId">Id продукта.</param>
        /// <param name="count">Количество заказанного продукта.</param>
        /// <param name="name">Имя заказчика.</param>
        /// <param name="isDelivery">Доставка?</param>
        /// <param name="phone">Телефон заказчика.</param>
        /// <param name="address">Адрес доставки.</param>
        /// <param name="dateTime">Дата доставки.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAnOrder(int productId, int count, string name,
            bool isDelivery, string? phone,
            [StringLength(20, MinimumLength = 2,
            ErrorMessage = "Ошибка заполнения, не меньше 2-х и не больше 20-ти символов.")] string? address, 
            [DataType(DataType.Date)] DateTime? dateTime)
        {
            if (_context.Orders == null)
            {
                return NotFound("Контекст пуст.");
            }

            OrdersModel ordersModel;
            try
            {
                Product product = await catalogApi.GetProduct(productId);
                
                if (product.Count < count || count <= 0)
                {
                    return NotFound("Столько товара нет на складе.");
                }

                ordersModel = new OrdersModel
                {
                    ProductId = productId,
                    Count = count,
                    Delivery = isDelivery,
                    Payment = false,
                    CustomerName = name,
                    Summ = count * product.Price
                };
                product.Count -= count;

                _ = await catalogApi.PutProduct(product);

                _context.Orders.Add(ordersModel);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Не удалось добавить объект.");
            }

            try
            {
                if (isDelivery)
                {
                    if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(address) && dateTime > DateTime.Now)
                    {
                        _ = await deliveryApi.CreateDelivery(new DeliveryModel
                        {
                            OrderId = ordersModel.Id,
                            DeliveryLocation = address,
                            DateOrder = dateTime,
                            IsCollected = false,
                            IsDelivered = false,
                            Phone = phone
                        });
                    }
                    else
                    {
                        return NotFound("Контекст для доставки заполнен не верно " +
                            "или указаны не все обязательные поля.");
                    }
                }
            }
            catch (Exception)
            {
                await DeleteOrder(ordersModel.Id);
                throw new Exception("Ой-ёй, что-то пошло не так!");
            }

            return Ok(ordersModel);
        }

        /// <summary>
        /// Удаление заказа из таблицы заказов по Id.
        /// </summary>
        /// <param name="id">Id заказа.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound("Контекст пуст");
            }

            var ordersModel = await _context.Orders.FindAsync(id);
            if (ordersModel == null)
            {
                return NotFound("Запись для удаления не найдена");
            }

            Product product = await catalogApi.GetProduct(ordersModel.ProductId);
            product.Count += ordersModel.Count;

            _context.Orders.Remove(ordersModel);
            try
            {
                var delivery = await deliveryApi.GetDeliveryByOrderId(id);
                if (delivery != null)
                {
                    await deliveryApi.DeleteDelivery(delivery.Id);
                }

                await _context.SaveChangesAsync();
                await catalogApi.PutProduct(product);
            }
            catch (Exception)
            {
                throw new Exception("Удаление из базы данных не удалось.");
            }
            
            return NoContent();
        }

        private bool OrdersModelExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
