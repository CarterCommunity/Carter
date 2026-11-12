namespace SingleFileSample;

using FluentValidation;

public class GreetingValidator : AbstractValidator<Greeting>
{
    public GreetingValidator()
    {
        this.RuleFor(x => x.Name).NotEmpty();
    }
}
