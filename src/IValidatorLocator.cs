namespace Carter
{
    using FluentValidation;

    /// <summary>
    /// An interface to provide functionaity on locating a <see cref="IValidator"/>
    /// </summary>
    public interface IValidatorLocator
    {
        /// <summary>
        /// Gets a validator for a given type
        /// </summary>
        /// <typeparam name="T">The model to find the validator for</typeparam>
        /// <returns>The <see cref="IValidator"/> for the model</returns>
        IValidator GetValidator<T>();
    }
}