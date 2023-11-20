using System.ComponentModel.DataAnnotations;

namespace LarekLib.Models
{
    public class DeliveryModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Ошибка заполнения, не меньше 3-х и не больше 100-ти символов.")]
        public string DeliveryLocation { get; set; }
        public DateTime? DateOrder { get; set; }
        public bool IsDelivered { get; set; } = false;
        public bool IsCollected { get; set; } = false;

        public string Phone { get; set; }
    }
}
