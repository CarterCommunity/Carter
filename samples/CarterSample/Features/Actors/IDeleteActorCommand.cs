namespace CarterSample.Features.Actors;

public interface IDeleteActorCommand
{
    void Execute(int id);
}