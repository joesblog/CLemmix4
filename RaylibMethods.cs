
//using Raylib_cs;
//using static Raylib_cs.Raylib;
//using static Raylib_cs.Color;
//using static Raylib_cs.Rlgl;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.Color;
using static Raylib_CsLo.RlGl;
using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;
using System.Runtime.InteropServices;
using System;
using CLemmix4.Lemmix.Utils;
using CLemmix4.Lemmix.Core;
using static CLemmix4.Lemmix.Utils.ext;

namespace CLemmix4
{
	public static class RaylibMethods
	{

		[DllImport("msvcrt.dll", SetLastError = false)]
		static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);
		private static int NULL = 0;


		public static unsafe void ImageDrawCS_ApplyAlphaMask(ref Image img, Image src, Rectangle srcRec, Rectangle dstRec, Color tint)
		{
			fixed (Image* ds = &img)
			{
				ImageDrawCS_ApplyAlphaMask(ds, src, srcRec, dstRec, tint);
			}
		}
		public static unsafe void ImageFlipVertical(ref Image img)
		{

			fixed (Image* p = &img)
			{
				Raylib.ImageFlipVertical(p);
			}
		}
		public static unsafe void ImageFlipHorizontal(ref Image img)
		{

			fixed (Image* p = &img)
			{
				Raylib.ImageFlipHorizontal(p);
			}
		}
		public static unsafe void ImageFormat(ref Image img, PixelFormat format)
		{
			fixed (Image* p = &img)
			{
				Raylib.ImageFormat(p, format);
			}
		}
		public static unsafe void ImageAlphaClear(ref Image img, Color c, float threshold)
		{
			fixed (Image* p = &img)
			{
				Raylib.ImageAlphaClear(p, c, threshold);
			}
		}
		public static unsafe void ImageDrawPixel(ref Image img, int x, int y, Color color)
		{
			fixed (Image* ptr = &img)
			{
				Raylib.ImageDrawPixel(ptr, x, y, color);
			}
		}


		public static unsafe void ImageDraw(ref Image img, Image src, Rectangle rectsrc, Rectangle rectdest, Color Tint)
		{
			fixed (Image* ptr = &img)
			{
				Raylib.ImageDraw(ptr, src, rectsrc, rectdest, Tint);
			}
		}

