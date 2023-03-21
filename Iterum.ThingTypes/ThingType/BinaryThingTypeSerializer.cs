using System;
using System.Collections.Generic;
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
        
        public void SerializeAll(string fileName, ThingTypeStore store)
        {
            fileName = Path.ChangeExtension(fileName, FileExtension);
            
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var fs = File.OpenWrite(fileName);
            var binaryFormatter = new BinaryFormatter();
            foreach (var (_, thingType) in store.ThingTypes)
            {
                var memoryStream = new MemoryStream();
                binaryFormatter.Serialize(memoryStream, thingType);
                var buffer = memoryStream.ToArray();
                var length = BitConverter.GetBytes(buffer.Length);
                fs.Write(length);
                fs.Write(memoryStream.ToArray());
            }
            
            fs.Flush();
            fs.Close();
        }
        
        public ThingTypeStore DeserializeAll(string fileName)
        {
            var fs = File.OpenRead(fileName);
            var binaryFormatter = new BinaryFormatter();

            var thingTypes = new List<ThingType>();
            while (fs.CanRead)
            {
                Span<byte> lengthBuffer = new byte[4];

                var _ = fs.Read(lengthBuffer);

                var length = BitConverter.ToInt32(lengthBuffer);

                var ms = new MemoryStream(new byte[length]);
                var thingType = (ThingType)binaryFormatter.Deserialize(ms);

                thingTypes.Add(thingType);
            }

            fs.Flush();
            fs.Close();

            return new ThingTypeStore(thingTypes);
        }

        
        
    }
}