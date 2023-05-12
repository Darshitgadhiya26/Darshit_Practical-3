using Darshit_Practical_3_Web.Model;
using Practical_3.Model.Model.VM;

namespace Practical_3.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
       
        void Update(Order order);
    }
}
