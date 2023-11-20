using System.ComponentModel.DataAnnotations;

namespace LarekLib.Models
{
    public class Product
    {
        public int Id { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Ошибка заполнения, не меньше 2-х и не больше 20-ти символов.")]
        public string Name { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public int BrandId { get; set; }
        public Brand? Brand { get; set; } = null;
        public int CategoryId { get; set; }
        public Category? Category { get; set; } = null;
    }
}
