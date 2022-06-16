using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static CLemmix4.SDL2Wrappist.Imports;
using static CLemmix4.SDL2Wrappist.Defs;
//using static JS.Common;
using System.Threading;
using CLemmix4.SDL2Wrappist.Colors;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO;
using System.Collections;
using static CLemmix4.SDL2Wrappist.Common;

namespace CLemmix4.SDL2Wrappist
{


	public class TextureLookup
	{
		public IntPtr ptrRenderer;
		public IntPtr ptrTexture;

	}



	public enum WRP_TextureType { IMAGE, DELEGATED };

	public class Texture : WrappistPTRHandler
	{

		public static TextureCache cache = new TextureCache();
		public static bool possibleInvalidation = false;


		public delegate IntPtr delgTexture(Renderer r);

		public WRP_TextureType textureType { get; private set; }
		public string textureFileName { get; set; }

		public delgTexture textureDelegate { get; set; }
		public Renderer assignedRenderer { get; set; }
		public int assignedRendererId { get; set; }
		public int cacheID { get; set; }

		public int height { get; set; }
		public int width { get; set; }

		public bool alphaBlend { get; set; }
		public Texture() : base(true)
		{
		}

		public Texture(string textureFileName, Renderer initRenderer, bool _alphaBlend = false) : this()
		{
			this.textureFileName = textureFileName;
			this.assignedRenderer = initRenderer;
			this.assignedRendererId = initRenderer.internalRendererID;
			this.textureType = WRP_TextureType.IMAGE;
			this.alphaBlend = _alphaBlend;

			initTexture_ImageFile();

		}

		public void handleAlphaBlend()
		{
			if (alphaBlend)
			{
				Imports.SDLW_AlphaBlendTextures((IntPtr)this, 1);
			}
			else {
				Imports.SDLW_AlphaBlendTextures((IntPtr)this, 0);
			}
		}



		public Texture(delgTexture op, Renderer initRenderer) : this()
		{

			this.assignedRenderer = initRenderer;
			this.assignedRendererId = initRenderer.internalRendererID;
			this.textureType = WRP_TextureType.DELEGATED;

			this.textureDelegate = op;

			initTexture_Delegate();


		}



		void querytexture()
		{

			int[] r = new int[2];

			var gch = GCHandle.Alloc(r, GCHandleType.Pinned);


			SDLW_GetTextureDims((IntPtr)this, gch.AddrOfPinnedObject());

			gch.Free();

			this.width = r[0];
			this.height = r[1];

		}


		public UInt32 queryPixelFormat()
		{

			UInt32[] r = new UInt32[1] ; r[0] = 0;

			var gch = GCHandle.Alloc(r, GCHandleType.Pinned);

			SDLW_GetTexturePixelFormat((IntPtr)this, gch.AddrOfPinnedObject());
			gch.Free();
			return r[0];

		}

		static object it_ImageFileLock = new object();

		static IntPtr safeLoadImagetTexture(Renderer r, string filename)
		{
			lock (it_ImageFileLock)
			{ 
			return  SDLW_ImageLoadToTexture((IntPtr)r, filename);

			}

		}

		private void initTexture_ImageFile()
		{

		



				bool isNew = true;
			
				if (this.DangerousGetHandle() != IntPtr.Zero)
				{
					SDLW_DestroyTexture(this.DangerousGetHandle());
					//ReleaseHandle(); //Maybe call this for mem management? idk how .net handles free's
					isNew = false;
				}

			//var ptr = SDLW_ImageLoadToTexture((IntPtr)this.assignedRenderer, this.textureFileName);

			var ptr = safeLoadImagetTexture(this.assignedRenderer, this.textureFileName);

		 

				wSetHandle(ptr);

				querytexture();

			if (isNew)
			{
				cacheID = cache.Add(this);
				Debug.WriteLine($"Added : {cacheID} {this.textureFileName}");

			}
					

		}

		private void initTexture_Delegate()
		{

			bool isNew = true;
			if (this.DangerousGetHandle() != IntPtr.Zero)
			{
				SDLW_DestroyTexture(this.DangerousGetHandle());
				ReleaseHandle(); //Maybe call this for mem management? idk how .net handles free's
				isNew = false;
			}

			if (this.textureDelegate != null)
			{
				var ptr = textureDelegate.Invoke(this.assignedRenderer);
				wSetHandle(ptr);
				querytexture();
			}

			if (isNew)
			{

				cacheID = cache.Add(this);
			}


		}

		protected override bool ReleaseHandle()
		{
			SDLW_DestroyTexture((IntPtr)this);
			return base.ReleaseHandle();

		}



		static bool rebuilding = false;
		public static void handleRendererInvalidation(Renderer newR)
		{

			if (!rebuilding)
			{
				rebuilding = true;
				cache.pauseAllRenderers();
				lock (cache.olock)
				{

					newR.Context.invokeLogEvent("Texture->Renderer", $"handling render invalidation ");
					//handleRendererInvalidation_Images(newR);
					//handleRendererInvalidation_Delegates(newR);
					handleRendererInvalidation_All(newR);
					newR.Context.invokeLogEvent("Texture->Renderer", "Render textures regenerated");
				}
				cache.resumeAllRenderers();
				rebuilding = false;
			}
			else {
				int asf = 44;
			}

		}
		
