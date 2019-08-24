using Magistr.Log;
using NetStack.Serialization;
using Sprache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Magistr.Protocol.Editor.PacketLayout;

namespace Magistr.Protocol.Editor
{
    public class PacketCreator
    {

        public static string TemplatePath = @"Templates/"
                                     .Replace('/', Path.DirectorySeparatorChar);

        public static string PacketsSource = "";

        public static string PathClient = "";
                                     
        public static string PathServer;

        static Dictionary<string, PacketLayout> packetLayouts = new Dictionary<string, PacketLayout>();

        public static void Save()
        {
            PacketsSource = PacketsSource.Replace('/', Path.DirectorySeparatorChar);
            PathClient = PathClient.Replace('/', Path.DirectorySeparatorChar);
            PathServer = PathServer.Replace('/', Path.DirectorySeparatorChar);
            LoadFiles();
            int channelId = 1;
            foreach (var layout in packetLayouts)
            {
                foreach (var item in layout.Value.Packets)
                {
                    item.ChannelID = channelId;
                    Save(item.Name, channelId, item.Fields);
                    Debug.Log($"[{channelId}] {item.Name} saved");
                    channelId++;
                }
            }
            SaveChannels(true);
            SaveChannels(false);
        }

        public static void LoadFiles()
        {
            packetLayouts.Clear();
            var files = Directory.EnumerateFiles(PacketsSource);
            foreach (var item in files)
            {
                var file = File.ReadAllText(item);
               
                if (Path.GetExtension(item) == ".meta") continue;
                if (Path.GetExtension(item) == ".asmdef") continue;
                if (file.Contains("{") && file.Contains("}") && file.Contains(";"))
                {
                    var layout = PacketLayoutDSL.ParseLayout(file);
                    packetLayouts.Add(item, layout);
                }
               
                

            }
        }
        public static void SaveChannels(bool isServer)
        {
            var template = File.ReadAllText(Path.Combine(TemplatePath, "ChannelPacketsTemplate.txt"));
            var templateMethod = File.ReadAllText(Path.Combine(TemplatePath, "ParseMethod.txt"));
            var templateCase = File.ReadAllText(Path.Combine(TemplatePath, "PacketCase.txt"));

            var p = "";
            var p2 = "";
            foreach (var layout in packetLayouts)
            {



                foreach (var item in layout.Value.Packets.Where(e=>e.IsServerToClient == !isServer || e.IsBidirectional))
                {
                    p += templateCase.Replace("{0}", item.Name).Replace("{1}", item.ChannelID.ToString()) + "\n";
                    p2 += templateMethod.Replace("{0}", item.Name) + "\n" + "\n";

                }

            }
            var result = template.Replace(@"{0}", p).Replace(@"{1}", p2);
            if (isServer)
                File.WriteAllText(Path.Combine(PathServer, "ChannelPackets.cs"), result);
            else
                File.WriteAllText(Path.Combine(PathClient, "ChannelPackets.cs"), result);

            if (isServer)
                Debug.Log("Saved server channels");
            else
                Debug.Log("Saved client channels");

        }

