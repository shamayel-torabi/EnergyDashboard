using System.Collections;

namespace AutoMapper.QueryableExtensions;

public static class AutoMapperExtensions
{
    public static IEnumerable<TDestination> ProjectTo<TDestination>(this IEnumerable source, IMapper mapper)
    {
        var result = new List<TDestination>();

        foreach (var item in source)
        {
            var n = mapper.Map<TDestination>(item);
            result.Add(n);
        }

        return result;
    }
}

