//using Raylib_cs;
using System.Collections.Generic;
using System.Linq;
/*using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.Rlgl;*/
using static Raylib_CsLo.Raylib;
using Raylib_CsLo;

namespace CLemmix4.Lemmix.Core
{
	public class SceneManager
	{

		public Dictionary<int, absScene> NavStack = new Dictionary<int, absScene>();
		public KeyboardKey quitKey = KeyboardKey.KEY_ESCAPE;
		public bool running = true;
		private int StartWidth { get; }
		private int StartHeight { get; }
		public string Title { get; }
		static int lastSceneId = 0;
		public int currentSceneId { get; set; } = -1;


		public SceneManager(int startWidth = 1024, int startHeight = 768, string Title = "Core")
		{
			StartWidth = startWidth;
			StartHeight = startHeight;
			ScreenWidth = StartWidth;
			ScreenHeight = StartHeight;
			this.Title = Title;
		}

		public void Initialize(absScene startScene)
		{
			Raylib.InitWindow(StartWidth, StartHeight, Title);
			ScreenWidth = StartWidth;
			ScreenHeight = StartHeight;
			currentSceneId = AddScene(startScene);

			currentScene.SetupScene();
			Loop();

		}

		public int AddScene(absScene scene)
		{
			int id = ++lastSceneId;
			NavStack.Add(id, scene);
			return id;

		}
		public void removeScene(int id)
		{
			NavStack.Remove(id);

			if (currentSceneId == id)
			{
				currentSceneId = NavStack.Last().Key;
			}



		}
		public void Loop()
		{

			while (running)
			{

				if (currentScene != null)
				{
					currentScene.Input();
					if (IsWindowResized())
					{
						this.ScreenWidth = GetScreenWidth();
						this.ScreenHeight = GetScreenHeight();
						currentScene.onWindowReisized(ScreenWidth, ScreenHeight);
					}
					currentScene.Render();
				}


			}
		}

		public absScene currentScene
		{
			get
			{
				if (NavStack.ContainsKey(currentSceneId))
					return NavStack[currentSceneId];
				else return null;
			}

		}

		public int ScreenWidth { get; private set; }
		public int ScreenHeight { get; private set; }
	}

 

}
