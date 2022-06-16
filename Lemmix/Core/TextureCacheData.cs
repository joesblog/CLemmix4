using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Collections.Concurrent;
using System.IO;
using System.Collections;

namespace CLemmix4.Lemmix.Core
{
	public class TextureCacheData : ICollection<TextureCacheData.TCDDesription>, IDisposable
	{

		public ConcurrentBag<TextureCacheData.TCDDesription> bag = new ConcurrentBag<TCDDesription>();




		public TextureCacheData.TCDDesription this[LevelPack.LevelData.LevelTerrain i]
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
			public unsafe TCDDesription(LevelPack.LevelData.LevelTerrain i)
			{
				this.Style = i.Style;
				this.Piece = i.Piece;

				if (File.Exists(i.filePath))
				{
					var img = LoadImage(i.filePath);
					if (img.format != PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8)
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






				}

			}








			public Image imgMain { get; private set; }

			public Image imgHorizFlip { get; set; }
			public Image imgVertFlip { get; set; }
			public Image imgVertHorizFlip { get; private set; }

			public Texture2D tex;

			public string Piece { get; set; }
			public string Style { get; set; }



			public int Width { get => tex.width; }
			public int Height { get => tex.height; }
		}






	}
}
