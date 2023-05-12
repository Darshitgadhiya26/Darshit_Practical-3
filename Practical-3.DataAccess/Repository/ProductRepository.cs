using Darshit_Practical_3_Web.Data;
using Darshit_Practical_3_Web.Model;
using Practical_3.DataAccess.Repository.IRepository;

namespace Practical_3.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            _db.Products.Update(product);
        }
    }
}