		public static unsafe void ImageDrawCS_ApplyAlphaMask(Image* dst, Image src, Rectangle srcRec, Rectangle dstRec, Color tint)
		{
			// Security check to avoid program crash
			if ((dst[0].data == null) || (dst[0].width == 0) || (dst[0].height == 0) ||
					(src.data == null) || (src.width == 0) || (src.height == 0)) return;

			//if (dst[0].mipmaps > 1) TRACELOG(LOG_WARNING, "Image drawing only applied to base mipmap level");
			//if (dst[0].format >= PIXELFORMAT_COMPRESSED_DXT1_RGB) TRACELOG(LOG_WARNING, "Image drawing not supported for compressed formats");
			else
			{
				Image srcMod = new Image() { data = null };       // Source copy (in case it was required)
				Image* srcPtr = &src;       // Pointer to source image
				bool useSrcMod = false;     // Track source copy required

				// Source rectangle out-of-bounds security checks
				if (srcRec.x < 0) { srcRec.width += srcRec.x; srcRec.x = 0; }
				if (srcRec.y < 0) { srcRec.height += srcRec.y; srcRec.y = 0; }
				if ((srcRec.x + srcRec.width) > src.width) srcRec.width = src.width - srcRec.x;
				if ((srcRec.y + srcRec.height) > src.height) srcRec.height = src.height - srcRec.y;

				// Check if source rectangle needs to be resized to destination rectangle
				// In that case, we make a copy of source and we apply all required transform
				if (((int)srcRec.width != (int)dstRec.width) || ((int)srcRec.height != (int)dstRec.height))
				{

					srcMod = ImageFromImage(src, srcRec);   // Create image from another image
					ImageResize(&srcMod, (int)dstRec.width, (int)dstRec.height);   // Resize to destination rectangle
																																				 //srcRec = (Rectangle){ 0, 0, (float)srcMod.width, (float)srcMod.height };
					srcRec = new Rectangle(0, 0, (float)srcMod.width, (float)srcMod.height);

					srcPtr = &srcMod;
					useSrcMod = true;
				}

				// Destination rectangle out-of-bounds security checks
				if (dstRec.x < 0)
				{
					srcRec.x = -dstRec.x;
					srcRec.width += dstRec.x;
					dstRec.x = 0;
				}
				else if ((dstRec.x + srcRec.width) > dst[0].width) srcRec.width = dst[0].width - dstRec.x;

				if (dstRec.y < 0)
				{
					srcRec.y = -dstRec.y;
					srcRec.height += dstRec.y;
					dstRec.y = 0;
				}
				else if ((dstRec.y + srcRec.height) > dst[0].height) srcRec.height = dst[0].height - dstRec.y;

				if (dst[0].width < srcRec.width) srcRec.width = (float)dst[0].width;
				if (dst[0].height < srcRec.height) srcRec.height = (float)dst[0].height;

				// This blitting method is quite fast! The process followed is:
				// for every pixel -> [get_src_format/get_dst_format -> blend -> format_to_dst]
				// Some optimization ideas:
				//    [x] Avoid creating source copy if not required (no resize required)
				//    [x] Optimize ImageResize() for pixel format (alternative: ImageResizeNN())
				//    [x] Optimize ColorAlphaBlend() to avoid processing (alpha = 0) and (alpha = 1)
				//    [x] Optimize ColorAlphaBlend() for faster operations (maybe avoiding divs?)
				//    [x] Consider fast path: no alpha blending required cases (src has no alpha)
				//    [x] Consider fast path: same src/dst format with no alpha -> direct line copy
				//    [-] GetPixelColor(): Get Vector4 instead of Color, easier for ColorAlphaBlend()
				//    [ ] Support f32bit channels drawing

				// TODO: Support PIXELFORMAT_UNCOMPRESSED_R32, PIXELFORMAT_UNCOMPRESSED_R32G32B32, PIXELFORMAT_UNCOMPRESSED_R32G32B32A32

				Color colSrc, colDst, blend;
				bool blendRequired = true;

				// Fast path: Avoid blend if source has no alpha to blend
				/*if ((tint.a == 255) && ((srcPtr->format == PixelFormat.PIXELFORMAT_UNCOMPRESSED_GRAYSCALE)
          || (srcPtr->format == PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8) || (srcPtr->format == PixelFormat.PIXELFORMAT_UNCOMPRESSED_R5G6B5))) blendRequired = false;*/

				if ((tint.a == 255) && ((srcPtr->format == (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_GRAYSCALE)
					|| (srcPtr->format == (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8) || (srcPtr->format == (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_R5G6B5))) blendRequired = false;

				int strideDst = GetPixelDataSize(dst[0].width, 1, dst[0].format);
				int bytesPerPixelDst = strideDst / (dst[0].width);

				int strideSrc = GetPixelDataSize(srcPtr->width, 1, srcPtr->format);
				int bytesPerPixelSrc = strideSrc / (srcPtr->width);

				byte* pSrcBase = (byte*)srcPtr->data + ((int)srcRec.y * srcPtr->width + (int)srcRec.x) * bytesPerPixelSrc;
				byte* pDstBase = (byte*)dst[0].data + ((int)dstRec.y * dst[0].width + (int)dstRec.x) * bytesPerPixelDst;

				for (int y = 0; y < (int)srcRec.height; y++)
				{
					byte* pSrc = pSrcBase;
					byte* pDst = pDstBase;

					// Fast path: Avoid moving pixel by pixel if no blend required and same format
					if (!blendRequired && (srcPtr->format == dst[0].format)) memcpy((IntPtr)pDst, (IntPtr)pSrc, (int)(srcRec.width) * bytesPerPixelSrc);
					else
					{
						for (int x = 0; x < (int)srcRec.width; x++)
						{
							colSrc = GetPixelColor(pSrc, srcPtr->format);
							colDst = GetPixelColor(pDst, dst[0].format);

							// Fast path: Avoid blend if source has no alpha to blend
							if (blendRequired) blend = ColorAlphaBlend(colDst, colSrc, tint);
							else blend = colSrc;



							if (colSrc.a == 255)
							{
								SetPixelColor(pDst, new Color(0, 0, 0, 0), dst[0].format);
							}

							pDst += bytesPerPixelDst;
							pSrc += bytesPerPixelSrc;
						}
					}

					pSrcBase += strideSrc;
					pDstBase += strideDst;
				}

				if (useSrcMod) UnloadImage(srcMod);     // Unload source modified image
			}


		}

		public enum ImageDrawCommand { NONE, NO_OVERWRITE }



		public static unsafe void ImageDrawCS3(ref Image dst, Image src, Rectangle srcRec, Rectangle dstRec, Color tint, LevelPlayManager lpm, Lemmix.LevelPack.LevelData.LevelGadget gdg, Image lastFrame, ref bool[] fmask)

		{
			fixed (Image* ptr = &dst)
			{
				ImageDrawCS(ptr, src, srcRec, dstRec, tint, lpm, gdg, lastFrame, ref fmask);
			}
		}
		public static unsafe void ImageWipeAlpha(ref Image dst)
		{
			fixed (Image* ptr = &dst) ImageWipeAlpha(ptr);
		}
		public static unsafe void ImageWipeAlpha(Image* dst)
		{
			ImageDrawRectangle(dst, 0, 0, dst[0].width, dst[0].height, BLANK);
		}
		public static unsafe void ImageDrawCS(Image* dst, Image src, Rectangle srcRec, Rectangle dstRec, Color tint, LevelPlayManager lpm, Lemmix.LevelPack.LevelData.LevelGadget gdg, Image lastFrame, ref bool[] fmask)
		{


			LevelPlayManager.MaskData MaskAt(Rectangle drec, int x, int y)
			{

				int rx = (int)drec.X + x;
				int ry = (int)drec.Y + y;

				int ix = ry * lpm.Width + rx;



				if (ix < 0 || ix > lpm.size) return LevelPlayManager.MaskData.EMPTY;

				return lpm.mask[ix];
			}

			int MaskIx(Rectangle drec, int x, int y)
			{

				int rx = (int)drec.X + x;
				int ry = (int)drec.Y + y;

				int ix = ry * lpm.Width + rx;



				if (ix < 0 || ix > lpm.size) return 0;

				return ix;
			}
			// Security check to avoid program crash
			if ((dst[0].data == null) || (dst[0].width == 0) || (dst[0].height == 0) ||
					(src.data == null) || (src.width == 0) || (src.height == 0)) return;

			else
			{
				Image srcMod = new Image() { data = null };       // Source copy (in case it was required)
				Image* srcPtr = &src;       // Pointer to source image
				bool useSrcMod = false;     // Track source copy required

				// Source rectangle out-of-bounds security checks
				if (srcRec.x < 0) { srcRec.width += srcRec.x; srcRec.x = 0; }
				if (srcRec.y < 0) { srcRec.height += srcRec.y; srcRec.y = 0; }
				if ((srcRec.x + srcRec.width) > src.width) srcRec.width = src.width - srcRec.x;
				if ((srcRec.y + srcRec.height) > src.height) srcRec.height = src.height - srcRec.y;

				// Check if source rectangle needs to be resized to destination rectangle
				// In that case, we make a copy of source and we apply all required transform
				if (((int)srcRec.width != (int)dstRec.width) || ((int)srcRec.height != (int)dstRec.height))
				{
					srcMod = ImageFromImage(src, srcRec);   // Create image from another image
					ImageResize(&srcMod, (int)dstRec.width, (int)dstRec.height);   // Resize to destination rectangle
																																				 //srcRec = (Rectangle){ 0, 0, (float)srcMod.width, (float)srcMod.height };
					srcRec = new Rectangle(0, 0, (float)srcMod.width, (float)srcMod.height);

					srcPtr = &srcMod;
					useSrcMod = true;
				}

				// Destination rectangle out-of-bounds security checks
				if (dstRec.x < 0)
				{
					srcRec.x = -dstRec.x;
					srcRec.width += dstRec.x;
					dstRec.x = 0;
				}
				else if ((dstRec.x + srcRec.width) > dst[0].width) srcRec.width = dst[0].width - dstRec.x;

				if (dstRec.y < 0)
				{
					srcRec.y = -dstRec.y;
					srcRec.height += dstRec.y;
					dstRec.y = 0;
				}
				else if ((dstRec.y + srcRec.height) > dst[0].height) srcRec.height = dst[0].height - dstRec.y;

				if (dst[0].width < srcRec.width) srcRec.width = (float)dst[0].width;
				if (dst[0].height < srcRec.height) srcRec.height = (float)dst[0].height;



				Color colSrc, colDst, colLF, colLvl, blend, blendlvl, blendlvl2, blendover;
				bool blendRequired = true;

				if (srcPtr->width <= 0 || srcPtr->height <= 0) return;
				// Fast path: Avoid blend if source has no alpha to blend
				//if ((tint.a == 255) && ((srcPtr->format == PixelFormat.PIXELFORMAT_UNCOMPRESSED_GRAYSCALE) 
				//  || (srcPtr->format == PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8) || (srcPtr->format == PixelFormat.PIXELFORMAT_UNCOMPRESSED_R5G6B5))) blendRequired = false; 

				if ((tint.a == 255) && ((srcPtr->format == (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_GRAYSCALE)
					|| (srcPtr->format == (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8) || (srcPtr->format == (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_R5G6B5))) blendRequired = false;

				int strideDst = GetPixelDataSize(dst[0].width, 1, dst[0].format);
				int bytesPerPixelDst = strideDst / (dst[0].width);

				int strideSrc = GetPixelDataSize(srcPtr->width, 1, srcPtr->format);
				int bytesPerPixelSrc = strideSrc / (srcPtr->width);

				int strideLF = GetPixelDataSize(lastFrame.width, 1, lastFrame.format);
				int bytesPerPixelLF = strideLF / (lastFrame.width);

				int strideLvl = GetPixelDataSize(lpm.imgLevel.width, 1, lpm.imgLevel.format);
				int bytesPerPixelLvl = strideLvl / (lpm.imgLevel.width);

				byte* pSrcBase = (byte*)srcPtr->data + ((int)srcRec.y * srcPtr->width + (int)srcRec.x) * bytesPerPixelSrc;
				byte* pDstBase = (byte*)dst[0].data + ((int)dstRec.y * dst[0].width + (int)dstRec.x) * bytesPerPixelDst;
				byte* pLFBase = (byte*)lastFrame.data + ((int)dstRec.y * lastFrame.width + (int)dstRec.x) * bytesPerPixelDst;
				byte* pLvlBase = (byte*)lpm.imgLevel.data + ((int)dstRec.y * lpm.imgLevel.width + (int)dstRec.x) * bytesPerPixelLvl;


				byte* pSrcBase2 = pSrcBase;
				byte* pDstBase2 = pDstBase;
				for (int y = 0; y < (int)srcRec.height; y++)

				{
					byte* pSrc = pSrcBase2;
					byte* pDst = pDstBase2;
					for (int x = 0; x < (int)srcRec.width; x++)
					{
						var mat = MaskAt(dstRec, x, y);

						colDst = GetPixelColor(pDst, dst[0].format);

						var blend2 = ColorAlphaBlend(colDst, BLANK, tint);
						//if (!mat.HasFlag(LevelPlayManager.MaskData.NO_OVERWRITE))


						pDst += bytesPerPixelDst;
						pSrc += bytesPerPixelSrc;
					}

					pSrcBase2 += strideSrc;
					pDstBase2 += strideDst;

				}

				for (int y = 0; y < (int)srcRec.height; y++)
				{
					byte* pSrc = pSrcBase;
					byte* pDst = pDstBase;
					byte* pLvl = pLvlBase;
					byte* pLF = pLFBase;

					// Fast path: Avoid moving pixel by pixel if no blend required and same format
					//if (!blendRequired && (srcPtr->format == dst[0].format)) memcpy((IntPtr)pDst, (IntPtr)pSrc, (int)(srcRec.width) * bytesPerPixelSrc);
					if (1 == 2) { }
					else
					{
						for (int x = 0; x < (int)srcRec.width; x++)
						{
							colSrc = GetPixelColor(pSrc, srcPtr->format);
							colDst = GetPixelColor(pDst, dst[0].format);
							colLF = GetPixelColor(pLF, lastFrame.format);
							colLvl = GetPixelColor(pLvl, lpm.imgLevel.format);

							// Fast path: Avoid blend if source has no alpha to blend
							blend = ColorAlphaBlend(colDst, colSrc, tint);
							blendover = ColorAlphaBlend(colDst, colSrc, PINK);
							blendlvl = ColorAlphaBlend(colDst, colLvl, RED);
							blendlvl2 = ColorAlphaBlend(colDst, colLvl, GREEN);



							//	blend.a = 100;
							var mat = MaskAt(dstRec, x, y);
							//SetPixelColor(pDst, blend, dst[0].format);
							bool gNoOverwrite = gdg.Flags.HasFlag(Lemmix.LevelPack.LevelData.LevelGadget.FlagsGadget.NO_OVERWRITE); // lpm.gadgHandler.noow[MaskIx(dstRec, x, y)];
							bool lNoOverwrite = mat.HasFlag(LevelPlayManager.MaskData.NO_OVERWRITE);
							bool lTerr = mat.HasFlag(LevelPlayManager.MaskData.TERRAIN);
							bool isAlpha = colSrc.a == 0;
							bool dAlpha = colDst.a == 0;
							bool lfalpha = colLF.a == 0;
							bool lAlpha = colLvl.a == 0;

							int mix = MaskIx(dstRec, x, y);
							lpm.gadgHandler.smask[mix] += gNoOverwrite ? 1: 0;

							int sm = lpm.gadgHandler.smask[mix];
							if (!lTerr)
							{
								if (!isAlpha)
									fmask[mix] = true;

						 
							}
							else if (lTerr)
							{

								if (!gNoOverwrite) // don't remove me, I work
								{
									if (!isAlpha)
									{
										fmask[mix] = true;
									}
								}
								else if (gNoOverwrite)
								{
									if (!isAlpha)
									{
										if (sm > 2)
										{ 
										fmask[mix] = true;

										}
									}
								}
					 

			 

							}
						
							if (!fmask[mix])
							{

								SetPixelColor(pDst, BLANK, dst[0].format);
							}
							else
							{
								SetPixelColor(pDst, blend, dst[0].format);

							}

							if (sm > 0)
							{
								//SetPixelColor(pDst, ColorAlphaBlend(new Color(0, 0, sm * 100, 10),colDst,WHITE), dst[0].format);
								
							}

							if (isAlpha)
							{


							}
					 
							pDst += bytesPerPixelDst;
							pLvl += bytesPerPixelLvl;
							pSrc += bytesPerPixelSrc;
							pLF += bytesPerPixelLF;
						}
					}

					pSrcBase += strideSrc;
					pDstBase += strideDst;
					pLvlBase += strideLvl;
					pLFBase += strideLF;
				}

				if (useSrcMod) UnloadImage(srcMod);     // Unload source modified image
			}


		}
		public static unsafe void ImageDrawCS(ref Image dst, Image src, Rectangle srcRec, Rectangle dstRec, Color tint)

		{
			fixed (Image* ptr = &dst)
			{
				ImageDrawCS(ptr, src, srcRec, dstRec, tint);
			}
		}
		public static unsafe void ImageDrawCS(Image* dst, Image src, Rectangle srcRec, Rectangle dstRec, Color tint)
		{
			// Security check to avoid program crash
			if ((dst[0].data == null) || (dst[0].width == 0) || (dst[0].height == 0) ||
					(src.data == null) || (src.width == 0) || (src.height == 0)) return;

			else
			{
				Image srcMod = new Image() { data = null };       // Source copy (in case it was required)
				Image* srcPtr = &src;       // Pointer to source image
				bool useSrcMod = false;     // Track source copy required

				// Source rectangle out-of-bounds security checks
				if (srcRec.x < 0) { srcRec.width += srcRec.x; srcRec.x = 0; }
				if (srcRec.y < 0) { srcRec.height += srcRec.y; srcRec.y = 0; }
				if ((srcRec.x + srcRec.width) > src.width) srcRec.width = src.width - srcRec.x;
				if ((srcRec.y + srcRec.height) > src.height) srcRec.height = src.height - srcRec.y;

				// Check if source rectangle needs to be resized to destination rectangle
				// In that case, we make a copy of source and we apply all required transform
				if (((int)srcRec.width != (int)dstRec.width) || ((int)srcRec.height != (int)dstRec.height))
				{
					srcMod = ImageFromImage(src, srcRec);   // Create image from another image
					ImageResize(&srcMod, (int)dstRec.width, (int)dstRec.height);   // Resize to destination rectangle
																																				 //srcRec = (Rectangle){ 0, 0, (float)srcMod.width, (float)srcMod.height };
					srcRec = new Rectangle(0, 0, (float)srcMod.width, (float)srcMod.height);

					srcPtr = &srcMod;
					useSrcMod = true;
				}

				// Destination rectangle out-of-bounds security checks
				if (dstRec.x < 0)
				{
					srcRec.x = -dstRec.x;
					srcRec.width += dstRec.x;
					dstRec.x = 0;
				}
				else if ((dstRec.x + srcRec.width) > dst[0].width) srcRec.width = dst[0].width - dstRec.x;

				if (dstRec.y < 0)
				{
					srcRec.y = -dstRec.y;
					srcRec.height += dstRec.y;
					dstRec.y = 0;
				}
				else if ((dstRec.y + srcRec.height) > dst[0].height) srcRec.height = dst[0].height - dstRec.y;

				if (dst[0].width < srcRec.width) srcRec.width = (float)dst[0].width;
				if (dst[0].height < srcRec.height) srcRec.height = (float)dst[0].height;



				Color colSrc, colDst, blend;
				bool blendRequired = true;

				// Fast path: Avoid blend if source has no alpha to blend
				//if ((tint.a == 255) && ((srcPtr->format == PixelFormat.PIXELFORMAT_UNCOMPRESSED_GRAYSCALE) 
				//  || (srcPtr->format == PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8) || (srcPtr->format == PixelFormat.PIXELFORMAT_UNCOMPRESSED_R5G6B5))) blendRequired = false; 

				if ((tint.a == 255) && ((srcPtr->format == (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_GRAYSCALE)
					|| (srcPtr->format == (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8) || (srcPtr->format == (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_R5G6B5))) blendRequired = false;

				int strideDst = GetPixelDataSize(dst[0].width, 1, dst[0].format);
				int bytesPerPixelDst = strideDst / (dst[0].width);

				int strideSrc = GetPixelDataSize(srcPtr->width, 1, srcPtr->format);
				int bytesPerPixelSrc = strideSrc / (srcPtr->width);

				byte* pSrcBase = (byte*)srcPtr->data + ((int)srcRec.y * srcPtr->width + (int)srcRec.x) * bytesPerPixelSrc;
				byte* pDstBase = (byte*)dst[0].data + ((int)dstRec.y * dst[0].width + (int)dstRec.x) * bytesPerPixelDst;

				for (int y = 0; y < (int)srcRec.height; y++)
				{
					byte* pSrc = pSrcBase;
					byte* pDst = pDstBase;

					// Fast path: Avoid moving pixel by pixel if no blend required and same format
					if (!blendRequired && (srcPtr->format == dst[0].format)) memcpy((IntPtr)pDst, (IntPtr)pSrc, (int)(srcRec.width) * bytesPerPixelSrc);
					else
					{
						for (int x = 0; x < (int)srcRec.width; x++)
						{
							colSrc = GetPixelColor(pSrc, srcPtr->format);
							colDst = GetPixelColor(pDst, dst[0].format);

							// Fast path: Avoid blend if source has no alpha to blend
							if (blendRequired) blend = ColorAlphaBlend(colDst, colSrc, tint);
							else blend = colSrc;


							SetPixelColor(pDst, blend, dst[0].format);


							pDst += bytesPerPixelDst;
							pSrc += bytesPerPixelSrc;
						}
					}

					pSrcBase += strideSrc;
					pDstBase += strideDst;
				}

				if (useSrcMod) UnloadImage(srcMod);     // Unload source modified image
			}


		}




		public static unsafe Image ImageCreateInverseAlpha(Image img)
		{
			Color tint = RED;
			Color colSrc, colDst, blend;
			bool blendRequired = true;

			Image* srcPtr = &img;
			Image dst = GenImageColor(srcPtr->width, srcPtr->height, tint);

			Rectangle srcRec = new Rectangle(0, 0, srcPtr->width, srcPtr->height);
			Rectangle dstRec = new Rectangle(0, 0, dst.width, dst.height);

			int strideDst = GetPixelDataSize(dst.width, 1, dst.format);
			int bytesPerPixelDst = strideDst / (dst.width);

			int strideSrc = GetPixelDataSize(srcPtr->width, 1, srcPtr->format);
			int bytesPerPixelSrc = strideSrc / (srcPtr->width);

			byte* pSrcBase = (byte*)srcPtr->data + ((int)srcRec.y * srcPtr->width + (int)srcRec.x) * bytesPerPixelSrc;
			byte* pDstBase = (byte*)dst.data + ((int)dstRec.y * dst.width + (int)dstRec.x) * bytesPerPixelDst;


			for (int y = 0; y < (int)srcRec.height; y++)
			{
				byte* pSrc = pSrcBase;
				byte* pDst = pDstBase;

				for (int x = 0; x < (int)srcRec.width; x++)
				{
					colSrc = GetPixelColor(pSrc, srcPtr->format);
					colDst = GetPixelColor(pDst, dst.format);


					if (blendRequired) blend = ColorAlphaBlend(colDst, colSrc, tint);
					else blend = colSrc;

					if (colSrc.a != 0)
					{
						SetPixelColor(pDst, tint, dst.format);

					}
					else
					{
						SetPixelColor(pDst, BLANK, dst.format);
					}
					pDst += bytesPerPixelDst;
					pSrc += bytesPerPixelSrc;
				}

				pSrcBase += strideSrc;
				pDstBase += strideDst;
			}


			return dst;
		}



		public static void DrawRectangle(Rectangle r, Color c) => Raylib.DrawRectangle((int)r.X, (int)r.Y, (int)r.width, (int)r.height, c);
	}
}
