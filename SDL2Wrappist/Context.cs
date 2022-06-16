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
using System.Windows.Forms;
using CLemmix4;

namespace CLemmix4.SDL2Wrappist
{
	public class ContextOptions
	{


		public bool autoResizeViewPort = true;
		public bool autoStartRenderThread = true;
		public bool useScreenSurface = false;
		public Size? logicalRenderSize;
		public Context.contextEvArgs extraSDLSetupSteps;
		public Context.contextRenderWindowEvArgs renderLogicFunction;
		public SDL4Color renderClearColor;
		public int SCREEN_FPS;
		public bool panClickEnabled = false;
		public bool panClickRelativeEnabled = true;
		public int panOnButton = 2;
		public bool disableSDLCursor = true;

		public static ContextOptions DEFAULT
		{
			get
			{
				return new ContextOptions()
				{
					autoResizeViewPort = true,
					extraSDLSetupSteps = null,
					renderLogicFunction = null,
					logicalRenderSize = null,
					renderClearColor = Color.Green,
					disableSDLCursor = true,
					SCREEN_FPS = 60
				};
			}
		}
	}

	public class Context
	{

		/// <summary>
		/// how long till wait before firing the resize functions
		/// </summary>
		const int MS_TIME_ALLOWED_FOR_RESIZE_MSG = 100;
		/// <summary>
		/// how many nPixels before a drag has happened.
		/// </summary>
		const int MOUSE_DRAG_TOLERENCE = 2;
		#region events
		public delegate void contextEvArgs(Context c, params object[] data);
		public delegate void contextRenderWindowEvArgs(Context c, Renderer r, Window w, params object[] data);
		public delegate void contextSizeEvArgs(Context c, Size newSize, bool letterbox);

		


		public event contextEvArgs onSetupComplete;
		public event contextEvArgs onSetupError;
		public event contextEvArgs onLogOutput;
		public event contextEvArgs onLogOutput2;
		public event contextSizeEvArgs onSizeChanged;
		public event contextEvArgs onPreRenderStartUp;
		#region mouse
		public delegate void contextMouseEvent(int button, int x, int y);
		public delegate void contextMousePanEventArgs(int button, WPoint amount, WPointF percentageMovement);
		public delegate void contextMousePanRelativeEventArgs(int button, WVector2 wv, WVector2F percentage);
		public delegate void contextMouseWheel(int x, int y);
		public bool[] isPanningOn = Enumerable.Repeat(false, 5).ToArray();


		public event contextMouseEvent onMouseHover;
		public event contextMouseEvent onMouseDown;
		public event contextMouseEvent onMouseUp;
		public event contextMousePanEventArgs onMousePan;
		public event contextMousePanRelativeEventArgs onMousePanRelative;
		public event contextMouseWheel onMouseWheel;

		internal void invokeMouseWheel(int x, int y) => onMouseWheel?.Invoke(x, y);

		internal void invokeMouseHover(int btn, int x, int y) {
			
			onMouseHover?.Invoke(btn, x, y);
		}
		internal void invokeMouseDown(int btn, int x, int y) 
		{
			onMouseDown?.Invoke(btn, x, y);
		}
		internal void invokeMouseUp(int btn, int x, int y) 
		{
			onMouseUp?.Invoke(btn, x, y);
		}

		internal void invokeMousePan(int btn, WPoint amnt)
		{

			if (onMousePan != null)
			{
				WPointF pc = new WPointF();

				float fx = (float)amnt.X + 1;
				float fy = (float)amnt.Y + 1;

				float sw = (float)currentRendererSize.Width + 1;
				float sh = (float)currentRendererSize.Height + 1;

				pc.X = (fx / sw) * 100;
				pc.Y = (fy / sh) * 100;

				onMousePan.Invoke(btn, amnt, pc);
			}
		

		}

