using CKGL;

namespace CKGLTest
{
	#region Layer
	public static class Layer
	{
		//public static float Debug { get; } = -1;
		//public static float Shadow { get; } = 0;
		//public static float Box { get; } = 1;
		//public static float Global { get; } = 2;
		//public static float Player { get; } = 3;
		public static float Tri { get; } = 4;
	}
	#endregion

	#region Shaders
	public static class Shaders
	{
		public static InternalShaders.RendererShader Renderer = new InternalShaders.RendererShader();
		public static InternalShaders.RendererFogShader RendererFog = new InternalShaders.RendererFogShader();
		public static InternalShaders.LinearizeDepthShader LinearizeDepth = new InternalShaders.LinearizeDepthShader();
		//public static GeometryTestShader GeometryTest = new GeometryTestShader();
	}
	#endregion

	#region Sounds
	public static class Sounds
	{
		public static Audio.Buffer sndPop1 = new Audio.Buffer("snd/sndPop1.wav");
		public static Audio.Buffer sndPop2 = new Audio.Buffer("snd/sndPop2.wav");
	}
	#endregion

	#region Sprites
	public static class SpriteSheets
	{
		public static SpriteSheet SpriteSheet = new SpriteSheet(128, 1);
	}

	public static class SpriteFonts
	{
		public static SpriteFont Font = new SpriteFont(SpriteSheets.SpriteSheet, "textures/font[5x7].png", 5, 7, '!', '~', 1, 3, 7, true);
	}

	public static class Sprites
	{
		public static Sprite Test1 = SpriteSheets.SpriteSheet.AddSprite("textures/Character1.png");
		public static Sprite Test2 = SpriteSheets.SpriteSheet.AddSprite("textures/Character2.png");
		public static Sprite Test3 = SpriteSheets.SpriteSheet.AddSprite("textures/Character3.png");
	}

	public static class Textures
	{
		public static Texture2D Test = Texture2D.CreateFromFile("textures/Character1.png");
	}
	#endregion

	public class CKGLTest : Game
	{
		private static int width = 320;
		private static int height = 180;
		private static int scale = 3;
		//private static int width = 2560;
		//private static int height = 1440;
		//private static int scale = 1;
		//private static int width = 1366;
		//private static int height = 768;
		//private static int scale = 1;
		//private static int width = 16;
		//private static int height = 9;
		//private static int scale = 100;

		public CKGLTest()
			: base(windowTitle: "CKGL Game!",
				   windowWidth: width * scale,
				   windowHeight: height * scale,
				   windowVSync: true,
				   windowFullscreen: false,
				   windowResizable: true,
				   windowBorderless: false,
				   msaa: 0)
		{ }

		string debugString = "";

		// Variable for moving window on mouse click and drag
		Point2 windowDraggingPosition = Point2.Zero;

		Camera Camera = new Camera();
		Camera2D Camera2D = new Camera2D();
		float cameraYaw = 0f;
		float cameraPitch = 0f;
		Vector3 cameraLookat = Vector3.Forward;
		Vector3 cameraLookatNoVertical = Vector3.Forward;

		RenderTarget surface;

		CullModeState cullModeState = CullModeState.Off;
		PolygonModeState polygonModeState = PolygonModeState.Fill;

		public override void Init()
		{
			Window.SetIcon("textures/Character1.png");

			Platform.RelativeMouseMode = true;
			//Platform.ShowCursor = false; // Default true

			Camera.FoV = 75f;
			Camera.AspectRatio = width / (float)height;
			Camera.Position = new Vector3(0f, 2f, -10f);
			//Camera.Projection = Projection.Orthographic;
			//Camera.Width = Window.Width / 10;
			//Camera.Height = Window.Height / 10;
			Camera.zNear = 0.1f;
			Camera.zFar = 150f;

			for (int i = 0; i < 20; i++)
			{
				new Tri { X = i * 2, Y = i * 2/*, Depth = -i * 20*/ };
			}

			// Debug, test spritesheet
			//SpriteSheets.SpriteSheet.Texture.SavePNG($@"{System.IO.Directory.GetCurrentDirectory()}/SpriteSheet.png");
			//SpriteSheets.SpriteSheet.Texture.SavePNG("SpriteSheet.png");

			surface = new RenderTarget(width, height, 1, TextureFormat.RGB8, TextureFormat.Depth);
		}