		private static void handleRendererInvalidation_All(Renderer newR)
		{





			lock (cache.olock)
			{ 
			
			
				var invalidIds = cache.getAllNotOfRenderer(newR).ToList();//.Select(o => new { cid = o.cacheID, typ = o.textureType }).ToList();
				foreach (var i in invalidIds)
				{
					switch (i.textureType)
					{
						case WRP_TextureType.IMAGE:
							cache[i.cacheID].assignedRenderer = newR;
							cache[i.cacheID].assignedRendererId = newR.internalRendererID;
							cache[i.cacheID].initTexture_ImageFile();
							break;
						case WRP_TextureType.DELEGATED:
							cache[i.cacheID].assignedRenderer = newR;

							cache[i.cacheID].assignedRendererId = newR.internalRendererID;
							cache[i.cacheID].initTexture_Delegate();
							break;
					}
				}


			}
		}

	 
		private static void handleRendererInvalidation_Delegates(Renderer newR)
		{

			cache.pauseAllRenderers();


			var invalidIds = cache.getAllDelegatesNotOfRenderer(newR).Select(o => o.cacheID).ToList();
			foreach (var i in invalidIds)
			{
				cache[i].assignedRenderer = newR;
				cache[i].assignedRendererId = newR.internalRendererID;
				cache[i].initTexture_Delegate();
			}
			cache.resumeAllRenderers();



		}

		private static void handleRendererInvalidation_Images(Renderer newR)
		{
			cache.pauseAllRenderers();

			var invalidIds = cache.getAllImagesNotOfRenderer(newR).Select(o => o.cacheID).ToList();
			foreach (var i in invalidIds)
			{


				cache[i].assignedRenderer = newR;
				cache[i].assignedRendererId = newR.internalRendererID;
				cache[i].initTexture_ImageFile();

			}
			cache.resumeAllRenderers();

		}

		internal bool renderCopy(Renderer r, int[] vs1, int[] vs2)
		{
			//	Context.clearError();
			bool ok = r.renderCopy(vs1, vs2, (IntPtr)this);
			if (!ok)
			{
				string er = Context.getError();
				//Debug.WriteLine($"Error on {this.textureFileName}")
				//	r.Context.invokeLogEvent2($"Error on {this.textureFileName}\r\n");

				this.assignedRenderer = r;
				this.assignedRendererId = r.internalRendererID;
				if (this.textureType == WRP_TextureType.IMAGE)
					this.initTexture_ImageFile();
				else if (this.textureType == WRP_TextureType.DELEGATED)
					this.initTexture_Delegate();
			}
			return ok;
		}	
		internal bool renderCopyF(Renderer r, int[] vs1, float[] vs2)
		{
			//	Context.clearError();
			bool ok = r.renderCopy(vs1, vs2, (IntPtr)this);
			if (!ok)
			{
				string er = Context.getError();
				//Debug.WriteLine($"Error on {this.textureFileName}")
				//	r.Context.invokeLogEvent2($"Error on {this.textureFileName}\r\n");

				this.assignedRenderer = r;
				this.assignedRendererId = r.internalRendererID;
				if (this.textureType == WRP_TextureType.IMAGE)
					this.initTexture_ImageFile();
				else if (this.textureType == WRP_TextureType.DELEGATED)
					this.initTexture_Delegate();
			}
			return ok;
		}

		int repaircount = 0;
		private bool repairRenderCopy(Renderer r, int[] vs1, int[] vs2)
		{
			handleRendererInvalidation(r);
			bool ok = r.renderCopy(vs1, vs2, (IntPtr)this);
			if (!ok && ++repaircount < 5)
			{
				repairRenderCopy(r, vs1, vs2);
			}

			return ok;
		}
	}

	public class TextureCache : LockedIncList<Texture>
	{

		public Texture this[int key]
		{
			get => this.Get(key);

		}



		public void pauseAllRenderers()
		{
			lock (base.olock)
			{
				foreach (var i in this)
				{
					i.assignedRenderer.textureRebuildPause = true;
				}
			}
		}

		public void resumeAllRenderers()
		{
			lock (base.olock)
			{
				foreach (var i in this)
				{
					i.assignedRenderer.textureRebuildPause = false;
				}
			}
		}


		public IEnumerable<Texture> getAllImagesOfRenderer(Renderer r)
		{
			return this.Where(o =>
			o.textureType == WRP_TextureType.IMAGE &&
			o.assignedRendererId == r.internalRendererID
			);
		}
		public IEnumerable<Texture> getAllImagesNotOfRenderer(Renderer r)
		{
			return this.Where(o =>
			o.textureType == WRP_TextureType.IMAGE &&
			o.assignedRendererId != r.internalRendererID
			);
		}

		public IEnumerable<Texture> getAllDelegatesOfRenderer(Renderer r)
		{
			return this.Where(o =>
			o.textureType == WRP_TextureType.DELEGATED &&
			o.assignedRendererId == r.internalRendererID
			);
		}
		public IEnumerable<Texture> getAllDelegatesNotOfRenderer(Renderer r)
		{
			return this.Where(o =>
			o.textureType == WRP_TextureType.DELEGATED &&
			o.assignedRendererId != r.internalRendererID
			);
		}
		public List<Texture> getAllNotOfRenderer(Renderer r)
		{
		
			lock (base.olock)
			{
				return this.Where(o =>

			o.assignedRendererId != r.internalRendererID
			).ToList();
			}
			
		}
	}
}
