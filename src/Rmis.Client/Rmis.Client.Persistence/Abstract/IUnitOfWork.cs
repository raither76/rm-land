namespace Rmis.Client.Persistence.Abstract
{
    public interface IUnitOfWork
    {
        int SaveChanges();
    }
}