using Magistr.Log;
using Magistr.Math;
using Magistr.Physics;
using Magistr.Things;
using Magistr.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Magistr.WorldMap
{
    public class Map
    {
        internal IPhysicsWorld PhysicsWorld;
        public List<IThing> Entities = new List<IThing>();


        public void Start()
        {
            try
            {
                PhysicsWorld.Start();
            }catch(Exception e)
            {
                Debug.LogError($"[Map][PhysicsWorld] " + e.ToString());
                throw;
            }
            finally
            {
                Debug.Log($"[Map][PhysicsWorld] started", ConsoleColor.Green);
            }
           
        }

        public Map(IPhysicsWorld world)
        {
            PhysicsWorld = world;
        }

        public void Add(IThing t)
        {
            t.Create(PhysicsWorld);
            Entities.Add(t);
        }

        public void Remove(IThing t)
        {
            t.Destroy();
            Entities.Remove(t);
        }

        private static MapArchive CreateArchive(string mapName)
        {
            var mapArchive = new MapArchive
            {
                Name = mapName,
                Created = DateTime.UtcNow
            };
            return mapArchive;

        }

        public void Save(FileStream fs, string mapName)
        {
            var mapArchive = CreateArchive(mapName);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, mapArchive);
            }
            catch (SerializationException e)
            {
                Debug.LogError("Failed to serializer MapArchive. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        sealed class BinaryTypeBinderLocal : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if(a.GetType(typeName)!=null)
                    {
                        typeToDeserialize = a.GetType(typeName);
                    }
                }
                // The following line of code returns the type.
                //////typeToDeserialize = Assembly.GetType(typeName);

                return typeToDeserialize;
            }
        }
        public Map Load(FileStream fs)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new BinaryTypeBinderLocal();
                var map = (MapArchive)formatter.Deserialize(fs);

                foreach (var item in map.Objects)
                {
                    if (!ThingTypeManager.HasThingType(item.ThingTypeId)) continue;
                    if (ThingTypeManager.GetTningType(item.ThingTypeId).HasAttr(ThingAttr.Static))
                    {
                        Add(new Thing(item.ThingTypeId, item.Position, item.Rotation, item.Scale));
                    }
                }
            }
            catch (SerializationException e)
            {
                Debug.LogError("[Map] Failed to deserialize MapArchive. Reason: " + e.Message);
                throw;
            }
            finally
            {
                Debug.Log($"[Map] {Path.GetFileName(fs.Name)} loaded", ConsoleColor.Green);
                fs.Close();
            }
            return this;
        }

        [Serializable]
        struct MapArchive
        {
            public DateTime Created;
            public string Name;
            public MapObject[] Objects;
        }

        [Serializable]
        struct MapObject
        {
            public int ThingTypeId;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
        }
    }
}
