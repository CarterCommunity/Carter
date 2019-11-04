namespace ValidatorOnlyProject
{
    using FluentValidation;

    public class CastMemberValidator : AbstractValidator<CastMember>
    {
        public CastMemberValidator()
        {
            this.RuleFor(x => x.Name).NotEmpty();
        }
    }
}
