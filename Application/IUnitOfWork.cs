namespace Application
{
    public interface IUnitOfWork
    {
        public void Commit();
        public bool IsSuccessCommited();
    }
}
