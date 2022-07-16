//using Raylib_cs;
using System.Collections.Generic;
/*using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.Rlgl;*/
using static Raylib_CsLo.Raylib;
using Raylib_CsLo;
using System.Numerics;
using System;
using CLemmix4.Lemmix.Skills;
using static CLemmix4.Lemmix.Skills.skillNameHolders;
namespace CLemmix4.Lemmix.Core
{
	public class Lemming
	{

		#region sprite stuff
		public int spriteH = 10;
		public int spriteW = 16;
		public int spriteRows = 8;
		public int spriteCols = 1;
		public Vector2 frameSize = new Vector2(32, 80);
		public string dbgString = "~";

		#endregion

		public LevelPlayManager pm { get; }
		public int ID { get; }

		public SkillHandler skillHandler;

		[Flags]
		public enum enmLemmingState
		{
			None,
			WALKING,
			ASCENDING,
			DIGGING,
			CLIMBING,
			DROWNING,
			HOISTING,
			BUILDING,
			BASHING,
			MINING,
			FALLING,
			FLOATING,
			SPLATTING,
			EXITING,
			VAPORIZING,
			BLOCKING,
			SHRUGGING,
			OHNOING,
			EXPLODING,
			TOWALKING,
			PLATFORMING,
			STACKING,
			STONING,
			DEHOISTING,

			STONEFINISH,
			SWIMMING,
			GLIDING,
			FIXING,
			FENCING,
			REACHING,
			SHIMMYING,
			JUMPING,
			SLIDING,
			LASERING,
			CLONING,


			RELEASESLOWER = -10,
			RELEASEFASTER = -11
		}

		public enum lemmingstate
		{
			none, walking, falling, ascending
		}

		//	Texture spriteTex;


		private Texture getSpriteTexture(absSkill act)
		{
			//if (LemHandler.dictLemmingSpriteDefs.ContainsKey(this.LemAction))
			//{
			//	var x = LemHandler.dictLemmingSpriteDefs[this.LemAction];
			//	x.initCheck();
			//	/*if (!x.TextureSetup)
			//	{
			//		x.Texture = LoadTexture(x.Path);
			//		x.TextureSetup = true;
			//	}*/
			//	return x.Texture;
			//}
			//else
			//{
			//	var x = LemHandler.dictLemmingSpriteDefs[enmLemmingState.WALKING];
			//	/*	if (!x.TextureSetup)
			//		{
			//			x.Texture = LoadTexture(x.Path);
			//		}*/
			//	x.initCheck();
			//	return x.Texture;


			//}*/

			return act.getTexture();
		}
		public Texture spriteTex
		{
			get
			{
				return getSpriteTexture(this.LemAction);

			}
		}

		public SpriteDefinition spriteDef
		{
			get
			{
				return this.LemAction.GetSpriteDefinition();
			}
		}


		public int LemX = 0;
		public int LemY = 0;

		public int LemParticleTimer { get; set; } //= 4;

		public bool IsNuking = false;
		public int LemNumberOfBricksLeft { get; set; }
		public bool LemConstructivePositionFreeze { get; internal set; }
		public bool LemHasBlockerField { get; internal set; }

		public bool UnderMouse = false;

		public int LemMaxPhysicsFrame = 0;

		//	public int LemDY = -1;

		public int LemXOld = 0;
		public int LemYOld = 0;

		public int LemDx = 1;
		/*public absSkill LemAction = (absSkill)SkillHandler.lupSkillNameSkill["NONE"];
		public absSkill LemActionOld = (absSkill)SkillHandler.lupSkillNameSkill["NONE"];
		public absSkill LemActionNext = (absSkill)SkillHandler.lupSkillNameSkill["NONE"];*/

		public absSkill LemAction { get { return this.skillHandler.ActionCurrent; } set { this.skillHandler.ActionCurrent = value; } }
		public bool fLemJupToHoistAdvance = false;


		public static int lastId = 1;
		public Lemming(LevelPlayManager _lpm)
		{

			this.pm = _lpm;
			this.ID = ++lastId;

			this.skillHandler = new SkillHandler(this.pm.lemHandler, this.pm, this);
		}

		public class LQueueItem
		{

			public enum QueueType { WAIT = 0, TRANSITION = 1, }

			public QueueType type { get; set; }
			public absSkill skill { get; set; }

			public int countdown { get; set; }

		}

		public List<LQueueItem> LemQueue;
		public int LemQueueSP = 0;

