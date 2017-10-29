using FluentValidation;

namespace Botwin
{
    public interface IValidatorLocator
    {
        IValidator GetValidator<T>();
    }
}