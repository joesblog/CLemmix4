﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CLemmix4.Lemmix.Core;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace CLemmix4.Lemmix.Gadget
{
	public class GadgetHandler : IDisposable
	{


		//not sure if we're going to offload this to another thread
		//or use lemhandler
		//private ThreadStart tsGadgetHandler;
		//public Thread thGadget;
		//public bool threadRunning { get; private set; }

		public TextureCacheData tcdGadgetAnims;

		public LevelPlayManager lpm { get; private set; }
		public LevelPack.LevelData levelData { get; private set; }



		public Dictionary<int, bool> gadIds;
		public unsafe void toggleGadget(int id)
		{

			if (gadIds == null || id == 11)
			{
				foreach (var i in gadIds) { gadIds[i.Key] = false; }
			}
			else if (gadIds != null && gadIds.ContainsKey(id))
			{
				gadIds[id] = !gadIds[id];
			}


		}

		public LemHandler lh { get; set; }

		public ConcurrentBag<absGadget> gadgets = new ConcurrentBag<absGadget>();
		public GadgetHandler(LevelPack.LevelData _levelData, LevelPlayManager _lpm)
		{
			lpm = _lpm;
			levelData = _levelData;
			lh = _lpm.lemHandler;
			tcdGadgetAnims = new TextureCacheData();
			noow = new bool[lpm.size];


		}

		public bool[] noow;

		static Dictionary<string, Type> gadgetTypes;

		public void Setup()
		{
			if (gadgetTypes == null)
			{
				gadgetTypes = new Dictionary<string, Type>();
				var insts = typeof(absGadget)
						.Assembly
						.GetTypes()
						.Where(o => o.IsSubclassOf(typeof(absGadget)) && !o.IsAbstract)
						.Select(o => (absGadget)Activator.CreateInstance(o));
				foreach (var i in insts)
				{
					if (!gadgetTypes.ContainsKey(i.GadgetName))
					{
						gadgetTypes.Add(i.GadgetName, i.GetType());
					}
				}

			}
			noow = new bool[lpm.size];
			gadIds = new Dictionary<int, bool>();
			foreach (var i in levelData.Gadget)
			{


				if (i.Flags.HasFlag(LevelPack.LevelData.LevelGadget.FlagsGadget.NO_OVERWRITE))
					for (var y = i.Y; y < i.Y + i.Height; y++)
					{
						for (var x = i.X; x < i.X + i.Width; x++)
						{
							int ix = y * lpm.Width + x;
							if (ix < 0 || ix > lpm.size - 1) continue;
							noow[ix] = true;

						}
					}
				var desc = tcdGadgetAnims[i];

				if (i.EffectData != null)
				{
					if (gadgetTypes.ContainsKey(i.EffectData.Effect))
					{
						var typ = gadgetTypes[i.EffectData.Effect];
						if (typ != null)
						{
							var gdg = (absGadget)Activator.CreateInstance(typ, new object[] { i, this });


							gadIds.Add(gdg.gadgetId, false);
							gadgets.Add(gdg);
							gdg.Setup();
						}
					}

				}


			}
		}




		public int orderd = 0;
		public bool[] fmask = null;
		public int[] smask = null;
		bool pauseRender = false;
		public unsafe void RenderAll()
		{
			if (pauseRender) return;
			if (fmask == null)
			{
				fmask = new bool[this.lpm.size];

			}
			else Array.Fill<bool>(fmask, false, 0, this.lpm.size);

			if (smask == null)
			{
				smask = new int[this.lpm.size];

			}
			else Array.Fill<int>(smask, 0, 0, this.lpm.size);



			orderd = 0;
			bool invalid = false;

			if (lpm.texGadgetsTarget.id == 0)
				lpm.texGadgetsTarget = LoadRenderTexture(lpm.Width, lpm.Height);
			//ImageAlphaClear(ref lpm.imgGadgets, BLANK, 1);
			/*	fixed (Image* ptr = &lpm.imgGadgets)
				{
					ImageClearBackground(ptr, BLANK);
				}*/

			BeginTextureMode(lpm.texGadgetsTarget);

			Image lastFrame = ImageCopy(lpm.imgGadgets);
			foreach (var i in gadgets)
			{
				i.DrawOfFrame(lastFrame);

				invalid = true;
			}
			UnloadImage(lastFrame);
	
			Color* pxGadgets = LoadImageColors(lpm.imgGadgets);
			UpdateTexture(lpm.texGadgets, pxGadgets);

			UnloadImageColors(pxGadgets);


			ClearBackground(BLANK);

			DrawTexture(lpm.texGadgets, 0, 0, WHITE);

			EndTextureMode();

			//lpm.gadImgInvalid = true;
		}





		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}


	public abstract class absGadget
	{
		public static int lastgadgetid = 0;

		public int frameMax { get; set; }
		public int frameCur { get; set; }
		internal TextureCacheData.TCDDesription gadgetAnimTexture { get; set; }
		public LevelPack.LevelData.LevelGadget GadgetDef { get; private set; }
		public GadgetHandler gadHandler { get; private set; }

		public int gadgetId = -1;

		public virtual string GadgetName { get; } = "none";

		public absGadget(LevelPack.LevelData.LevelGadget lvlGadget, GadgetHandler handler)
		{
			this.GadgetDef = lvlGadget;
			this.gadHandler = handler;
			this.gadgetId = ++lastgadgetid;
		}

		public virtual void Setup()
		{
			if (this.GadgetDef.EffectData != null)
			{
				if (this.GadgetDef.EffectData.Primary_Animation != null)
				{
					this.frameMax = this.GadgetDef.EffectData.Primary_Animation.Frames;
				}
			}


			gadgetAnimTexture = gadHandler.tcdGadgetAnims[this.GadgetDef];
		}

		public virtual void LogicOfFrame()
		{

		}
	
		public unsafe virtual void DrawOfFrame(Image lastFrame)
		{

			

		}

		public virtual bool HasTriggerd(Lemming L, out bool AbortChecks)
		{
			AbortChecks = false;
			return false;
		}

		public virtual void TriggerEffect(Lemming L)
		{

		}

	}



}
