using CLemmix4.Lemmix.Core;
using Raylib_CsLo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CLemmix4.Lemmix.Skills.skillNameHolders;
using static Raylib_CsLo.Raylib;
using Raylib_CsLo;
using System.Numerics;
namespace CLemmix4.Lemmix.Skills
{

	public static class skillNameHolders
	{
		public const string ASCENDING = "ASCENDING";
		public const string BASHING = "BASHING";
		public const string BLOCKING = "BLOCKING";
		public const string STARTBOMBING = "STARTBOMBING";
		public const string PARTICLEEXPLODE = "PARTICLEEXPLODE";
		public const string BUILDING = "BUILDING";
		public const string CLIMBING = "CLIMBING";
		public const string CLONING = "CLONING";
		public const string DEHOISTING = "DEHOISTING";
		public const string DIGGING = "DIGGING";
		public const string DROWNING = "DROWNING";
		public const string EXITING = "EXITING";
		public const string EXPLODING = "EXPLODING";
		public const string FALLING = "FALLING";
		public const string FENCING = "FENCING";
		public const string FIXING = "FIXING";
		public const string FLOATING = "FLOATING";
		public const string GLIDING = "GLIDING";
		public const string HOISTING = "HOISTING";
		public const string JUMPING = "JUMPING";
		public const string LASERING = "LASERING";
		public const string MINING = "MINING";
		public const string NONE = "NONE";
		public const string OHNOING = "OHNOING";
		public const string PLATFORMING = "PLATFORMING";
		public const string REACHING = "REACHING";
		public const string RELEASEFASTER = "RELEASEFASTER";
		public const string RELEASESLOWER = "RELEASESLOWER";
		public const string SHIMMYING = "SHIMMYING";
		public const string SHRUGGING = "SHRUGGING";
		public const string SLIDING = "SLIDING";
		public const string SPLATTING = "SPLATTING";
		public const string STACKING = "STACKING";
		public const string STONEFINISH = "STONEFINISH";
		public const string STONING = "STONING";
		public const string SWIMMING = "SWIMMING";
		public const string TOWALKING = "TOWALKING";
		public const string VAPORIZING = "VAPORIZING";
		public const string WALKING = "WALKING";
	}



	public abstract class absSkill : IEquatable<absSkill>
	{
		public enum SkillDrawType { SPRITE,PARTICLE, SKILLANDPARTICLE}



		public virtual SpriteDefinition SpriteDef { get; } = null;
		public virtual int SpriteAnimFrames { get; } = 0;


		public virtual absSkill[] NotAssignableFrom { get; } = null;
		public virtual absSkill[] OnlyAssignableFrom { get; } = null;


		public virtual SkillDrawType SkillType { get; } = SkillDrawType.SPRITE;

		public virtual bool IsAssignableSkill { get; } = true;
		public virtual string Name { get; } = "UNDEFINED";



		public static bool operator ==(absSkill ob1, absSkill ob2)
		{
			return ob1.Name.Equals(ob2.Name, StringComparison.OrdinalIgnoreCase);
		}

		public static bool operator !=(absSkill ob1, absSkill ob2)
		{
			return !ob1.Name.Equals(ob2.Name, StringComparison.OrdinalIgnoreCase);
		}

		public static bool operator ==(absSkill ob1, string name)
		{
			return ob1.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
		}

		public static bool operator !=(absSkill ob1, string name)
		{
			return !ob1.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
		}

		public static implicit operator absSkill(string SkillName)
		{
			return SkillHandler.lupSkillNameSkill[SkillName];
		}

		public bool Equals(absSkill other)
		{


			return this.Name == other.Name;

		}


		public bool In(params string[] arr)
		{
			bool r = false;

			foreach (var i in arr)
			{
				if (this == i)
					return true;
			}

			return r;


		}



		public bool MayAssign(Lemming L)
		{
			return true;

		}

		public virtual bool Handle(Lemming L)
		{
			return false;
		}

		protected bool HasPixelAt(Lemming L, int x, int y) => L.pm.lemHandler.HasPixelAt(x, y);

		protected int FindGroundPixel(Lemming L, int x, int y) => L.pm.lemHandler.FindGroundPixel(x, y);

		protected void RemoveTerrain(Lemming L, Rectangle rectangle) => L.pm.lemHandler.RemoveTerrain(rectangle);

		protected void RemovePixelAt(Lemming L, int cx, int cy) => L.pm.lemHandler.RemovePixelAt(cx, cy);


		protected  static void AddConstructivePixel(Lemming L, int x, int y, Color c) => L.pm.lemHandler.AddConstructivePixel(x, y, c);


		protected static void RemoveLemming(Lemming L, LemHandler.enmRemovalMode RemovalMode, bool Silent = false) => L.pm.lemHandler.RemoveLemming(L, RemovalMode, Silent);


		public virtual void DrawBespoke(Lemming L)
		{ 
		
		}
		public virtual bool TryAssign(Lemming L)
		{

			Transition(L);
			return true;
		}

