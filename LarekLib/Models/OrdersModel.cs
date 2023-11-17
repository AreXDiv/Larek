namespace LarekLib.Models
{
    public class OrdersModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public bool Delivery { get; set; }
        public bool Payment { get; set; }
        public string CustomerName { get; set; }
        public double Summ { get; set; }
    }
}
