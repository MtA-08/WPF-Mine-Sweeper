using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Mine_Sweeper
{
	public enum CellVisibleState
		{
			IsUnclicked,
			ClickedHasNoNeighbouringMines,
			ClickedHasNeighbouringMines,
			IsFlagged,
			HasMineClicked
		}
	public class CellViewModel : INotifyPropertyChanged
	{
		private CellVisibleState _state = new CellVisibleState();
		public CellVisibleState State
		{
			get => _state;
			set
			{
				if (_state != value)
				{
					_state = value;

					OnPropertyChanged();
				}
			}
		}
		public int Row { get; set; }
		public int Col { get; set; }
		public bool IsMine { get; set; }
		public int NeighbouringMines { get; set; }

		

		public class ButtonTypeSelector : DataTemplateSelector
		{
			public DataTemplate IsUnclcikedTemplate { get; set; }
			public DataTemplate HasNoNeighbouringMinesTemplate { get; set; }
			public DataTemplate HasNeighbouringMinesTemplate { get; set; }
			public DataTemplate IsFlaggedTemplate { get; set; }
			public DataTemplate hasMineClickedTemplate { get; set; }

			public override DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				if (item is CellViewModel cell)
				{
					switch (cell.State)
					{
						case CellVisibleState.ClickedHasNoNeighbouringMines:
							return HasNoNeighbouringMinesTemplate;

						case CellVisibleState.ClickedHasNeighbouringMines:
							return HasNeighbouringMinesTemplate;

						case CellVisibleState.IsFlagged:
							return IsFlaggedTemplate;

						case CellVisibleState.HasMineClicked:
							return hasMineClickedTemplate;

						default:
							return IsUnclcikedTemplate;
					}
				}

				return base.SelectTemplate(item, container);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
