using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog.Context;
using LarekLib.Models;
using System.Xml.Linq;

namespace Catalog.Controllers
{
    /// <summary>
    /// Контроллер продуктов.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CatalogContext _context;

        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        public ProductsController(CatalogContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Список всех позиции таблицы продуктов.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            if (_context.Products == null)
            {
                return NotFound("Контекст пуст.");
            }
            return await _context.Products
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .ToListAsync();
        }

        /// <summary>
        /// Получить продукт по Id.
        /// </summary>
        /// <param name="id">Id продукта.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound("Контекст пуст.");
            }

            Product? product = await _context.Products
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound("Запись не найдена.");
            }

            return Ok(product);
        }

        /// <summary>
        /// Изменить запись в таблице продуктов.
        /// </summary>
        /// <param name="product">Модель продукта.</param>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<ActionResult<Product>> PutProduct(Product product)
        {
            if (!ProductExists(product.Id))
            {
                return BadRequest("Запись по ИД не найдена.");
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Не удалось обновить объект.");
            }

            return Ok(product);
        }

        /// <summary>
        /// Добавить запись в таблицу продуктов.
        /// </summary>
        /// <param name="product">Модель продукта.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            if (_context.Products == null)
            {
                return NotFound("Контекст пуст.");
            }

            //_context.Products.Add(new Product 
            //{
            //    Name = product.Name,
            //    Count = product.Count,
            //    Price = product.Price,
            //    BrandId = product.BrandId,
            //    CategoryId = product.CategoryId
            //});
            _context.Products.Add(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw new Exception("Не удалось добавить объект.");
            }

            return Ok(product);
        }

        /// <summary>
        /// Удаление продукта из таблицы продуктов по Id.
        /// </summary>
        /// <param name="id">Id продукта.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound("Контекст пуст.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Запись для удаления не найдена.");
            }

            _context.Products.Remove(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Удаление из базы данных не удалось.");
            }

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
