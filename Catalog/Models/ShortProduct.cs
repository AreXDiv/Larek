using System.ComponentModel.DataAnnotations;

namespace Catalog.Models
{
    public class ShortProduct
    {
        public int Id { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Ошибка заполнения, не меньше 2-х и не больше 20-ти символов.")]
        public string Name { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
    }
}
