using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static CLemmix4.SDL2Wrappist.Imports;
using static CLemmix4.SDL2Wrappist.Defs;
using System.Threading;
using CLemmix4.SDL2Wrappist.Colors;
using System.Diagnostics;
namespace CLemmix4.SDL2Wrappist
{
	public class Surface : WrappistPTRHandler
	{


		public Context c { get; }
		public Window w { get; }

		public Surface() : base(true) { }

		private Surface(IntPtr ptr) : this() { wSetHandle(ptr); }

		private Surface(Context c) : base(ownsHandle:true)
		{
			this.c = c;
		}


		public Surface(Context c, Window w, IntPtr _ptr) : this(c, w)
		{
			wSetHandle(_ptr);
		}

		public Surface(Context c, Window w) : this(c)
		{
			this.w = w;

			var ptr = SDLW_GetWindowSurface((IntPtr)w);

			wSetHandle(ptr);

		}

		protected override bool ReleaseHandle()
		{
			SDLW_FreeSurface((IntPtr)this);
			return base.ReleaseHandle();
		}





		public static Surface FromImage(string filename)
		{

			var p = SDLW_ImageLoadToSurface(filename);
			if (p != IntPtr.Zero)
			{
				return new Surface(p);
			}

			return null;
		}


	}

}
