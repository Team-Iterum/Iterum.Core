using System;
using System.Collections.Generic;
using System.Linq;

namespace Magistr.Protocol.Editor
{
    public enum FieldType
    {
        _Enum,
        Byte,
        String,
        Float,
        Int,
        Uint,
        Ushort,
        Short,
        Bool,
        Vector3,
        Quaternion,
        _Bit,
    }

    public class PacketLayout
    {
        public PacketLayout(IEnumerable<PacketTemplate> sections)
        {

            Packets = sections;
        }

        [System.Serializable]
        public class PacketField
        {
            public string Name = string.Empty;
            public FieldType FieldType = FieldType.Byte;
            public PacketEnum EnumType;
            public int BitValue = -1;
            public PacketField(string name, string type)
            {
                Name = name;

                if (Enum.TryParse<FieldType>(type[0].ToString().ToUpper() + type.Substring(1), out FieldType result))
                {
                    FieldType = result;
                }
                else
                {
                    if(type.Contains("bit_"))
                    {
                        var val = type.Split('_')[1];
                        var value = int.Parse(val);
                        FieldType = FieldType._Bit;
                        BitValue = value;
                    }
                    // create enum
                    else if (type.Contains(","))
                    {
                        var list = type.Split(',').ToList();
                        EnumType = new PacketEnum(name, list);
                        EnumType.IsDeclaration = true;
                        FieldType = FieldType._Enum;
                    }
                    // use enum
                    else
                    {
                        FieldType = FieldType._Enum;
                        EnumType = new PacketEnum(type, new List<string>());
                    }
               }
                
            }
        }
        [System.Serializable]
        public class PacketEnum
        {
            public string Name;
            public List<string> Values;
            public bool IsDeclaration;

            public PacketEnum(string name, List<string> values)
            {
                Name = name;
                Values = values;
            }
        }

        [System.Serializable]
        public class PacketTemplate
        {
            public int ChannelID;
            public string Name;
            public List<PacketField> Fields;
            public bool IsServerToClient;
            public bool IsClientToServer;
            public bool IsBidirectional;

            public PacketTemplate(int id, string title, IEnumerable<PacketField> fields)
            {
                var name = title.Split('_');
                this.ChannelID = id;
                this.Name = name[0];
                if (name.Length > 1)
                {
                    if (name[1] == "SC")
                    {
                        IsServerToClient = true;
                    }
                    else if (name[1] == "CS")
                    {
                        IsClientToServer = true;
                    }
                }
                else
                {
                    IsBidirectional = true;
                }
                this.Fields = fields.ToList();
            }
        }

        public IEnumerable<PacketTemplate> Packets;

    }

}
