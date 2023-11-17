namespace LarekLib.Models
{
    public class DeliveryModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string DeliveryLocation { get; set; }
        public DateTime? DateOrder { get; set; }
        public bool IsDelivered { get; set; } = false;
        public bool IsCollected { get; set; } = false;
        public string Phone { get; set; }
    }
}
