using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static CLemmix4.SDL2Wrappist.Imports;
using static CLemmix4.SDL2Wrappist.Defs;
//using static JS.Common;
using System.Threading;
using CLemmix4.SDL2Wrappist.Colors;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO;
using System.Collections;
using static CLemmix4.SDL2Wrappist.Common;
using System.Drawing.Imaging;

namespace CLemmix4.SDL2Wrappist
{
	public class SDLFont : WrappistPTRHandler
	{

		public const bool textCaching = true;
		private const int reqBeforeClean = 20;
		private const int secondsBeforeClean = 5;
		public static bool TTFStarted = false;
		public string name { get; set; }
		public int size { get; set; }
		public bool fontAtlas { get; set; }
		public Dictionary<char, RectangleF> fontAtlasPosition { get; set; }
		public static Dictionary<fontLookup, SDLFont> fontCache;

		public Surface atlasSurface;
		public Texture atlasTexture;
		public Context atlasContext;
		public Window atlasWindow;
		internal DateTime lastUsed;
		internal bool disposed;
		public static float Kerning = 0.5f;
		public SDLFont() : base(true)
		{

			if (!TTFStarted)
			{
				int r = SDLW_TTF_Init();
				if (r != -1)
				{
					TTFStarted = true;
				}
			}
		}



		private SDLFont(string name, int size) : this()
		{
			var a = SDLW_TTF_OpenFont(name, (int)size);
			this.wSetHandle(a);
		}


