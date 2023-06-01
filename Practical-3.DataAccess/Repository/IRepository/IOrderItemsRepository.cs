using Darshit_Practical_3_Web.Model;
using Practical_3.Model.Model;

namespace Practical_3.DataAccess.Repository.IRepository
{
    public interface IOrderItemsRepository : IRepository<OrderItems>
    {
        void Remove(OrderItems item);
        void Update(Order order);
    }
}
