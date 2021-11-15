namespace CarterSample.Features.Actors;

public class GetActorByIdQuery : IGetActorByIdQuery
{
    public Actor Execute(int id)
    {
        return ActorsModule.ActorsDatabase.FirstOrDefault(x => x.Id == id);
    }
}
