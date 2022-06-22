//using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static Raylib_cs.Raylib;
//using static Raylib_cs.Color;
using System.Collections.Concurrent;
using System.IO;
using System.Collections;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.Color;
using static CLemmix4.RaylibMethods;
using CLemmix4.Lemmix.Utils;

namespace CLemmix4.Lemmix.Core
{
	public class TextureCacheData : ICollection<TextureCacheData.TCDDesription>, IDisposable
	{

		public ConcurrentBag<TextureCacheData.TCDDesription> bag = new ConcurrentBag<TCDDesription>();




		public TextureCacheData.TCDDesription this[iLevelDrawable i]
		{

			get
			{

				var tcd = bag.FirstOrDefault(o => o.Piece == i.Piece && o.Style == o.Style);

				if (tcd == null)
				{
					tcd = new TCDDesription(i);
					bag.Add(tcd);
				}
				return tcd;

			}

		}

		public void DisposeImages()
		{
			foreach (var i in bag)
			{
				UnloadImage(i.imgMain);
				UnloadImage(i.imgVertFlip);
				UnloadImage(i.imgHorizFlip);
			}
		}
		Texture _texAtlas;
		List<Atlas> _Atlas;

		public List<Atlas> AtlasData
		{
			get
			{
				return _Atlas;
			}
		}

		public Texture texAtlas
		{
			get
			{


				return _texAtlas;

			}

		}

		public Atlas[] getAtl(iLevelDrawable i)
		{
			var r = AtlasData.Where(o => o.Style == i.Style && o.Piece == i.Piece).ToArray();
			return r;
		}

		public void BuildImageAtlas()
		{
			if (_texAtlas.id == 0)
			{
				_Atlas = new List<Atlas>();
				int aW = bag.Sum(o => o.Width);
				int ah = bag.Max(o => o.Height); //std,erase,fh,fv,fhv

				var enms = typeof(Atlas.enmAtlasType).ToDict();
				Image img = GenImageColor(aW, ah * (enms.Max(o => o.Key)), BLANK);
				ImageFormat(ref img, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
				int cx = 0;
				int cy = 0;

				Image getForEnm(TCDDesription t, Atlas.enmAtlasType e)
				{
					switch (e)
					{
						case Atlas.enmAtlasType.STANDARD: return t.imgMain;
						case Atlas.enmAtlasType.ERASE: return t.imgAlphaInverse;
						case Atlas.enmAtlasType.FH: return t.imgHorizFlip;
						case Atlas.enmAtlasType.FV: return t.imgVertFlip;
						case Atlas.enmAtlasType.FHV: return t.imgVertHorizFlip;
					}
					return t.imgMain;
				}

				foreach (var e in enms)
				{
					cy = ah * e.Key;
					cx = 0;

					foreach (var g in bag)
					{

						Rectangle srcRect = new Rectangle(0, 0, g.Width, g.Height);
						Rectangle dstRect = new Rectangle(cx, cy, g.Width, g.Height);
						ImageDraw(ref img, getForEnm(g, (Atlas.enmAtlasType)e.Key), srcRect, dstRect, WHITE);
						cx += g.Width;
						_Atlas.Add(new Atlas()
						{
							pos = dstRect,
							AtlasType = (Atlas.enmAtlasType)e.Key,
							Style = g.Style,
							Piece = g.Piece

						});
					}


				}
				_texAtlas = LoadTextureFromImage(img);
				UnloadImage(img);

			}
		}

		#region interface overloads
		public int Count => bag.Count;

		public bool IsReadOnly => false;

		public void Add(TCDDesription item)
		{
			bag.Add(item);
		}

		public void Clear()
		{
			var oldBag = bag;
			bag = new ConcurrentBag<TCDDesription>();
			oldBag = null;
		}

		public bool Contains(TCDDesription item)
		{
			return bag.Contains(item);
		}

		public void CopyTo(TCDDesription[] array, int arrayIndex)
		{
			bag.CopyTo(array, arrayIndex);
		}

		public IEnumerator<TCDDesription> GetEnumerator()
		{
			return bag.GetEnumerator();
		}

		public bool Remove(TCDDesription item)
		{
			TCDDesription op = null;
			bag.TryTake(out op);
			return op != null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Dispose()
		{
			DisposeImages();
		}


		#endregion
		public class TCDDesription
		{
			public unsafe TCDDesription(iLevelDrawable i)
			{
				this.Style = i.Style;
				this.Piece = i.Piece;

				if (File.Exists(i.filePath))
				{
					var img = LoadImage(i.filePath);
					//if (img.format != PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8)
					//	ImageFormat(ref img, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);

					if (img.format != (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8)
						ImageFormat(ref img, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);



					this.imgMain = img;

					var _imgHorizFlip = ImageFromImage(img, new Rectangle(0, 0, img.width, img.height));
					ImageFlipHorizontal(ref _imgHorizFlip);
					this.imgHorizFlip = _imgHorizFlip;

					var _imgVertFlip = ImageFromImage(img, new Rectangle(0, 0, img.width, img.height));


					ImageFlipVertical(ref _imgVertFlip);
					this.imgVertFlip = _imgVertFlip;


					var _imgVertHorizFlip = ImageFromImage(img, new Rectangle(0, 0, img.width, img.height));
					ImageFlipVertical(ref _imgVertHorizFlip);
					ImageFlipHorizontal(ref _imgVertHorizFlip);
					this.imgVertHorizFlip = _imgVertHorizFlip;

					tex = LoadTextureFromImage(imgMain);


					imgAlphaInverse = ImageCreateInverseAlpha(img);

					texAlphaInverse = LoadTextureFromImage(imgAlphaInverse);

				}

			}








			public Image imgMain { get; private set; }

			public Image imgHorizFlip { get; set; }
			public Image imgVertFlip { get; set; }
			public Image imgVertHorizFlip { get; private set; }
			public Image imgAlphaInverse { get; private set; }

			public Texture tex;
			public Texture texAlphaInverse;

			public string Piece { get; set; }
			public string Style { get; set; }



			public int Width { get => tex.width; }
			public int Height { get => tex.height; }
		}

		public class Atlas
		{
			public string Piece { get; set; }
			public string Style { get; set; }
			public enum enmAtlasType { STANDARD = 1, ERASE = 2, FH = 3, FV = 4, FHV = 5 }

			public static int lastId = 1;

			public Atlas()
			{
				id = ++lastId;
			}

			public int id { get; private set; }
			/*	public int x { get; set; }
				public int y { get; set; }
				public int w { get; set; }
				public int h { get; set; }*/
			public Rectangle pos { get; set; }
			public enmAtlasType AtlasType { get; set; }

		}




	}
}
