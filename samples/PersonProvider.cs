namespace Botwin.Samples
{
    using System.Collections.Generic;
    using System.Linq;

    public class PersonProvider : IPersonProvider
    {
        private static IEnumerable<Person> database = new[] { new Person { Name = "Brad Pitt", Id = 1, Age = 51 }, new Person { Name = "Jason Statham", Id = 2, Age = 43 } };

        public IEnumerable<Person> Get()
        {
            return database;
        }

        public Person Get(int id)
        {
            return database.FirstOrDefault(x => x.Id == id);
        }
    }
}