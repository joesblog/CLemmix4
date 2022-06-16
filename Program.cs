using CLemmix4.Lemmix;
using CLemmix4.Lemmix.Utils;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using CLemmix4.Lemmix.Core;

namespace CLemmix4
{
	internal static class Program
	{
		private static LevelPack lp;
		private static TextureCacheData tchterrain;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// 

		[STAThread]
		static unsafe void Main()
		{ 
			lp = new LevelPack(@"D:\_tempdown\NeoLemmix_V12.12.4\levels\Lemmings\");
			SceneManager sm = new SceneManager();
			sm.Initialize(new LevelScene(sm, lp.testLevel));
		}

		static unsafe void Main2()
		{
			lp = new LevelPack(@"D:\_tempdown\NeoLemmix_V12.12.4\levels\Lemmings\");

			int width = 1024;
			int height = 768;
			int ow = 320;
			int oh = 240;

			float rW = (float)width / (float)ow;
			float xOff = -lp.testLevel.Start_X;
			float yOff = lp.testLevel.Start_Y - 80;
			float scale = rW;
			Raylib.InitWindow(width, height, "test");
			//https://gist.github.com/JeffM2501/3c7da5c2b7e078e254d673f91489c78f
			Camera2D cam = new Camera2D(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(0), 0, 1);
			cam.zoom = rW;
			cam.target = GetScreenToWorld2D(cam.offset + new System.Numerics.Vector2(lp.testLevel.Start_X,lp.testLevel.Start_Y -80), cam);
			tchterrain = new TextureCacheData();
			foreach (var i in lp.testLevel.Terrain.GroupBy(o => o.Style + o.Style).Select(o => o.First()))
			{
				var op = tchterrain[i];
			}


		
			SetTargetFPS(60);
			SetMousePosition(width / 2, height / 2);
			var lvl = lp.testLevel;


			var img = GenImageColor(lvl.Width, lvl.Height, WHITE);
			ImageFormat(ref img, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);

			ImageAlphaClear(ref img, Color.WHITE, 1f);
			ImageAlphaClear(ref img, Color.BLANK, 1f);
			int size = lvl.Width * lvl.Height;
			bool[] mask = new bool[lvl.Width * lvl.Height];
			List<Rectangle> hps = new List<Rectangle>();
			foreach (var i in lvl.Terrain)
			{
				var tc = tchterrain[i];

				if (!i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.ERASE))
				{
					hps.Add(new Rectangle(i.X, i.Y, tc.Width, tc.Height));
				}
				for (int y = 0; y < tc.Height; y++)
				{
					int ypos = y + i.Y;
					for (int x = 0; x < tc.Width; x++)
					{
						int xpos = x + i.X;
						int ix = ypos * lvl.Width + xpos;
						int locix = y * tc.Width + x;
						if (ix > size || ix < 0) continue;
						if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.NO_OVERWRITE) && mask[ix])
						{
							//	continue;
						}

						var imgToUse = tc.imgMain;

						if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_HORIZONTAL) && i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_VERTICAL)) imgToUse = tc.imgVertHorizFlip;
						else if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_HORIZONTAL)) imgToUse = tc.imgHorizFlip;
						else if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_VERTICAL)) imgToUse = tc.imgVertFlip;

						var colsrc = GetImageColor(imgToUse, x, y);

						byte a = colsrc.a;
						byte r = colsrc.r;
						byte g = colsrc.g;
						byte b = colsrc.b;

						if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.ERASE))
						{

							if (a > 0)
							{
								ImageDrawPixel(ref img, xpos, ypos, Color.BLANK);
								mask[ix] = false;

								continue;
							}
						}
						if (a > 0)
						{
							if (!(i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.NO_OVERWRITE) && mask[ix]))
							{
								ImageDrawPixel(ref img, xpos, ypos, colsrc);

							}

							mask[ix] = true;
							continue;
						}


					}
				}



			}


			var tex = LoadTextureFromImage(img);



			var tx = tchterrain.First();
			tchterrain.DisposeImages();

			var t = new Rectangle();
			Color bg = new Color(0, 0, 52, 255);
			bool quit = false;
			//	while (!Raylib.WindowShouldClose())

			var prevMouse = GetMousePosition();
			var screenW = GetScreenWidth();
			var screenWM = screenW * 0.05;
		while(!quit)
			{
				float mod = 1;
				if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT)) mod += 9f;
				if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL)) mod += 9f;

				if (IsKeyDown(KeyboardKey.KEY_RIGHT)) xOff -= 0.2f * mod;
				if (IsKeyDown(KeyboardKey.KEY_LEFT)) xOff += 0.2f * mod;
				if (IsKeyDown(KeyboardKey.KEY_PAGE_UP)) scale += 0.01f * mod;
				if (IsKeyDown(KeyboardKey.KEY_PAGE_DOWN)) scale -= 0.01f * mod;
				if (IsKeyDown(KeyboardKey.KEY_END)) scale = rW;
				if (IsKeyDown(KeyboardKey.KEY_ESCAPE)) quit = true;
				var thisPos = GetMousePosition();
				var delta = prevMouse - thisPos;

				if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
				{
					cam.target.X += GetMouseDelta().X; // = GetScreenToWorld2D(cam.offset + delta, cam);

				}
				else {
					if (thisPos.X > screenW - (screenWM))
						cam.target.X += 0.75f;
					else if (thisPos.X < screenWM)
						cam.target.X -= 0.75f;
				}
				prevMouse = thisPos;



				Raylib.BeginDrawing();
				Raylib.ClearBackground(bg);

				BeginMode2D(cam);

				//	DrawTextureEx(tex, new System.Numerics.Vector2(0 ,0), 0f, scale,WHITE );
				DrawTexture(tex, 0, 0, WHITE);
				EndMode2D();
			/*	if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
				{
					var del = GetMouseDelta();
					//DrawText($"{del}", 200, 200, 18, WHITE);
					xOff += del.X;
					yOff += del.Y;
				}
				foreach (var i in hps)
				{
					t = new Rectangle((i.x *scale) + xOff, (i.y * scale)+yOff, i.width * (scale), i.height * (scale));
					if (CheckCollisionPointRec(mousePos, t))
					{
						

						DrawRectangleLinesEx(  t, 2f, RED);
						break;
					}

				}*/

 

				Raylib.EndDrawing();
			}
			UnloadTexture(tex);
		}

		public static int[] generateMaskLayer(LevelPack.LevelData ld, out int w, out int h)
		{

			w = 0; h = 0; return null;
		}


		#region randomshit
		public static string findAllNestedObjects(string root)
		{
			StringBuilder sb = new StringBuilder();

			return fano(new DirectoryInfo(root), ref sb);



		}
		static string fano(DirectoryInfo dinf, ref StringBuilder sb)
		{
			foreach (FileInfo finf in dinf.GetFiles())
			{
				if (finf.Name.EndsWith(".nxmi") || finf.Name.EndsWith("nxlv"))
				{
					//StringBuilder sb = new StringBuilder();
					string[] ln = File.ReadAllLines(finf.FullName);

					for (int i = 0; i < ln.Length; i++)
					{
						string c = ln[i];
						if (c.Trim().StartsWith("$") && c != "$END".Trim())
						{
							processFano(ln, ref i, ref sb, true, finf.FullName);
						}
						if (i > ln.Length - 1) break;
					}
				}
			}
			foreach (DirectoryInfo sub in dinf.GetDirectories())
			{
				fano(sub, ref sb);
			}

			return sb.ToString();
		}

		private static void processFano(string[] ln, ref int i, ref StringBuilder sb, bool root = false, string filename = "")
		{

			string rob = root ? "" : $"@NESTED@{filename}";
			while (true)
			{
				i++;
				if (i > ln.Length - 1) break;
				string c = ln[i];
				if (c.Trim().StartsWith("$") && c != "$END".Trim())
				{
					processFano(ln, ref i, ref sb, false, filename);
				}

				sb.AppendLine(c);



			}
		}

		public static Dictionary<string, string> findAllOptionsForObject(string root, string oname = "GADGET")
		{
			Dictionary<string, string> r = new Dictionary<string, string>();
			DirectoryInfo dinf = new DirectoryInfo(root);


			faofo(dinf, oname, ref r);
			return r;
		}


		static Regex rProp = new Regex(@"^\s{0,}(?<id>[A-Z_]+).*?$");

		public static void faofo(DirectoryInfo dinf, string onobj, ref Dictionary<string, string> r)
		{

			foreach (var i in dinf.GetFiles())
			{
				if (!i.Name.EndsWith(".nxlv")) continue;
				string[] ln = File.ReadAllLines(i.FullName);
				int x = 0;
				while (true)
				{
					if (x > ln.Length - 1) break;
					string l = ln[x].Trim();

					if (l == $"${onobj}")
					{
						while (true)
						{
							x++;

							if (x > ln.Length - 1) break;
							l = ln[x];
							if (l == "$END") break;

							if (rProp.IsMatch(l))
							{
								string id = rProp.Match(l).Groups["id"].Value;
								if (!r.ContainsKey(id)) r.Add(id, $"{i.FullName}:{x:0000}");
							}

						}
					}

					x++;
				}

			}
			foreach (var i in dinf.GetDirectories())
			{
				faofo(i, onobj, ref r);
			}

		}



		#endregion
	}
}
