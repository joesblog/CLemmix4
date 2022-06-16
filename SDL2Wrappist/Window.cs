using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static CLemmix4.SDL2Wrappist.Imports;
using static CLemmix4.SDL2Wrappist.Defs;
using CLemmix4;

namespace CLemmix4.SDL2Wrappist
{
	public class Window : WrappistPTRHandler
	{
		public Context Context { get; }

		public  Surface Surface { get; private set; }

	 

		public Window(Context c, int x, int y, int w, int h, Defs.SDL_WindowFlags flags) : base(ownsHandle: true)
		{
			this.Context = c;
			var ptr = Imports.SDLW_CreateWindow(x, y, w, h, (int)flags);
			this.wSetHandle(ptr);

		}

		public void setupScreenSurface()
		{
			if (Surface == null)
			{
				Surface = new Surface(Context, this);
			}
		}

		public void moveWinWindow(int x, int y, int w, int h, bool repaint)
		{
			Sys.MoveWindow((IntPtr)this.getSDLWindowWinHWND(), x, y, w, h, repaint);
		}

		public void moveWinWindow(int x, int y, Size s, bool repaint)
		{
			moveWinWindow(x, y, s.Width, s.Height, repaint);
		}

		public void setSDLWindowSize(Size s)
		{
			setSDLWindowSize(s.Width, s.Height);
		}
		public void setSDLWindowSize(int w, int h)
		{
			SDLW_SetWindowSize((IntPtr)this, w, h);
		}

		private Size getSDLWindowSize(IntPtr gWindow)
		{
			IntPtr w = Marshal.AllocHGlobal(sizeof(int));
			IntPtr h = Marshal.AllocHGlobal(sizeof(int));
			SDLW_GetWindowSize((IntPtr)this, w, h);

			Size r = new Size(w.MarshalInt32(), h.MarshalInt32());
			w.free(); h.free();
			return r;

		}

		protected override bool ReleaseHandle()
		{
			Imports.SDLW_DestroyWindow((IntPtr)this);
			return true;
		}

		public IntPtr getSDLWindowWinHWND()
		{
			return Imports.SDLW_getWindowHWND((IntPtr)this);
		}
	}
}
