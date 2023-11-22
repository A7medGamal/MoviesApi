using AutoMapper;

namespace MoviesApi.Helper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie,MovieDetailsDto>();
            CreateMap<MovieDetailsDto, Movie>().ForMember(src=>src.poster,opt=>opt.Ignore());
        }
    }
}
