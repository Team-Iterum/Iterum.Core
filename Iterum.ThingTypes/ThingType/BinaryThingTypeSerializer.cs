using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Iterum.ThingTypes
{
    public class BinaryThingTypeSerializer : IThingTypeSerializer
    {
        public string FileExtension => "bin";

        public ThingType Deserialize(string fileName)
        {
            fileName = Path.ChangeExtension(fileName, FileExtension);
            
            var binaryFormatter = new BinaryFormatter();
            var tt = (ThingType)binaryFormatter.Deserialize(File.OpenRead(fileName));

            return tt;
        }

        public void Serialize(string fileName, ThingType tt)
        {
            fileName = Path.ChangeExtension(fileName, FileExtension);
            
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(File.OpenWrite(fileName), tt);
        }
        
        
    }
}