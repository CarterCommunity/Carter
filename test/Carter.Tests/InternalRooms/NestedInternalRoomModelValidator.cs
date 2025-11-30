namespace Carter.Tests.InternalRooms;

using FluentValidation;

internal static class NestedInternalTestModelValidatorWrapper
{
    internal class InternalTestModelValidator : AbstractValidator<InternalRoomModel>
    {
        public InternalTestModelValidator()
        {
            this.RuleFor(x => x.Name).NotEmpty();
        }
    }
}

