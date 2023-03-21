using System.IO;
using System.Xml.Serialization;

namespace Iterum.ThingTypes
{
    public class XmlThingTypeSerializer : IThingTypeSerializer
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
            
            
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            
            var xmlSerializer = new XmlSerializer(typeof(ThingType));
            xmlSerializer.Serialize(File.OpenWrite(fileName), tt);
        }
        
        public ThingTypeStore DeserializeAll(string directory)
        {
            return new ThingTypeStore(new[] { Deserialize(directory) });
        }
        
    }
}