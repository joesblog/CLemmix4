//using Raylib_cs;
using System;
/*using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.Rlgl;*/
using static Raylib_CsLo.Raylib;
using Raylib_CsLo;
using System.Numerics;

namespace CLemmix4.Lemmix.Core
{
	public class SpriteFont : IDisposable
	{
		private string FileName;
		public Texture txtFont;
		public bool okay = false;
		public int fontWidth;
		public int fontHeight;
		public char startChar;
		public char endChar;
		private readonly Rectangle startRectangle;

		public SpriteFont(string fileName, int w, int h, char start, char end)
		{
			this.FileName = fileName;
			this.txtFont = LoadTexture(fileName);
			fontWidth = w;
			fontHeight = h;
			startChar = start;
			endChar = end;
			this.startRectangle = new Rectangle(0, 0, fontWidth, fontHeight);

		}

		public void DrawChar(char c, Vector2 pos, float scale = 1)
		{
			int sChar = (int)startChar;
			int cChar = (int)c;
			int x = (cChar - sChar) * fontWidth;

			DrawTexturePro(txtFont, new Rectangle(x, 0, fontWidth, fontHeight), new Rectangle(pos.X, pos.Y, fontWidth * scale, fontHeight * scale), new Vector2(0, 0), 0, WHITE);
		}



		public void DrawString(string str, Rectangle r, float scale, float kerning = 10)
		{
			var fh = fontHeight * scale;
			var fw = fontWidth * scale;
			var th = r.height;
			var yp = th / 2;
			yp -= (fh / 2);

			//var cw = str.Length - fw;

			DrawString(str, new Vector2(r.x, r.y + yp), scale, kerning: kerning);

		}
		public void DrawString(string str, Vector2 pos, float scale, float lineheight = 32, float kerning = -8, bool spaceBlank = true)
		{
			float xpos = pos.X;
			float ypos = pos.Y;
			foreach (var c in str)
			{
				if (c == '\n')
				{
					ypos += lineheight * scale;
					xpos = pos.X;
					continue;
				}
				if (spaceBlank && c == ' ')
				{
					xpos += fontWidth + kerning;
					continue;
				}

				if (spaceBlank && c == '\t')
				{
					xpos += (fontWidth + kerning) * 2;
					continue;
				}

				DrawChar(c, new Vector2(xpos, ypos), scale);
				xpos += fontWidth + kerning;



			}

		}


		public void Dispose()
		{
			if (txtFont.id > 0)
				UnloadTexture(txtFont);
		}


	}



}
