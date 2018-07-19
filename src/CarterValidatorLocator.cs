namespace Carter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using static System.String;

    public class CarterValidatorLocator : IValidatorLocator
    {
        private readonly Dictionary<Type, IValidator> validators = new Dictionary<Type, IValidator>();

        public CarterValidatorLocator(IEnumerable<IValidator> validators)
        {
            foreach (var validator in validators) 
            {
                var validatorType = validator.GetType();
                // ReSharper disable once PossibleNullReferenceException
                var modelType = validatorType.BaseType.GetGenericArguments()[0];
                if (this.validators.ContainsKey(modelType))
                {
                    throw new InvalidOperationException(
                        $"Ambiguous choice between multiple validators for type {validator.GetType().Name}. " +
                        $"The validators available are: {Join(", ", validators.Select(v => v.GetType().Name))}");
                }
                this.validators.Add(modelType, validator);
            }
        }

        public IValidator GetValidator<T>() 
            => this.validators.ContainsKey(typeof(T)) ? this.validators[typeof(T)] : null;
    }
}