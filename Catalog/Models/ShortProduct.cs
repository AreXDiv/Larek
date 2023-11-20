using System.ComponentModel.DataAnnotations;

namespace Catalog.Models
{
    public class ShortProduct
    {
        public int Id { get; set; }

        [MaxLength(20)]
        [MinLength(2)]
        public string Name { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
    }
}