		/// <summary>
		/// real slow, not used.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="c"></param>
		/// <param name="w"></param>
		private SDLFont(Font f, Context c, Window w) : this()
		{

			atlasContext = c;
			atlasWindow = w;
			fontAtlas = true;
			fontAtlasPosition = new Dictionary<char, RectangleF>();

			//ToDo: Migrate to surface stuff on SDL

			int aStart = 32; int aEnd = 255;
			float cWidth = 0;
			float cMaxHeight = 0;

			using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
			{

				for (int i = aStart; i <= aEnd; i++)
				{
					var sz = g.MeasureString(new string((char)i, 1), f);


					RectangleF t = new RectangleF(cWidth, 0, sz.Width, sz.Height);

					fontAtlasPosition.Add((char)i, t);


					cWidth += sz.Width;

					if (sz.Height > cMaxHeight) cMaxHeight = sz.Height;


				}
			}

			Bitmap b = new Bitmap((int)Math.Ceiling(cWidth), (int)Math.Ceiling(cMaxHeight));
			//Format32bppArgb

			Brush br = new SolidBrush(Color.Red);
			using (Graphics g = Graphics.FromImage(b))
			{

				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				foreach (var i in fontAtlasPosition)
				{
					g.DrawString(new string(i.Key, 1), f, br, i.Value);
				}

			}
			BitmapData bd =
				b.LockBits(new Rectangle(new Point(0, 0), b.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


			atlasSurface = new Surface(atlasContext, atlasWindow, JLSCreateSurfaceTextAtlas(bd.Scan0, bd.Width, bd.Height));

			b.UnlockBits(bd);

			b.Save(@"C:\_temp\fontatlastest.png");

		}

		private static object fontCachecLock = new object();
		public static SDLFont getFont(string name, int size)
		{





			if (fontCache == null) fontCache = new Dictionary<fontLookup, SDLFont>();
			var lup = new fontLookup(name, size);

			if (!fontCache.ContainsKey(lup))
			{
				var fnt = new SDLFont(name, size);
				fontCache.Add(lup, fnt);
			}

			var r = fontCache[lup];

			return r;

		}



		protected override bool ReleaseHandle()
		{
			SDLW_TTF_CloseFont((IntPtr)this);
			return base.ReleaseHandle();

		}




		public static ConcurrentDictionary<textureTextRequest, Texture>
			renderTextCache = new ConcurrentDictionary<textureTextRequest, Texture>();


		/// <summary>
		/// real slow, not used
		/// </summary>
		/// <param name="r"></param>
		/// <param name="text"></param>
		/// <param name="col"></param>
		/// <param name="startX"></param>
		/// <param name="startY"></param>
		private void RenderTextAtlas(Renderer r, string text, SDL4Color col, int startX, int startY)
		{
			List<Rectangle> toDo = new List<Rectangle>();

			int p = 0;
			int widthwritten = 0;

			if (atlasTexture == null)
			{
				atlasTexture = new SDL2Wrappist.Texture(new SDL2Wrappist.Texture.delgTexture((o1) =>
				{

					IntPtr rptr = SDL2Wrappist.Imports.SDLW_CreateTextureFromSurface((IntPtr)this.atlasSurface,
						(IntPtr)o1);
					return rptr;
				}), r);
			}
			foreach (char c in text.ToCharArray())
			{
				if (fontAtlasPosition.ContainsKey(c))
				{
					var f = fontAtlasPosition[c];
					int[] src = new int[] {
					(int)f.X,(int)f.Y,(int)f.Width,(int)f.Height
					};

					int[] dest = new int[] {
					widthwritten, 0, (int)f.Width, (int)f.Height
					};

					widthwritten += (int)(f.Width + Kerning);

					r.renderCopy(src, dest, atlasTexture);
				}
				p++;
			}

			r.renderCopy(new int[] { 0, 0, 10, 10 }, new int[] { 0, 0, 10, 10 }, atlasTexture);



		}

		public Texture RenderText(Renderer r, string text, SDL4Color color)
		{


			var req = new textureTextRequest(this, r, text, color);

			if (!renderTextCache.ContainsKey(req))
			{
				Texture.delgTexture delg = new Texture.delgTexture((rend) =>
				{

					IntPtr ptr = SDLW_TextureFromRenderedText((IntPtr)rend, (IntPtr)this, text, color);

					return ptr;
				});
				Texture res = new Texture(delg, r);
				renderTextCache.TryAdd(req, res);
			}


			return renderTextCache[req];




		}
		public enum textAlign { LEFT = 0, CENTER = 1, RIGHT = 2 };
		public class textOptions
		{
			public textAlign align { get; set; }
			public Size bounds { get; set; }
		}

		public void RenderText(Renderer r, string text, SDL4Color color, float x, float y, textOptions opt = null)
		{
			RenderText(r, text, color, (int)x, (int)y, opt);
		}
		public void RenderText(Renderer r, string text, SDL4Color color, int x, int y, textOptions opt = null)
		{
			Texture texture = RenderText(r, text, color);

			if (opt != null)
			{
				if (opt.align == textAlign.CENTER)
				{

					int sx = opt.bounds.Width / 2;
					sx -= texture.width/2;
					int sy = opt.bounds.Height / 2;
					sy -= texture.height / 2;
					r.renderCopy(new int[] { 0, 0, texture.width, texture.height }, new int[] { x+sx, y+sy, texture.width, texture.height }, (IntPtr)texture);
				}
				else {
					r.renderCopy(new int[] { 0, 0, texture.width, texture.height }, new int[] { x, y, texture.width, texture.height }, (IntPtr)texture);
				}

			}
			else
			{
				
			}






		}

		public class fontLookup : IEquatable<fontLookup>
		{
			public fontLookup(string fontname, int size)
			{
				this.fontname = fontname;
				this.size = size;

			}

			public string fontname { get; set; }
			public int size { get; set; }

			public bool Equals(fontLookup other)
			{
				return this.fontname == other.fontname && size == other.size;
			}

			public override int GetHashCode()
			{
				return this.fontname.GetHashCode() + this.size.GetHashCode();
			}
		}

		public class textureTextRequest : IEquatable<textureTextRequest>
		{

			public SDLFont f;
			public Renderer r;
			public string txt;
			public SDL4Color c;



			public textureTextRequest(SDLFont f, Renderer r, string txt, SDL4Color c)
			{
				this.f = f;
				this.r = r;
				this.txt = txt;
				this.c = c;
			}

			public override int GetHashCode()
			{

				return f.GetHashCode() + r.internalRendererID.GetHashCode() + txt.GetHashCode() + c.GetHashCode();

			}

			public bool Equals(textureTextRequest other)
			{


				return (

					r.internalRendererID.Equals(other.r.internalRendererID) &&
					txt.Equals(other.txt)) &&
					c.Equals(other.c)

					;

			}
		}


	}


}
