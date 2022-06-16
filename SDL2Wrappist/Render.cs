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

	public class Renderer : WrappistPTRHandler
	{
		private static int lastId = 0;
		public delegate bool rendererEVArgs(Renderer r, IntPtr rPtr);
		/// <summary>
		/// handle texture recreation here?
		/// </summary>
		public event rendererEVArgs onRendererChanged;

		//Threads
		ThreadStart tsRenderer;
		Thread thRenderer;

		public Context Context { get; }
		public Window Window { get; private set; }
		public SDL_RendererFlags flags { get; private set; }
		public int ix { get; private set; }

		public bool Running = false;
		public bool Paused = false;
		public bool textureRebuildPause = false;
		public string rendererId { get; set; }
		public int internalRendererID { get; set; }
		public Renderer(Context _con, Window _win, int _ix, Defs.SDL_RendererFlags _flags) : base(ownsHandle: true)
		{
			this.Context = _con;
			this.Window = _win;
			this.flags = _flags;
			this.ix = _ix;
			IntPtr v = SDLW_CreateRenderer((IntPtr)_win, _ix, (uint)_flags);
			this.rendererId = Guid.NewGuid().ToString();
			this.wSetHandle(v);

		}


		public override void wSetHandle(IntPtr handle)
		{
			base.wSetHandle(handle);
			this.internalRendererID = (++lastId);
		}

		/// <summary>
		/// Destroys and Recreates the renderer
		/// </summary>
		public void Reset()
		{
			SDLW_DestroyRenderer((IntPtr)this);
			IntPtr v = SDLW_CreateRenderer((IntPtr)Window, ix, (uint)flags);
			this.rendererId = Guid.NewGuid().ToString();

			this.wSetHandle(v);

			if (onRendererChanged != null)
			{

				this.Paused = true;
				if (onRendererChanged.Invoke(this, this.DangerousGetHandle()))
				{
					this.Paused = false;
				}
			}


		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			SDLW_DestroyRenderer(this.DangerousGetHandle());

		}
		protected override bool ReleaseHandle()
		{

			SDLW_DestroyRenderer(this.DangerousGetHandle());
			return base.ReleaseHandle();
		}

		public void RendererStart()
		{
			if (tsRenderer == null)
				tsRenderer = new ThreadStart(renderLogic);
			if (thRenderer == null)
				thRenderer = new Thread(tsRenderer);


			thRenderer.Name = "RENDER THREAD";
			Running = true;
			Paused = false;
			//if (!thRenderer.IsAlive && !thRenderer.ThreadState.HasFlag(System.Threading.ThreadState.Running))
			thRenderer.Start();


		}

		public void RendererStop()
		{
			Running = false;
			thRenderer.Join();


		}

		public bool SetDrawColor(SDL4Color color)
		{
			return SDLW_SetRenderDrawColor((IntPtr)this, (byte[])color) != -1;
		}

		public int SetLogicalSize(Size value)
		{
			return SetLogicalSize(value.Width, value.Height);
		}
		public int SetLogicalSize(int w, int h)
		{
			return SDLW_RenderSetLogicalSize((IntPtr)this, w, h);
		}
		public void RendererPause()
		{
			Paused = true;
		}
		public void RendererUnPause()
		{
			Paused = false;
		}

		public void RenderClear()
		{
			SDLW_RenderClear((IntPtr)this);
		}
		public void RenderPresent()
		{
			SDLW_RenderPresent((IntPtr)this);
		}
		public long Frame = 0;
		public long MultFrame = 0;




		public virtual void renderLogic()
		{
			if (Context.Opts.renderClearColor != null)
			{
				SetDrawColor(Context.Opts.renderClearColor);
			}
			uint delayTime = (uint)(1000 / (Context.Opts.SCREEN_FPS <= 0 ? ContextOptions.DEFAULT.SCREEN_FPS : Context.Opts.SCREEN_FPS));
			while (Running)
			{

				Context.handleSizeChange();

				while (Paused)
				{
					Debug.WriteLine("Paused Renderer");
					SDLW_Delay(delayTime);

				 
				}
				while (textureRebuildPause)
				{
					//Debug.WriteLine("Paused Renderer for textures");
					Context.invokeLogEvent("Texture rebuild pause");
					SDLW_Delay(delayTime);


				}


				RenderClear();

				//startEvents
				int pollr = 0;


				bool panMode = false;
				WPoint panStart = new WPoint(0, 0);
				while ((pollr = SDLW_PollEvent(out Context.ev)) != 0)
				{


					var evx = Context.ev;
					if (Context.Opts.panClickEnabled)
						handleMousePanningClickUpDown(evx, Context, Context.Opts.panOnButton);

					if (Context.Opts.panClickRelativeEnabled)
					{

						handleMousePanningClickRelative(Context, Context.Opts.panOnButton, evx);
					}
					switch (Context.ev.type)
					{
						case SEvent.SDL_EventType.SDL_MOUSEWHEEL:
							{
								Context.invokeMouseWheel(evx.wheel.x, evx.wheel.y);
								break;
							}
						case SEvent.SDL_EventType.SDL_MOUSEBUTTONDOWN:
							{

								Context.invokeMouseDown(Context.ev.button.button, Context.ev.button.x, Context.ev.button.y);

								if (evx.button.button == SDL_BUTTON_LEFT)
								{
									/*panMode = true;
									panStart = new int[] { evx.motion.x, evx.motion.y };

									SDLW_SetRelativeMouseMode(1);
									Context.invokeLogEvent2("Pan Mode On");*/
								}



								break;

							}

						case SEvent.SDL_EventType.SDL_MOUSEBUTTONUP:
							{
								Context.invokeMouseUp(Context.ev.button.button, Context.ev.button.x, Context.ev.button.y);

								if (evx.button.button == SDL_BUTTON_LEFT)
								{
									/*	panMode = false;

										SDLW_SetRelativeMouseMode(0);

										int oX = 0; int oY = 0;

										var m = SDLW_GetRelativeMouseState(ref oX, ref oY);
										Context.invokeLogEvent2("Pan Mode Off\r\n" +
											$"relX{oX},relY{oY}\r\n" +
											$"m:{m}");*/



								}



								break;
							}
						case SEvent.SDL_EventType.SDL_MOUSEMOTION:
							{
								Context.invokeMouseHover(Context.ev.button.button, Context.ev.motion.x, Context.ev.motion.y);



								/*	if (evx.button.button == SDL_BUTTON_LEFT)
									{
										if (evx.motion.state == SDL_BUTTON_LMASK)
										{
											int oX = 0; int oY = 0;

											var m = SDLW_GetRelativeMouseState(ref oX, ref oY);
											Context.invokeLogEvent2("Pan Mode test\r\n" +
					$"relX{oX},relY{oY}\r\n" +
					$"m:{m}");


										}
									}*/
								/*Context.invokeLogEvent(
														$"MMotion:\r\n" +
														$"xpo:{evx.motion.x}ypo:{evx.motion.x}\r\n" +
														$"xrl:{evx.motion.xrel}yrl:{evx.motion.yrel}\r\n" +
														$"btn:-{evx.button.button}"
														);*/

								break;
							}




					}
				}


				//endEvents

				Context.Opts.renderLogicFunction?.Invoke(this.Context, this, this.Context.mWindow, Frame);
				RenderPresent();
				SDLW_Delay(delayTime);

				if (Frame++ > long.MaxValue - 1)
				{
					MultFrame++;
					Frame = 0;
				}

			}


		}

		internal void DrawLine(int x1, int y1, int x2, float y2,SDL4Color color)
		{
			SDL2Wrappist.Imports.SDLW_DrawLine((IntPtr)this, x1, y1, x2, (int)y2, color);
		}

		private void handleMousePanningClickRelative(Context context, int panOnButton, SEvent.SDL_Event evx)
		{

			int ClickRelativeoX = 0; int clickRelativeoY = 0;
			switch (evx.type)
			{
				case SEvent.SDL_EventType.SDL_MOUSEMOTION:
					{

						if (evx.button.button == panOnButton)
						{
							if (evx.motion.state == SDL_BUTTON((uint)panOnButton))
							{


								var m = SDLW_GetRelativeMouseState(ref ClickRelativeoX, ref clickRelativeoY);

								//	context.invokeMousePan(panOnButton, new WVertex(ClickRelativeoX, clickRelativeoY), ClickTrue);

								context.invokeMousePan(panOnButton, new WVector2(evx.motion.xrel, evx.motion.yrel));
								/*	Context.invokeLogEvent2("Pan Mode test\r\n" +
							$"relX{ClickRelativeoX},relY{clickRelativeoY}\r\n" +
							$"rel2X{evx.motion.xrel},re2lY{evx.motion.yrel}\r\n" +
							$"m:{m}");*/





							}

						}
						break;
					}
				case SEvent.SDL_EventType.SDL_MOUSEBUTTONUP:
					{
						if (evx.button.button == panOnButton)
						{



						}
						break;
					}

				case SEvent.SDL_EventType.SDL_MOUSEBUTTONDOWN:
					{
						if (evx.button.button == panOnButton)
						{

						}
						break;
					}
			}




		}

		private void handleMousePanningClickUpDown(SEvent.SDL_Event evx, Context context, int panOnButton)
		{
			switch (evx.type)
			{
				case SEvent.SDL_EventType.SDL_MOUSEBUTTONDOWN:
					{
						if (evx.button.button == panOnButton)
						{
							context.isPanningOn[panOnButton - 1] = true;
							SDLW_SetRelativeMouseMode(1);

							//context.invokeLogEvent2("Pan Mode On");
						}

						break;
					}
				case SEvent.SDL_EventType.SDL_MOUSEBUTTONUP:
					{
						if (evx.button.button == panOnButton)
						{
							context.isPanningOn[panOnButton - 1] = false;
							SDLW_SetRelativeMouseMode(0);
							int oX = 0; int oY = 0;
							var m = SDLW_GetRelativeMouseState(ref oX, ref oY);

							context.invokeMousePan(panOnButton, new int[] { oX, oY });

							/*context.invokeLogEvent2("Pan Mode Off\r\n" +
							$"relX{oX},relY{oY}\r\n" +
							$"m:{m}");*/

						}
						break;
					}
			}
		}

		public bool renderCopy(SDL_Rect src, SDL_Rect dest, Texture texture)
		{
			return renderCopy(src, dest, (IntPtr)texture);
		}
		public bool renderCopy(SDL_Rect src, SDL_FRect dest, Texture texture)
		{
			return renderCopy(src, dest, (IntPtr)texture);
		}
		public bool renderCopy(SDL_Rect src, SDL_Rect dest, IntPtr texture)
		{
			/*IntPtr src_iptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SDL2Wrappist.Defs.SDL_Rect)));
			Marshal.StructureToPtr(src, src_iptr, true);

			IntPtr dst_iptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SDL2Wrappist.Defs.SDL_Rect)));
			Marshal.StructureToPtr(dest, dst_iptr, true);

			bool r =  SDL2Wrappist.Imports.SDLW_RenderCopy((IntPtr)this, texture, src_iptr, dst_iptr) == 0;
			Marshal.FreeHGlobal(src_iptr);
			Marshal.FreeHGlobal(dst_iptr);
			return r;*/

			return SDL2Wrappist.Imports.SDLW_RenderCopy2((IntPtr)this, texture, ref src, ref dest) == 0;
		}

		public bool renderCopy(SDL_Rect src, SDL_FRect dest, IntPtr texture)
		{
			/*IntPtr src_iptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SDL2Wrappist.Defs.SDL_Rect)));
			Marshal.StructureToPtr(src, src_iptr, true);

			IntPtr dst_iptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SDL2Wrappist.Defs.SDL_FRect)));
			Marshal.StructureToPtr(dest, dst_iptr, true);

			bool r = SDL2Wrappist.Imports.SDLW_RenderCopy((IntPtr)this, texture, src_iptr, dst_iptr) == 0;
			Marshal.FreeHGlobal(src_iptr);
			Marshal.FreeHGlobal(dst_iptr);
			return r;*/

			return SDL2Wrappist.Imports.SDLW_RenderCopy2F((IntPtr)this, texture, ref src, ref dest) == 0;

		}
	}

}
