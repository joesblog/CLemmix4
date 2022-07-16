using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using Raylib_CsLo;
using CLemmix4.Lemmix.Gadget;
using CLemmix4.Lemmix.Utils;
using CLemmix4.Lemmix.Core;
using static Raylib_CsLo.Raylib;
using static CLemmix4.RaylibMethods;
using static CLemmix4.Lemmix.Utils.Common;
using static CLemmix4.Lemmix.Skills.skillNameHolders;

namespace CLemmix4.Lemmix.Skills
{

	public class SklExploding : absSkill
	{
		public override string Name => EXPLODING;


		public override SkillDrawType SkillType => SkillDrawType.SKILLANDPARTICLE;
		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 32, CellW = 32, Cols = 2, Rows = 1, Name = "EXPLODING", Path = "styles/default/lemmings/bomber.png", WidthFromCenter = 8, PosXOffset=-8, PosYOffset = 16 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 0;

		private SpriteDefinition _SpriteDefMask;
		public SpriteDefinition SpriteDefMask
		{
			get
			{

				if (_SpriteDefMask == null)
					_SpriteDefMask = new SpriteDefinition() { CellH = 22, CellW = 16, Cols = 1, Rows = 1, Name = "BOMBER", Path = "styles/default/mask/bomber.png", WidthFromCenter = 8 };
				return _SpriteDefMask;
			}
		}


		public override bool TryAssign(Lemming L)
		{
			if (L.LemAction.In(OHNOING, STONING, DROWNING, EXPLODING, STONEFINISH, VAPORIZING, SPLATTING, EXITING))
			{
				return false;
			}
			return base.TryAssign(L);
		}

		public override void InitCheckSprite()
		{

			SpriteDef.initCheck();
			SpriteDefMask.initCheck();

		}

		public override void DrawBespoke(Lemming L)
		{
	 

			if (L.LemEndOfAnimation)
			{
				particleDraw(L);

			}
			else {

				StandardDraw(L);
				ApplyMaskAfterBlowingUp(L);
			//	particleDraw(L);


			}
		}
		public unsafe void ApplyMaskAfterBlowingUp(Lemming L, bool Simulating = false)
		{
			Rectangle S;
			Rectangle D;
			S = new Rectangle(0, 0, SpriteDefMask.CellW, SpriteDefMask.CellH);
			D = new Rectangle();
			D.x = L.LemX - 8;
			D.y = L.LemY - 14;
			D.width = SpriteDefMask.CellW;
			D.height = SpriteDefMask.CellH;
			var msd = SpriteDefMask;
			ImageDrawCS_ApplyAlphaMask(ref L.pm.imgLevel, msd.imgSprite, S, D, BLANK);
			ImageDrawCS_ApplyAlphaMask(ref L.pm.imgPhysics, msd.imgSprite, S, D, BLANK);

			
			L.pm.imgInvalid = true;

		}

		public unsafe void ApplyMaskAfterBlowingUp( LevelPlayManager pm, int x, int y)
		{

			Rectangle S;
			Rectangle D;
			S = new Rectangle(0, 0, SpriteDefMask.CellW, SpriteDefMask.CellH);
			D = new Rectangle();
			D.x = x - 8;
			D.y = y - 14;
			D.width = SpriteDefMask.CellW;
			D.height = SpriteDefMask.CellH;
			var msd = SpriteDefMask;
			ImageDrawCS_ApplyAlphaMask(ref pm.imgLevel, msd.imgSprite, S, D, BLANK);
			ImageDrawCS_ApplyAlphaMask(ref pm.imgPhysics, msd.imgSprite, S, D, BLANK);


			pm.imgInvalid = true;

		}
		void particleDraw(Lemming L)
		{
			int i, X, Y;

			for (i = 0; i < 79; i++)
			{
				X = Particle.ParticleOffset[Particle.defaultParticleFrameCount - L.LemParticleTimer][i].dx;
				Y = Particle.ParticleOffset[Particle.defaultParticleFrameCount - L.LemParticleTimer][i].dy;
				if (X != -128 && Y != -128)
				{
					X = L.LemX + X;
					Y = L.LemY + Y;
					//DrawPixel(X, Y, Particle.ParticleColors[i % 8]);
					DrawRectangle(X, Y, 1, 1, Particle.ParticleColors[i % 8]);

				}

			}
		}

