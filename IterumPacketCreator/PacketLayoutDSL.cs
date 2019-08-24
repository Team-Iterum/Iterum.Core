using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using static Magistr.Protocol.Editor.PacketLayout;

namespace Magistr.Protocol.Editor
{
    internal static class PacketLayoutDSL
    {
        public static PacketLayout ParseLayout(string text)
        {
            return Layout.End().Parse(text);
        }
        internal static Parser<string> Identifier =
            from l in Parse.Char('[').Token().Optional()
            from i in Parse.LetterOrDigit.Or(Parse.Char(',').Or(Parse.Char('_'))).AtLeastOnce().Text().Token()
            from r in Parse.Char(']').Token().Optional()
            select i;

        internal static Parser<PacketField> PacketField =
              from type in Identifier
              from value in Identifier
              from comma in Parse.Char(';').Token()
              select new PacketField(value, type);

        internal static Parser<PacketTemplate> Section =
              from title in Identifier
              //from equal in Parse.Char('#').Token()
              //from id in Parse.Digit.AtLeastOnce().Text().Token()
              from lbracket in Parse.Char('{').Token()
              from fields in PacketField.AtLeastOnce().Commented()
              from rbracket in Parse.Char('}').Token()
              select new PacketTemplate(0, title, fields.Value);

        internal static Parser<PacketLayout> Layout =
             from sections in Section.AtLeastOnce()
             select new PacketLayout(sections);
    }
}
