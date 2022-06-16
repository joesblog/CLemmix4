 
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static CLemmix4.SDL2Wrappist.Defs;
namespace CLemmix4.SDL2Wrappist
{
	public static partial class Imports
		{
 
 /// <summary>
  /// calls sdl init with SDL_INIT_EVERYTHING
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_Init();

  /// <summary>
  /// DLL Import of SDLW_SetHint
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_SetHint(string strHint,string strValue);

  /// <summary>
  /// Create sdl window, returns a pointer to an SDL_Window struct
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_CreateWindow(int x,int y,int w,int h,int flags);

  /// <summary>
  /// calls sdl_setWindowSize, supply a sdl_window ptr.
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_SetWindowSize(IntPtr window,int w,int h);

  /// <summary>
  /// set the window size
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_GetWindowSize(IntPtr window,IntPtr w,IntPtr h);

  /// <summary>
  /// dispose window
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_DestroyWindow(IntPtr window);

  /// <summary>
  /// Get the window handle
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_getWindowHWND(IntPtr _window);

  /// <summary>
  /// create a renderer
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_CreateRenderer(IntPtr window,int ix,UInt32 flags);

  /// <summary>
  /// DLL Import of SDLW_DestroyRenderer
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_DestroyRenderer(IntPtr renderer);

  /// <summary>
  /// SDL_RenderSetLogicalSize
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_RenderSetLogicalSize(IntPtr renderer,int w,int h);

  /// <summary>
  /// SDL_RenderGetLogicalSize
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_RenderGetLogicalSize(IntPtr renderer,IntPtr w,IntPtr h);

  /// <summary>
  /// SDL_SetRenderDrawColor
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_SetRenderDrawColor(IntPtr renderer,byte[] data);

  /// <summary>
  /// DLL Import of SDLW_RenderClear
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_RenderClear(IntPtr renderer);

  /// <summary>
  /// DLL Import of SDLW_RenderPresent
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_RenderPresent(IntPtr renderer);

  /// <summary>
  /// DLL Import of SDLW_Delay
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_Delay(UInt32 ms);

  /// <summary>
  /// DLL Import of SDL_PollEvent
  /// </summary>
  [DllImport("SDL2.DLL", EntryPoint = "SDL_PollEvent", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_PollEvent(out SDL2Wrappist.SEvent.SDL_Event ev);
/// <summary>
  /// DLL Import of SDLW_GetWindowSurface
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_GetWindowSurface(IntPtr win);

  /// <summary>
  /// DLL Import of SDLW_FreeSurface
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_FreeSurface(IntPtr surf);

  /// <summary>
  /// DLL Import of SDLW_CreateRGBSurface
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_CreateRGBSurface(int width,int height,int depth,UInt32 rmask,UInt32 gmask,UInt32 bmask,UInt32 amask);

  /// <summary>
  /// DLL Import of SDLW_ImageLoadToSurface
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_ImageLoadToSurface(string strFile);

  /// <summary>
  /// DLL Import of SDLW_ImageLoadToTexture
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_ImageLoadToTexture(IntPtr r,string strFile);

  /// <summary>
  /// DLL Import of SDLW_CreateTexture
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_CreateTexture(IntPtr r,UInt32 format,int access,int w,int h);

  /// <summary>
  /// DLL Import of SDLW_CreateTextureFromSurface
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_CreateTextureFromSurface(IntPtr s,IntPtr r);

  /// <summary>
  /// DLL Import of SDLW_DestroyTexture
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_DestroyTexture(IntPtr t);

  /// <summary>
  /// DLL Import of SDLW_GetTextureDims
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_GetTextureDims(IntPtr t,IntPtr r);

  /// <summary>
  /// DLL Import of SDLW_GetTexturePixelFormat
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern UInt32 SDLW_GetTexturePixelFormat(IntPtr t,IntPtr r);

  /// <summary>
  /// DLL Import of SDLW_FillRectangle
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_FillRectangle(IntPtr gRenderer,int x,int y,int w,int h,byte[] colors);

  /// <summary>
  /// DLL Import of SDLW_DrawRectangle
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_DrawRectangle(IntPtr gRenderer,int x,int y,int w,int h,byte[] colors);

  /// <summary>
  /// DLL Import of SDLW_DrawLine
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_DrawLine(IntPtr r,int x1,int y1,int x2,int y2,byte[] colordata);

  /// <summary>
  /// DLL Import of SDLW_RenderCopy
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_RenderCopy(IntPtr renderer,IntPtr text,IntPtr src,IntPtr dest);

  /// <summary>
  /// DLL Import of SDLW_RenderCopy2
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_RenderCopy2(IntPtr renderer,IntPtr text,ref SDL_Rect src_byref,ref SDL_Rect dest_byref);

  /// <summary>
  /// DLL Import of SDLW_RenderCopy2F
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_RenderCopy2F(IntPtr renderer,IntPtr text,ref SDL_Rect src_byref,ref SDL_FRect dest_byref);

  /// <summary>
  /// DLL Import of SDLW_SetRelativeMouseMode
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_SetRelativeMouseMode(int enabled);

  /// <summary>
  /// DLL Import of SDLW_GetRelativeMouseMode
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_GetRelativeMouseMode();

  /// <summary>
  /// DLL Import of SDL_GetRelativeMouseState
  /// </summary>
  [DllImport("SDL2.DLL", EntryPoint = "SDL_GetRelativeMouseState", CallingConvention = CallingConvention.Cdecl)]
  public static extern UInt32 SDLW_GetRelativeMouseState(ref int x, ref int y);
/// <summary>
  /// DLL Import of SDL_ShowCursor
  /// </summary>
  [DllImport("SDL2.DLL", EntryPoint = "SDL_ShowCursor", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_ShowCursor(int toggle);
/// <summary>
  /// DLL Import of SDLW_WarpMouseInWindow
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_WarpMouseInWindow(IntPtr w,int x,int y);

  /// <summary>
  /// DLL Import of SDLW_TTF_Init
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_TTF_Init();

  /// <summary>
  /// returns a pointer to a ttf font object.
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_TTF_OpenFont(string strName,int size);

  /// <summary>
  /// Returns an SDL Surface
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_TTF_RenderText_Solid(IntPtr font,string strText,byte[] colordata);

  /// <summary>
  /// DLL Import of SDLW_TTF_CloseFont
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_TTF_CloseFont(IntPtr fontPointer);

  /// <summary>
  ///  Return a texture with the rendered text
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_TextureFromRenderedText(IntPtr r,IntPtr font,string strText,byte[] colorData);

  /// <summary>
  /// DLL Import of JLSCreateSurfaceTextAtlas
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr JLSCreateSurfaceTextAtlas(IntPtr data,int width,int height);

  /// <summary>
  /// DLL Import of SDLW_SetRenderTarget
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_SetRenderTarget(IntPtr r,IntPtr t);

  /// <summary>
  /// DLL Import of SDLW_ClearRenderTarget
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int SDLW_ClearRenderTarget(IntPtr r);

  /// <summary>
  /// DLL Import of SDL_GetError
  /// </summary>
  [DllImport("SDL2.DLL", EntryPoint = "SDL_GetError", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr SDLW_GetError();
/// <summary>
  /// DLL Import of SDL_ClearError
  /// </summary>
  [DllImport("SDL2.DLL", EntryPoint = "SDL_ClearError", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_ClearError();
/// <summary>
  /// DLL Import of SDLW_AlphaBlendTextures
  /// </summary>
  [DllImport("SDLWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void SDLW_AlphaBlendTextures(IntPtr tex,int onoff);

  	}
}