		public override void Update()
		{
			if (Input.Keyboard.Down(KeyCode.Backspace) || Input.Controllers.First.SelectDown)
				Platform.Quit();

			if (Input.Keyboard.Pressed(KeyCode.F11))
				Window.Fullscreen = !Window.Fullscreen;

			if (Input.Keyboard.Pressed(KeyCode.F10))
				Window.Borderless = !Window.Borderless;

			if (Input.Keyboard.Pressed(KeyCode.F8))
				Window.Resizable = !Window.Resizable;

			if (Input.Keyboard.Pressed(KeyCode.F7))
				Window.VSync = !Window.VSync;

			if (Input.Keyboard.Pressed(KeyCode.Escape))
				Platform.RelativeMouseMode = !Platform.RelativeMouseMode;

			if (Input.Keyboard.Pressed(KeyCode.F2))
			{
				if (polygonModeState == PolygonModeState.Fill)
					polygonModeState = PolygonModeState.Line;
				else if (polygonModeState == PolygonModeState.Line)
					polygonModeState = PolygonModeState.Point;
				else if (polygonModeState == PolygonModeState.Point)
					polygonModeState = PolygonModeState.Fill;
			}

			if (Input.Keyboard.Pressed(KeyCode.F3))
			{
				if (cullModeState == CullModeState.Off)
					cullModeState = CullModeState.Back;
				else if (cullModeState == CullModeState.Back)
					cullModeState = CullModeState.Front;
				else if (cullModeState == CullModeState.Front)
					cullModeState = CullModeState.FrontAndBack;
				else if (cullModeState == CullModeState.FrontAndBack)
					cullModeState = CullModeState.Off;
			}

			if (!Platform.RelativeMouseMode)
			{
				if (Input.Mouse.LeftPressed)
					windowDraggingPosition = Input.Mouse.LastPosition;
				else if (Input.Mouse.LeftDown)
					Window.Position = Input.Mouse.PositionDisplay - windowDraggingPosition;
			}

			if (Input.Keyboard.Pressed(KeyCode.Space) || Input.Mouse.LeftPressed || Input.Controllers.First.APressed)
				Sounds.sndPop1.Play();
			if (Input.Keyboard.Released(KeyCode.Space) || Input.Mouse.LeftReleased || Input.Controllers.First.AReleased)
				Sounds.sndPop2.Play();

			float speed = 10f;
			if (Input.Keyboard.Down(KeyCode.Z) || Input.Controllers.First.L2Down)
				Camera2D.Rotation += 0.01f;
			if (Input.Keyboard.Down(KeyCode.C) || Input.Controllers.First.R2Down)
				Camera2D.Rotation -= 0.01f;
			if ((Input.Keyboard.Down(KeyCode.Z) && Input.Keyboard.Down(KeyCode.C)) || (Input.Controllers.First.L2Down && Input.Controllers.First.R2Down))
				Camera2D.Rotation = 0f;
			Vector3 direction = Vector3.Zero;
			if (Input.Keyboard.Down(KeyCode.A) || Input.Controllers.First.LeftStickDigitalLeftDown || Input.Controllers.First.LeftDown)
				direction += Vector3.Cross(cameraLookatNoVertical, Vector3.Up).Normalized;
			if (Input.Keyboard.Down(KeyCode.D) || Input.Controllers.First.LeftStickDigitalRightDown || Input.Controllers.First.RightDown)
				direction += Vector3.Cross(Vector3.Up, cameraLookatNoVertical).Normalized;
			if (Input.Keyboard.Down(KeyCode.W) || Input.Controllers.First.LeftStickDigitalUpDown || Input.Controllers.First.UpDown)
				direction += cameraLookatNoVertical;
			if (Input.Keyboard.Down(KeyCode.S) || Input.Controllers.First.LeftStickDigitalDownDown || Input.Controllers.First.DownDown)
				direction -= cameraLookatNoVertical;
			if (Input.Keyboard.Down(KeyCode.Q) || Input.Controllers.First.L1Down)
				direction += Vector3.Down;
			if (Input.Keyboard.Down(KeyCode.E) || Input.Controllers.First.R1Down)
				direction += Vector3.Up;
			if (Input.Mouse.ScrollY != 0)
				Camera.FoV -= Input.Mouse.ScrollY;

			Camera.Position += direction.Normalized * speed * Time.DeltaTime;

			//  Mouse look
			if (Platform.RelativeMouseMode)
			{
				float mouseSpeed = 0.0005f;
				cameraYaw = Math.Clamp(cameraYaw + (Input.Mouse.PositionRelative.Y) * mouseSpeed, -0.249f, 0.249f);
				cameraPitch += (Input.Mouse.PositionRelative.X) * mouseSpeed;
			}

			// Controller look
			float controllerSpeed = 0.01f;
			cameraYaw = Math.Clamp(cameraYaw + (-Input.Controllers.First.RightStickY * controllerSpeed), -0.249f, 0.249f);
			cameraPitch += Input.Controllers.First.RightStickX * controllerSpeed;

			//Camera.Rotation = Quaternion.CreateLookAt(cameraLookat, Vector3.Up);
			Camera.Rotation = Quaternion.CreateRotationY(cameraPitch) * Quaternion.CreateRotationX(cameraYaw);

			cameraLookat = Vector3.Forward * Camera.Rotation;
			cameraLookatNoVertical = new Vector3(cameraLookat.X, 0f, cameraLookat.Z).Normalized;

			test.Position = new Vector3(-15f, 2f, 5f);
			test.Position += new Vector3(Math.Sin(Time.TotalSeconds * 3.7f) * 2f,
										 Math.Sin(Time.TotalSeconds * 3.5f) * 2f,
										 Math.Sin(Time.TotalSeconds * 3.3f) * 2f);
			test.Rotation = Quaternion.CreateFromEuler(new Vector3(-Time.TotalSeconds * 0.3f, -Time.TotalSeconds * 0.25f, -Time.TotalSeconds * 0.09f));
			//test.Rotation = Quaternion.CreateFromEuler(new Vector3(-Time.TotalSeconds * 0.73f, -Time.TotalSeconds * 0.525f, -Time.TotalSeconds * 0.22f));
			test.Scale = Vector3.One + Vector3.One * Math.SinNormalized(Time.TotalSeconds) * 1f;
			test.Shear = new Shear3D(Math.Sin(Time.TotalSeconds * 1.7f) * 0.2f, Math.Sin(Time.TotalSeconds * 1.9f) * 0.4f,
									 Math.Sin(Time.TotalSeconds * 1.8f) * 0.3f, Math.Sin(Time.TotalSeconds * 1.8f) * 0.3f,
									 Math.Sin(Time.TotalSeconds * 1.9f) * 0.4f, Math.Sin(Time.TotalSeconds * 1.7f) * 0.2f);

			test2.Position = new Vector3(-10f, 2f, 10f);
			//test2.Rotation = Quaternion.CreateFromEuler(new Vector3(Time.TotalSeconds * 0.3f, Time.TotalSeconds * 0.25f, Time.TotalSeconds * 0.09f));
			test2.Rotation = test.Rotation;
			test2.Scale = Vector3.One + Vector3.One * Math.SinNormalized(-Time.TotalSeconds) * 1f;

			test3 = test2.Clone();
			test3.Position -= new Vector3(8f, 0f, 0f);

			debugString = $"|:outline=1,0.01,0,0,0,1:|Cam Pos: {Camera.Position.X:n1}, {Camera.Position.Y:n1}, {Camera.Position.Z:n1}\nCam Rot: {Camera.Rotation.Euler.X:n2}, {Camera.Rotation.Euler.Y:n2}, {Camera.Rotation.Euler.Z:n2}\nMem: {RAM:n1}MB\nVSync: {Window.GetVSyncMode()}\n{Time.UPS:n0}ups | {Time.FPSSmoothed:n0}fps\nDraw Calls: {Graphics.DrawCalls}\nState Changes: {Graphics.State.Changes}\nRenderTarget Swaps/Blits: {RenderTarget.Swaps}/{RenderTarget.Blits}\nTexture Swaps: {Texture.Swaps}\nShader/Uniform Swaps: {Shader.Swaps}/{Shader.UniformSwaps}\nWinPos: [{Window.X}, {Window.Y}]\nSize: [{Window.Size}]\nMouse Global: [{Input.Mouse.PositionDisplay}]\nMouse: [{Input.Mouse.Position}]\nMouse Relative: [{Input.Mouse.PositionRelative}]";

			Scene.Current?.BeforeUpdate();
			Scene.Current?.Update();
			Scene.Current?.AfterUpdate();
		}

