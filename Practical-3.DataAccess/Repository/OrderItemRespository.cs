using Darshit_Practical_3_Web.Data;
using Darshit_Practical_3_Web.Model;
using Practical_3.DataAccess.Repository.IRepository;
using Practical_3.Model.Model;

namespace Practical_3.DataAccess.Repository
{
    public class OrderItemRespository : Repository<OrderItems>, IOrderItemsRepository
    {
        private ApplicationDbContext _db;

        public OrderItemRespository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Remove(OrderItems item)
        {
            _db.Remove(item);
        }

        public void Update(Order order)
        {
            _db.Orders.Update(order);
        }

        
    }
}
