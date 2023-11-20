using System.ComponentModel.DataAnnotations;

namespace LarekLib.Models
{
    public class Category
    {
        public int Id { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Ошибка заполнения, не меньше 2-х и не больше 20-ти символов.")]
        public string Name { get; set; }
    }
}
