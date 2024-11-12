using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XInputium;
using XInputium.XInput;
using System.Diagnostics;
using WebSocketSharp;
using System.Runtime.InteropServices;
// I dont think I need all of these but it doesnt matter



namespace Personal_Driver_Station
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//defining the gamepad and websockets
		//if you reading this dev I didnt end up switching I really didnt wanna rewrite the whole thing
		//TODO - get the battery voltage from the robot
		//TODO - make a ui (learning xaml 🤮🤮🤮 (thats the vomit emoji))
		//TODO - get the esp32 to arduino mega serial working (ive been trying for like 2 weeks it might be over)
		private readonly XGamepad gamepad;
		private WebSocket ws;

		public MainWindow()
		{
			//Im gonna be real I dont actually remember what initializecomponent does but I added it a long time ago and it doesnt work if I take it out
			InitializeComponent();
			gamepad = new();
			XInputDevice device = new(XInputUserIndex.One);

			Debug.WriteLine("Starting");

			//
			Thread gamepadThread = new(CheckIsconnected);
			gamepadThread.Start();

		}

		private void Controller()
		{
			while (true)
			{
				Controlling inp = new()
				{
					leftStickY = gamepad.LeftJoystick.Y,
					rightStickY = gamepad.RightJoystick.Y,
					rightTrigger = gamepad.RightTrigger.Value,
					leftTrigger = gamepad.LeftTrigger.Value
				};

				byte[] data = ByteConversion(inp);
				SendData(data);
				gamepad.Update();
			}
		}

		private void SendData(byte[] data)
		{
			//Right now I have to make due with the websocket connecting and disconnecting like 20 times a second because
			//idk how to have it send a bunch of stuff without closing. it works fine though
			ws = new WebSocket("ws://192.168.4.1:80");
			ws.OnMessage += (sender, e) => Debug.WriteLine("WebSocket Message: " + e.Data);
			ws.OnOpen += (sender, e) => Debug.WriteLine("WebSocket connected.");
			ws.OnClose += (sender, e) => Debug.WriteLine("WebSocket disconnected.");
			ws.Connect();
			ws.Send(data);
		}

		private void CheckIsconnected()
		{
			while (true)
			{

				Thread.Sleep(100); // essentially just like delay() in arduino
				XInputDeviceState state = XInputDevice.GetState(XInputUserIndex.One);

				if (state.IsConnected)
				{
					gamepad.Update(); 
					Controller(); 
				}
				else
				{
					Debug.WriteLine("The device is not connected.");
				}
			}
		}

		public static byte[] ByteConversion(Controlling inp)
		{
			//I copy pasted this off of something online but it works
			int size = Marshal.SizeOf(inp);
			byte[] arr = new byte[size];
			IntPtr ptr = Marshal.AllocHGlobal(size);

			//gang wtf is try and finally
			try
			{
				Marshal.StructureToPtr(inp, ptr, true);
				Marshal.Copy(ptr, arr, 0, size);
			}
			finally
			{
				Marshal.FreeHGlobal(ptr);
			}

			return arr;
		}

		public struct Controlling
		{
			//basically just a struct with all the things I thought I needed. ill have to figure out how to include buttons later
			public double leftStickY;
			public double rightStickY;
			public double rightTrigger;
			public double leftTrigger;
		}

		//apparently commenting out code is a bad practice and Im not gonna use it again
		//but it has too much sentimental value

		/**public static void Websockets()
		{

			XGamepad gamepad = new();
			while (true)
			{
				// Get the left and right stick axes (X and Y).
				double leftStickY = gamepad.LeftJoystick.Y;
				// Print the axis values.
				gamepad.Update();


				using (var ws = new WebSocket("ws://192.168.4.1:80"))
				{
					ws.OnMessage += (sender, e) =>
									  Debug.WriteLine("We are connected");
					int x = 4;
					ws.Connect();
					ws.Send(leftStickY.ToString());
					Debug.WriteLine(leftStickY);
				}
				Thread.Sleep(100);
			}

			

		}**/


	}
}