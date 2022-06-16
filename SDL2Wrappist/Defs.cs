using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CLemmix4.SDL2Wrappist
{

	/// <summary>
	/// Borrowed heavily the donkey work of porting some of the SDL2 header files from https://github.com/flibitijibibo/SDL2-CS/blob/master/src/SDL2.cs,
	/// I'm not writing >1000s of struct and enums when someone else has kindly done thw work!
	/// </summary>
	public static class Defs
  {
    #region sdl_consts
    public const int SDL_TRUE = 1;
    public const int SDL_FALSE = 0;
		#endregion


		#region SDLMouse
		public static uint SDL_BUTTON(uint X)
		{
			// If only there were a better way of doing this in C#
			return (uint)(1 << ((int)X - 1));
		}

		public const uint SDL_BUTTON_LEFT = 1;
		public const uint SDL_BUTTON_MIDDLE = 2;
		public const uint SDL_BUTTON_RIGHT = 3;
		public const uint SDL_BUTTON_X1 = 4;
		public const uint SDL_BUTTON_X2 = 5;
		public static readonly UInt32 SDL_BUTTON_LMASK = SDL_BUTTON(SDL_BUTTON_LEFT);
		public static readonly UInt32 SDL_BUTTON_MMASK = SDL_BUTTON(SDL_BUTTON_MIDDLE);
		public static readonly UInt32 SDL_BUTTON_RMASK = SDL_BUTTON(SDL_BUTTON_RIGHT);
		public static readonly UInt32 SDL_BUTTON_X1MASK = SDL_BUTTON(SDL_BUTTON_X1);
		public static readonly UInt32 SDL_BUTTON_X2MASK = SDL_BUTTON(SDL_BUTTON_X2);


		#endregion
		#region defs_sdl
		public enum SDL_WindowFlags
    {
      SDL_WINDOW_FULLSCREEN = 0x00000001,         /**< fullscreen window */
      SDL_WINDOW_OPENGL = 0x00000002,             /**< window usable with OpenGL context */
      SDL_WINDOW_SHOWN = 0x00000004,              /**< window is visible */
      SDL_WINDOW_HIDDEN = 0x00000008,             /**< window is not visible */
      SDL_WINDOW_BORDERLESS = 0x00000010,         /**< no window decoration */
      SDL_WINDOW_RESIZABLE = 0x00000020,          /**< window can be resized */
      SDL_WINDOW_MINIMIZED = 0x00000040,          /**< window is minimized */
      SDL_WINDOW_MAXIMIZED = 0x00000080,          /**< window is maximized */
      SDL_WINDOW_INPUT_GRABBED = 0x00000100,      /**< window has grabbed input focus */
      SDL_WINDOW_INPUT_FOCUS = 0x00000200,        /**< window has input focus */
      SDL_WINDOW_MOUSE_FOCUS = 0x00000400,        /**< window has mouse focus */
      SDL_WINDOW_FULLSCREEN_DESKTOP = (SDL_WINDOW_FULLSCREEN | 0x00001000),
      SDL_WINDOW_FOREIGN = 0x00000800,            /**< window not created by SDL */
      SDL_WINDOW_ALLOW_HIGHDPI = 0x00002000,      /**< window should be created in high-DPI mode if supported.
                                                     On macOS NSHighResolutionCapable must be set true in the
                                                     application's Info.plist for this to have any effect. */
      SDL_WINDOW_MOUSE_CAPTURE = 0x00004000,      /**< window has mouse captured (unrelated to INPUT_GRABBED) */
      SDL_WINDOW_ALWAYS_ON_TOP = 0x00008000,      /**< window should always be above others */
      SDL_WINDOW_SKIP_TASKBAR = 0x00010000,      /**< window should not be added to the taskbar */
      SDL_WINDOW_UTILITY = 0x00020000,      /**< window should be treated as a utility window */
      SDL_WINDOW_TOOLTIP = 0x00040000,      /**< window should be treated as a tooltip */
      SDL_WINDOW_POPUP_MENU = 0x00080000,      /**< window should be treated as a popup menu */
      SDL_WINDOW_VULKAN = 0x10000000       /**< window usable for Vulkan surface */
    }
    public enum SDL_RendererFlags
    {
      SDL_RENDERER_SOFTWARE = 0x00000001,         /**< The renderer is a software fallback */
      SDL_RENDERER_ACCELERATED = 0x00000002,      /**< The renderer uses hardware
                                                     acceleration */
      SDL_RENDERER_PRESENTVSYNC = 0x00000004,     /**< Present is synchronized
                                                     with the refresh rate */
      SDL_RENDERER_TARGETTEXTURE = 0x00000008     /**< The renderer supports
                                                     rendering to texture */
    }
    public static class SDL_HINT
    {
      public const string
          ACCELEROMETER_AS_JOYSTICK = "ACCELEROMETER_AS_JOYSTICK",
  ANDROID_APK_EXPANSION_MAIN_FILE_VERSION = "ANDROID_APK_EXPANSION_MAIN_FILE_VERSION",
  ANDROID_APK_EXPANSION_PATCH_FILE_VERSION = "ANDROID_APK_EXPANSION_PATCH_FILE_VERSION",
  ANDROID_SEPARATE_MOUSE_AND_TOUCH = "ANDROID_SEPARATE_MOUSE_AND_TOUCH",
  APPLE_TV_CONTROLLER_UI_EVENTS = "APPLE_TV_CONTROLLER_UI_EVENTS",
  APPLE_TV_REMOTE_ALLOW_ROTATION = "APPLE_TV_REMOTE_ALLOW_ROTATION",
  BMP_SAVE_LEGACY_FORMAT = "BMP_SAVE_LEGACY_FORMAT",
  EMSCRIPTEN_KEYBOARD_ELEMENT = "EMSCRIPTEN_KEYBOARD_ELEMENT",
  FRAMEBUFFER_ACCELERATION = "FRAMEBUFFER_ACCELERATION",
  GAMECONTROLLERCONFIG = "GAMECONTROLLERCONFIG",
  GRAB_KEYBOARD = "GRAB_KEYBOARD",
  IDLE_TIMER_DISABLED = "IDLE_TIMER_DISABLED",
  IME_INTERNAL_EDITING = "IME_INTERNAL_EDITING",
  JOYSTICK_ALLOW_BACKGROUND_EVENTS = "JOYSTICK_ALLOW_BACKGROUND_EVENTS",
  MAC_BACKGROUND_APP = "MAC_BACKGROUND_APP",
  MAC_CTRL_CLICK_EMULATE_RIGHT_CLICK = "MAC_CTRL_CLICK_EMULATE_RIGHT_CLICK",
  MOUSE_FOCUS_CLICKTHROUGH = "MOUSE_FOCUS_CLICKTHROUGH",
  MOUSE_RELATIVE_MODE_WARP = "MOUSE_RELATIVE_MODE_WARP",
  NO_SIGNAL_HANDLERS = "NO_SIGNAL_HANDLERS",
  ORIENTATIONS = "ORIENTATIONS",
  RENDER_DIRECT3D11_DEBUG = "RENDER_DIRECT3D11_DEBUG",
  RENDER_DIRECT3D_THREADSAFE = "RENDER_DIRECT3D_THREADSAFE",
  RENDER_DRIVER = "RENDER_DRIVER",
  RENDER_OPENGL_SHADERS = "RENDER_OPENGL_SHADERS",
  RENDER_SCALE_QUALITY = "RENDER_SCALE_QUALITY",
  RENDER_VSYNC = "RENDER_VSYNC",
  RPI_VIDEO_LAYER = "RPI_VIDEO_LAYER",
  THREAD_STACK_SIZE = "THREAD_STACK_SIZE",
  TIMER_RESOLUTION = "TIMER_RESOLUTION",
  VIDEO_ALLOW_SCREENSAVER = "VIDEO_ALLOW_SCREENSAVER",
  VIDEO_HIGHDPI_DISABLED = "VIDEO_HIGHDPI_DISABLED",
  VIDEO_MAC_FULLSCREEN_SPACES = "VIDEO_MAC_FULLSCREEN_SPACES",
  VIDEO_MINIMIZE_ON_FOCUS_LOSS = "VIDEO_MINIMIZE_ON_FOCUS_LOSS",
  VIDEO_WINDOW_SHARE_PIXEL_FORMAT = "VIDEO_WINDOW_SHARE_PIXEL_FORMAT",
  VIDEO_WIN_D3DCOMPILER = "VIDEO_WIN_D3DCOMPILER",
  VIDEO_X11_NET_WM_PING = "VIDEO_X11_NET_WM_PING",
  VIDEO_X11_XINERAMA = "VIDEO_X11_XINERAMA",
  VIDEO_X11_XRANDR = "VIDEO_X11_XRANDR",
  VIDEO_X11_XVIDMODE = "VIDEO_X11_XVIDMODE",
  WINDOWS_DISABLE_THREAD_NAMING = "WINDOWS_DISABLE_THREAD_NAMING",
  WINDOWS_ENABLE_MESSAGELOOP = "WINDOWS_ENABLE_MESSAGELOOP",
  WINDOWS_NO_CLOSE_ON_ALT_F4 = "WINDOWS_NO_CLOSE_ON_ALT_F4",
  WINDOW_FRAME_USABLE_WHILE_CURSOR_HIDDEN = "WINDOW_FRAME_USABLE_WHILE_CURSOR_HIDDEN",
  WINRT_HANDLE_BACK_BUTTON = "WINRT_HANDLE_BACK_BUTTON",
  WINRT_PRIVACY_POLICY_LABEL = "WINRT_PRIVACY_POLICY_LABEL",
  WINRT_PRIVACY_POLICY_URL = "WINRT_PRIVACY_POLICY_URL",
  XINPUT_ENABLED = "XINPUT_ENABLED",
  XINPUT_USE_OLD_JOYSTICK_MAPPING = "XINPUT_USE_OLD_JOYSTICK_MAPPING";

      public class SCALE_QUALITY
      {
        public const string
           NEAREST = "nearest",
           LINEAR = "linear",
           BEST = "best";
      }
    }


		[StructLayout(LayoutKind.Sequential)]
		public struct SDL_Point
		{
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SDL_Rect
		{
			public int x;
			public int y;
			public int w;
			public int h;

			public static implicit operator SDL_Rect(int[] i)
			{
				return new SDL_Rect() { x = i[0], y = i[1], w = i[2], h = i[3] };
			}
		}

		/* Only available in 2.0.10 or higher. */
		[StructLayout(LayoutKind.Sequential)]
		public struct SDL_FPoint
		{
			public float x;
			public float y;
		}

		/* Only available in 2.0.10 or higher. */
		[StructLayout(LayoutKind.Sequential)]
		public struct SDL_FRect
		{
			public float x;
			public float y;
			public float w;
			public float h;

			public static implicit operator SDL_FRect(float[] i)
			{
				return new SDL_FRect() { x = i[0], y = i[1], w = i[2], h = i[3] };
			}
		}
		public enum SDL_TextureAccess
		{
			SDL_TEXTUREACCESS_STATIC,
			SDL_TEXTUREACCESS_STREAMING,
			SDL_TEXTUREACCESS_TARGET
		}

		public static class SDL_PIXELENUM
		{
			public static readonly uint SDL_PIXELFORMAT_UNKNOWN = 0;
			public static readonly uint SDL_PIXELFORMAT_INDEX1LSB =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_INDEX1,
					(uint)SDL_BitmapOrder.SDL_BITMAPORDER_4321,
					0,
					1, 0
				);
			public static readonly uint SDL_PIXELFORMAT_INDEX1MSB =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_INDEX1,
					(uint)SDL_BitmapOrder.SDL_BITMAPORDER_1234,
					0,
					1, 0
				);
			public static readonly uint SDL_PIXELFORMAT_INDEX4LSB =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_INDEX4,
					(uint)SDL_BitmapOrder.SDL_BITMAPORDER_4321,
					0,
					4, 0
				);
			public static readonly uint SDL_PIXELFORMAT_INDEX4MSB =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_INDEX4,
					(uint)SDL_BitmapOrder.SDL_BITMAPORDER_1234,
					0,
					4, 0
				);
			public static readonly uint SDL_PIXELFORMAT_INDEX8 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_INDEX8,
					0,
					0,
					8, 1
				);
			public static readonly uint SDL_PIXELFORMAT_RGB332 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED8,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_XRGB,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_332,
					8, 1
				);
			public static readonly uint SDL_PIXELFORMAT_RGB444 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_XRGB,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
					12, 2
				);
			public static readonly uint SDL_PIXELFORMAT_BGR444 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_XBGR,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
					12, 2
				);
			public static readonly uint SDL_PIXELFORMAT_RGB555 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_XRGB,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_1555,
					15, 2
				);
			public static readonly uint SDL_PIXELFORMAT_BGR555 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_INDEX1,
					(uint)SDL_BitmapOrder.SDL_BITMAPORDER_4321,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_1555,
					15, 2
				);
			public static readonly uint SDL_PIXELFORMAT_ARGB4444 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_ARGB,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
					16, 2
				);
			public static readonly uint SDL_PIXELFORMAT_RGBA4444 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_RGBA,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
					16, 2
				);
			public static readonly uint SDL_PIXELFORMAT_ABGR4444 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_ABGR,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
					16, 2
				);
			public static readonly uint SDL_PIXELFORMAT_BGRA4444 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_BGRA,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
					16, 2
				);
			public static readonly uint SDL_PIXELFORMAT_ARGB1555 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_ARGB,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_1555,
					16, 2
				);
			public static readonly uint SDL_PIXELFORMAT_RGBA5551 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_RGBA,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_5551,
					16, 2
				);
			public static readonly uint SDL_PIXELFORMAT_ABGR1555 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_ABGR,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_1555,
					16, 2
				);
			public static readonly uint SDL_PIXELFORMAT_BGRA5551 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_BGRA,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_5551,
					16, 2
				);
			public static readonly uint SDL_PIXELFORMAT_RGB565 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_XRGB,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_565,
					16, 2
				);
			public static readonly uint SDL_PIXELFORMAT_BGR565 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED16,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_XBGR,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_565,
					16, 2
				);
			public static readonly uint SDL_PIXELFORMAT_RGB24 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_ARRAYU8,
					(uint)SDL_ArrayOrder.SDL_ARRAYORDER_RGB,
					0,
					24, 3
				);
			public static readonly uint SDL_PIXELFORMAT_BGR24 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_ARRAYU8,
					(uint)SDL_ArrayOrder.SDL_ARRAYORDER_BGR,
					0,
					24, 3
				);
			public static readonly uint SDL_PIXELFORMAT_RGB888 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED32,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_XRGB,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
					24, 4
				);
			public static readonly uint SDL_PIXELFORMAT_RGBX8888 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED32,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_RGBX,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
					24, 4
				);
			public static readonly uint SDL_PIXELFORMAT_BGR888 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED32,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_XBGR,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
					24, 4
				);
			public static readonly uint SDL_PIXELFORMAT_BGRX8888 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED32,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_BGRX,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
					24, 4
				);
			public static readonly uint SDL_PIXELFORMAT_ARGB8888 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED32,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_ARGB,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
					32, 4
				);
			public static readonly uint SDL_PIXELFORMAT_RGBA8888 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED32,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_RGBA,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
					32, 4
				);
			public static readonly uint SDL_PIXELFORMAT_ABGR8888 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED32,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_ABGR,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
					32, 4
				);
			public static readonly uint SDL_PIXELFORMAT_BGRA8888 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED32,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_BGRA,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
					32, 4
				);
			public static readonly uint SDL_PIXELFORMAT_ARGB2101010 =
				SDL_DEFINE_PIXELFORMAT(
					SDL_PixelType.SDL_PIXELTYPE_PACKED32,
					(uint)SDL_PackedOrder.SDL_PACKEDORDER_ARGB,
					SDL_PackedLayout.SDL_PACKEDLAYOUT_2101010,
					32, 4
				);
			public static readonly uint SDL_PIXELFORMAT_YV12 =
				SDL_DEFINE_PIXELFOURCC(
					(byte)'Y', (byte)'V', (byte)'1', (byte)'2'
				);
			public static readonly uint SDL_PIXELFORMAT_IYUV =
				SDL_DEFINE_PIXELFOURCC(
					(byte)'I', (byte)'Y', (byte)'U', (byte)'V'
				);
			public static readonly uint SDL_PIXELFORMAT_YUY2 =
				SDL_DEFINE_PIXELFOURCC(
					(byte)'Y', (byte)'U', (byte)'Y', (byte)'2'
				);
			public static readonly uint SDL_PIXELFORMAT_UYVY =
				SDL_DEFINE_PIXELFOURCC(
					(byte)'U', (byte)'Y', (byte)'V', (byte)'Y'
				);
			public static readonly uint SDL_PIXELFORMAT_YVYU =
				SDL_DEFINE_PIXELFOURCC(
					(byte)'Y', (byte)'V', (byte)'Y', (byte)'U'
				);
			public static uint SDL_DEFINE_PIXELFOURCC(byte A, byte B, byte C, byte D)
			{
				return SDL_FOURCC(A, B, C, D);
			}

			public static uint SDL_FOURCC(byte A, byte B, byte C, byte D)
			{
				return (uint)(A | (B << 8) | (C << 16) | (D << 24));
			}

			public static uint SDL_DEFINE_PIXELFORMAT(
		SDL_PixelType type,
		uint order,
		SDL_PackedLayout layout,
		byte bits,
		byte bytes
	)
			{
				return (uint)(
					(1 << 28) |
					(((byte)type) << 24) |
					(((byte)order) << 20) |
					(((byte)layout) << 16) |
					(bits << 8) |
					(bytes)
				);
			}

			public enum SDL_PixelType
			{
				SDL_PIXELTYPE_UNKNOWN,
				SDL_PIXELTYPE_INDEX1,
				SDL_PIXELTYPE_INDEX4,
				SDL_PIXELTYPE_INDEX8,
				SDL_PIXELTYPE_PACKED8,
				SDL_PIXELTYPE_PACKED16,
				SDL_PIXELTYPE_PACKED32,
				SDL_PIXELTYPE_ARRAYU8,
				SDL_PIXELTYPE_ARRAYU16,
				SDL_PIXELTYPE_ARRAYU32,
				SDL_PIXELTYPE_ARRAYF16,
				SDL_PIXELTYPE_ARRAYF32
			}


			public enum SDL_BitmapOrder
			{
				SDL_BITMAPORDER_NONE,
				SDL_BITMAPORDER_4321,
				SDL_BITMAPORDER_1234
			}

			public enum SDL_PackedOrder
			{
				SDL_PACKEDORDER_NONE,
				SDL_PACKEDORDER_XRGB,
				SDL_PACKEDORDER_RGBX,
				SDL_PACKEDORDER_ARGB,
				SDL_PACKEDORDER_RGBA,
				SDL_PACKEDORDER_XBGR,
				SDL_PACKEDORDER_BGRX,
				SDL_PACKEDORDER_ABGR,
				SDL_PACKEDORDER_BGRA
			}

			public enum SDL_ArrayOrder
			{
				SDL_ARRAYORDER_NONE,
				SDL_ARRAYORDER_RGB,
				SDL_ARRAYORDER_RGBA,
				SDL_ARRAYORDER_ARGB,
				SDL_ARRAYORDER_BGR,
				SDL_ARRAYORDER_BGRA,
				SDL_ARRAYORDER_ABGR
			}

			public enum SDL_PackedLayout
			{
				SDL_PACKEDLAYOUT_NONE,
				SDL_PACKEDLAYOUT_332,
				SDL_PACKEDLAYOUT_4444,
				SDL_PACKEDLAYOUT_1555,
				SDL_PACKEDLAYOUT_5551,
				SDL_PACKEDLAYOUT_565,
				SDL_PACKEDLAYOUT_8888,
				SDL_PACKEDLAYOUT_2101010,
				SDL_PACKEDLAYOUT_1010102
			}

		}
		#endregion




		#region defs_user

		[Flags()]
    private enum WindowStyles : uint
    {
      /// <summary>The window has a thin-line border.</summary>
      WS_BORDER = 0x800000,

      /// <summary>The window has a title bar (includes the WS_BORDER style).</summary>
      WS_CAPTION = 0xc00000,

      /// <summary>The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.</summary>
      WS_CHILD = 0x40000000,

      /// <summary>Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.</summary>
      WS_CLIPCHILDREN = 0x2000000,

      /// <summary>
      /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
      /// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
      /// </summary>
      WS_CLIPSIBLINGS = 0x4000000,

      /// <summary>The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.</summary>
      WS_DISABLED = 0x8000000,

      /// <summary>The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.</summary>
      WS_DLGFRAME = 0x400000,

      /// <summary>
      /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style.
      /// The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
      /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
      /// </summary>
      WS_GROUP = 0x20000,

      /// <summary>The window has a horizontal scroll bar.</summary>
      WS_HSCROLL = 0x100000,

      /// <summary>The window is initially maximized.</summary>
      WS_MAXIMIZE = 0x1000000,

      /// <summary>The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
      WS_MAXIMIZEBOX = 0x10000,

      /// <summary>The window is initially minimized.</summary>
      WS_MINIMIZE = 0x20000000,

      /// <summary>The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
      WS_MINIMIZEBOX = 0x20000,

      /// <summary>The window is an overlapped window. An overlapped window has a title bar and a border.</summary>
      WS_OVERLAPPED = 0x0,

      /// <summary>The window is an overlapped window.</summary>
      WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

      /// <summary>The window is a pop-up window. This style cannot be used with the WS_CHILD style.</summary>
      WS_POPUP = 0x80000000u,

      /// <summary>The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.</summary>
      WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

      /// <summary>The window has a sizing border.</summary>
      WS_SIZEFRAME = 0x40000,

      /// <summary>The window has a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
      WS_SYSMENU = 0x80000,

      /// <summary>
      /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
      /// Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.  
      /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
      /// For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
      /// </summary>
      WS_TABSTOP = 0x10000,

      /// <summary>The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.</summary>
      WS_VISIBLE = 0x10000000,

      /// <summary>The window has a vertical scroll bar.</summary>
      WS_VSCROLL = 0x200000
    }

    public enum GWL
    {
      GWL_WNDPROC = (-4),
      GWL_HINSTANCE = (-6),
      GWL_HWNDPARENT = (-8),
      GWL_STYLE = (-16),
      GWL_EXSTYLE = (-20),
      GWL_USERDATA = (-21),
      GWL_ID = (-12)
    }
    #endregion

  }
}
