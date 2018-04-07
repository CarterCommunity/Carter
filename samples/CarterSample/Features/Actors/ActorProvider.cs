namespace CarterSample.Features.Actors
{
    using System.Collections.Generic;
    using System.Linq;

    public class ActorProvider : IActorProvider
    {
        private static IEnumerable<Actor> database = new[] { new Actor { Name = "Brad Pitt", Id = 1, Age = 51 }, new Actor { Name = "Jason Statham", Id = 2, Age = 43 } };

        public IEnumerable<Actor> Get()
        {
            return database;
        }

        public Actor Get(int id)
        {
            return database.FirstOrDefault(x => x.Id == id);
        }
    }
}