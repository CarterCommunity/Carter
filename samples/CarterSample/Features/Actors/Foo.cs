namespace CarterSample.Features.Actors
{
    using System.Collections.Generic;

    public class Foo
    {
        public IEnumerable<Bar> EnumBars { get; set; }

        public List<Bar> ListBars { get; set; }
    }
}