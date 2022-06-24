using CLemmix4.Lemmix.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.Color;
using static Raylib_CsLo.RlGl;
using static Raylib_CsLo.ShaderUniformDataType;
using static Raylib_CsLo.ShaderLocationIndex;
using Raylib_CsLo;
using static CLemmix4.RaylibMethods;
using System.Numerics;

namespace CLemmix4.Lemmix.Scenes
{
	internal unsafe class LevelSceneGL : absScene
	{
		public const float FOVY_PERSPECTIVE = 45.0f;
		public const float WIDTH_ORTHOGRAPHIC = 10.0f;
		TextureCacheData tchterrain;

		Camera2D cam;

		public int ow = 320;
		public int oh = 240;
		public float rW;
		public float screenWM { get; private set; }
		Color bg = new Color(0, 0, 52, 255);
		Vector2 prevMouse;
		private LevelPack.LevelData lvl;
		private int lWidth;
		private int lHeight;
		private int lSize;
		Image imgMask;
		
		public LevelSceneGL(SceneManager coreManager, LevelPack.LevelData testLevel) : base(coreManager)
		{
			lvl = testLevel;
			this.lWidth = lvl.Width;
			this.lHeight = lvl.Height;
			this.lSize = lWidth * lHeight;
		}

		string shdFS = @"#version 330

// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;

// Input uniform values
uniform sampler2D texture0;
uniform sampler2D mask;
uniform int frame;

// Output fragment color
out vec4 finalColor;

void main()
{
    vec4 maskColour = texture(mask, fragTexCoord+vec2(sin(-frame/150.0)/10.0,cos(-frame/170.0)/10.0));
    if (maskColour.r > 0.25) discard;
    vec4 texelColor = texture(texture0, fragTexCoord+vec2(sin(frame/90.0)/8.0,cos(frame/60.0)/8.0));

    finalColor = texelColor;//* maskColour;
}
";

		string shdVS = @"#version 330

// Input vertex attributes
in vec3 vertexPosition;
in vec2 vertexTexCoord;

// Input uniform values
uniform mat4 mvp;
uniform mat4 matModel;

// Output vertex attributes (to fragment shader)
out vec2 fragTexCoord;

void main()
{
    // Send vertex attributes to fragment shader
    fragTexCoord = vertexTexCoord;

    // Calculate final vertex position
    gl_Position = mvp*vec4(vertexPosition, 0.9);
}
";

		string shd2 = @"#version 330

// Input vertex attributes (from vertex shader)
in vec3 vertexPos;
in vec2 fragTexCoord;
in vec4 fragColor;

// Input uniform values
uniform sampler2D texture0;
uniform sampler2D texture1;
uniform vec4 colDiffuse;

uniform float divider = 0.5;

out vec4 finalColor;

void main()
{
    // Texel color fetching from texture sampler
    vec4 texelColor0 = texture(texture0, fragTexCoord);
    vec4 texelColor1 = texture(texture1, fragTexCoord);

    float x = fract(fragTexCoord.s);
    float final = smoothstep(divider - 0.1, divider + 0.1, x);

    finalColor = mix(texelColor0, texelColor1, final);
}";
		public override void Input()
		{
			float mw = GetMouseWheelMove();
			if (mw != 0)
			{


				cam.zoom += GetMouseWheelMove();

			}
			base.Input();
		}
		public override void onWindowReisized(int newWidth, int newHeight)
		{
			rW = (float)manager.ScreenWidth / (float)ow;
			SetMousePosition(manager.ScreenWidth / 2, manager.ScreenHeight / 2);
			screenWM = manager.ScreenWidth * 0.05f;
			base.onWindowReisized(newWidth, newHeight);
		}
		int tex1 = 0;
		public override void SetupScene()
		{

			tchterrain = new TextureCacheData();
			//	cam = new Camera3D(new Vector3(0, 10, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0), WIDTH_ORTHOGRAPHIC, CameraProjection.CAMERA_ORTHOGRAPHIC);
			cam = new Camera2D() { offset = new System.Numerics.Vector2(0, 0), target = new System.Numerics.Vector2(0, 0), rotation = 0, zoom = 1 };

			//cam.target = GetScreenToWorld2D(cam.offset + new Vector2(lvl.Start_X, lvl.Start_Y - 80), cam);
			foreach (var i in lvl.Terrain.GroupBy(o => o.Style + o.Piece).Select(o => o.First()))
			{
				var op = tchterrain[i];
			}
			tchterrain.BuildImageAtlas();
			SetTargetFPS(60);
			SetMousePosition(manager.ScreenWidth / 2, manager.ScreenHeight / 2);
			bool ok = BuildTerrainMask();
			shader = LoadShaderFromMemory(shdVS, shdFS);
	 
			base.SetupScene();
		}

		private unsafe bool BuildTerrainMask()
		{ 
			bool r = true;

			this.imgMask = GenImageColor(lWidth, lHeight, BLANK);


			 


			foreach (var i in lvl.Terrain)
			{
				var  tc = tchterrain[i];
				for (int y = 0; y < tc.Height; y++)
				{
					int ypos = y + i.Y;

					for (int x = 0; x < tc.Width; x++)
					{
						Color srcCol, dstCol;
						int xpos = x + i.X;
						int ix = ypos * lWidth + xpos;
						int locix = y * tc.Width + x;
						if (ix > lSize - 1 || ix < 0) continue;
						 dstCol = GetImageColor(imgMask, xpos, ypos);
						  
						srcCol = GetImageColor(tc.imgMain, xpos, ypos);



					}
				}
				 

			}


			return r;
		}
		Vector3 billPosition = new Vector3(0.0f, 2.0f, 0.0f);
		private Shader shader;

		int framesCounter = 0;
		public unsafe override void Render()
		{
			framesCounter++;
			BeginDrawing();
 
			ClearBackground(bg);
			//DrawTexture(tchterrain.texAtlas, 0, 0, WHITE);
			BeginShaderMode(shader);

 

			foreach (var i in lvl.Terrain)
			{
				var atl = tchterrain.getAtl(i);

			 	DrawTextureRec(tchterrain.texAtlas, atl[0].pos, new Vector2(i.X,i.Y), WHITE);
				//DrawTexture(tchterrain[i].tex, i.X, i.Y, WHITE);

			}
			EndShaderMode();


			DrawFPS(10, 10);
			EndDrawing();

		}
	}
}
