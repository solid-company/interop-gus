using System.Xml.Serialization;

namespace SolidCompany.Interop.Gus.Models.ErrorDeserialization
{
    [XmlType("root")]
#pragma warning disable 1591
    public class Root
    {
        [XmlElement("dane")]
        public Error Error { get; set; }
    }
}