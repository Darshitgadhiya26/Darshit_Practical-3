namespace Practical_3.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork 
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IOrderRepository Order { get; }
        IOrderItemsRepository OrderItems { get; }
        void Save();
    }
}
