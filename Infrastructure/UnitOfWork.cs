using Application;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _context;

        public UnitOfWork(AppDBContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public bool IsSuccessCommited()
        {
            if (_context.SaveChanges() > 0) return true;

            return false;
        }
    }
}
