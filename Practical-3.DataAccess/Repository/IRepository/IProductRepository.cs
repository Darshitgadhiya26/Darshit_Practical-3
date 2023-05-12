using Darshit_Practical_3_Web.Model;


namespace Practical_3.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);

    }
}
