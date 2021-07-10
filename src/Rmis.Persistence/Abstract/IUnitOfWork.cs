namespace Rmis.Persistence.Abstract
{
    public interface IUnitOfWork
    {
        int SaveChanges();
    }
}