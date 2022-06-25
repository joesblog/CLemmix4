using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.Color;
using static Raylib_CsLo.RlGl;
using Raylib_CsLo;
namespace CLemmix4.Lemmix.Utils
{
	public static class Common
	{

		public static DlRectangle SizedRect(float left, float top, float width, float height) => SizedRect((int)left, (int)top, (int)width, (int)height);
		public static DlRectangle SizedRect(int left, int top, int width, int height)
		{
			return new DlRectangle(left, top, left + width, top + height);
		}

		public class DlRectangle
		{
			public DlRectangle(int left, int right, int top, int bottom)
			{
				Left = left;
				Right = right;
				Top = top;
				Bottom = bottom;
			}

			public int Left { get; set; }
			public int Right { get; set; }
			public int Top { get; set; }
			public int Bottom { get; set; }

			public int Width
			{
				get
				{
					return Math.Abs(Right - Left);
				}
			}

			public int Height
			{
				get
				{
					return Math.Abs(Bottom - Top);
				}
			}

			public static explicit operator Rectangle(DlRectangle dl)
			{
				return new Rectangle(dl.Left, dl.Top, dl.Width, dl.Height);
			}


			public DlRectangle() { }
			public void Offset(int dx, int dy)
			{
				Left += dx; Right += dx;
				Top += dy; Bottom += dy;
			}
			public static explicit operator DlRectangle(Rectangle rl)
			{
				return new DlRectangle()
				{
					Left = (int)rl.x,
					Top = (int)rl.y,
					Bottom = (int)(rl.y + rl.height),
					Right = (int)(rl.x + rl.width)
				};
			}
		}


		public delegate void DrawDelegate(Rectangle DstRect, Rectangle SrcRect, Image src);