		internal void invokeMousePan(int button, WVector2 vert)
		{

			if (onMousePanRelative != null)
			{
				WVector2F pc = new WVector2F();

				float fx = (float)vert.X + 1;
				float fy = (float)vert.Y + 1;

				float sw = (float)currentRendererSize.Width + 1;
				float sh = (float)currentRendererSize.Height + 1;

				vert.negate();
				pc.X = (fx / sw) * 100;
				pc.Y = (fy / sh) * 100;
			onMousePanRelative.Invoke(button, vert,pc);

			}
		}

	 
		#endregion
		internal void invokeLogEvent(string from, string msg) => onLogOutput?.Invoke(this, from, msg);
		internal void invokeLogEvent( string msg) => onLogOutput?.Invoke(this,  msg);
		internal void invokeLogEvent2( string msg) => onLogOutput2?.Invoke(this,  msg);
		#endregion
		#region fields
		/// <summary>
		/// where the thing draws
		/// </summary>
		public readonly Control Parent;

		/// <summary>
		/// setups complete
		/// </summary>
		public readonly bool setupComplete = false;


		/// <summary>
		/// Last time a resize was called
		/// </summary>
		DateTime lastChangeSizeReq = DateTime.Now;

		/// <summary>
		/// has the size Changed?
		/// </summary>
		bool sizeChanged = false;
		//public Size currentRendererSize = new Size(0, 0);
		private Size _currentRendererSize = new Size(0, 0);

		public Size currentRendererSize
		{
			get { return _currentRendererSize; }
			set {
				if (value != _currentRendererSize)
				{
					_currentRendererSize = value;

					fOffset.X = (float)(-_currentRendererSize.Width / 2);
					fOffset.Y = (float)(-_currentRendererSize.Height / 2);
				}
				
			}

		}

		private WPointF _fOffset = new WPointF(1f, 1f);

		public WPointF fOffset
		{
			get { return _fOffset; }
			set { _fOffset = value; }
		}



		//Pointers
		public Window mWindow;
		public IntPtr mWindowHandle;
		public Renderer mRenderer;

		public SEvent.SDL_Event ev;


		//Draw Options
		public bool showCursor = true;
		 

		#endregion

		#region properties
		private ContextOptions _opts;

		public ContextOptions Opts
		{
			get
			{
				if (_opts == null) _opts = ContextOptions.DEFAULT.CopyProperties();
				return _opts;
			}
			set
			{
				_opts = value;
			}
		}


		#endregion

		#region constructors
		public Context(Control _parent, Size? _renderSize = null)
		{
			Parent = _parent;
			Opts.logicalRenderSize = _renderSize;
			_SetupRoutines();
		}
		public Context(Control _parent, ContextOptions options)
		{
			Opts = options;

			Parent = _parent;
			_SetupRoutines();
		}

	
		#endregion


		#region handleSize methods

		public void changeSize()
		{
			if (Parent.Width <= 0 || Parent.Height <= 0) return;

			mRenderer.Paused = true;


			mWindow.setSDLWindowSize(Parent.Width, Parent.Height);
			mWindow.moveWinWindow(0, 0, Parent.Width, Parent.Height, true);
			//SDLW_SetWindowSize(mWindow.DangerousGetHandle(), Parent.Width, Parent.Height);
			//Sys.MoveWindow(mWindow.getWindowHWND(), 0, 0, Parent.Width, Parent.Height, true);

			 mRenderer.Reset();


			//mRenderer.SetDrawColor(Opts.renderClearColor);

			SetHint(SDL_HINT.RENDER_SCALE_QUALITY, SDL_HINT.SCALE_QUALITY.LINEAR);
			Size changedTo = new Size();
			if (Opts.logicalRenderSize.HasValue && Opts.autoResizeViewPort)
			{
				mRenderer.SetLogicalSize(Opts.logicalRenderSize.Value.Width, Opts.logicalRenderSize.Value.Height);
				changedTo = Opts.logicalRenderSize.Value;

			}
			else //if (Opts.logicalRenderSize.HasValue && !Opts.autoResizeViewPort)
			{
				mRenderer.SetLogicalSize(Parent.Width, Parent.Height);
				changedTo = Parent.Size;

			}

			currentRendererSize = changedTo;
			onSizeChanged?.Invoke(this, changedTo,Opts.autoResizeViewPort);
			sizeChanged = false;
			mRenderer.Paused = false;
		}

		public void handleSizeChange()
		{
			if (!sizeChanged) return;

			if ((DateTime.Now - lastChangeSizeReq).TotalMilliseconds >= MS_TIME_ALLOWED_FOR_RESIZE_MSG)
			{
				changeSize();
			}
		}


