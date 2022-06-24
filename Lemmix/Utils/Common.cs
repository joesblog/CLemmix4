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



		public delegate void DrawDelegate(Rectangle DstRect, Rectangle SrcRect, Image src);

		/// <summary>
		/// From lemtypes:228
		/// </summary>
		/// <param name="drawDel"></param>
		/// <param name="DstRect"></param>
		/// <param name="SrcRect"></param>
		/// <param name="Margins"></param>
		/// <param name="src"></param>
		public static void DrawNineSlice(DrawDelegate drawDel, Rectangle DstRect, Rectangle SrcRect, Rectangle Margins, Image src)
		{

			Rectangle[] srcRects = new Rectangle[8];
			Rectangle[] dstRects = new Rectangle[8];
			int i = 0;

			bool VerifyInput()
			{
				bool result = false;
				Rectangle CenterRect;
				// We need to ensure:
				// - Horizontal size is <= the margin sizes, if Left Margin + Right Margin = Total Source Width
				// - Equivalent for height

				//    CenterRect := Rect(Margins.Left, Margins.Top, SrcRect.Width - Margins.Right, SrcRect.Height - Margins.Bottom);
				CenterRect = new Rectangle(Margins.X, Margins.Y, SrcRect.width - Margins.getRight(), SrcRect.height - Margins.getBottom());
				result = false;

				//if (CenterRect.Width <= 0) and(DstRect.Width > Margins.Left + Margins.Right) then Exit;
				if ((CenterRect.width <= 0) && (DstRect.width > Margins.x + Margins.getRight())) return result;
				if ((CenterRect.height <= 0) && (DstRect.height > Margins.Y + Margins.getBottom())) return result;

				result = true;
				return result;
			}

			void TrimMargins(ref int LeftMargin, ref int RightMargin, int dstSize)
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

			Rectangle[] MakeNineSliceRects(Rectangle aInput)//lemtypes : 282
			{
				Rectangle[] Result = new Rectangle[9];
				float VarWidth, VarHeight;
				int i;

				VarWidth = aInput.width - (Margins.X - Margins.getRight());
				VarHeight = aInput.height - (Margins.height - Margins.getBottom());

				Result[0] = new Rectangle(0, 0, Margins.X, Margins.Y);
				Result[1] = new Rectangle(Margins.X, 0, VarWidth, Margins.Y);
				Result[2] = new Rectangle(Margins.X + VarWidth, 0, Margins.getRight(), Margins.Y);

				Result[3] = new Rectangle(0, Margins.Y, Margins.X, VarHeight);
				Result[4] = new Rectangle(Margins.X, Margins.Y, VarWidth, VarHeight);
				Result[5] = new Rectangle(Margins.X + VarWidth, Margins.Y, Margins.getRight(), VarHeight);

				Result[6] = new Rectangle(0, Margins.Y + VarHeight, Margins.X, Margins.getBottom());
				Result[7] = new Rectangle(Margins.X, Margins.Y + VarHeight, VarWidth, Margins.getBottom());
				Result[8] = new Rectangle(Margins.X + VarWidth, Margins.Y + VarHeight, Margins.getRight(), Margins.getBottom());

				for (i = 0; i < 8; i++)
					Result[i].Offset(aInput.X, aInput.Y);
				return Result;
			}

			void DrawTiles(Rectangle TotalSrcRect, Rectangle TotalDstRect)
			{
				int CountX, CountY;
				int iX, iY;
				Rectangle SrcRect, DstRect;
				if ((TotalSrcRect.width <= 0) || (TotalSrcRect.height <= 0) ||
				(TotalDstRect.width <= 0) || (TotalDstRect.height <= 0)) return;

				CountX = (int)(TotalDstRect.width - 1) / (int)TotalSrcRect.width;
				CountY = (int)(TotalDstRect.height - 1) / (int)TotalSrcRect.height;

				for (iY = 0; iY <= CountY; iY++)
				{
					SrcRect = TotalSrcRect;
					DstRect = new Rectangle(TotalDstRect.X, TotalDstRect.Y + (iY * TotalSrcRect.height), TotalSrcRect.width, TotalSrcRect.height);

					if (iY == CountY)
					{
						DstRect = new Rectangle(DstRect.X, DstRect.Y, DstRect.width, ((TotalDstRect.height - 1) % TotalSrcRect.height) + 1);
						SrcRect = new Rectangle(SrcRect.X, SrcRect.Y, DstRect.width, DstRect.height);
					}

					for (iX = 0; iX <= CountX; iX++)
					{
						if (iX == CountX)
						{
							DstRect = new Rectangle(DstRect.X, DstRect.Y, ((TotalDstRect.width - 1) % TotalSrcRect.width) + 1, DstRect.height);
							SrcRect = new Rectangle(SrcRect.X, SrcRect.Y, DstRect.width, DstRect.height);

						}

						drawDel?.Invoke(DstRect, SrcRect, src);

						DstRect.Offset(TotalSrcRect.width, 0);
					}

				}

			}


			if ((DstRect.width == SrcRect.width) && (DstRect.height == SrcRect.height))
			{
				//Src.DrawTo(Dst, DstRect.Left, DstRect.Top)) 
				drawDel?.Invoke(DstRect, SrcRect, src);

			}

			int marginLeft = (int)Margins.x;
			int marginRight = (int)Margins.getRight();
			int marginTop = (int)Margins.y;
			int marginBottom = (int)Margins.getBottom();

			TrimMargins(ref marginLeft, ref marginRight, (int)DstRect.width);
			TrimMargins(ref marginTop, ref marginBottom, (int)DstRect.height);
			Margins.x = marginLeft;
			Margins.y = marginTop;
			Margins.width = marginRight - marginLeft;
			Margins.height = marginBottom - marginTop;

			srcRects = MakeNineSliceRects(SrcRect);
			dstRects = MakeNineSliceRects(DstRect);

			for (i = 0; i < 8; i++)
			{
		drawDel?.Invoke(dstRects[i], srcRects[i], src);
			}

		}
	}
}
