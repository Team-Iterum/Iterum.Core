using System;
using System.Runtime.Serialization;

namespace Magistr.Utils
{
    public sealed class Binder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            // The following line of code returns the type.
            var typeToDeserialize = Type.GetType(typeName);

            return typeToDeserialize;
        }
    }
}
