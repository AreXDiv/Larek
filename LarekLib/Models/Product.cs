namespace LarekLib.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public int BrandId { get; set; }
        public Brand? Brand { get; set; } = null;
        public int CategoryId { get; set; }
        public Category? Category { get; set; } = null;
    }
}
