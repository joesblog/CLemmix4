using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.Color;
using static Raylib_CsLo.RlGl;
using Raylib_CsLo;
using static CLemmix4.RaylibMethods;
using System.Numerics;
namespace CLemmix4.Lemmix.Core
{
	public class BackingMap<T> : IDisposable
	{

		public int width { get; private set; }
		public int height { get; private set; }

		public T[] store;
		private readonly T initValue;
		private readonly bool hasInitValue;
		public Texture tex;
		public Color RenderBG = BLACK;

		public BackingMap(int width, int height)
		{
			this.width = width;
			this.height = height;
			this.store = new T[this.width * this.height];
		}
		public BackingMap(int width, int height, T InitValue) : this(width, height)
		{
			initValue = InitValue;
			hasInitValue = true;

			this.store = Enumerable.Repeat(initValue, store.Length).ToArray();

		}

		public void Clear()
		{
			this.store = Enumerable.Repeat(initValue, store.Length).ToArray();
		}

		public unsafe void UpdateRenderTexture(Camera2D cam, Dictionary<T, Color> colorMap = null)
		{


			Image tImg = GenImageColor(GetScreenWidth(), GetScreenHeight(), BLANK);
			ImageFormat(ref tImg, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
			Color FG = WHITE;



			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{

					var dt = this[x, y];
					if (colorMap != null && colorMap.ContainsKey(dt)) FG = colorMap[dt];
					//var cm = GetScreenToWorld2D(new Vector2(x, y), cam);
					var cm = GetWorldToScreen2D(new Vector2(x, y), cam);

					ImageDrawPixel(ref tImg, (int)cm.X, (int)cm.Y, FG);
				}
			}

			if (tex.id <= 0)
				tex = LoadTextureFromImage(tImg);


			Color* pxImg = LoadImageColors(tImg);
			UpdateTexture(tex, pxImg);
			UnloadImageColors(pxImg);

			UnloadImage(tImg);
		}

		public void Dispose()
		{
			if (tex.id > 0)
				UnloadTexture(tex);

		}

		public T this[int x, int y]
		{
			get
			{
				int ix = y * width + x;
				if (ix >= 0 && ix < store.Length)
					return this.store[y * width + x];
				else return default;
			}
			set
			{
				int ix = y * width + x;
				if (ix >= 0 && ix < store.Length)
					this.store[ix] = value;
			}
		}



	}
}
