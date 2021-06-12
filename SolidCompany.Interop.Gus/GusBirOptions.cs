using SolidCompany.Interop.Gus.Connected_Services;

namespace SolidCompany.Interop.Gus
{
    /// <summary>
    /// GUS BIR Client configuration options.
    /// </summary>
    public sealed class GusBirOptions
    {
        /// <summary>
        /// Environment
        /// </summary>
        public GusEnironment Environment { get; set; } = GusEnironment.Production;

        /// <summary>
        /// User key
        /// </summary>
        public string Key { get; set; }
    }
}