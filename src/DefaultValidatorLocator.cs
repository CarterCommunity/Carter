namespace Carter
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

        public IValidator GetValidator<T>() => this.foundValidators.GetOrAdd(typeof(T), this.FindValidator);

        private IValidator FindValidator(Type type)
        {
            var fullType = CreateValidatorType(type);

            var available = this.validators
                .Where(validator => fullType.GetTypeInfo().IsInstanceOfType(validator))
                .ToArray();

            if (available.Length > 1)
            {
                var names = string.Join(", ", available.Select(v => v.GetType().Name));
                var message = string.Concat(
                    "Ambiguous choice between multiple validators for type ",
                    type.Name,
                    ". The validators available are: ",
                    names);

                throw new InvalidOperationException(message);
            }

            return available.FirstOrDefault();
        }

        private static Type CreateValidatorType(Type type)
        {
            return typeof(AbstractValidator<>).MakeGenericType(type);
        }
    }
}