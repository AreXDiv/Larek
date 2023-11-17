using System.ComponentModel.DataAnnotations;

namespace LarekLib.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string? Name { get; set; }
    }
}
