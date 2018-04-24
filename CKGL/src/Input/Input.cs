﻿using System.Collections.Generic;

namespace CKGL
{
	public static class Input
	{
		public static void Init()
		{
			Keyboard.Init();
			Mouse.Init();
		}

		public static void Clear()
		{
			Keyboard.Clear();
			Mouse.Clear();
		}

		public static void Update()
		{
			//Keyboard.Update();
			Mouse.Update();
		}

		public static class Keyboard
		{
			public const int ScanCodeMask = Platform.ScanCodeMask;

			private static HashSet<KeyCode> downKeyCode = new HashSet<KeyCode>();
			private static HashSet<KeyCode> pressedKeyCode = new HashSet<KeyCode>();
			private static HashSet<KeyCode> releasedKeyCode = new HashSet<KeyCode>();
			private static HashSet<KeyCode> repeatedKeyCode = new HashSet<KeyCode>();

			private static HashSet<ScanCode> downScanCode = new HashSet<ScanCode>();
			private static HashSet<ScanCode> pressedScanCode = new HashSet<ScanCode>();
			private static HashSet<ScanCode> releasedScanCode = new HashSet<ScanCode>();
			private static HashSet<ScanCode> repeatedScanCode = new HashSet<ScanCode>();

			public static void Init()
			{
				Platform.Events.OnKeyDown += (keycode, scancode, repeated) =>
				{
					downKeyCode.Add(keycode);
					downScanCode.Add(scancode);

					if (repeated)
					{
						repeatedKeyCode.Add(keycode);
						repeatedScanCode.Add(scancode);

						//Output.WriteLine("repeated: " + keycode);
					}
					else
					{
						pressedKeyCode.Add(keycode);
						pressedScanCode.Add(scancode);

						//Output.WriteLine("pressed:  " + keycode);
					}
				};

				Platform.Events.OnKeyUp += (keycode, scancode, repeated) =>
				{
					downKeyCode.Remove(keycode);
					releasedKeyCode.Add(keycode);
					repeatedKeyCode.Add(keycode);

					downScanCode.Remove(scancode);
					releasedScanCode.Add(scancode);
					repeatedScanCode.Add(scancode);

					//Output.WriteLine("released: " + keycode);
				};
			}

			public static void Clear()
			{
				//Output.WriteLine("down:     " + string.Join(", ", downKeyCode));

				pressedKeyCode.Clear();
				releasedKeyCode.Clear();
				repeatedKeyCode.Clear();

				pressedScanCode.Clear();
				releasedScanCode.Clear();
				repeatedScanCode.Clear();
			}

			public static bool Down(KeyCode key)
			{
				return downKeyCode.Contains(key);
			}
			public static bool Down(ScanCode key)
			{
				return downScanCode.Contains(key);
			}

			public static bool Pressed(KeyCode key)
			{
				return pressedKeyCode.Contains(key);
			}
			public static bool Pressed(ScanCode key)
			{
				return pressedScanCode.Contains(key);
			}

			public static bool Released(KeyCode key)
			{
				return releasedKeyCode.Contains(key);
			}
			public static bool Released(ScanCode key)
			{
				return releasedScanCode.Contains(key);
			}

			public static bool Repeated(KeyCode key)
			{
				return repeatedKeyCode.Contains(key);
			}
			public static bool Repeated(ScanCode key)
			{
				return repeatedScanCode.Contains(key);
			}

			public static bool PressedOrRepeated(KeyCode key)
			{
				return Pressed(key) || Repeated(key);
			}
			public static bool PressedOrRepeated(ScanCode key)
			{
				return Pressed(key) || Repeated(key);
			}
		}

		public static class Mouse
		{
			public static Point2 LastPosition { get; set; }
			public static Point2 Position { get; private set; }
			public static Point2 LastPositionDisplay { get; private set; }
			public static Point2 PositionDisplay { get; private set; }
			public static Point2 PositionRelative { get; private set; }
			public static Point2 Scroll { get; private set; }

			private static bool[] down = new bool[16];
			private static bool[] pressed = new bool[16];
			private static bool[] released = new bool[16];

			public static void Init()
			{
				Platform.Events.OnMouseButtonDown += (id) =>
				{
					down[id] = true;
					pressed[id] = true;
				};

				Platform.Events.OnMouseButtonUp += (id) =>
				{
					down[id] = false;
					released[id] = true;
				};

				Platform.Events.OnMouseScroll += (x, y) =>
				{
					Scroll = new Point2(x, y);
				};
			}

			public static void Clear()
			{
				Scroll = Point2.Zero;

				for (int i = 0; i < 16; i++)
				{
					pressed[i] = false;
					released[i] = false;
				}
			}

			public static void Update()
			{
				LastPositionDisplay = PositionDisplay;
				Platform.GetGlobalMousePosition(out int mouseDisplayX, out int mouseDisplayY);
				PositionDisplay = new Point2(mouseDisplayX, mouseDisplayY);

				LastPosition = Position;
				Platform.GetMousePosition(out int mouseX, out int mouseY);
				Position = new Point2(mouseX, mouseY);

				Platform.GetRelativeMousePosition(out int mouseRelativeX, out int mmouseDisplayY);
				PositionRelative = new Point2(mouseRelativeX, mmouseDisplayY);
			}

			public static bool Down(MouseButton button)
			{
				return down[(int)button];
			}

			public static bool Pressed(MouseButton button)
			{
				return pressed[(int)button];
			}

			public static bool Released(MouseButton button)
			{
				return released[(int)button];
			}

			public static bool LeftDown
			{
				get { return Down(MouseButton.Left); }
			}

			public static bool LeftPressed
			{
				get { return Pressed(MouseButton.Left); }
			}

			public static bool LeftReleased
			{
				get { return Released(MouseButton.Left); }
			}

			public static bool RightDown
			{
				get { return Down(MouseButton.Right); }
			}

			public static bool RightPressed
			{
				get { return Pressed(MouseButton.Right); }
			}

			public static bool RightReleased
			{
				get { return Released(MouseButton.Right); }
			}

			public static bool MiddleDown
			{
				get { return Down(MouseButton.Middle); }
			}

			public static bool MiddlePressed
			{
				get { return Pressed(MouseButton.Middle); }
			}

			public static bool MiddleReleased
			{
				get { return Released(MouseButton.Middle); }
			}

			public static int X
			{
				get { return Position.X; }
			}

			public static int Y
			{
				get { return Position.Y; }
			}

			public static int ScrollX
			{
				get { return Scroll.X; }
			}

			public static int ScrollY
			{
				get { return Scroll.Y; }
			}
		}
	}

	public enum MouseButton
	{
		Left = 1,
		Middle = 2,
		Right = 3,
		X1 = 4,
		X2 = 5
	}
}