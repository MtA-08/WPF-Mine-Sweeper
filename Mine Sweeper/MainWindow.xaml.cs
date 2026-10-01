using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Mine_Sweeper
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			Time.InitializeTimer();
			WriteReadData.InitializeAppDataFolder();
			InitializeComponent();

			GridData.GenerateGridDataAsync();
			DataContext = cellViewModel;
			GameGridControl.DataContext = GridData;
			RemainingMinesTextblock.DataContext = GridData;
			ResultsList.ItemsSource = WriteReadData.Results;

			GridSizeComboBox.SelectedIndex = 0;
		}

		public TimerService Time = new TimerService();
		public GridDataService GridData = new GridDataService();
		public CellViewModel cellViewModel = new CellViewModel();
		public WriteReadDataFileService WriteReadData = new WriteReadDataFileService();

		public bool RightClickOnCell { get; set; } = false;

		public void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if(e.ChangedButton == MouseButton.Right)
			{
				RightClickOnCell = true;
				GridButton_Click(sender, e);
				RightClickOnCell = false;
			}
		}
		public void GridButton_Click(object sender, RoutedEventArgs e)
		{
			var button = (Button)sender;
			var cell = (CellViewModel)button.DataContext;

			if (!GridData.TimerStarted)
			{
				Time.SecondsCounter = 0;
				TimerDisplay.Text = "0";
				Time.timer.Start();
				GridData.TimerStarted = true;
			}

			if (RightClickOnCell && cell.State == CellVisibleState.IsUnclicked)
			{
				if (GridData.RemainingMineCount != 0)
				{
					cell.State = CellVisibleState.IsFlagged;
					GridData.RemainingMineCount--;
				}
			}
			else if (RightClickOnCell && cell.State == CellVisibleState.IsFlagged)
			{
				cell.State = CellVisibleState.IsUnclicked;
				GridData.RemainingMineCount++;
			}
			else
			{
				GridData.RevealCell(cell.Row, cell.Col);
			}

			if (!GridData.IsLost && GridData.revealedCellCount == (GridData.RowCount * GridData.ColCount) - GridData.MineCount && GridData.RemainingMineCount == 0)
			{
				GridData.IsWon = true;
			}

			if (GridData.IsWon || GridData.IsLost)
			{ 
				Time.timer.Stop();
				var result = "";
				if (GridData.IsWon) { result = "Gewonnen"; } else if (GridData.IsLost) { result = "Verloren"; }
				WriteReadData.AddCurrentRunDataToFile(Time.SecondsCounter, result);
				WriteReadData.WriteFileDataToList();
			}

			UpdateConditionText();
		}

		private void NewGameButton_Click(object sender, RoutedEventArgs e)
		{
			Time.timer.Stop();
			TimerDisplay.Text = "0";
			GridData.GenerateGridDataAsync();
			GridData.TimerStarted = false;
			Time.SecondsCounter = 0;
			GridData.RemainingMineCount = GridData.MineCount;
			UpdateConditionText();
		}		
		
		public void UpdateConditionText()
		{
			if (GridData.IsLost)
			{
				ConditionText1.Text = "VERLOREN";
				ConditionText1.Foreground = Brushes.Red;

				ConditionText2.Text = "VERLOREN";
				ConditionText2.Foreground = Brushes.Red;
			}
			else if (GridData.IsWon)
			{
				ConditionText1.Text = "GEWONNEN";
				ConditionText1.Foreground = Brushes.Green;

				ConditionText2.Text = "GEWONNEN";
				ConditionText2.Foreground = Brushes.Green;
			}
			else
			{
				ConditionText1.Text = "";
				ConditionText2.Text = "";
			}
		}

		public RunDifficulty CurrentRunDifficulty { get; set; }
		public void GridSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			if (GridSizeComboBox.SelectedItem is ComboBoxItem selectedItem)
			{

				string selectedGridSize = selectedItem.Content.ToString();
				if (selectedGridSize == "Einfach")
				{
					GridData.RowCount = 10;
					GridData.ColCount = 10;
					GridData.MineCount = 12;
					GridData.RemainingMineCount = 12;
					CurrentRunDifficulty = RunDifficulty.Einfach;
				}
				else if (selectedGridSize == "Mittel")
				{
					GridData.RowCount = 16;
					GridData.ColCount = 16;
					GridData.MineCount = 40;
					GridData.RemainingMineCount = 40;
					CurrentRunDifficulty = RunDifficulty.Mittel;
				}
				else 
				{
					GridData.RowCount = 20;
					GridData.ColCount = 16;
					GridData.MineCount = 60;
					GridData.RemainingMineCount = 60;
					CurrentRunDifficulty = RunDifficulty.Schwer;
				}

				NewGameButton_Click(sender, e);
			}
		}

		public CustomPopupPlacement[] CenterPopup(Size popUpSize, Size targetSize, Point offset)
		{
			double x = (targetSize.Width - popUpSize.Width) / 2;
			double y = (targetSize.Height - popUpSize.Height) / 2;

			var centerPoint = new Point(x, y);

			var placement = new CustomPopupPlacement(centerPoint, PopupPrimaryAxis.None);

			return new[] { placement };
		}

		private void OpenPopup_Click(object sender, RoutedEventArgs e)
		{
			ResultsPopUp.IsOpen = true;
		}

		private void ClosePopUp_Click(object sender, RoutedEventArgs e)
		{
			ResultsPopUp.IsOpen = false;
		}

		private void ClearResults_Click(object sender, RoutedEventArgs e)
		{
			var messageBoxText = "Alle Ergebnisse wirklich löschen?";
			var caption = "Bestätigung erforderlich";
			var button = MessageBoxButton.YesNo;
			var icon = MessageBoxImage.Question;

			var result = MessageBox.Show(messageBoxText, caption, button, icon);

			if (result == MessageBoxResult.Yes)
			{
				WriteReadData.Results.Clear();
				WriteReadData.WriteListDataToFile();
			}
		}
	}	
}