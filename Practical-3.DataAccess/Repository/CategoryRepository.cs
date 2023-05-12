
using Darshit_Practical_3_Web.Data;
using Darshit_Practical_3_Web.Model;
using Practical_3.DataAccess.Repository.IRepository;

namespace Practical_3.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