        public static void Save(string packetName, int channelId, List<PacketField> fields)
        {
            var templateServer = File.ReadAllText(Path.Combine(TemplatePath, "PacketTemplateServer.txt"));
            var templateClient = File.ReadAllText(Path.Combine(TemplatePath, "PacketTemplateClient.txt"));
            var templateEnum = File.ReadAllText(Path.Combine(TemplatePath, "Enum.txt"));

            var deser = "";
            var ser = "";
            var fs = "";
            var enumData = "";
            
            int i = 0;
            foreach (var f in fields)
            {
                i++;

                var fName = f.FieldType.ToString().ToLower();
                if (f.FieldType == FieldType._Enum)
                {
                    if(f.EnumType.IsDeclaration)
                    {
                        var enumData0 = templateEnum.Replace("{0}", f.EnumType.Name);
                        var enumFields = "";
                        foreach (var item in f.EnumType.Values)
                        {
                            enumFields += "        	" + item + ",\n";
                        }
                        enumData0 = enumData0.Replace("{1}", enumFields);
                        enumData += enumData0+"\n";
                        continue;
                    } else
                    {
                        fName = f.EnumType.Name;
                    }
                    
                }
                if (f.FieldType == FieldType._Bit) fName = FieldType.Uint.ToString().ToLower(); ;

                
                var fieldser = "";
                var fielddeser = "";
                var ff = $"{fName} {f.Name};";

                BitBuffer b;
                switch (f.FieldType)
                {
                    case FieldType.Vector3:

                        ff = $"{fName.First().ToString().ToUpper() + fName.Substring(1)} {f.Name};";
                        var local = $"m_{f.Name.ToLower()}_{i}";
                        fieldser = $"var {local} = {f.Name};\n            data.AddUShort(HalfPrecision.Compress({local}.#x));\n            data.AddUShort(HalfPrecision.Compress({local}.#y));\n            data.AddUShort(HalfPrecision.Compress({local}.#z));";
                        fielddeser = $"{f.Name} = new Vector3(HalfPrecision.Decompress(data.ReadUShort()), HalfPrecision.Decompress(data.ReadUShort()), HalfPrecision.Decompress(data.ReadUShort()));";
                        break;
                    case FieldType.Quaternion:
                        ff = $"{fName.First().ToString().ToUpper() + fName.Substring(1)} {f.Name};";
                        local = $"m_{f.Name.ToLower()}_{i}";
                        fieldser = $"var {local} = SmallestThree.Compress({f.Name});\n            data.AddByte({local}.m);\n            data.AddShort({local}.a);\n            data.AddShort({local}.b);\n            data.AddShort({local}.c);";
                        fielddeser = $"{f.Name} = SmallestThree.Decompress(new CompressedQuaternion(data.ReadByte(), data.ReadShort(), data.ReadShort(), data.ReadShort()));";
                        break;
                    case FieldType.String:
                        fieldser = $"data.AddString({f.Name});";
                        fielddeser = $"{f.Name} = data.ReadString();";
                        break;
                    case FieldType.Float:
                        fieldser = $"data.AddUShort(HalfPrecision.Compress({f.Name}));";
                        fielddeser = $"{f.Name} = HalfPrecision.Decompress(data.ReadUShort());";
                        break;
                    case FieldType.Int:
                        fieldser = $"data.AddInt({f.Name});";
                        fielddeser = $"{f.Name} = data.ReadInt();";
                        break;
                    case FieldType.Uint:
                        fieldser = $"data.AddUInt({f.Name});";
                        fielddeser = $"{f.Name} = data.ReadUInt();";
                        break;
                    case FieldType.Ushort:
                        fieldser = $"data.AddUShort({f.Name});";
                        fielddeser = $"{f.Name} = data.ReadUShort();";
                        break;
                    case FieldType.Short:
                        fieldser = $"data.AddShort({f.Name});";
                        fielddeser = $"{f.Name} = data.ReadShort();";
                        break;
                    case FieldType.Byte:
                        fieldser = $"data.AddByte({f.Name});";
                        fielddeser = $"{f.Name} = data.ReadByte();";
                        break;
                    case FieldType.Bool:
                        fieldser = $"data.AddBool({f.Name});";
                        fielddeser = $"{f.Name} = data.ReadBool();";
                        break;
                    case FieldType._Bit:
                        fieldser = $"data.Add({f.BitValue}, {f.Name});";
                        fielddeser = $"{f.Name} = data.Read({f.BitValue});";
                        break;
                    default:
                        fieldser = $"data.AddByte((byte){f.Name});";
                        fielddeser = $"{f.Name} = ({fName})data.ReadByte();";
                        break;
                }

                ser += "            " + fieldser + "\n";
                deser += "            " + fielddeser + "\n";
                fs += "        " + "public " + ff + "\n";

            }

            File.WriteAllText(Path.Combine(PathServer, packetName + ".cs"), templateServer.Replace(@"{0}", packetName).Replace(@"{1}", deser).Replace(@"{2}", ser).Replace(@"{3}", fs).Replace(@"{5}", enumData).Replace(@"{4}", channelId.ToString()).Replace("#x", "x").Replace("#y", "y").Replace("#z", "z"));
            File.WriteAllText(Path.Combine(PathClient, packetName + ".cs"), templateClient.Replace(@"{0}", packetName).Replace(@"{1}", deser).Replace(@"{2}", ser).Replace(@"{3}", fs).Replace(@"{5}", enumData).Replace(@"{4}", channelId.ToString()).Replace("#x", "x").Replace("#y", "y").Replace("#z", "z"));

        }

    }
}
