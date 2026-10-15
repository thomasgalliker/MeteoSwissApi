namespace MeteoSwissApi.Tests.Utils
{
    /// <summary>
    /// Sets <see cref="CultureInfo.CurrentCulture"/> and <see cref="CultureInfo.CurrentUICulture"/>
    /// until the scope is disposed, then restores the previous cultures.
    /// </summary>
    internal sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo previousCulture;
        private readonly CultureInfo previousUICulture;

        /// <summary>
        /// Sets both cultures to <paramref name="cultureName"/>.
        /// </summary>
        public CultureScope(string cultureName)
            : this(cultureName, cultureName)
        {
        }

        /// <summary>
        /// Sets the culture, which formats numbers and dates, to <paramref name="cultureName"/>
        /// and the UI culture, which selects the language of texts, to <paramref name="uiCultureName"/>.
        /// </summary>
        public CultureScope(string cultureName, string uiCultureName)
        {
            this.previousCulture = CultureInfo.CurrentCulture;
            this.previousUICulture = CultureInfo.CurrentUICulture;

            CultureInfo.CurrentCulture = new CultureInfo(cultureName);
            CultureInfo.CurrentUICulture = new CultureInfo(uiCultureName);
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = this.previousCulture;
            CultureInfo.CurrentUICulture = this.previousUICulture;
        }
    }
}
