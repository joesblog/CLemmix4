using CLemmix4.Lemmix;
using CLemmix4.Lemmix.Utils;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CLemmix4.Lemmix.Scenes;
//using Raylib_cs;
//using static Raylib_cs.Raylib;
//using static Raylib_cs.Color;
using CLemmix4.Lemmix.Core;

namespace CLemmix4
{
	internal static class Program
	{
		private static LevelPack lp;
		private static TextureCacheData tchterrain;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// 

		[STAThread]
		static unsafe void Main()
		{ 
			lp = new LevelPack(@"D:\_tempdown\NeoLemmix_V12.12.4\levels\Lemmings\");
			SceneManager sm = new SceneManager();
			//sm.Initialize(new LevelScene(sm, lp.testLevel));
			sm.Initialize(new LevelScene(sm, lp.testLevel));
		}

	
	}
}
