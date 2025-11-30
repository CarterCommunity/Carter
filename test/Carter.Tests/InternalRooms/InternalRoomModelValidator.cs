namespace Carter.Tests.InternalRooms;

using FluentValidation;

internal class InternalRoomModelValidator : AbstractValidator<InternalRoomModel>
{
    public InternalRoomModelValidator()
    {
        this.RuleFor(x => x.Name).NotEmpty();
    }
}
