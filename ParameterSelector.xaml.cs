using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WaveShare_Robot_Arm_Controller
{
	/// <summary>
	/// Interaction logic for ParameterSelector.xaml
	/// </summary>
	public partial class ParameterSelector : UserControl
	{
		private List<(string Name, Type Type)> _origParameters;

		private int _brightness;
		private int _joint;
		private double _degrees;
		private double _speed;
		private double _accel;
		private double _torque;
		private int _miliseconds;

		private static readonly Regex _regex = new Regex(@"^-?\d*\.?\d*$");

		public ParameterSelector()
		{
			InitializeComponent();
		}

		public int NewBrightness { get => _brightness; }
		public double NewDegrees { get => _degrees; }
		public int NewJoint { get => _joint; }
		public double NewSpeed { get => _speed; }
		public double NewAcceleration { get => _accel; }
		public double NewTorque { get => _torque; }
		public int NewMiliseconds { get => _miliseconds; }

		public void DisplayParameterInputs(List<(string Name, Type Type)> parameters)
		{
			CollapseAllColumns();
			HideAllControls();
			
			_origParameters = parameters;

			if (parameters != null)
			{
				parameters.ForEach(p =>
				{
					if (p.Name == "brightness")
					{
						BrightnessControl.Visibility = Visibility.Visible;

						ContainerGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
					}
					else if (p.Name == "joint")
					{
						JointControl.Visibility = Visibility.Visible;

						ContainerGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
					}
					else if (p.Name == "degrees")
					{
						DegreesControl.Visibility = Visibility.Visible;

						ContainerGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
					}
					else if (p.Name == "speed")
					{
						SpeedControl.Visibility = Visibility.Visible;

						ContainerGrid.ColumnDefinitions[3].Width = new GridLength(1, GridUnitType.Star);
					}
					else if (p.Name == "acceleration")
					{
						AccelControl.Visibility = Visibility.Visible;

						ContainerGrid.ColumnDefinitions[4].Width = new GridLength(1, GridUnitType.Star);
					}
					else if (p.Name == "torque")
					{
						TorqueControl.Visibility = Visibility.Visible;

						ContainerGrid.ColumnDefinitions[5].Width = new GridLength(1, GridUnitType.Star);
					}
					else if (p.Name == "miliseconds")
					{
						MilisecondsControl.Visibility = Visibility.Visible;

						ContainerGrid.ColumnDefinitions[6].Width = new GridLength(1, GridUnitType.Star);
					}
				});
			}
		}

		public void Reset()
		{
			CollapseAllColumns();
			HideAllControls();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			
		}

		//force textbox to only accept number in floating point format 0.000000000
		private void DegreeInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed((sender as TextBox).Text, e.Text);
		}

		private void DegreeInput_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true; // Prevent spaces
			}
		}

		private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(string)))
			{
				string pastedText = (string)e.DataObject.GetData(typeof(string));
				if (!IsTextAllowed((sender as TextBox).Text, pastedText))
				{
					e.CancelCommand();
				}
			}
			else
			{
				e.CancelCommand();
			}
		}

		private bool IsTextAllowed(string currentText, string newText)
		{
			string updatedText = currentText + newText;
			return _regex.IsMatch(updatedText);
		}

		private void HideAllControls()
		{
			BrightnessControl.Visibility = Visibility.Hidden;
			JointControl.Visibility = Visibility.Hidden;
			DegreesControl.Visibility = Visibility.Hidden;
			SpeedControl.Visibility = Visibility.Hidden;
			AccelControl.Visibility = Visibility.Hidden;
			TorqueControl.Visibility = Visibility.Hidden;
			MilisecondsControl.Visibility = Visibility.Hidden;
		}

		private void CollapseAllColumns()
		{
			ContainerGrid.ColumnDefinitions.ToList().ForEach(def =>
			{
				def.Width = new GridLength(0);
			});
		}

		private void GetSelectedRadioButtonValue()
		{
			foreach (var child in JointControl.Children)
			{
				if (child is RadioButton radioButton && radioButton.IsChecked == true)
				{
					_joint = Int32.Parse(radioButton.Content.ToString());
				}
			}
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try
			{
				_origParameters.ForEach(p =>
				{
					if (p.Name == "brightness")
					{
						_brightness = (int)e.NewValue;
					}
				});
			}
			catch (Exception)
			{
			}
		}

		private void RadioButton_Click(object sender, RoutedEventArgs e)
		{
			GetSelectedRadioButtonValue();
		}

		private void DegreeInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			_degrees = Double.Parse(DegreeInput.Text);
		}

		private void SpeedInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			_speed = Double.Parse(SpeedInput.Text);
		}

		private void AccelInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			_accel = Double.Parse(AccelInput.Text);
		}

		private void TorqueInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			_torque = Double.Parse(TorqueInput.Text);
		}

		private void MSInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			_miliseconds = Int32.Parse(MSInput.Text);
		}
	}
}
