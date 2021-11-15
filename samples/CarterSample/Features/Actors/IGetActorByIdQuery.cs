namespace CarterSample.Features.Actors;

public interface IGetActorByIdQuery
{
    Actor Execute(int id);
}