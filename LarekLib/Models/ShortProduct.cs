namespace LarekLib.Models
{
    public class ShortProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
    }
}
