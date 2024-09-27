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



namespace Personal_Driver_Station
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private readonly XGamepad gamepad;

		public MainWindow()
		{
			InitializeComponent();
			gamepad = new();
			XInputDevice device = new(XInputUserIndex.One);
			Websockets();

			while (true)
			{
				XInputDeviceState state = XInputDevice.GetState(XInputUserIndex.One);

				if (state.IsConnected)
				{
					//Debug.WriteLine($"The device is connected. Left joystick: {state.LeftJoystick}");

					//Debug.WriteLine(state.LeftJoystick);

					device.SetMotorsSpeed(0.0f, 0.0f);
					Websockets();

					//gamepad.Update();
					//Gamepad_InputReceived();
				}
				else
				{
					//Debug.WriteLine("The device is not connected.");
				}
			}
		;
		}

		/** private static void Gamepad_InputReceived()
		{
			// Access the gamepad axis (left stick and right stick).
			XGamepad gamepad = new();
			while (true)
			{
				// Get the left and right stick axes (X and Y).
				double leftStickY = gamepad.LeftJoystick.Y;

				// Print the axis values.
				Debug.WriteLine($"I love skibidi toilet {leftStickY}");

				gamepad.Update();
			}
		}
		**/

		public static void Websockets()
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
				}
			}

			

		}


	}
}

