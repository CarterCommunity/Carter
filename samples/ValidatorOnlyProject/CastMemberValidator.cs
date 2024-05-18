namespace ValidatorOnlyProject
{
    using System.Threading.Tasks;
    using FluentValidation;

    public class CastMemberValidator : AbstractValidator<CastMember>
    {
        public CastMemberValidator()
        {
            this.RuleFor(x => x.Name).NotEmpty().MustAsync(async (name, cancellation) =>
            {
                await Task.Delay(50);
                return true;
            });
        }
    }
}