		internal virtual void Transition(Lemming L, bool DoTurn = false)
		{
			bool OldIsStartingAction = false;

			if (DoTurn) TurnAround(L);
			if (this.Name == TOWALKING)
			{
				((absSkill)WALKING).Transition(L, DoTurn); return;
			};

			if (!L.pm.lemHandler.HasPixelAt(L.LemX, L.LemY) && this.Name == WALKING)
			{
				((absSkill)FALLING).Transition(L, DoTurn); return;
			}



			if (L.LemAction == this.Name) return;


			if (this.Name == FALLING)
			{
				//ignore swimming TODO

				L.LemFallen = 1;
				//	if ((new Lemming.enmLemmingState[] { Lemming.enmLemmingState.WALKING, Lemming.enmLemmingState.BASHING }).Contains(L.LemAction))
				if (L.LemAction.In(WALKING, BASHING))
				{
					L.LemFallen = 3;
				}

				//ToDo, handle mining, digging, blocking, jumping, and lazering

				L.LemTrueFallen = L.LemFallen;
			}
		//	L.skillHandler.ActionOld = L.skillHandler.ActionCurrent;
			L.LemAction = this.Name;
			L.LemFrame = 0;
			L.LemPhysicsFrame = 0;
			L.LemEndOfAnimation = false;
			OldIsStartingAction = L.LemIsStartingAction; //ToDo
			L.LemIsStartingAction = true;
			L.LemInitialFall = false;

			L.LemMaxFrame = -1;
			L.LemMaxFrame = L.LemAction.SpriteAnimFrames;
			L.LemMaxPhysicsFrame = L.LemAction.SpriteAnimFrames - 1;

		}




		public void TurnAround(Lemming L)
		{
			L.LemDx = -L.LemDx;
		}

		public Texture getTexture()
		{
			if (this.SpriteDef != null)
			{
				this.SpriteDef.initCheck();
				return this.SpriteDef.Texture;
			}
			else
			{
				var x = SkillHandler.lupSkillNameSkill["WALKING"];
				x.SpriteDef.initCheck();
				return x.SpriteDef.Texture;
			}

		}

		public SpriteDefinition GetSpriteDefinition()
		{
			if (this.SpriteDef != null) return SpriteDef;
			else return SkillHandler.lupSkillNameSkill["WALKING"].SpriteDef;
		}

		public virtual void InitCheckSprite()
		{
			if (SpriteDef != null)
				SpriteDef.initCheck();
		}

		
		/// <summary>
		/// the standard draw routine for a sprite.
		/// </summary>
		/// <param name="L"></param>
		public void StandardDraw(Lemming L)
		{

			Rectangle curFrame = new Rectangle(0, 0, SpriteDef.CellW, SpriteDef.CellH);

			curFrame.y = SpriteDef.CellH * L.LemPhysicsFrame;
			DrawTextureRec(SpriteDef.Texture, curFrame, new Vector2(L.LemX - SpriteDef.WidthFromCenter + SpriteDef.PosXOffset, L.LemY - SpriteDef.CellH + SpriteDef.PosYOffset), WHITE);
		}
	}

	public class SkillHandler
	{

		public static Dictionary<int, absSkill> lupIdSkill = InitSkillLookup();
		public static Dictionary<absSkill, int> lupSkillId;
		public static Dictionary<string, int> lupSkillNameId;
		public static Dictionary<string, absSkill> lupSkillNameSkill;

		public LemHandler lhdl { get; }
		public LevelPlayManager pm { get; }
		public Lemming lem { get; }

		public absSkill ActionCurrent { get; set; }
		public absSkill ActionOld { get; set; }
		public absSkill ActionNext { get; set; }

		public SkillHandler(LemHandler lemmingHandler, LevelPlayManager playerManager, Lemming Lem)
		{
			lhdl = lemmingHandler;
			pm = playerManager;
			this.lem = Lem;
			this.ActionNext = lupSkillNameSkill["NONE"];
		}

		public static IEnumerable<absSkill> allSkills
		{
			get
			{
				return lupSkillId.Select(o => o.Key);
			}
		}

		public absSkill ActionPriorToExploting { get; internal set; }

		public void Transition(bool DoTurn = false)
		{
			//	lem.skillHandler.ActionCurrent.TransitionTo(lem, lem.skillHandler.ActionNext.Name, turnaroudn);

			ActionNext.Transition(lem, DoTurn);
		}
		public void Transition(absSkill ToSkill, bool DoTurn = false)
		{
			//	lem.LemAction = ToSkill;
			//lem.skillHandler.ActionCurrent.TransitionTo(lem, ToSkill.Name, turnaround);
			ToSkill.Transition(lem, DoTurn);
		}



		private static Dictionary<int, absSkill> InitSkillLookup()
		{
			var abs = typeof(absSkill)
				 .Assembly.GetTypes()
				 .Where(o => o.IsSubclassOf(typeof(absSkill)) && !o.IsAbstract)
				 .Select(o => (absSkill)Activator.CreateInstance(o)).ToArray();
			Dictionary<int, absSkill> r = new Dictionary<int, absSkill>();
			Dictionary<string, int> r2 = new Dictionary<string, int>();
			Dictionary<absSkill, int> r3 = new Dictionary<absSkill, int>();
			Dictionary<string, absSkill> r4 = new Dictionary<string, absSkill>();


			for (int i = 0; i < abs.Length; i++)
			{
				var c = abs[i];
				r.Add(i, c);
				r2.Add(c.Name, i);
				r3.Add(c, i);
				r4.Add(c.Name, c);
			}

			lupSkillId = r3;
			lupSkillNameId = r2;
			lupSkillNameSkill = r4;
			return r;

		}
		public bool Handle()
		{

			return lem.skillHandler.ActionCurrent.Handle(this.lem);

		}

		public static void InitCheckSprites()
		{
			/*foreach (var i in allSkills)
			{
				if (i.SpriteDef != null) i.SpriteDef.initCheck();
				
			}*/

			foreach (var i in lupSkillNameSkill)
			{
				i.Value.InitCheckSprite();
			}
		}
	}

}
