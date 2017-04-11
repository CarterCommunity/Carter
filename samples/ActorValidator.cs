namespace Botwin.Samples
{
    using FluentValidation;

    public class ActorValidator : AbstractValidator<Actor>
    {
        public ActorValidator()
        {
            this.RuleFor(x => x.Name).NotEmpty();
        }
    }
}