namespace CarterSample.Features.Actors;

public interface IGetActorsQuery
{
    IEnumerable<Actor> Execute();
}