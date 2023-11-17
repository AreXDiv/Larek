using LarekLib.Models;
using Refit;

namespace Orders.Client
{
    public interface IDeliveryApi
    {
        [Get("/api/Delivery/GetDeliveryByOrderId")]
        Task<DeliveryModel> GetDeliveryByOrderId(int orderId);

        [Post("/api/Delivery/CreateDelivery")]
        Task<DeliveryModel> CreateDelivery(DeliveryModel deliveryModel);

        [Put("/api/Delivery/PutDelivery")]
        Task<DeliveryModel> PutDelivery(DeliveryModel deliveryModel);

        [Delete("/api/Delivery/{id}")]
        Task DeleteDelivery(int id);
    }
}
