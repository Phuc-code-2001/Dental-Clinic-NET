using System;
using System.Linq;

namespace Dental_Clinic_NET.API.Utils
{
    public class SearchFilter<T> : PageFilter
    {
        public string Keyword { get; set; }

        public virtual IQueryable<T> FilteredQuery(IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> handleLogic)
        {
            if(string.IsNullOrWhiteSpace(Keyword))
            {
                return source;
            }
            return handleLogic(source, Keyword);
        }

    }
}
