using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLemmix4.SDL2Wrappist
{
	public static class Common
	{
		public class LockedIncList<T> : IEnumerable<T>
		{



			Dictionary<int, T> backingDict;
			int c = 0;
			public object olock = new object();

			public LockedIncList()
			{
				lock (olock)
					backingDict = new Dictionary<int, T>();
			}

			public int LatestId()
			{
				lock (olock)
					return c;
			}

			public int Add(T item)
			{
				lock (olock)
				{
					backingDict.Add(++c, item);
					return c;
				}
			}

			public bool Remove(int Id)
			{
				bool ok = false;
				lock (olock)
				{

					if (backingDict.ContainsKey(Id))
					{
						ok = backingDict.Remove(Id);
					}
				}
				return ok;
			}

			public T Get(int Id)
			{
				lock (olock)
				{
					if (backingDict.ContainsKey(Id))
					{
						return backingDict[Id];
					}
					else return default;
				}

			}


			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				lock (olock)
					foreach (var i in backingDict)
					{
						yield return i.Value;
					}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				lock (olock)
					foreach (var i in backingDict)
					{
						yield return i;
					}
			}


		}

		static float hype(float a, float b) => (float)hypo((double)a, (double)b);
		static float legB(float legA, float hypoC) => (float)legB((double)legA, (double)hypoC);
		static float legA(float legB, float hypoC) => (float)legA((double)legB, (double)hypoC);

		static double hypo(double a, double b)
		{

			//return Math.Pow((Math.Pow(a, 2) + Math.Pow(b, 2)), (1 / 2));

			return Math.Sqrt((Math.Pow(a, 2) + Math.Pow(b, 2)));

		}

		static double legB(double legA, double hypoC)
		{
			return Math.Sqrt(Math.Pow(hypoC, 2) - Math.Pow(legA, 2));
		}
		static double legA(double legB, double hypoC)
		{

			return Math.Sqrt(Math.Pow(hypoC, 2) - Math.Pow(legB, 2));
		}



	}
	public class WPoint
	{
		public WPoint()
		{
		}

		public WPoint(int x, int y)
		{
			X = x;
			Y = y;
		}

		public int X { get; set; }
		public int Y { get; set; }


		public static implicit operator WPoint(System.Drawing.Point p)
		{
			return new WPoint() { X = p.X, Y = p.Y };
		}

		public static implicit operator System.Drawing.Point(WPoint w)
		{
			return new System.Drawing.Point(w.X, w.Y);
		}

		public static implicit operator WPoint(int[] a)
		{
			WPoint r = new WPoint();

			if (a.Length == 2)
			{
				r.X = a[0]; r.Y = a[1];
			}

			return r;
		}

	}
	public class WPointF
	{
		public WPointF()
		{
		}

		public WPointF(float x, float y)
		{
			X = x;
			Y = y;
		}

		public float X { get; set; }
		public float Y { get; set; }


		public static implicit operator WPointF(System.Drawing.PointF p)
		{
			return new WPointF() { X = p.X, Y = p.Y };
		}

		public static implicit operator System.Drawing.PointF(WPointF w)
		{
			return new System.Drawing.PointF(w.X, w.Y);
		}

		public static implicit operator WPointF(float[] a)
		{
			WPointF r = new WPointF();

			if (a.Length == 2)
			{
				r.X = a[0]; r.Y = a[1];
			}

			return r;
		}

	}
	public class WVector2
	{
		public WVector2(int x, int y)
		{
			X = x;
			Y = y;
		}

		public int X { get; set; }
		public int Y { get; set; }

		public void negate()
		{
			X *= 1;
			Y *= 1;
		}

		public float CrossProduct(WVector2 other)
		{
			var u = (WVector2F)this;
			var v = (WVector2F)other;
			return u.X * v.Y - u.Y * v.X;
		}
	}

	public class WRectangle : WVector2
	{
		public int width { get; set; }
		public int height { get; set; }

		public WRectangle(int x, int y, int w, int h) : base(x, y)
		{
			width = w; height = h;
		}


	}

	public class WVector2F
	{
		public WVector2F()
		{
		}

		public WVector2F(int x, int y)
		{
			X = x; Y = y;
		}
		public WVector2F(float x, float y)
		{
			X = x;
			Y = y;
		}

		public float X { get; set; }
		public float Y { get; set; }

		//public static implicit operator WvertexF(wVertex o) => new WvertexF((float)o.X,(float)o.Y);
		public static explicit operator WVector2F(WVector2 o) => new WVector2F((float)o.X, (float)o.Y);

		public float CrossProduct(WVector2F other)
		{
			return CrossProduct(this, other);
		}

		public static float CrossProduct(WVector2F u, WVector2F v)
		{

			return u.X * v.Y - u.Y * v.X;
		}

		public static WVector2F operator +(WVector2F a, WVector2F b)
		{
			return new WVector2F((a.X + b.X), (a.Y + b.Y));
		}

		public static WVector2F operator -(WVector2F a, WVector2F b)
		{
			return new WVector2F((a.X - b.X), (a.Y - b.Y));
		}
		public static WVector2F operator *(WVector2F a, WVector2F b)
		{
			return new WVector2F((a.X * b.X), (a.Y * b.Y));
		}
		public static WVector2F operator /(WVector2F a, WVector2F b)
		{
			return new WVector2F((a.X * b.X), (a.Y * b.Y));
		}

		public static float dot(WVector2F v1, WVector2F v2)
		{
			if (v1 == null || v2 == null) return 0;
			float r = 0;

			r += v1.X * v2.X;
			r += v1.Y * v2.Y;
			return r;

		}

    public override string ToString()
    {
			return $"{{X={X},Y={Y}}}";
    }

  }




}
