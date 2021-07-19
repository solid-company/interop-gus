using System.Xml.Serialization;

namespace SolidCompany.Interop.Gus.Models
{
    /// <summary>
    /// Represents an error response.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Error code
        /// </summary>
        [XmlElement("ErrorCode")]
        public int Code { get; set; }

        /// <summary>
        /// Error message in Polish.
        /// </summary>
        [XmlElement("ErrorMessagePl")]
        public string MessagePl { get; set; }

        /// <summary>
        /// Error message in English.
        /// </summary>
        [XmlElement("ErrorMessageEn")]
        public string MessageEn { get; set; }
    }
}