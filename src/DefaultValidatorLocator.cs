namespace Botwin
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FluentValidation;

    public class DefaultValidatorLocator : IValidatorLocator
    {
        private readonly IEnumerable<IValidator> validators;

        private readonly ConcurrentDictionary<Type, IValidator> foundValidators = new ConcurrentDictionary<Type, IValidator>();

        public DefaultValidatorLocator(IEnumerable<IValidator> validators) => this.validators = validators;

        public IValidator GetValidator<T>() => this.foundValidators.GetOrAdd(typeof(T), this.FindValidator<T>());

        private IValidator FindValidator<T>()
        {
            var fullType = CreateValidatorType(typeof(T));

            var available = this.validators
                .Where(validator => fullType.GetTypeInfo().IsAssignableFrom(validator.GetType()))
                .ToArray();

            if (available.Length > 1)
            {

            }

            return available.FirstOrDefault();

        }

        private static Type CreateValidatorType(Type type)
        {
            return typeof(AbstractValidator<>).MakeGenericType(type);
        }
    }
}