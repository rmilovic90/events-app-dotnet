using System.Runtime.CompilerServices;

namespace Events.Domain;

internal static class ExceptionExtensions
{
    extension(ArgumentException)
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="argument"/> is not longer then given <paramref name="maxLength"/>.
        /// </summary>
        /// <param name="argument">Argument which length is checked against given <paramref name="maxLenght"/>.</param>
        /// <param name="maxLength">Maximum allowed length for <paramref name="argument"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        public static void ThrowIfLongerThan
        (
            string argument,
            int maxLength,
            [CallerArgumentExpression(nameof(argument))] string? paramName = null
        )
        {
            if (argument.Length <= maxLength) return;

            throw new ArgumentException
            (
                $"{argument} cannot be longer then {maxLength}.",
                paramName
            );
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="argument"/> does not fulfill given <paramref name="condition"/>.
        /// </summary>
        /// <typeparam name="T">Type of <paramref name="argument"/>.</typeparam>
        /// <param name="argument">Type argument which is evaluated against given <paramref name="condition"/>.</param>
        /// <param name="condition">Predicate against which <paramref name="argument"/> is evaluated.</param>
        /// <param name="message">The error message.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        public static void ThrowIfUnfulfilled<T>
        (
            T argument,
            Predicate<T> condition,
            string message,
            [CallerArgumentExpression(nameof(argument))] string? paramName = null
        )
        {
            if (condition(argument)) return;

            throw new ArgumentException(message, paramName);
        }
    }
}