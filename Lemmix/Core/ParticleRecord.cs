using Raylib_CsLo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CLemmix4.Lemmix.Core
{


	public class Particle
	{


		public const int defaultParticleFrameCount = 50;
		public static Color[] ParticleColors = new Color[] {

		new Color(0x40,0x40,0xE0,0xFF),
 new Color(0x00,0xB0,0x00,0xFF),
 new Color(0xF0,0xD0,0xD0,0xFF),
 new Color(0xF0,0x20,0x20,0xFF),
new Color(0x40,0x40,0xE0,0xC0),
 new Color(0x00,0xB0,0x00,0xC0),
 new Color(0xF0,0xD0,0xD0,0xC0),
 new Color(0xF0,0x20,0x20,0xC0)

		};
		public static ParticleRecord[][] _particleOffset;// = ParticleRecord.loadJson("data/particle.json").Reverse().ToArray();
		public static ParticleRecord[][] ParticleOffset { get {

		 
				return _particleOffset;
			} }

		public static ParticleRecord[][] init()
		{
			var a = ParticleRecord.loadJson("data/particle.json").Reverse().ToArray();
			
			return a.Reverse().ToArray();
		}


	}

	[Serializable]
	public struct ParticleRecord
	{

		public sbyte dx;
		public sbyte dy;

		public static void dumpJson(ParticleRecord[][] data, string path)
		{
			String op = Newtonsoft.Json.JsonConvert.SerializeObject(data);
			File.WriteAllText(path, op);

		}

		public static void dumpBin(ParticleRecord[][] data, string path)
		{
			using (Stream ms = File.OpenWrite(path))
			{
				BinaryFormatter bf = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
				bf.Serialize(ms, data);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
				ms.Flush();
				ms.Close();
			}
		}

		public static ParticleRecord[][] loadBin(string path)
		{
			using (Stream ms = File.Open(path, FileMode.Open))
			{
				BinaryFormatter bf = new BinaryFormatter();

#pragma warning disable SYSLIB0011 // Type or member is obsolete
				ParticleRecord[][] r = (ParticleRecord[][])bf.Deserialize(ms);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
				ms.Flush();
				ms.Close();
				return r;

			}


		}



		public static ParticleRecord[][] loadJson(string path)
		{
			if (File.Exists(path))
			{
				ParticleRecord[][] r = Newtonsoft.Json.JsonConvert.DeserializeObject<ParticleRecord[][]>(File.ReadAllText(path));

				return r;
			}
			return null;

		}



	}
}
