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
	public class WrappistPTRHandler : SafeHandleZeroOrMinusOneIsInvalid
	{

		

		public delegate void wptrEvArgs(IntPtr cptr, IntPtr nptr, string c);

		public static event wptrEvArgs onPointerCreated;
		public static event wptrEvArgs onPointerReleased;
		public static event wptrEvArgs onPointerChanged;


		private IntPtr _myPointer;
		public IntPtr myPointer { get {
				return _myPointer;
			}
			set {
				if (_myPointer != value)
				{
					onPointerChanged?.Invoke(_myPointer, value, "Pointer Changed");
				}

				_myPointer = value;
			}
		}
		public WrappistPTRHandler(bool ownsHandle) : base(ownsHandle)
		{
		}

		public virtual void wSetHandle(IntPtr handle)
		{




			

			myPointer = handle;
			this.SetHandle(myPointer);
#if DBG_POINTERS
			onPointerCreated?.Invoke(IntPtr.Zero, handle, this.GetType().Name);
#endif
		}


		public static explicit operator IntPtr(WrappistPTRHandler ptr)
		{
			if (ptr == null) return IntPtr.Zero;
			IntPtr r = ptr.DangerousGetHandle();
			return r;
		}

		public static explicit operator WrappistPTRHandler(IntPtr ptr)
		{
			var r = new WrappistPTRHandler(true);
			//r.SetHandle(ptr);
			r.wSetHandle(ptr);
			return r;
		}

		protected override bool ReleaseHandle()
		{
#if DBG_POINTERS
			onPointerReleased?.Invoke(this.DangerousGetHandle(), IntPtr.Zero, this.GetType().Name);
#endif
			return true;

		}
	}
}
