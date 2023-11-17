using LarekLib.Models;
using Refit;
using System.ComponentModel.DataAnnotations;

namespace Delivery.Client
{
    public interface IOrderApi
    {
        [Put("/api/Orders/ToIssue")]
        Task ToIssue(int orderId);

        [Get("/api/Orders/GetOrder")]
        Task<OrdersModel> GetOrder(int id);

        [Put("/api/Orders/PutOrder")]
        Task PutOrder(int orderId, string name, bool isDelivery,
            string? phone, string? address,
            [DataType(DataType.Date)] DateTime? dateTime);
    }
}
