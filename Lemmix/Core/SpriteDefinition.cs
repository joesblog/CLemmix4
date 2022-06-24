//using Raylib_cs;
/*using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.Rlgl;*/
using static Raylib_CsLo.Raylib;
using Raylib_CsLo;

namespace CLemmix4.Lemmix.Core
{
	public class SpriteDefinition
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public Texture Texture;
		public Image imgSprite;
		public bool CanBeDisposed = false;

		public bool TextureSetup = false;
		public bool ImageSetup = false;
		public int Rows { get; set; }
		public int Cols { get; set; }
		public int CellW { get; set; }
		public int CellH { get; set; }

		public int WidthFromCenter { get; set; }

		public virtual void initCheck()
		{
			if (!TextureSetup)
			{
				imgSprite = LoadImage(Path);
		/*		if (imgSprite.format != PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8)
					ImageFormat(ref imgSprite, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);*/
				Texture = LoadTextureFromImage(imgSprite);
				TextureSetup = true;
			}
		}

	}

 

}
