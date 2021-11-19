namespace CarterSample.Features.Actors;

public class GetActorsQuery : IGetActorsQuery
{
    public IEnumerable<Actor> Execute()
    {
        return ActorsModule.ActorsDatabase;
    }
}
