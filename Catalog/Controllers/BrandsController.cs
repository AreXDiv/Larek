using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog.Context;
using LarekLib.Models;

namespace Catalog.Controllers
{
    /// <summary>
    /// Контроллер брендов.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly CatalogContext _context;

        /// <summary>
        /// Конструктор контроллера брендов.
        /// </summary>
        public BrandsController(CatalogContext context)
        {
            _context = context;
        }

        /// <summary>
        ///  Список всех позиции таблицы брендов.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
        {
            if (_context.Brands == null)
            {
                return NotFound();
            }

            return await _context.Brands.ToListAsync();
        }

        /// <summary>
        /// Получить бренд по Id.
        /// </summary>
        /// <param name="id">Id бренда.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            if (_context.Brands == null)
            {
                return NotFound("Контекст пуст.");
            }

            Brand? brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound("Запись не найдена.");
            }

            return brand;
        }

        /// <summary>
        /// Изменить запись в таблице брендов.
        /// </summary>
        /// <param name="brand">Модель бренда.</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> PutBrand([FromQuery] Brand brand) 
        {
            if (!BrandExists(brand.Id))
            {
                return BadRequest("Запись по ИД не найдена.");
            }

            _context.Entry(brand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            { 
                    throw new Exception("Не удалось обновить объект.");
            }

            return NoContent();
        }

        /// <summary>
        /// Добавить запись в таблицу брендов.
        /// </summary>
        /// <param name="brand">Модель бренда.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostBrand([FromQuery] Brand brand)
        {
            if (_context.Brands == null)
            {
                return NotFound("Контекст пуст.");
            }
            _context.Brands.Add(brand);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Не удалось добавить объект.");
            }
            
            return Ok(brand);
        }

        /// <summary>
        /// Удаление бренда из таблицы брендов по Id.
        /// </summary>
        /// <param name="id">Id бренда.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            if (_context.Brands == null)
            {
                return NotFound("Контекст пуст.");
            }
            Brand? brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound("Запись для удаления не найдена.");
            }

            _context.Brands.Remove(brand);
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

        private bool BrandExists(int id)
        {
            return (_context.Brands?.Any(x => x.Id == id)).GetValueOrDefault();
        }
    }
}
