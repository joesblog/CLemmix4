#include <stdio.h>
#include <SDL.h>
#include "SDL_syswm.h"
#include "SDL_image.h"
#ifndef dlx
#define dlx _declspec(dllexport)
#endif
 
 extern "C"{
//SDL init
dlx int sdl_SDL_Init()
{
	return SDL_Init(SDL_INIT_EVERYTHING);
}


dlx int sdl_SDL_SetHint(const char* name, const char* value)
{
	return SDL_SetHint(name, value);
}

//SDL Create Window
dlx int* sdl_SDL_CreateWindow(int x, int y, int w, int h, int flags)
{
	return (int*)SDL_CreateWindow("", x, y, w, h, flags);
}


//SDL_Setwindowsize
dlx void sdl_SDL_SetWindowSize(SDL_Window* window, int w, int h)
{

	SDL_SetWindowSize(window, w, h);
}

dlx void sdl_SDL_GetWindowSize(SDL_Window* window, int* w, int* h)
{
	SDL_GetWindowSize(window, w, h);
}
//SDL Get WindowHWND
dlx int* sdl_getWindowHWND(int* _window)
{
	if (_window != nullptr)
	{
		SDL_SysWMinfo wmInfo;
		SDL_VERSION(&wmInfo.version);
		SDL_GetWindowWMInfo((SDL_Window*)_window, &wmInfo);
		HWND hwnd = wmInfo.info.win.window;

		return (int*)hwnd;
	}
	return 0;

}

//SDL Create Rendere
dlx int* sdl_SDL_CreateRenderer(SDL_Window* window, int ix, int flags)
{
	return (int*)SDL_CreateRenderer(window, ix, flags);
}
//SDL_RenderClear
dlx int sdl_SDL_RenderClear(int* renderer)
{
	return SDL_RenderClear((SDL_Renderer*)renderer);
}

//SDL_RenderPresent
dlx void sdl_SDL_RenderPresent(int* renderer)
{
	SDL_RenderPresent((SDL_Renderer*)renderer);
}
//SDL_Delay
dlx void sdl_SDL_Delay(Uint32 DelayBy)
{
	SDL_Delay(DelayBy);
}
//SDL_RenderSetLogicalSize
dlx int sdl_SDL_RenderSetLogicalSize(int* renderer, int w, int h)
{
	return SDL_RenderSetLogicalSize((SDL_Renderer*)renderer, w, h);
}
//SDL_RenderGetLogicalSize
dlx void sdl_SDL_RenderGetLogicalSize(int* renderer, int* w, int* h)
{
	SDL_RenderGetLogicalSize((SDL_Renderer*)renderer, w, h);


}

dlx void sdl_SDL_RenderSetScale(int* renderer, float x, float y)
{
	SDL_RenderSetScale((SDL_Renderer* )renderer, x, y);
}

//SDL_SetRenderDrawColor
dlx void sdl_SDL_SetRenderDrawColor(int* renderer, unsigned  char* data)
{

	char r = *&data[0];
	char g = *&data[1];
	char b = *&data[2];
	char a = *&data[3];

	//SDL_GetRenderDrawColor((SDL_Renderer* )renderer, a[0],a[1],a[2],a[3]);
	SDL_SetRenderDrawColor((SDL_Renderer*)renderer, r, g, b, a);
}




//sdl destroy renderer

dlx void sdl_SDL_DestroyRenderer(int* renderer)
{
	SDL_DestroyRenderer((SDL_Renderer*)renderer);

}



dlx void sdl_FillRectangle(SDL_Renderer* gRenderer, int x, int y, int w, int h, char* colors)
{
	unsigned char oldr, oldg, oldb, olda;
	char r = *&colors[0];
	char g = *&colors[1];
	char b = *&colors[2];
	char a = *&colors[3];

	SDL_GetRenderDrawColor(gRenderer, &oldr, &oldg, &oldb, &olda);
	SDL_SetRenderDrawColor(gRenderer, r, g, b, a);
	SDL_Rect _fillRect = { x,y,w,h };
	SDL_RenderFillRect(gRenderer, &_fillRect);

	SDL_SetRenderDrawColor(gRenderer, oldr, oldg, oldb, olda);

}

dlx void sdl_DrawRectangle(SDL_Renderer* gRenderer, int x, int y, int w, int h, char* colors)
{
	unsigned char oldr, oldg, oldb, olda;
	char r = *&colors[0];
	char g = *&colors[1];
	char b = *&colors[2];
	char a = *&colors[3];

	SDL_GetRenderDrawColor(gRenderer, &oldr, &oldg, &oldb, &olda);
	SDL_SetRenderDrawColor(gRenderer, r, g, b, a);
	SDL_Rect _fillRect = { x,y,w,h };
	SDL_RenderDrawRect(gRenderer, &_fillRect);

	SDL_SetRenderDrawColor(gRenderer, oldr, oldg, oldb, olda);

}

dlx int* SDL_CreateEventReference()
{
	int* ptr = (int*)malloc(sizeof(SDL_Event));
	return ptr;

}



dlx int* sdl_IMG_Load(const char* strFile)
{
	auto x = IMG_Load(strFile);
	auto m = SDL_GetError();
	return (int*)x;
}

dlx void sdl_FreeSurface(int* surface)
{
	SDL_FreeSurface((SDL_Surface*)surface);

}

dlx int* sdl_SDL_CreateTextureFromSurface(SDL_Renderer* renderer, SDL_Surface* surface)
{

	return (int*)SDL_CreateTextureFromSurface((SDL_Renderer*)renderer, (SDL_Surface*)surface);
}

dlx void sdl_SDL_DestroyTexture(SDL_Texture* texture)
{
	SDL_DestroyTexture(texture);
}

dlx int sdl_SDL_BlitSurface(SDL_Surface* src, const SDL_Rect* srcrect, SDL_Surface* dst, SDL_Rect* dstrect)
{
	return SDL_BlitSurface(src, srcrect, dst, dstrect);
}

dlx int sdl_SDL_SetColorKey(SDL_Surface* surface, int flag, Uint32 key)
{
	return SDL_SetColorKey(surface, flag, key);

}
//    public static extern UInt32 sdl_SDL_MAPRGB(IntPtr format, byte[] colors);

dlx Uint32 sdl_SDL_MAPRGB(const SDL_PixelFormat* format, char* colors)
{
	char r = *&colors[0];
	char g = *&colors[1];
	char b = *&colors[2];
	//SDL_SetRenderDrawColor(gRenderer, r, g, b, a);
	return SDL_MapRGB(format, r, g, b);

}

dlx int add22(int a, int b, int c)
{
	return a + b + c;
}
}