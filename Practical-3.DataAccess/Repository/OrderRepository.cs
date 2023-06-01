using Darshit_Practical_3_Web.Data;
using Darshit_Practical_3_Web.Model;
using Practical_3.DataAccess.Repository.IRepository;
using Practical_3.Model.Model;

namespace Practical_3.DataAccess.Repository
{ 
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Order order)
        {
            _db.Orders.Update(order);
        }
    }
}
