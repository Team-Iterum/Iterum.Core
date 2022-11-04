using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Iterum.DataBlocks;

namespace Iterum.ThingTypes
{
    public static class DataBlockUtils
    {
        public static IEnumerable<Type> GetDataBlocksTypes() 
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) 
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if(!type.GetInterfaces().Contains(typeof(IDataBlock))) continue;
                    
                    yield return type;
                }
            }
        }
    }
}