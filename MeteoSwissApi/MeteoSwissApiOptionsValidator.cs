using Microsoft.Extensions.Options;

namespace MeteoSwissApi
{
    /// <summary>
    /// Validates <see cref="MeteoSwissApiOptions"/>: for the service constructors and, as <see cref="IValidateOptions{TOptions}"/>,
    /// for options that come from dependency injection.
    /// </summary>
    internal sealed class MeteoSwissApiOptionsValidator : IValidateOptions<MeteoSwissApiOptions>
    {
        public ValidateOptionsResult Validate(string? name, MeteoSwissApiOptions options)
        {
            var errors = GetErrors(options).ToArray();
            return errors.Length == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
        }

        /// <summary>
        /// Throws if <paramref name="options"/> is null or invalid.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid; the message names every invalid value.</exception>
        internal static void ThrowIfInvalid(MeteoSwissApiOptions options, string parameterName)
        {
            if (options is null)
            {
                throw new ArgumentNullException(parameterName);
            }

            var errors = GetErrors(options).ToArray();
            if (errors.Length > 0)
            {
                throw new ArgumentException(string.Join(" ", errors), parameterName);
            }
        }

        private static IEnumerable<string> GetErrors(MeteoSwissApiOptions options)
        {
            if (!MeteoSwissApiOptions.SupportedLanguages.Contains(options.Language, StringComparer.OrdinalIgnoreCase))
            {
                yield return $"{nameof(MeteoSwissApiOptions.Language)} must be one of: {string.Join(", ", MeteoSwissApiOptions.SupportedLanguages)}.";
            }

            if (options.SwissMetNet is null)
            {
                yield return $"{nameof(MeteoSwissApiOptions.SwissMetNet)} must not be null.";
            }
            else if (options.SwissMetNet.CacheExpiration is TimeSpan cacheExpiration && cacheExpiration <= TimeSpan.Zero)
            {
                yield return $"{nameof(MeteoSwissApiOptions.SwissMetNet)}.{nameof(SwissMetNetOptions.CacheExpiration)} must be positive, or null to disable caching.";
            }
        }
    }
}
