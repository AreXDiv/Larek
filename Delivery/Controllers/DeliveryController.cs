using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery.Context;
using LarekLib.Models;
using Delivery.Client;

namespace Delivery.Controllers
{
    /// <summary>
    /// Контроллер доставки.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly DeliveryContext _context;
        private readonly IOrderApi orderApi;

        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        public DeliveryController(DeliveryContext context , IOrderApi orderApi)
        {
            _context = context;
            this.orderApi = orderApi;
        }

        /// <summary>
        /// Список всех позиции таблицы доставки, которые не доставлены.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryModel>>> GetDeliveries()
        {
            if (_context.Deliveries == null)
            {
                return NotFound("Контекст пуст.");
            }
            return await _context.Deliveries.Where(x => !x.IsDelivered).ToListAsync();
        }

        /// <summary>
        /// Получить позицию доставки по Id.
        /// </summary>
        /// <param name="id">Id доставки.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryModel>> GetDelivery(int id)
        {
            if (_context.Deliveries == null)
            {
                return NotFound("Контекст пуст.");
            }

            DeliveryModel? deliveryModel = await _context.Deliveries.FindAsync(id);
            if (deliveryModel == null)
            {
                return NotFound("Запись не найдена.");
            }

            return deliveryModel;
        }

        /// <summary>
        /// Получить запись доставки по заказу.
        /// </summary>
        /// <param name="orderId">Id заказа.</param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<DeliveryModel>> GetDeliveryByOrderId(int orderId)
        {
            if (_context.Deliveries == null)
            {
                return NotFound("Контекст пуст.");
            }

            DeliveryModel? deliveryModel = await _context.Deliveries.FirstOrDefaultAsync(x => x.OrderId == orderId && !x.IsDelivered);
            if (deliveryModel == null)
            {
                return NotFound("Запись не найдена.");
            }

            return deliveryModel;
        }

        /// <summary>
        /// Собрать заказ или разобрать.
        /// </summary>
        /// <param name="id">Id доставки.</param>
        /// <param name="toCollect">Собрать?</param>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<IActionResult> ToCollect(int id, bool toCollect)
        {
            if (!DeliveryModelExists(id))
            {
                return BadRequest("Запись по ИД не найдена");
            }
            DeliveryModel deliveryModel =  await _context.Deliveries.FirstAsync(x => x.Id == id);
            deliveryModel.IsCollected = toCollect;

            _context.Entry(deliveryModel).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Запись о сборе товара не произошла.");
            }
            return Ok();
        }

        /// <summary>
        /// Отметить, что заказ доставлен.
        /// </summary>
        /// <param name="id">Id заказа.</param>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<IActionResult> ToDeliver(int id)
        {
            if (!DeliveryModelExists(id))
            {
                return BadRequest("Запись по ИД не найдена");
            }
            DeliveryModel deliveryModel = await _context.Deliveries.FirstAsync(x => x.Id == id);
            deliveryModel.IsDelivered = true;

            _context.Entry(deliveryModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await orderApi.ToIssue(deliveryModel.OrderId);
            }
            catch (Exception)
            {
                throw new Exception("Запись о сборе товара не произошла.");
            }
            return Ok();
        }

        /// <summary>
        /// Добавить запись в таблицу доставки.
        /// </summary>
        /// <param name="deliveryModel">Модель доставки.</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<ActionResult<DeliveryModel>> CreateDelivery(DeliveryModel deliveryModel)
        {
            if (_context.Deliveries == null)
            {
                return NotFound("Контекст пуст.");
            }

            _context.Deliveries.Add(deliveryModel);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Не удалось добавить объект.");
            }

            return Ok(deliveryModel);
        }

        /// <summary>
        /// Удаление доставки из таблицы доставок по Id.
        /// </summary>
        /// <param name="id">Id доставки.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelivery(int id)
        {
            if (_context.Deliveries == null)
            {
                return NotFound("Контекст пуст");
            }

            DeliveryModel? deliveryModel = await _context.Deliveries.FindAsync(id);
            if (deliveryModel == null)
            {
                return NotFound("Запись для удаления не найдена.");
            }

            _context.Deliveries.Remove(deliveryModel);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception ("Удаление из базы данных не удалось.");
            }
           
            return Ok();
        }

        /// <summary>
        /// Отменить доставку заказа.
        /// </summary>
        /// <param name="id">Id отменяемой доставки.</param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        public async Task<IActionResult> CancelOrderDelivery(int id)
        {
            if (_context.Deliveries == null)
            {
                return NotFound("Контекст пуст.");
            }
            try
            {
                int? orderId = (await _context.Deliveries.FindAsync(id))?.OrderId;

                if (orderId == null)
                {
                    return NotFound("Запись доставки не найдена.");
                }
                OrdersModel ordersModel = await orderApi.GetOrder(orderId.Value);

                await orderApi.PutOrder(ordersModel.Id, ordersModel.CustomerName, false, null, null, null);
            }
            catch (Exception)
            {
                throw new Exception("Не удалось отменить доставку.");
            }

            return Ok();
        }

        private bool DeliveryModelExists(int id)
        {
            return (_context.Deliveries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
