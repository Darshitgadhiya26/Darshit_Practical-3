using Darshit_Practical_3_Web.Data;
using Practical_3.DataAccess.Repository;
using Practical_3.DataAccess.Repository.IRepository;

namespace Practical_3.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Order = new OrderRepository(_db);
            OrderItems = new OrderItemRespository(_db);
        }

        public ICategoryRepository Category { get;  set; }
        public IOrderRepository Order { get;  set; }
        public IProductRepository Product { get; set; }
        public IOrderItemsRepository OrderItems { get; set; }


        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
