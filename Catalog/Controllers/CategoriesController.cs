using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog.Context;
using LarekLib.Models;


namespace Catalog.Controllers
{
    /// <summary>
    /// Контроллер категорий.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CatalogContext _context;

        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        public CategoriesController(CatalogContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Список всех позиции таблицы категорий.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            if (_context.Categories == null)
            {
                return NotFound("Контекст пуст.");
            }
            return await _context.Categories.ToListAsync();
        }

        /// <summary>
        /// Получить категорию по Id.
        /// </summary>
        /// <param name="id">Id категории.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound("Контекст пуст.");
            }

            Category? category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Запись не найдена");
            }

            return category;
        }

        /// <summary>
        /// Изменить запись в таблице категорий.
        /// </summary>
        /// <param name="category">Модель кетегории.</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> PutCategory([FromQuery] Category category)
        {
            if (!CategoryExists(category.Id))
            {
                return BadRequest("Запись по ИД не найдена.");
            }

            _context.Entry(category).State = EntityState.Modified;

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
        /// Добавить запись в таблицу категорий.
        /// </summary>
        /// <param name="category">Модель кетегории.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostCategory([FromQuery] Category category)
        {
            if (_context.Categories == null)
            {
                return NotFound("Контекст пуст.");
            }

            _context.Categories.Add(category);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Не удалось добавить объект.");
            }

            return Ok(category);
        }

        /// <summary>
        /// Удаление категории из таблицы категорий по Id.
        /// </summary>
        /// <param name="id">Id категории.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (_context.Categories == null )
            {
                return NotFound("Контекст пуст.");
            }

            Category? category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Запись для удаления не найдена.");
            }

            _context.Categories.Remove(category);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Не удалось удалить объект.");
            }

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
