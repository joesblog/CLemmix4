//using Raylib_cs;
/*using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.Rlgl;*/
using Raylib_CsLo;

namespace CLemmix4.Lemmix.Core
{
	public abstract class absScene
	{

		protected Camera2D cam;
		protected SceneManager manager;
		public virtual void onWindowReisized(int newWidth, int newHeight)
		{

		}


		public absScene(SceneManager coreManager)
		{
			manager = coreManager;
		}


		public virtual void SetupScene()
		{

		}



		public unsafe virtual void Input()
		{

		}

		public abstract void Render();

		public virtual void CleanUp() { }

	}

 

}
