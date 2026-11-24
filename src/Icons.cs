using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace StingerReadded
{
    static class Icons
    {
        const string Resource = "StingerReadded.icons.bin";
        static Dictionary<string, Texture2D> made;

        internal static Texture2D Get(string name)
        {
            if (made == null) made = Load();
            Texture2D icon;
            if (made.TryGetValue(name, out icon)) return icon;
            Mod.Log.Warning("No icon named " + name + ", that slot shows none.");
            return null;
        }

        static Dictionary<string, Texture2D> Load()
        {
            Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(Resource))
            {
                if (s == null) throw new InvalidOperationException("the icon pack " + Resource + " is missing from the DLL");
                BinaryReader r = new BinaryReader(s);
                if (Encoding.ASCII.GetString(r.ReadBytes(4)) != "SRQ1") throw new InvalidDataException("the icon pack is not SRQ1");
                int count = r.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    string name = Encoding.UTF8.GetString(r.ReadBytes(r.ReadInt32()));
                    int width = r.ReadInt32(), height = r.ReadInt32(), mips = r.ReadInt32();
                    int filter = r.ReadInt32(), wrap = r.ReadInt32(), aniso = r.ReadInt32();
                    int rawLength = r.ReadInt32(), packedLength = r.ReadInt32();
                    byte[] raw = Inflate(r.ReadBytes(packedLength), rawLength);
                    icons[name] = Plat.NewTexture(name, width, height, mips, (FilterMode)filter, (TextureWrapMode)wrap, aniso, raw);
                }
            }
            return icons;
        }

        static byte[] Inflate(byte[] packed, int length)
        {
            byte[] raw = new byte[length];
            using (DeflateStream d = new DeflateStream(new MemoryStream(packed), CompressionMode.Decompress))
            {
                int at = 0;
                while (at < length)
                {
                    int n = d.Read(raw, at, length - at);
                    if (n <= 0) throw new InvalidDataException("the icon pack ends early");
                    at += n;
                }
            }
            return raw;
        }
    }
}
