using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using static CLemmix4.SDL2Wrappist.Imports;
using static CLemmix4.SDL2Wrappist.Defs;
using System.ComponentModel;
using System.Threading;
using CLemmix4.SDL2Wrappist.Colors;
using System.Diagnostics;

namespace CLemmix4.SDL2Wrappist
{
	/// <summary>
	/// Main
	/// </summary>
	public partial class Wrappist : IDisposable
	{
		public Context context { get; private set; }

		public Wrappist(Control parent, ContextOptions opts = null)
		{

			if (opts == null)
			{

				context = new Context(parent, parent.Size);
			}
			else
			{
				context = new Context(parent, opts);
			}

		 
		}

 
		public Wrappist(Control parent, Size size)
		{
			context = new Context(parent, size);

		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}


	}










	public class Sys
	{
		public static bool MoveWindow(IntPtr windowHandle, int x, int y, int width, int height, bool rePaint)
		{
			return Imports.MoveWindow(windowHandle, x, y, width, height, rePaint);
		}

		public static IntPtr SetParentWindow(IntPtr handleChild, IntPtr handleParent)
		{
			return Imports.SetParent(handleChild, handleParent);
		}

		public static IntPtr SetParentWindow(Window child, Control Parent)
		{
			return SetParentWindow((IntPtr)child, Parent.Handle);
		}
	}


}
