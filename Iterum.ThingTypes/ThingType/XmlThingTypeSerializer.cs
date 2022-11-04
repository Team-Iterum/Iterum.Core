using System.IO;
using System.Xml.Serialization;

namespace Iterum.ThingTypes
{
    class XmlThingTypeSerializer : IThingTypeSerializer
    {
        public string FileExtension => "xml";

        public ThingType Deserialize(string fileName)
        {
            fileName = Path.ChangeExtension(fileName, FileExtension);
            
            var xmlSerializer = new XmlSerializer(typeof(ThingType));
            var tt = (ThingType)xmlSerializer.Deserialize(File.OpenRead(fileName));

            return tt;
        }

        public void Serialize(string fileName, ThingType tt)
        {
            fileName = Path.ChangeExtension(fileName, FileExtension);
            
            var xmlSerializer = new XmlSerializer(typeof(ThingType));
            xmlSerializer.Serialize(File.OpenWrite(fileName), tt);
        }
        
        
    }
}