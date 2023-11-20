using System.ComponentModel.DataAnnotations;

namespace LarekLib.Models
{
    public class OrdersModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public bool Delivery { get; set; }
        public bool Payment { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Ошибка заполнения, не меньше 2-х и не больше 20-ти символов.")]
        public string CustomerName { get; set; }
        public double Summ { get; set; }
    }
}
