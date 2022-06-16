#include <stdio.h>
#include <SDL.h>
#include "SDL_syswm.h"
#include "SDL_image.h"
#include "SDL_ttf.h"
#define ENABLEDBG

#define dlx _declspec(dllexport)
#define csString const char * str

//extern SDL_Surface* gScreenSurface = NULL;
//extern SDL_Renderer* gRenderer = NULL;
//extern SDL_Window* gWindow = NULL;
extern SDL_mutex* tMutex = NULL;
//P:[out SDL2Wrappist.Defs.Event.SDL_Event outEvent]
/*	dlx int SDLW_PollEvent(SDL_Event* outEvent)
	{

		return SDL_PollEvent(*&outEvent);
	}*/

SDL_Color csharpByteArrToColor(char* colors)
{
	SDL_Color col = { *&colors[0] ,*&colors[1],*&colors[2],*&colors[3] };
	return col;
}

extern "C"
{
	//calls sdl init with SDL_INIT_EVERYTHING
	dlx int SDLW_Init()
	{
		return SDL_Init(SDL_INIT_EVERYTHING);
	}

	dlx void SDLW_SetHint(const char* strHint, const char* strValue)
	{
		SDL_SetHint(strHint, strValue);
	}



	//Create sdl window, returns a pointer to an SDL_Window struct
	dlx int* SDLW_CreateWindow(int x, int y, int w, int h, int flags)
	{
		return (int*)SDL_CreateWindow("", x, y, w, h, flags);
	}
	//calls sdl_setWindowSize, supply a sdl_window ptr.
	dlx void SDLW_SetWindowSize(SDL_Window* window, int w, int h)
	{

		SDL_SetWindowSize(window, w, h);
	}
	//set the window size
	dlx void SDLW_GetWindowSize(SDL_Window* window, int* w, int* h)
	{
		SDL_GetWindowSize(window, w, h);
	}

	//dispose window
	dlx void SDLW_DestroyWindow(SDL_Window* window)
	{
		SDL_DestroyWindow(window);
	}


	//Get the window handle
	dlx int* SDLW_getWindowHWND(int* _window)
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

	//create a renderer
	dlx int* SDLW_CreateRenderer(SDL_Window* window, int ix, Uint32 flags)
	{
		return (int*)SDL_CreateRenderer(window, ix, flags);
	}

	dlx void SDLW_DestroyRenderer(SDL_Renderer* renderer)
	{
		SDL_DestroyRenderer(renderer);
	}


	//SDL_RenderSetLogicalSize
	dlx int SDLW_RenderSetLogicalSize(int* renderer, int w, int h)
	{
		return SDL_RenderSetLogicalSize((SDL_Renderer*)renderer, w, h);
	}

	//SDL_RenderGetLogicalSize
	dlx void SDLW_RenderGetLogicalSize(int* renderer, int* w, int* h)
	{
		SDL_RenderGetLogicalSize((SDL_Renderer*)renderer, w, h);
	}


	//SDL_SetRenderDrawColor
	dlx int SDLW_SetRenderDrawColor(int* renderer, unsigned  char* data)
	{

		char r = *&data[0];
		char g = *&data[1];
		char b = *&data[2];
		char a = *&data[3];

		//SDL_GetRenderDrawColor((SDL_Renderer* )renderer, a[0],a[1],a[2],a[3]);
		return 	SDL_SetRenderDrawColor((SDL_Renderer*)renderer, r, g, b, a);
	}

	dlx int SDLW_RenderClear(SDL_Renderer* renderer)
	{
		return SDL_RenderClear(renderer);
	}

	dlx void SDLW_RenderPresent(SDL_Renderer* renderer)
	{
		SDL_RenderPresent(renderer);
	}
	dlx void SDLW_Delay(Uint32 ms)
	{
		SDL_Delay(ms);
	}



	//EX:[SDL2.DLL,SDL_PollEvent,public static extern int SDLW_PollEvent(out SDL2Wrappist.SEvent.SDL_Event ev)]



	//surface and texture stuff

	dlx int* SDLW_GetWindowSurface(SDL_Window* win)
	{
		return (int*)SDL_GetWindowSurface(win);
	}



	dlx void SDLW_FreeSurface(SDL_Surface* surf)
	{
		SDL_FreeSurface(surf);

	}


	dlx  int* SDLW_CreateRGBSurface(int width, int height, int depth, Uint32 rmask, Uint32 gmask, Uint32 bmask, Uint32 amask)
	{
		return (int*)SDL_CreateRGBSurface(0, width, height, depth, rmask, gmask, bmask, amask);
	}




	dlx int* SDLW_ImageLoadToSurface(const char* strFile)
	{
		auto x = IMG_Load(strFile);
		auto m = SDL_GetError();


		return (int*)x;
	}


	void mutexCheck()
	{
		if (tMutex == NULL)
		{
			tMutex = SDL_CreateMutex();
		}
	}

	dlx int* SDLW_ImageLoadToTexture(SDL_Renderer* r, const char* strFile)
	{

		SDL_LockMutex(tMutex);

		SDL_ClearError();
		auto x = IMG_Load(strFile);
		auto m = SDL_GetError();
		auto t = SDL_CreateTextureFromSurface(r, x);
		int elen = strlen(m);
		SDL_RendererInfo inf;
		if (elen > 0)
		{
			auto m2 = SDL_GetError();
			int ok1 = SDL_GetRendererInfo(r, &inf);
			int a, w, h;
			SDL_QueryTexture(t, NULL, &a, &w, &h);
			int asdf = 44;

		}
		SDL_FreeSurface(x);
		SDL_UnlockMutex(tMutex);
		return (int*)t;
	}


	dlx int* SDLW_CreateTexture(SDL_Renderer* r, Uint32 format, int access, int w, int h)
	{
		return (int*)SDL_CreateTexture(r, format, access, w, h);

	}

	dlx int* SDLW_CreateTextureFromSurface(SDL_Surface* s, SDL_Renderer* r)
	{
		return (int*)SDL_CreateTextureFromSurface(r, s);
	}

	dlx void SDLW_DestroyTexture(SDL_Texture* t)
	{

		SDL_DestroyTexture(t);
	}

	dlx int SDLW_GetTextureDims(SDL_Texture* t, int* r)
	{
		int w = 0;
		int h = 0;

		int o = SDL_QueryTexture(t, NULL, NULL, &w, &h);

		//int* r = (int*)malloc(sizeof(int)* 3);

		r[0] = w;
		r[1] = h;
		return o;
	}

	dlx void SDLW_FillRectangle(SDL_Renderer* gRenderer, int x, int y, int w, int h, char* colors)
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

	dlx void SDLW_DrawRectangle(SDL_Renderer* gRenderer, int x, int y, int w, int h, char* colors)
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

	dlx void SDLW_DrawLine(SDL_Renderer* r, int x1, int y1, int x2, int y2, char* colordata)
	{
		auto textColor = csharpByteArrToColor(colordata);
		unsigned char oldr, oldg, oldb, olda;
		SDL_GetRenderDrawColor(r, &oldr, &oldg, &oldb, &olda);

		SDL_SetRenderDrawColor(r, textColor.r, textColor.g, textColor.b, textColor.a);
		SDL_RenderDrawLine(r, x1, y1, x2, y2);

		SDL_SetRenderDrawColor(r, oldr, oldg, oldb, olda);
	}

	dlx int SDLW_RenderCopy(SDL_Renderer* renderer, SDL_Texture* text, const SDL_Rect* src, const  SDL_Rect* dest)
	{
		return 	SDL_RenderCopy(renderer, text, src, dest);
	}
	dlx int SDLW_RenderCopy2(SDL_Renderer* renderer, SDL_Texture* text, const SDL_Rect* src_byref, const  SDL_Rect* dest_byref)
	{
		return 	SDL_RenderCopy(renderer, text, src_byref, dest_byref);
	}
dlx int SDLW_RenderCopy2F(SDL_Renderer* renderer, SDL_Texture* text, const SDL_Rect* src_byref, const  SDL_FRect* dest_byref)
	{
		return 	SDL_RenderCopyF(renderer, text, src_byref, dest_byref);
	}
	dlx int SDLW_SetRelativeMouseMode(int enabled)
	{
#ifdef ENABLEDBG
		switch (enabled)
		{

		case SDL_TRUE:
			OutputDebugString(L"Set Relative mouse to true\n");
			break;
		case SDL_FALSE:
			OutputDebugString(L"Set Relative mouse to false\n");

			break;
		default:
			OutputDebugString(L"Set Relative mouse to something else\n");

			break;
		}
#endif // !ENABLEDBG


		return SDL_SetRelativeMouseMode((SDL_bool)enabled);


	}
	dlx int SDLW_GetRelativeMouseMode()
	{
		return (int)SDL_GetRelativeMouseMode();
	}

	//EX:[SDL2.DLL,SDL_GetRelativeMouseState,public static extern UInt32 SDLW_GetRelativeMouseState(ref int x, ref int y)]
	//EX:[SDL2.DLL,SDL_ShowCursor,public static extern int SDLW_ShowCursor(int toggle)]

	dlx void	SDLW_WarpMouseInWindow(SDL_Window* w, int x, int y)
	{
		return SDL_WarpMouseInWindow(w, x, y);
	}


	//Font Stuff



	dlx int SDLW_TTF_Init()
	{
		return TTF_Init();
	}

	//returns a pointer to a ttf font object.
	dlx int* SDLW_TTF_OpenFont(const char* strName, int size)
	{

		return (int*)TTF_OpenFont(strName, size);
	}

	//Returns an SDL Surface
	dlx int* SDLW_TTF_RenderText_Solid(TTF_Font* font, const char* strText, char* colordata)
	{
		auto textColor = csharpByteArrToColor(colordata);

		return (int*)TTF_RenderText_Solid(font, strText, textColor);

	}


	dlx void SDLW_TTF_CloseFont(TTF_Font* fontPointer)
	{
		TTF_CloseFont(fontPointer);
	}




	// Return a texture with the rendered text
	dlx int* SDLW_TextureFromRenderedText(SDL_Renderer* r, TTF_Font* font, const char* strText, char* colorData)
	{

		auto sdlColor = csharpByteArrToColor(colorData);
		SDL_Surface* textSurface = TTF_RenderText_Solid(font, strText, sdlColor);

		if (textSurface != NULL)
		{
			return (int*)SDL_CreateTextureFromSurface(r, textSurface);
		}
		else return nullptr;

	}

	dlx int* JLSCreateSurfaceTextAtlas(int* data, int width, int height)
	{
		auto surf = SDL_CreateRGBSurface(0, width, height, 32, 0, 0, 0, 0);
		SDL_LockSurface(surf);

		int size = width * height * 4;




		SDL_memcpy(surf->pixels, data, size);


		SDL_UnlockSurface(surf);

		return (int*)surf;
	}


	//EX:[SDL2.DLL,SDL_GetError,public static extern IntPtr SDLW_GetError()]
	//EX:[SDL2.DLL,SDL_ClearError,public static extern void SDLW_ClearError()]

	dlx void SDLW_AlphaBlendTextures(SDL_Texture* tex, int onoff)
	{
		if (onoff)
		{
			SDL_SetTextureBlendMode(tex, SDL_BLENDMODE_BLEND);
		}
		else {
			SDL_SetTextureBlendMode(tex, SDL_BLENDMODE_NONE);

		}
	}

	void scrap()
	{
	 
	 
		/*auto A = SDLW_TextureFromRenderedText(NULL, NULL, "ASDF", NULL);


		//auto res = TTF_Init();
		//	auto fontpointer = TTF_OpenFont("FNAME",12);

		char* colors = (char*)malloc(20);


		auto textColor = csharpByteArrToColor(colors);



		TTF_Quit();
		SDL_Quit();*/


	}

}//EXTCEND