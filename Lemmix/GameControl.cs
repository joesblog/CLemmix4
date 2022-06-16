using CLemmix4.Lemmix.Types;
using CLemmix4.Lemmix.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLemmix4.Lemmix
{

	public interface iLevelPlaceable
	{
		string Style { get; set; }
		string Piece { get; set; }
		int X { get; set; }
		int Y { get; set; }
	}

	public interface iFlaggable<E> where E : System.Enum
	{ 
	 E Flags { get; set; }
	}
	public abstract class abParsable<T>
	{

		public LevelPack levelPack { get; set; }
		protected abParsable(string source, LevelPack lp)
		{
			this.levelPack = lp;
			this.source = source;
			Parse();
		}

		public List<string> NonAssignedProperties { get; set; }
		public List<Token[]> NonAssignedTokens { get; set; }

		

		public virtual void Parse()
		{

			this.source = null;
		}
		public string source { get; set; }
	}

	public class LevelPack
	{
		public string RootDir { get; }

		public Info LevelInfo { get; }
		public LevelGroupHolder LevelGroups { get; }
		public Music Musics { get; set; }

		public PostView PostViews { get; set; }
		public LevelData testLevel { get; private set; }

		public LevelPack(string rootDir)
		{
			/*	RootDir = rootDir;
				if (File.Exists($@"{rootDir}\info.nxmi"))
					LevelInfo = new Info(File.ReadAllText($@"{rootDir}\info.nxmi"), this);



				if (File.Exists($@"{rootDir}\levels.nxmi"))
					LevelGroups = new LevelGroupHolder(File.ReadAllText($@"{rootDir}\levels.nxmi"), this);

				if (File.Exists($@"{rootDir}\info.nxmi"))
					Musics = new Music(File.ReadAllText($@"{rootDir}\music.nxmi"), this);*/

				/*if (File.Exists($@"{rootDir}\postview.nxmi"))
					PostViews = new PostView(File.ReadAllText($@"{rootDir}\postview.nxmi"), this);*/
				
		testLevel = new LevelData(File.ReadAllText(@"D:\_tempdown\NeoLemmix_V12.12.4\levels\Lemmings\Mayhem\We_all_fall_down.nxlv"),this);

			

		}

		
		public class LevelData  : abParsable<LevelData>
		{
			public LevelData(string source, LevelPack lp) : base(source, lp)
			{

			}

			public override void Parse()
			{

				using (NXMIParser<LevelData> prs = new NXMIParser<LevelData>(this))
				{
					prs.Parse();
				}
				base.Parse();
			}

			public string Title { get; set; }
			public string Theme { get; set; }
			public string ID { get; set; }
			public int Lemmings { get; set; }
			public int Save_Requirement { get; set; }
			public int Time_Limit { get; set; }
			public int Max_Spawn_interval { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
			public int Start_X { get; set; }
			public int Start_Y { get; set; }

			public List<LevelGadget> Gadget { get; set; }
			public List<LevelTerrain> Terrain { get; set; }
			public Dictionary<string, int> SkillSet { get; set; }
			public List<string> PreText { get; set; }
			public class LevelGadget : iLevelPlaceable, iFlaggable<LevelGadget.FlagsGadget>
			{

				[Flags]
				public enum FlagsGadget {

					NO_OVERWRITE = 1<<1 ,
					ONLY_ON_TERRAIN = 1<<2,
					FLIP_VERTICAL= 1 <<3,
					FLIP_HORIZONTAL = 1<<4
				
				}
				public FlagsGadget Flags { get; set; }

				public string Style { get; set;}
				public string Piece { get; set; }
				public int X { get; set; }
				public int Y { get; set; }
				public int Width { get; set; }
				public int Height { get; set; }
				public int Pairing { get; set; }
		/*		public bool No_Overwrite { get; set; }
				public bool Only_On_Terrain { get; set; }

				public bool Flip_Vertical { get; set; }
				public bool Flip_Horizontal { get; set; }*/

				public int Angle { get; set; }
				public int Speed { get; set; }
				public string Skill { get; set; }
				public int Skill_Count { get; set; }
			}

			
			public class LevelTerrain : iLevelPlaceable, iFlaggable<LevelTerrain.FlagsTerrain>
			{


				[Flags]
				public enum FlagsTerrain
				{
					NO_OVERWRITE = 1 << 1,

					ROTATE = 1<<2, 
				FLIP_HORIZONTAL = 1<<3,
				FLIP_VERTICAL = 1<<4,
				ONE_WAY = 1 << 5,
				ERASE = 1<< 6
				}
				public FlagsTerrain Flags { get; set; }

				public string Style { get; set; }
				public string Piece { get; set; }
				public int X { get; set; }
				public int Y { get; set; }


				public Vector2 pos
				{
					get {
						return new Vector2(this.X, this.Y);
					}
				}
			
				public string filePath { get {
						return $"styles/{Style}/terrain/{Piece}.png";
					} }
				 
			}
 

		}


		public class Info : abParsable<Info>
		{
			public Info(string source, LevelPack lp) : base(source, lp)
			{

			}

			public string Title { get; set; }
			public string Author { get; set; }

			public ScrollerData Scroller { get; set; }

			public override void Parse()
			{

				using (NXMIParser<Info> prs = new NXMIParser<Info>(this))
				{
					prs.Parse();
				}

				base.Parse();


			}

			public class ScrollerData
			{
				public string Moo { get; set; }
				public List<string> Line { get; set; } = new List<string>();
			}
		}

		public class LevelGroupHolder : abParsable<LevelGroupHolder>
		{
			public LevelGroupHolder(string source, LevelPack lp) : base(source, lp)
			{
			}
			public override void Parse()
			{
				using (NXMIParser<LevelGroupHolder> prs = new NXMIParser<LevelGroupHolder>(this))
				{
					prs.Parse();
				}

				base.Parse();


			}
			public bool Base { get; set; }
			public List<LevelGroup> Group { get; set; }
			public class LevelGroup
			{
				public string Name { get; set; }
				public string Folder { get; set; }
			}



		}

		public class Music : abParsable<Music>
		{
			public Music(string source, LevelPack lp) : base(source, lp)
			{
			}

			public List<string> Track { get; set; }

			public override void Parse()
			{
				using (NXMIParser<Music> prs = new NXMIParser<Music>(this))
				{
					prs.Parse();
				}


				base.Parse();

			}

		}


		public class PostView : abParsable<PostView>
		{
			public PostView(string source, LevelPack lp) : base(source, lp)
			{
			}
			public override void Parse()
			{
	
				using (NXMIParser<PostView> prs = new NXMIParser<PostView>(this))
				{
					prs.Parse();
				}

				base.Parse();

			}
			public List<ResultData> Result { get; set; }
			public class ResultData
			{
				public AffixedInteger Condition { get; set; }
				public bool percentage { get; set; }
				public List<string> Line { get; set; }
			}


		}


	}


	public class GameControl
	{

	}
}
