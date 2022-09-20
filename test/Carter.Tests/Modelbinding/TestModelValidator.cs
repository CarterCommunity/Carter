namespace Carter.Tests.Modelbinding
{
    using FluentValidation;

    public class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            this.RuleFor(x => x.MyIntProperty).GreaterThan(0);
            this.RuleFor(x => x.MyStringProperty).NotEmpty();
        }
    }
}