namespace CinemaECommerce.Repositories.IRepositories
{
    public interface IMovieSubImgRepository:IRepository<MovieSubImg>
    {
        void DeleteRange(List<MovieSubImg> movieSubImgs);
    }
}