		Transform test = new Transform();
		Transform test2 = new Transform();
		Transform test3 = new Transform();
		public override void Draw()
		{
			surface.Bind();

			// Clear the screen
			if (Input.Keyboard.Down(KeyCode.Space))
				Graphics.Clear(1d);
			else
				Graphics.Clear(Colour.Black);

			Graphics.State.Reset();
			CullModeState.Set(cullModeState);
			DepthState.LessEqual.Set();
			PolygonModeState.Set(polygonModeState);

			if (Input.Mouse.RightDown)
			{
				Shaders.RendererFog.Bind();
				Shaders.RendererFog.MVP = Camera.Matrix;
				Shaders.RendererFog.MV = Camera.ViewMatrix;
				Shaders.RendererFog.FogType = InternalShaders.FogType.Linear;
				//Shaders.RendererFog.FogDensity = 0.013f;
				Shaders.RendererFog.FogColour = Colour.Black;
				Shaders.RendererFog.FogStart = Camera.zNear;
				Shaders.RendererFog.FogEnd = Camera.zFar;
			}
			else
			{
				Shaders.Renderer.Bind();
				Shaders.Renderer.MVP = Camera.Matrix;
			}

			Renderer.Draw.ResetTransform();
			Renderer.Draw3D.ResetTransform();

			// Start Drawing

			Renderer.Draw3D.SetTransform(test);
			Renderer.Draw3D.Cube(Colour.Cyan,
								 Colour.Yellow,
								 Colour.Red,
								 Colour.Blue,
								 Colour.Green,
								 Colour.Magenta);
			Renderer.Draw3D.ResetTransform();

			Renderer.Draw3D.SetTransform(test2);
			Colour c2 = new Colour(Math.CosNormalized(Time.TotalSeconds * 1.5f), Math.CosNormalized(Time.TotalSeconds * 1.4f), Math.CosNormalized(Time.TotalSeconds * 1.3f), 1f);
			Renderer.Draw3D.CubeWireframe(c2);
			Renderer.Draw3D.ResetTransform();

			Renderer.Draw3D.SetTransform(test3);
			Renderer.Draw3D.Cube(new Colour(0f, 0f, 0f, 1f),
								 new Colour(1f, 0f, 0f, 1f),
								 new Colour(0f, 1f, 0f, 1f),
								 new Colour(1f, 1f, 0f, 1f),
								 new Colour(0f, 0f, 1f, 1f),
								 new Colour(1f, 0f, 1f, 1f),
								 new Colour(0f, 1f, 1f, 1f),
								 new Colour(1f, 1f, 1f, 1f));
			Renderer.Draw3D.ResetTransform();

			Transform cubeGridTransformParent = new Transform();
			cubeGridTransformParent.Position = new Vector3(80, 0, 80);
			for (int x = 0; x < 5; x++)
			{
				for (int y = 0; y < 5; y++)
				{
					for (int z = 0; z < 5; z++)
					{
						Transform cubeGridTransform = new Transform();
						cubeGridTransform.Parent = cubeGridTransformParent;
						cubeGridTransform.Position = new Vector3(x * 6, y * 6, z * 6);
						cubeGridTransform.Rotation = Quaternion.CreateFromEuler(new Vector3(-Time.TotalSeconds * 0.3f * x, -Time.TotalSeconds * 0.25f * y, -Time.TotalSeconds * 0.09f * z));
						cubeGridTransform.Shear = new Shear3D(Math.Sin(y * Time.TotalSeconds * 1.7f) * 0.2f, z * Math.Sin(Time.TotalSeconds * 1.9f) * 0.4f,
															  Math.Sin(x * Time.TotalSeconds * 1.8f) * 0.3f, z * Math.Sin(Time.TotalSeconds * 1.8f) * 0.3f,
															  Math.Sin(x * Time.TotalSeconds * 1.9f) * 0.4f, y * Math.Sin(Time.TotalSeconds * 1.7f) * 0.2f);
						Renderer.Draw3D.SetTransform(cubeGridTransform);
						//Renderer.Draw3D.CubeLines(new Colour(Math.SinNormalized(x + Time.TotalSeconds * 1.5f), Math.SinNormalized(y + Time.TotalSeconds * 1.4f), Math.SinNormalized(z + Time.TotalSeconds * 1.3f), 1f));
						Renderer.Draw3D.Cube(Colour.Cyan,
											 Colour.Yellow,
											 Colour.Red,
											 Colour.Blue,
											 Colour.Green,
											 Colour.Magenta);
						//Renderer.Draw3D.Cube(new Colour(0f, 0f, 0f, 1f),
						//						  new Colour(1f, 0f, 0f, 1f),
						//						  new Colour(0f, 1f, 0f, 1f),
						//						  new Colour(1f, 1f, 0f, 1f),
						//						  new Colour(0f, 0f, 1f, 1f),
						//						  new Colour(1f, 0f, 1f, 1f),
						//						  new Colour(0f, 1f, 1f, 1f),
						//						  new Colour(1f, 1f, 1f, 1f));
					}
				}
			}
			Renderer.Draw3D.ResetTransform();

			Transform2D t2D = new Transform2D();
			//t2D.Rotation = Math.Sin(Time.TotalSeconds) * 0.03f;
			t2D.ShearX = Math.Sin(Time.TotalSeconds) * 0.4f;
			t2D.ShearY = Math.Sin(Time.TotalSeconds * 0.7f) * 0.5f;
			Renderer.Draw.SetTransform(t2D);
			Transform t = new Transform();
			//t.Rotation = Quaternion.CreateRotationZ(Math.Sin(Time.TotalSeconds) * 0.01f);
			//t.Shear = new Shear3D(0, 0, Math.Sin(Time.TotalSeconds * 0.7f) * 0.5f, Math.Cos(Time.TotalSeconds * 0.7f) * 0.5f, 0, 0);
			Renderer.Draw3D.SetTransform(t);

			Colour gridColour = new Colour(0.1f, 0.1f, 0.1f, 1f);
			int length = 100;
			//for (int yy = -length; yy <= length; yy++)
			//for (int yy = 0; yy <= 0; yy++)
			//{
			//float yy = cameraPosition.Y - 2f;
			float yy = -5f;
			for (int i = 0; i <= length; i++)
			{
				Renderer.Draw3D.Line(new Vector3(-length, yy, i),
									 new Vector3(length, yy, i),
									 gridColour);
				if (i != 0)
					Renderer.Draw3D.Line(new Vector3(-length, yy, -i),
										 new Vector3(length, yy, -i),
										 gridColour);

				Renderer.Draw3D.Line(new Vector3(i, yy, -length),
									 new Vector3(i, yy, length),
									 gridColour);
				if (i != 0)
					Renderer.Draw3D.Line(new Vector3(-i, yy, -length),
									 new Vector3(-i, yy, length),
									 gridColour);
			}
			Renderer.Draw3D.Line(new Vector3(-length, yy, -length),
								 new Vector3(-length, yy + length, -length),
								 gridColour);
			Renderer.Draw3D.Line(new Vector3(length, yy, -length),
								 new Vector3(length, yy + length, -length),
								 gridColour);
			Renderer.Draw3D.Line(new Vector3(-length, yy, length),
								 new Vector3(-length, yy + length, length),
								 gridColour);
			Renderer.Draw3D.Line(new Vector3(length, yy, length),
								 new Vector3(length, yy + length, length),
								 gridColour);
			//}
			//for (int y = -length; y <= length; y++)
			//{
			//	for (int x = -length; x <= length; x++)
			//	{
			//		for (int z = -length; z <= length; z++)
			//		{
			//			Renderer.Draw.Points.Point(new Vector3(x, y, z), gridColour);
			//		}
			//	}
			//}

			Renderer.Draw.Triangle(new Vector2(0f, 1f),
					   new Vector2(0f, 1f) * Matrix2D.CreateRotationZ(Rotation.Third),
					   new Vector2(0f, 1f) * Matrix2D.CreateRotationZ(Rotation.TwoThirds),
					   Colour.Red,
					   Colour.Green,
					   Colour.Blue,
					   null,
					   null,
					   null,
					   -Time.TotalSeconds * 0.5f,
					   Vector2.Zero);

			// Right
			Renderer.Draw3D.Triangle(new Vector3(20f, 10f, 0f),
									 new Vector3(20f, 10f, 0f) * Quaternion.CreateRotationX(Rotation.Third),
									 new Vector3(20f, 10f, 0f) * Quaternion.CreateRotationX(Rotation.TwoThirds),
									 Colour.Red,
									 Colour.Green,
									 Colour.Blue);
			Renderer.Draw3D.Triangle(new Vector3(50f, 50f, 0f),
									 new Vector3(50f, 50f, 0f) * Quaternion.CreateRotationX(Rotation.Third),
									 new Vector3(50f, 50f, 0f) * Quaternion.CreateRotationX(Rotation.TwoThirds),
									 Colour.Red,
									 Colour.Green,
									 Colour.Blue);

			// Left
			Renderer.Draw3D.Triangle(new Vector3(-20f, 10f, 0f),
									 new Vector3(-20f, 10f, 0f) * Quaternion.CreateRotationX(Rotation.TwoThirds),
									 new Vector3(-20f, 10f, 0f) * Quaternion.CreateRotationX(Rotation.Third),
									 Colour.Red,
									 Colour.Green,
									 Colour.Blue);
			Renderer.Draw3D.Triangle(new Vector3(-50f, 50f, 0f),
									 new Vector3(-50f, 50f, 0f) * Quaternion.CreateFromAxisAngle(Vector3.Left, Rotation.Third),
									 new Vector3(-50f, 50f, 0f) * Quaternion.CreateFromAxisAngle(Vector3.Left, Rotation.TwoThirds),
									 Colour.Red,
									 Colour.Green,
									 Colour.Blue);

			// Forward
			//Renderer.Draw3D.Triangle(new Vector3(0f, 2f, 10f),
			//						 new Vector3(0f, 2f, 10f) * Quaternion.CreateRotationZ(Rotation.Third),
			//						 new Vector3(0f, 2f, 10f) * Quaternion.CreateRotationZ(Rotation.TwoThirds),
			//						 Colour.Red,
			//						 Colour.Green,
			//						 Colour.Blue);
			//Renderer.Draw3D.Triangle(new Vector3(0f, 10f, 20f),
			//						 new Vector3(0f, 10f, 20f) * Quaternion.CreateRotationZ(Rotation.Third),
			//						 new Vector3(0f, 10f, 20f) * Quaternion.CreateRotationZ(Rotation.TwoThirds),
			//						 Colour.Red,
			//						 Colour.Green,
			//						 Colour.Blue);
			//Renderer.Draw3D.Triangle(new Vector3(0f, 50f, 50f),
			//						 new Vector3(0f, 50f, 50f) * Quaternion.CreateRotationZ(Rotation.Third).Matrix,
			//						 new Vector3(0f, 50f, 50f) * Quaternion.CreateRotationZ(Rotation.TwoThirds).Matrix,
			//						 Colour.Red,
			//						 Colour.Green,
			//						 Colour.Blue);
			for (int i = 0; i < 500; i += 5)
				Renderer.Draw3D.Triangle(new Vector3(0f, i * 0.2f, i * 0.5f) * Quaternion.CreateRotationZ(Rotation.Zero + i * 0.001f - Time.TotalSeconds * 0.1f),
										 new Vector3(0f, i * 0.2f, i * 0.5f) * Quaternion.CreateRotationZ(Rotation.Third + i * 0.001f - Time.TotalSeconds * 0.1f),
										 new Vector3(0f, i * 0.2f, i * 0.5f) * Quaternion.CreateRotationZ(Rotation.TwoThirds + i * 0.001f - Time.TotalSeconds * 0.1f),
										 Colour.Red,
										 Colour.Green,
										 Colour.Blue);

			// Backward
			Renderer.Draw3D.Triangle(new Vector3(0f, 10f, -20f),
									 new Vector3(0f, 10f, -20f) * Quaternion.CreateRotationZ(Rotation.TwoThirds),
									 new Vector3(0f, 10f, -20f) * Quaternion.CreateRotationZ(Rotation.Third),
									 Colour.Red,
									 Colour.Green,
									 Colour.Blue);
			Renderer.Draw3D.Triangle(new Vector3(0f, 50f, -50f),
									 new Vector3(0f, 50f, -50f) * Quaternion.CreateFromAxisAngle(Vector3.Backward, Rotation.Third),
									 new Vector3(0f, 50f, -50f) * Quaternion.CreateFromAxisAngle(Vector3.Backward, Rotation.TwoThirds),
									 Colour.Red,
									 Colour.Green,
									 Colour.Blue);

			// Up
			Renderer.Draw3D.Triangle(new Vector3(0f, 20f, -10f),
									 new Vector3(0f, 20f, -10f) * Quaternion.CreateRotationY(Rotation.Third),
									 new Vector3(0f, 20f, -10f) * Quaternion.CreateRotationY(Rotation.TwoThirds),
									 Colour.Red,
									 Colour.Green,
									 Colour.Blue);
			Renderer.Draw3D.Triangle(new Vector3(0f, 50f, -50f),
									 new Vector3(0f, 50f, -50f) * Quaternion.CreateRotationY(Rotation.Third),
									 new Vector3(0f, 50f, -50f) * Quaternion.CreateRotationY(Rotation.TwoThirds),
									 Colour.Red,
									 Colour.Green,
									 Colour.Blue);

			// Down
			Renderer.Draw3D.Triangle(new Vector3(0f, -20f, -10f),
									 new Vector3(0f, -20f, -10f) * Quaternion.CreateRotationY(Rotation.TwoThirds),
									 new Vector3(0f, -20f, -10f) * Quaternion.CreateRotationY(Rotation.Third),
									 Colour.Red,
									 Colour.Green,
									 Colour.Blue);
			Renderer.Draw3D.Triangle(new Vector3(0f, -50f, -50f),
									 new Vector3(0f, -50f, -50f) * Quaternion.CreateFromAxisAngle(Vector3.Down, Rotation.Third),
									 new Vector3(0f, -50f, -50f) * Quaternion.CreateFromAxisAngle(Vector3.Down, Rotation.TwoThirds),
									 Colour.Red,
									 Colour.Green,
									 Colour.Blue);

			DepthMaskState.Disabled.Set();
			Textures.Test.Bind();
			Renderer.Draw.Rectangle(2f,
									-0.5f,
									1f,
									1f,
									Colour.White,
									Colour.White,
									Colour.White,
									Colour.White,
									UV.BottomLeft,
									UV.BottomRight,
									UV.TopLeft,
									UV.TopRight,
									-Time.TotalSeconds * 0.5f,
									new Vector2(2.5f, 0f));

			Renderer.Draw.Sprite(Sprites.Test1,
								 new Vector2(4f, -0.5f),
								 Vector2.One / Sprites.Test1.MaxLength,
								 Colour.White);

			Renderer.Draw.Sprite(Sprites.Test2,
								 new Vector2(6f, -0.5f),
								 Vector2.One / Sprites.Test2.MaxLength,
								 Colour.White);

			Renderer.Draw.Sprite(Sprites.Test3,
								 new Vector2(8f, -0.5f),
								 Vector2.One / Sprites.Test3.MaxLength,
								 Colour.White);

			Renderer.Draw.Text(SpriteFonts.Font,
							   "|:shadow=0,-1,0.01,0,0,0,0.5:|ABCDEFGHIJKLMNOPQRSTUVWXYZ\nabcdefghijklmnopqrstuvwxyz\n1234567890\n_-+=(){}[]<>\\|/;:'\"?.,!@#$%^&*~`",
							   new Vector2(0f, 4f),
							   Vector2.One / 7f,
							   Colour.White,
							   HAlign.Center,
							   VAlign.Middle);
			DepthMaskState.Reset();

			Renderer.Draw.ResetTransform();

			// Test Geometry Shader
			//Shaders.GeometryTest.Bind();
			//Shaders.GeometryTest.MVP = Camera.Matrix;
			//for (int x = 0; x < 20; x++)
			//{
			//	for (int y = 0; y < 20; y++)
			//	{
			//		Renderer.Draw3D.Point(x * 1.1f, y * 1.1f, 0f, Colour.Red);
			//	}
			//}

			// GUI Layer

			DepthState.Off.Set();
			Scene.Current?.Draw();

			Camera2D.Width = RenderTarget.Current.Width;
			Camera2D.Height = RenderTarget.Current.Height;
			Shaders.Renderer.Bind();
			Shaders.Renderer.MVP = Camera2D.Matrix;

			//Renderer.Draw.Text(SpriteFonts.Font,
			//				   "|:shadow=0,-1,0.01,0,0,0,0.5:|Test Test\nStill testing...\nhhhheeeelllloooo",
			//				   new Vector2(2, height - 1),
			//				   Vector2.One * (1f + Math.SinNormalized(Time.TotalSeconds * 2f)),
			//				   Colour.White,
			//				   HAlign.Left,
			//				   VAlign.Top);

			//Renderer.Draw.Text(SpriteFonts.Font,
			//				   $"|:shadow=0,-1,0.01,0,0,0,0.5:|{Camera.Rotation}\nStill testing...",
			//				   new Vector2(2, 1),
			//				   Vector2.One * (1f + Math.SinNormalized(Time.TotalSeconds * 2f)),
			//				   Colour.White,
			//				   HAlign.Left,
			//				   VAlign.Bottom);

			//Renderer.Draw.Text(SpriteFonts.Font,
			//				   "|:shadow=0,-1,0.01,0,0,0,0.5:|Test Test",
			//				   new Vector2(width - 1, 1),
			//				   Vector2.One * (1f + Math.SinNormalized(Time.TotalSeconds * 2f)),
			//				   Colour.White,
			//				   HAlign.Right,
			//				   VAlign.Bottom);

			//Renderer.Draw.Text(SpriteFonts.Font,
			//				   "|:shadow=0,-1,0.01,0,0,0,0.5:|Test Test\nStill testing...\nhhhheeeelllloooo",
			//				   new Vector2(width - 1, height - 1),
			//				   Vector2.One * (1f + Math.SinNormalized(Time.TotalSeconds * 2f)),
			//				   Colour.White,
			//				   HAlign.Right,
			//				   VAlign.Top);

			//Renderer.Draw.Text(SpriteFonts.Font,
			//				   "|:outline=1,0.01,0,0,0,1:|Test Test Test Test Test Test Test Test Test\nStill testing... ... ...",
			//				   new Vector2(width / 2, height / 2),
			//				   Vector2.One * (1f + Math.SinNormalized(Time.TotalSeconds * 2f)),
			//				   Colour.White,
			//				   HAlign.Center,
			//				   VAlign.Middle);

			//Renderer.Draw.Text(SpriteFonts.Font,
			//				   "|:shadow=0,-1,0.01,0,0,0,0.5:|Test Test\nStill testing...",
			//				   new Vector2(width / 2, height / 2 + 50),
			//				   Vector2.One * (1f + Math.SinNormalized(Time.TotalSeconds * 2f) * 3f),
			//				   Colour.White,
			//				   HAlign.Center,
			//				   VAlign.Middle);

			//Renderer.Draw.Text(SpriteFonts.Font,
			//				   "|:shadow=0,-1,0.01,0,0,0,0.5:|Test Test\nStill testing...",
			//				   new Vector2(width / 2, height / 2 - 50),
			//				   Vector2.One * (1f + Math.SinNormalized(Time.TotalSeconds * 2f) * 2f),
			//				   Colour.White,
			//				   HAlign.Center,
			//				   VAlign.Middle);

			//Renderer.Draw.TriangleListStrip.Begin();
			////int ii = Random.Range(1000, 10000);
			//for (int i = 0; i < 10000; i++)
			//	Renderer.Draw.TriangleListStrip.AddVertex(new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), 0.0f),
			//										  new Colour(Random.Range(1f), Random.Range(1f), Random.Range(1f), Random.Range(1f)));
			//Renderer.Draw.TriangleListStrip.End();

			// TODO - LineListStrip
			//Renderer.Draw.LineListStrip.Begin();
			//for (int i = 0; i < 10; i++)
			//	Renderer.Draw.LineListStrip.AddVertex(new Vector3(Random.Range(-6f, -2f), Random.Range(-2f, 2f), 0.0f),
			//									  new Colour(Random.Range(1f), Random.Range(1f), Random.Range(1f), Random.Range(1f)));
			//Renderer.Draw.LineListStrip.End();

			//Renderer.Draw.Circle(new Vector2(0f, -8f), 10f, Colour.Green, Colour.Green.Alpha(0.1f), (int)Math.Lerp(8f, 64f, Math.SinNormalized(Time.TotalSeconds)));

			//for (int i = 0; i < 10000; i++)
			//	Renderer.Draw.PolyPoint(new Vector2(Random.Range(0, width), Random.Range(0, height)),
			//							new Colour(Random.Range(1f), Random.Range(1f), Random.Range(1f), 1f));

			//RenderTarget.Bind(null);
			//Renderer.Draw.RenderTarget(surface, 0,
			//						   -5f, 5f, 0.1f,
			//						   Colour.White);

			//Renderer.Draw.Text(SpriteFonts.Font,
			//				   debugString,
			//				   new Vector2(2, RenderTarget.Current.Height - 1),
			//				   Vector2.One,
			//				   Colour.White,
			//				   HAlign.Left,
			//				   VAlign.Top);

			// Draw Depth Buffer
			if (Input.Keyboard.Down(KeyCode.F1))
			{
				Graphics.State.Reset();
				Shaders.LinearizeDepth.Bind();
				Shaders.LinearizeDepth.MVP = surface.Matrix;
				Shaders.LinearizeDepth.zNear = Camera.zNear;
				Shaders.LinearizeDepth.zFar = Camera.zFar;
				Renderer.Draw.RenderTarget(surface, TextureSlot.Depth, 0, 0, Colour.White);

				Shaders.Renderer.Bind();
				Shaders.Renderer.MVP = RenderTarget.Default.Matrix;
			}

			// Draw to Screen
			RenderTarget.Default.Bind();
			Graphics.Clear(new Colour(0.1f, 0.1f, 0.1f, 1f));
			Graphics.State.Reset();

			scale = Math.Max(1, Math.Min(Window.Width / width, Window.Height / height));

			// Render RenderTarget
			Shaders.Renderer.Bind();
			Shaders.Renderer.MVP = RenderTarget.Default.Matrix;
			Renderer.Draw.RenderTarget(surface, TextureSlot.Colour0, (Window.Width - width * scale) / 2, (Window.Height - height * scale) / 2, scale, Colour.White);
			//Renderer.Draw.RenderTarget(surface, TextureSlot.Colour0, (Window.Width - width * scale) / 2, (Window.Height - height * scale) / 2, scale, Math.Sin(Time.TotalSeconds) * 0.03f, new Vector2(Window.Width / 2f, Window.Height / 2f), Colour.White);

			// Blit RenderTarget
			//surface.BlitTextureTo(RenderTarget.Default, TextureSlot.Colour0, BlitFilter.Nearest, new RectangleI((Window.Width - width * scale) / 2, (Window.Height - height * scale) / 2, width * scale, height * scale));

			Renderer.Draw.Text(SpriteFonts.Font,
							   debugString,
							   new Vector2(2, RenderTarget.Current.Height - 1),
							   Vector2.One * 3f,
							   Colour.White,
							   HAlign.Left,
							   VAlign.Top);

			Renderer.Flush();

			// Screenshot
			if (Input.Keyboard.Pressed(KeyCode.F9))
			{
				string s = "X:/Dropbox/Clint/Gamedev/2018-03-22 CKGL/screenshots/";
				if (!System.IO.Directory.Exists(s))
					s = "C:/Users/Clint Kilmer/Dropbox/Clint/Gamedev/2018-03-22 CKGL/screenshots/";

				int sequentialNumber = 1;
				while (System.IO.File.Exists($@"{s}{System.DateTime.Now:yyyy-MM-dd HH.mm.ss}-{sequentialNumber} [CKGL].png"))
				{
					sequentialNumber++;
				}

				surface.textures[0].SavePNG($@"{s}{System.DateTime.Now:yyyy-MM-dd HH.mm.ss}-{sequentialNumber} [CKGL].png");

				//System.GC.Collect();
			}
		}

		public override void Destroy()
		{
		}

		public override void OnFocusGained()
		{
		}

		public override void OnFocusLost()
		{
		}

		public override void OnWindowResized()
		{
			//surface.Destroy();
			//width = Window.Width;
			//height = Window.Height;
			//surface = new RenderTarget(width, height, 1, TextureFormat.RGB8, TextureFormat.Depth);

			Camera.AspectRatio = surface.AspectRatio;
			//Camera.AspectRatio = Window.AspectRatio;
		}
	}
}