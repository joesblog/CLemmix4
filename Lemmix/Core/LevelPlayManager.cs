//using Raylib_cs;
/*using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.Rlgl;*/
using CLemmix4.Lemmix.Gadget;
using Raylib_CsLo;
using System;

namespace CLemmix4.Lemmix.Core
{
	public class LevelPlayManager
	{
		public LemHandler lemHandler { get; set; }
		public GadgetHandler gadgHandler { get; set; }
		public int Width { get; internal set; }
		public int Height { get; internal set; }

		//public int[] mask;
		public MaskData[] mask;
		public Image imgLevel;
		public Texture texLevel;

		public Image imgPhysics;
		public Texture texPhysics;

		public Image imgGadgets;
		public Texture texGadgets;
		public RenderTexture texGadgetsTarget;

		public int size;
		public bool imgInvalid = false;

		public bool gadImgInvalid = false;
		[Flags]
		public enum MaskData
		{
			EMPTY = 0, TERRAIN = 1 << 1, ERASE = 1 << 2, NO_OVERWRITE = 1 << 3
		}

	}



}
