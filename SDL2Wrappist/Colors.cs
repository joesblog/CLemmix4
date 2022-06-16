using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace CLemmix4.SDL2Wrappist.Colors
{
	public class SDL4Color : IEquatable<SDL4Color>
	{


		public SDL4Color(byte r, byte g, byte b, byte a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		public byte R { get; set; }
		public byte G { get; set; }

		public byte B { get; set; }
		public byte A { get; set; }

		public bool Equals(SDL4Color other)
		{
			return (this.R == other.R &&
this.G == other.G &&
this.B == other.B &&
this.A == other.A
);
		}

		public override int GetHashCode()
		{
			return R.GetHashCode() + G.GetHashCode() + B.GetHashCode() + A.GetHashCode();
		}

		public static implicit operator byte[](SDL4Color c)
		{
			return new byte[] { c.R, c.G, c.B, c.A };
		}

		public static implicit operator SDL4Color(Color c)
		{
			return new SDL4Color(c.R, c.G, c.B, c.A);
		}

		public static implicit operator SDL4Color(byte[] d)
		{
			if (d.Length == 3)
			{
				return new SDL4Color(d[0], d[1], d[2], 0xff);

			}
			else
			{
				return new SDL4Color(d[0], d[1], d[2], d[3]);

			}
		}
	}

	public class SDL3Color
	{


		public SDL3Color(byte r, byte g, byte b)
		{
			this.R = r;
			this.G = g;
			this.B = b;

		}

		public byte R { get; set; }
		public byte G { get; set; }

		public byte B { get; set; }


		public static implicit operator byte[](SDL3Color c)
		{
			return new byte[] { c.R, c.G, c.B };
		}

		public static implicit operator SDL3Color(Color c)
		{
			return new SDL3Color(c.R, c.G, c.B);
		}

		public static implicit operator SDL3Color(byte[] d)
		{
			if (d.Length == 3)
			{
				return new SDL3Color(d[0], d[1], d[2]);

			}
			else
			{
				//ToDo throw exception.
				return null;
			}

		}
	}
}