		public bool handleParticle(Lemming L)
		{
			//	((absSkill)PARTICLEEXPLODE).Transition(L);
			if (Particle.defaultParticleFrameCount - (L.LemParticleTimer + 1) >= 0)
			{
				L.LemParticleTimer++;
			}
			else
			{
				L.LemRemoved = true;
				L.pm.lemHandler.RemoveLemming(L, LemHandler.enmRemovalMode.RM_KILL);
				return false;
			}
			return false;
		}

		public override bool Handle(Lemming L)
		{
			if (L.LemEndOfAnimation)
			{
				return handleParticle(L);
			}
			else {

	
			}

			return true;
		}


	}

	public class SklParticleExplode : absSkill
	{

		public override string Name => PARTICLEEXPLODE;
		public override SkillDrawType SkillType => SkillDrawType.PARTICLE;
		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 32, CellW = 32, Cols = 2, Rows = 1, Name = "EXPLODING", Path = "styles/default/lemmings/bomber.png", WidthFromCenter = 8, PosYOffset = 16 };
				return _SpriteDef;
			}
		}

		private SpriteDefinition _SpriteDefMask;
		public SpriteDefinition SpriteDefMask
		{
			get
			{

				if (_SpriteDefMask == null)
					_SpriteDefMask = new SpriteDefinition() { CellH = 22, CellW = 16, Cols =1, Rows = 1, Name = "BOMBER", Path = "styles/default/mask/bomber.png", WidthFromCenter = 8 };
				return _SpriteDefMask;
			}
		}
		public override void InitCheckSprite()
		{
			SpriteDef.initCheck();
			SpriteDefMask.initCheck();
		}
		public override int SpriteAnimFrames => 1;


	
		public override bool Handle(Lemming L)
		{

		 
			if (Particle.defaultParticleFrameCount - (L.LemParticleTimer + 1) >= 0)
			{
				L.LemParticleTimer++;
			}
			else
			{
			//	L.LemRemoved = true;
				L.pm.lemHandler.RemoveLemming(L, LemHandler.enmRemovalMode.RM_KILL);
			
				return false;
			}
			return true;
			//return base.Handle(L);
		}
		internal override void Transition(Lemming L, bool DoTurn = false)
		{
			
			base.Transition(L, DoTurn);
			L.LemParticleTimer = 1;
		}

		public override void DrawBespoke(Lemming L)
		{


			int i, X, Y;

			for (i = 0; i < 79; i++)
			{
				X = Particle.ParticleOffset[Particle.defaultParticleFrameCount - L.LemParticleTimer][i].dx;
				Y = Particle.ParticleOffset[Particle.defaultParticleFrameCount - L.LemParticleTimer][i].dy;
				if (X != -128 && Y != -128)
				{
					X = L.LemX + X;
					Y = L.LemY + Y;
					//DrawPixel(X, Y, Particle.ParticleColors[i % 8]);
					DrawRectangle(X, Y, 1, 1, Particle.ParticleColors[i % 8]);

				}

			}
			//			base.DrawExtra(L);
		}



	}
	public class SklStartBombing : absSkill
	{
		public override string Name => STARTBOMBING;

		public override bool TryAssign(Lemming L)
		{
			if (L == null) return false;
			L.LemQueue = new List<Lemming.LQueueItem>();
			L.LemQueue.Add(new Lemming.LQueueItem() { countdown = 6, type = Lemming.LQueueItem.QueueType.TRANSITION, skill = OHNOING });
			L.LemQueueSP = 0;
			L.pm.lemHandler.ProcessLemQueue(L);
			return false;
			//return base.TryAssign(L);

		}

	}


}