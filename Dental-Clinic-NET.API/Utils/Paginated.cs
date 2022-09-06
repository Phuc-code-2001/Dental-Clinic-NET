using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dental_Clinic_NET.API.Utils
{
    public class Paginated<T>
    {

        public int PageSize = 6;

        public bool HasPrevious { get; private set; }
        public bool HasNext { get; private set; }

        public int PageCount { get; private set; }
        public int ColectionCount { get; private set; }

        public IQueryable<T> Items;

        public Paginated(IQueryable<T> collection, int pageIndex)
        {
            ColectionCount = collection.Count<T>();
            PageCount = (int) Math.Ceiling((double) ColectionCount / PageSize);

            // Page 1 => pageSize first elements
            // Page 2 => skip pageSize * 1 element, take pageSize element
            Items = collection.Skip(PageSize * (pageIndex - 1)).Take(PageSize);

            HasNext = pageIndex + 1 <= PageCount;
            HasPrevious = pageIndex - 1 > 0;
        }

        
    }
}
