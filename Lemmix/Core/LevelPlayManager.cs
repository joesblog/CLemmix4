//using Raylib_cs;
/*using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.Rlgl;*/
using CLemmix4.Lemmix.Gadget;
using Raylib_CsLo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CLemmix4.Lemmix.Core
{
	public class LevelPlayManager
	{

		public const int MAXIMUM_SI = 102;
		public const int MINIMUM_SI = 4;

		public LevelPlayManager(LevelPack.LevelData lp) {
			this.LevelData = lp;
		}
		public LemHandler lemHandler { get; set; }
		public GadgetHandler gadgHandler { get; set; }
		public int Width { get; internal set; }
		public int Height { get; internal set; }
		public LevelPack.LevelData LevelData { get; private set; }

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

		private int _SpawnInterval = -123456;
		public int SpawnInterval
		{
			get {
				if (_SpawnInterval == -123456)
				{
					_SpawnInterval = LevelData.Max_Spawn_interval;
				}
				return _SpawnInterval;

			}
			 set {

				_SpawnInterval = value;
				 
			}
		}

		public int ReleaseRate
		{
			get {
				return SpawnIntervalToReleaseRate(_SpawnInterval);
			}
			set {
				_SpawnInterval = ReleaseRateToSpawnInterval(value);
			}
		}


		public static int ReleaseRateToSpawnInterval(int releaseRate) => 103 - releaseRate;
		public static int SpawnIntervalToReleaseRate(int spawnInterval) => 103 - spawnInterval;

		[Flags]
		public enum MaskData
		{
			EMPTY = 0, TERRAIN = 1 << 1, ERASE = 1 << 2, NO_OVERWRITE = 1 << 3
		}

		public List<SpawnSystem> spawners;
		internal int NextLemmingCoundown = 20;
		internal int LemmingsOut;
		internal int _LemmingsToRelease = -1;
		internal bool ReleaseRateChanging;
		internal int SpawnIntervalModifier;

		public int LemmingsToRelease
		{
			get {

				if (_LemmingsToRelease == -1)
				{
					_LemmingsToRelease = this.LevelData.Lemmings;
				}

				return _LemmingsToRelease;
			}

			set {
				_LemmingsToRelease = value;
			}
		}

		public void SetupSpawners()
		{
			if (gadgHandler == null) return;
			spawners = new List<SpawnSystem>();

			var entrances = gadgHandler.gadgets.Where(o => o.GetType() == typeof(Gadget_Entrance)).Select(o => (Gadget_Entrance)o).ToList();

			if (entrances.Count() > 1)
			{
				foreach (var e in entrances)
				{
					//ToDo
				}
			}
			else if (entrances.Count == 1)
			{
				spawners.Add(new SpawnSystem()
				{
					amount = this.LevelData.Lemmings,
					Pos = entrances[0].Pos,
					Spawner = entrances[0]
				});
			}
		}

		public void EnableSpawneres()
		{
			if (spawners != null)
			{
				foreach (var i in spawners)
				{
					i.Spawner.AnimReadyToGo = true;
				}
			}
		}



	}

	public class SpawnSystem
	{

		public Gadget_Entrance Spawner { get; set; }
		public Vector2 Pos { get; set; }

		public int amount { get; set; }

		public bool Open
		{
			get
			{
				return Spawner.EntranceOpen;
			}
		}
	}



}
