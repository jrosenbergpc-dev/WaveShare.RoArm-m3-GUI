using System.Net;
using System.Net.Http;
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
using Waveshare.RoArm_m3;

namespace WaveShare_Robot_Arm_Controller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private HttpClient _client;

		public MainWindow()
        {
			HttpClientHandler handler = new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.All
			};

			_client = new HttpClient();

			InitializeComponent();
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			CommandBox.ItemsSource = CommandDefinitions.GetAvailableCommands().ToList();
			CommandBox.DisplayMemberPath = "Key";
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			foreach (string jsoncmd in json_box.Text.Split(Environment.NewLine))
			{
				string result = await GetAsync($"http://{ipfield.Text}/js?json={jsoncmd}");
				await Task.Delay(4000);
			}		
		}

		public async Task<string> GetAsync(string uri)
		{
			using HttpResponseMessage response = await _client.GetAsync(uri);

			return await response.Content.ReadAsStringAsync();
		}

		private void AddCmdBtn_Click(object sender, RoutedEventArgs e)
		{
			JsonCommand result = null;

			if (CommandBox.SelectedItem is KeyValuePair<string, bool> selectedCommand)
			{
				string methodName = selectedCommand.Key;
				bool hasParameters = selectedCommand.Value;

				if (hasParameters)
				{
					var xParams = CommandDefinitions.GetCommandParameters(methodName);

					object[] paramValues = xParams.Select(p => GetUserInputForType(p.Name)).ToArray();

					result = CommandDefinitions.GetJsonCommandByName(methodName, paramValues);

					if (json_box.Text != string.Empty)
					{
						json_box.AppendText(Environment.NewLine);
					}

					json_box.AppendText(result.RawOutput);
				}
				else
				{
					JsonCommand x = CommandDefinitions.GetJsonCommandByName(methodName);

					if (json_box.Text != string.Empty)
					{
						json_box.AppendText(Environment.NewLine);
					}

					json_box.AppendText(x.RawOutput);
				}
			}
		}

		private object GetUserInputForType(String paramName)
		{
			if (paramName == "joint") return PSelector.NewJoint; // change to gui pass through
			if (paramName == "degrees") return PSelector.NewDegrees; // change to gui pass through
			if (paramName == "speed") return PSelector.NewSpeed; // change to gui pass through
			if (paramName == "acceleration") return PSelector.NewAcceleration; // change to gui pass through
			if (paramName == "torque") return PSelector.NewTorque; // change to gui pass through
			if (paramName == "miliseconds") return PSelector.NewMiliseconds; // change to gui pass through
			if (paramName == "brightness") return PSelector.NewBrightness; // change to gui pass through
			return null;
		}

		private void CommandBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (CommandBox.SelectedItem is KeyValuePair<string, bool> selectedCommand)
			{
				string methodName = selectedCommand.Key;
				bool hasParameters = selectedCommand.Value;

				if (hasParameters)
				{
					var xParams = CommandDefinitions.GetCommandParameters(methodName);
					PSelector.DisplayParameterInputs(xParams);
				}
				else
				{
					PSelector.Reset();
				}
			}
		}

		private void ClearAllButton_Click(object sender, RoutedEventArgs e)
		{
			json_box.Text = string.Empty;
		}

		private async void EmergencyStopButton_Click(object sender, RoutedEventArgs e)
		{
			string result = await GetAsync($"http://{ipfield.Text}/js?json={CommandDefinitions.EmergencyStop().RawOutput}");
			MessageBox.Show("Sent Emergency Stop Command!!");
		}
	}
}