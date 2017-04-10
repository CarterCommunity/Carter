namespace Botwin.Samples
{
    using System.Collections.Generic;

    public interface IPersonProvider
    {
        IEnumerable<Person> Get();
        Person Get(int id);
    }
}