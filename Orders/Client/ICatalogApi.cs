using LarekLib.Models;
using Refit;

namespace Orders.Client
{
    public interface ICatalogApi
    {
        [Get("/api/Products/{id}")]
        Task<Product> GetProduct(int id);

        [Put("/api/Products/PutProduct")]
        Task<Product> PutProduct(Product product);
    }
}