		static void DrawTiles(DrawDelegate drawDel, DlRectangle TotalSrcRect, DlRectangle TotalDstRect, Image src)
		{
			int CountX, CountY;
			int iX, iY;
			DlRectangle SrcRect, DstRect;

			if ((TotalSrcRect.Width <= 0) || (TotalSrcRect.Height <= 0) ||
				(TotalDstRect.Width <= 0) || (TotalDstRect.Height <= 0)) return;
			CountX = (TotalDstRect.Width - 1) / TotalSrcRect.Width;
			CountY = (TotalDstRect.Height - 1) / TotalSrcRect.Height;
			for (iY = 0; iY <= CountY; iY++)
			{
				SrcRect = TotalSrcRect;
				DstRect = SizedRect(TotalDstRect.Left, TotalDstRect.Top + (iY * TotalSrcRect.Height), TotalSrcRect.Width, TotalSrcRect.Height);
				if (iY == CountY)
				{
					DstRect = SizedRect(DstRect.Left, DstRect.Top, DstRect.Width, ((TotalDstRect.Height - 1) % TotalSrcRect.Height) + 1);
					SrcRect = SizedRect(SrcRect.Left, SrcRect.Top, DstRect.Width, DstRect.Height);
				}
				for (iX = 0; iX <= CountX; iX++)
				{
					{
						if (iX == CountX)
						{
							DstRect = SizedRect(DstRect.Left, DstRect.Top, ((TotalDstRect.Width - 1) % TotalSrcRect.Width) + 1, DstRect.Height);
							SrcRect = SizedRect(SrcRect.Left, SrcRect.Top, DstRect.Width, DstRect.Height);
						}
						if (DstRect.Width > 0 && DstRect.Height > 0)
							drawDel((Rectangle)DstRect, (Rectangle)SrcRect, src);
						DstRect.Offset(TotalSrcRect.Width, 0);
					}
				}
			}

		}

 

 
	/// <summary>
	/// From lemtypes:228
	/// </summary>
	/// <param name="drawDel"></param>
	/// <param name="DstRect"></param>
	/// <param name="SrcRect"></param>
	/// <param name="Margins"></param>
	/// <param name="src"></param>
	public static void DrawNineSlice(DrawDelegate drawDel, Rectangle DstRect, Rectangle SrcRect, DlRectangle Margins, Image src)
	{

		DlRectangle[] srcRects = new DlRectangle[8];
		DlRectangle[] dstRects = new DlRectangle[8];
		int i = 0;

		bool VerifyInput()
		{
			bool result = false;
			DlRectangle CenterRect;
			// We need to ensure:
			// - Horizontal size is <= the margin sizes, if Left Margin + Right Margin = Total Source Width
			// - Equivalent for height

			//    CenterRect := Rect(Margins.Left, Margins.Top, SrcRect.Width - Margins.Right, SrcRect.Height - Margins.Bottom);
			CenterRect = new DlRectangle(Margins.Left, Margins.Top, (int)(SrcRect.width - Margins.Right), src.height - Margins.Bottom);
			result = false;

			//if (CenterRect.Width <= 0) and(DstRect.Width > Margins.Left + Margins.Right) then Exit;
			if ((CenterRect.Width <= 0) && (DstRect.width > Margins.Left + Margins.Right)) return result;
			if ((CenterRect.Height <= 0) && (DstRect.height > Margins.Top + Margins.Bottom)) return result;

			result = true;
			return result;
		}

		void TrimMargins(int LeftMargin, int RightMargin, int dstSize)
		{
			int Overlap = 0;
			Overlap = (LeftMargin + RightMargin) - dstSize;
			if (Overlap <= 0) return;

			LeftMargin = LeftMargin - (Overlap / 2);
			RightMargin = RightMargin - (Overlap / 2);

			if (Overlap % 2 == 1)
			{
				if (LeftMargin >= RightMargin)
					LeftMargin = LeftMargin - 1;
				else
					RightMargin = RightMargin - 1;
			}

			if (LeftMargin < 0)
			{
				RightMargin = RightMargin + LeftMargin;
				LeftMargin = 0;
			}
			if (RightMargin < 0)
			{
				LeftMargin = LeftMargin + RightMargin;
				RightMargin = 0;
			}

		}

		DlRectangle[] MakeNineSliceRects(DlRectangle aInput)//lemtypes : 282
		{
			DlRectangle[] Result = new DlRectangle[9];
			float VarWidth, VarHeight;
			int j;

			VarWidth = aInput.Width - (Margins.Left + Margins.Right);
			VarHeight = aInput.Height - (Margins.Top + Margins.Bottom);

				Result[0] = SizedRect(0, 0, Margins.Left, Margins.Top);
				Result[1] = SizedRect(Margins.Left, 0, VarWidth, Margins.Top);
				Result[2] = SizedRect(Margins.Left + VarWidth, 0, Margins.Right, Margins.Top);
				Result[3] = SizedRect(0, Margins.Top, Margins.Left, VarHeight);
				Result[4] = SizedRect(Margins.Left, Margins.Top, VarWidth, VarHeight);
				Result[5] = SizedRect(Margins.Left + VarWidth, Margins.Top, Margins.Right, VarHeight);
				Result[6] = SizedRect(0, Margins.Top + VarHeight, Margins.Left, Margins.Bottom);
				Result[7] = SizedRect(Margins.Left, Margins.Top + VarHeight, VarWidth, Margins.Bottom);
				Result[8] = SizedRect(Margins.Left + VarWidth, Margins.Top + VarHeight, Margins.Right, Margins.Bottom);


				for (j = 0; j <= 8; j++)
				Result[j].Offset((int)aInput.Left, (int)aInput.Top);
			return Result;
		}




		if ((DstRect.width == SrcRect.width) && (DstRect.height == SrcRect.height))
		{
			//Src.DrawTo(Dst, DstRect.Left, DstRect.Top)) 
			drawDel?.Invoke(DstRect, SrcRect, src);

		}



		TrimMargins(Margins.Left, Margins.Right, (int)DstRect.width);
		TrimMargins(Margins.Top, Margins.Bottom, (int)DstRect.height);


		srcRects = MakeNineSliceRects((DlRectangle)SrcRect);
		dstRects = MakeNineSliceRects((DlRectangle)DstRect);

		for (i = 0; i < 8; i++)
		{
				 
				DrawTiles(drawDel, srcRects[i], dstRects[i], src);
			//			drawDel?.Invoke((Rectangle)dstRects[i], (Rectangle)srcRects[i], src);
		}

	}
}
}
