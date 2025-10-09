namespace CarterSample.Features.Actors;

using FluentValidation;

[ValidatorLifetimeAttribute(ServiceLifetime.Scoped)]
public class ActorValidator : AbstractValidator<Actor>
{
    public ActorValidator()
    {
        this.RuleFor(x => x.Name).NotEmpty();

        this.RuleFor(x => x.Age).GreaterThan(0);
    }
}