namespace CarterSample.Features.Actors
{
    using System.Collections.Generic;

    public class PagedItems<T>
    {
        public IEnumerable<T> Data { get; }
    }
}