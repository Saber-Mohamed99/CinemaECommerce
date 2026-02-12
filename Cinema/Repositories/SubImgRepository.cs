using CinemaECommerce.Repositories.IRepositories;

namespace CinemaECommerce.Repositories
{
    public class SubImgRepository :Repository<MovieSubImg>,IMovieSubImgRepository
    {
        private ApplicationDbContext _context;//=new();

        public SubImgRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public void DeleteRange(List<MovieSubImg> productSubImgs)
        {
            _context.RemoveRange(productSubImgs);
        }
    }
}