		//public int framecounter = 0;
		internal bool LemRemoved;
		internal int lemDXOld;
		internal int LemFrame;
		internal int LemPhysicsFrame;
		public bool LemIsClimber = false;
		public bool LemIsFloater = false;
		internal int LemAscended = 0;
		internal int LemFallen = 0;
		internal int LemTrueFallen = 0;
		internal bool LemEndOfAnimation;
		internal bool LemIsStartingAction;
		internal bool LemInitialFall;
		internal int LemMaxFrame;
		public bool _particleDrawingOn = false;

		public bool particleDrawingOn { get {
				return _particleDrawingOn;
			}
			set {
				if (_particleDrawingOn != value)
				{
					this.LemParticleTimer = 0;
					_particleDrawingOn = value;
				}
			}
		}



		public Dictionary<string, object> TagData = new Dictionary<string, object>();

		public void SetTag<T>(string name, T data)
		{

			if (TagData.ContainsKey(name)) TagData[name] = data;
			else TagData.Add(name, data);
		}

		public T GetTag<T>(string name)
		{
			if (TagData.ContainsKey(name))
				return (T)TagData[name];


			return default;

		}

		public bool HasTag(string name) => TagData.ContainsKey(name);

		public void Draw(Vector2 vec) => Draw((int)vec.X, (int)vec.Y);
		public void Draw(int _x, int _y)
		{
			LemX = _x;
			LemY = _y;

		}



		int frame = 0;
		int maxFrame = 8;
		internal Rectangle PositionalRectangle;
		internal string OverHeadText;

	
		public void DrawLemmingParticles() //:680
		{
			int i, X, Y;

			for (i = 0; i < 79; i++)
			{
				X = Particle.ParticleOffset[Particle.defaultParticleFrameCount - LemParticleTimer][i].dx;
				Y = Particle.ParticleOffset[Particle.defaultParticleFrameCount - LemParticleTimer][i].dy;
				if (X != -128 && Y != -128)
				{
					X = LemX + X;
					Y = LemY + Y;
					//DrawPixel(X, Y, Particle.ParticleColors[i % 8]);
					DrawRectangle(X, Y, 1, 1, Particle.ParticleColors[i % 8]);

				}

			}

		}

		public void Draw()
		{


			if (skillHandler.ActionCurrent.SkillType != absSkill.SkillDrawType.SPRITE)
			{
				skillHandler.ActionCurrent.DrawBespoke(this);
				//DrawLemmingParticles();
				return;
			}
			Rectangle curFrame = new Rectangle(0, 0, spriteDef.CellW, spriteDef.CellH);

			/*LemFrame++;
			if (LemFrame > LemMaxFrame)
			{
				frame++;
				if (frame >= spriteDef.Rows)
				{
					frame = 0;
				}
				LemFrame = 0;

			}*/


			curFrame.y = spriteDef.CellH * LemPhysicsFrame;

			if (LemDx < 0)
				curFrame.x = 0;
			else
				curFrame.x = spriteDef.CellW;

			Color ToDraw = WHITE;
			if (UnderMouse) ToDraw = RED;

			//	DrawTextureRec(spriteTex, curFrame, new Vector2(LemX - 8, LemY - 10), ToDraw);
			DrawTextureRec(spriteTex, curFrame, new Vector2(LemX - spriteDef.WidthFromCenter, LemY - spriteDef.CellH + spriteDef.PosYOffset), ToDraw);
		//	DrawLemmingParticles();
			if (this.LemAction == BUILDING)
			{
				DrawText($"{this.LemNumberOfBricksLeft}", LemX - spriteDef.WidthFromCenter, LemY, 10, WHITE);
			}
			else
			{
				//	DrawText($"{LemAction.Name}", LemX - spriteDef.WidthFromCenter, LemY, 10, WHITE);


				//DrawText($"{this.LemX},{this.LemY}", LemX - spriteDef.WidthFromCenter/2, LemY-10, 4, WHITE);

			}
			//	DrawText(dbgString, LemX - 5, LemY - 20, 10, WHITE);
			//DrawRectangleLinesEx(new Rectangle(LemX  - spriteDef.WidthFromCenter/2, LemY- spriteDef.CellH, spriteDef.WidthFromCenter, spriteDef.CellH), 1f, ToDraw);

			if (OverHeadText != null && OverHeadText.Length > 0)
				DrawText(OverHeadText, LemX - spriteDef.WidthFromCenter, LemY - 17, 10, WHITE);
			//skillHandler.ActionCurrent.DrawExtra(this);
		}


	}



}
