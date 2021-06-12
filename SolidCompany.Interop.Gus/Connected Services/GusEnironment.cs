namespace SolidCompany.Interop.Gus.Connected_Services
{
    /// <summary>
    /// GUS Environment.
    /// </summary>
    public abstract class GusEnironment
    {
        /// <summary>
        /// GUS Production environment.
        /// </summary>
        public static GusEnironment Production => new GusProductionEnvironment();

        /// <summary>
        /// GUS Test environment.
        /// </summary>
        public static GusEnironment Test => new GusTestEnvironment();

        /// <summary>
        /// Return endpoint URL.
        /// </summary>
        public abstract string ServiceUrl { get; }

        private class GusProductionEnvironment : GusEnironment
        {
            public override string ServiceUrl => "https://wyszukiwarkaregon.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc";
        }

        private class GusTestEnvironment : GusEnironment
        {
            public override string ServiceUrl => "https://wyszukiwarkaregontest.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc";
        }
    }
}