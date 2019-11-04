namespace CarterSample.Features.FunctionalProgramming
{
    using FluentValidation;

    public class DirectorValidator : AbstractValidator<Director>
    {
        public DirectorValidator()
        {
            this.RuleFor(x => x.Name).NotEmpty();
        }
    }
}