		#endregion
		private void _SetupRoutines()
		{

			ev = new SEvent.SDL_Event();


			Parent.SizeChanged += (s, e) =>
			{
				if (mWindowHandle != IntPtr.Zero)
					sizeChanged = true;
			};

			if (Opts.autoStartRenderThread == true) //auto start render thread.
			{
				onSetupComplete += (c, d) =>
				{
					onPreRenderStartUp?.Invoke(c);
					mRenderer.RendererStart();

				};
				//mRenderer.RendererStart();
			}

			SDLSetup();


			if (Opts.panClickRelativeEnabled)
			{
		 
			}
		}
 

		public virtual bool SDLSetup()
		{

			bool success = true;
			if (Imports.SDLW_Init() >= 0)
			{

				mWindow = new Window(this, 0, 0, Parent.Height, Parent.Width, Defs.SDL_WindowFlags.SDL_WINDOW_SHOWN | Defs.SDL_WindowFlags.SDL_WINDOW_BORDERLESS);
				mWindow.setSDLWindowSize(Parent.Width, Parent.Height);
				currentRendererSize = Parent.Size;

				if (!mWindow.IsInvalid)
				{
					mWindowHandle = mWindow.getSDLWindowWinHWND();
					if (mWindowHandle == IntPtr.Zero)
					{
						onSetupError?.Invoke(this, "Window handle is zero ptr");
						return false;
					}

					if (Opts.useScreenSurface)
					{
						mWindow.setupScreenSurface();
					}

					mRenderer = new Renderer(this, mWindow, -1, Defs.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC) ;
					if (!mRenderer.IsInvalid)
					{
						SDLW_SetHint(SDL_HINT.RENDER_SCALE_QUALITY, SDL_HINT.SCALE_QUALITY.LINEAR);

						if (Opts.logicalRenderSize.HasValue)
						{
							//SDLW_RenderSetLogicalSize((IntPtr)this.mRenderer, Opts.logicalRenderSize.Value.Width, Opts.logicalRenderSize.Value.Height);
							mRenderer.SetLogicalSize(Opts.logicalRenderSize.Value);
							currentRendererSize = Opts.logicalRenderSize.Value;
						}

						Sys.SetParentWindow(mWindowHandle, Parent.Handle);


						if (Opts.disableSDLCursor)
						{
							SDLW_ShowCursor(0);
						}
						if (Opts.extraSDLSetupSteps != null)
						{
							Opts.extraSDLSetupSteps.Invoke(this, null);

						}


						///var x1 = SDLW_GetRelativeMouseMode();
 

						mRenderer.onRendererChanged += MRenderer_onRendererChanged;
					}
					else
					{
						onSetupError.Invoke(this, "Renderer setup was invalid");
						return false;

					}
				}
				else
				{
					onSetupError.Invoke(this, "Window setup was invalid");
					return false;

				}

			}
			else
			{
				//ToDo exception here.
				onSetupError?.Invoke(this, "Init failed");
				return false;
			}

			if (success)
				onSetupComplete?.Invoke(this, null);

			return success;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="r"></param>
		/// <param name="rPtr"></param>
		/// <returns></returns>
		private bool MRenderer_onRendererChanged(Renderer r, IntPtr rPtr)
		{
			Texture.possibleInvalidation = true;

			Texture.handleRendererInvalidation(r);

			return true;


		}

		public void SetHint(string key, string value)
		{
			SDLW_SetHint(key, value);
		} 


		#region drawing methdos
		public void FillRect(Rectangle rect, SDL4Color col)
		{
			SDLW_FillRectangle((IntPtr)mRenderer, rect.X, rect.Y, rect.Width, rect.Height, col);
		}

		public void DrawRect(Rectangle rect, SDL4Color col)
		{
			SDLW_DrawRectangle((IntPtr)mRenderer, rect.X, rect.Y, rect.Width, rect.Height, col);
		}

		#endregion

		public static void clearError()
		{
			Imports.SDLW_ClearError();
		}
		public static string getError()
		{
			IntPtr er = SDLW_GetError();
			var x = Marshal.PtrToStringAnsi(er);
			if (!String.IsNullOrWhiteSpace(x)) return x;
			return "";
		}
	 
	}


}
