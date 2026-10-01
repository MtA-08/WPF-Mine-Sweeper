using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mine_Sweeper
{
	public class GridDataService : INotifyPropertyChanged
	{
		public TimerService Time = new TimerService();

		private List<List<CellViewModel>> _gridData = new List<List<CellViewModel>>();
		public List<List<CellViewModel>> GridData { get => _gridData; set { _gridData = value; OnPropertyChanged(); } }
		public class MinePosition { public int row; public int col; };
		public List<MinePosition> MinePositions { get; set; } = new List<MinePosition>();
		public CellVisibleState CellVisibleState { get; private set; } = new CellVisibleState();
		public int MineCount { get; set; } = 10;
		private int _rowCount = 9;
		public int RowCount { get => _rowCount; set { _rowCount = value; OnPropertyChanged(); } }
		private int _colCount = 9;
		public int ColCount { get => _colCount; set { _colCount = value; OnPropertyChanged(); } }
		private int _remainingMineCount = 10;
		public int RemainingMineCount { get => _remainingMineCount; set { _remainingMineCount = value; OnPropertyChanged(); } }
		private bool _isLost = false;
		public bool IsLost { get => _isLost; set { _isLost = value; OnPropertyChanged(); } }
		private bool _isWon = false;
		public bool IsWon { get => _isWon; set { _isWon = value; OnPropertyChanged(); } }
		public int revealedCellCount = 0;
		private readonly Random _random = new Random();
		public bool TimerStarted { get; set; } = false;

		public void GenerateGridDataAsync()
		{
			revealedCellCount = 0;
			IsLost = false;
			IsWon = false;
			GridData.Clear();
			var newGridData = new List<List<CellViewModel>>();
			var isMine = false;
			GenerateMinePositionsAsync(RowCount, ColCount);

			for (int r = 0; r < RowCount; r++)
			{
				var rowList = new List<CellViewModel>();

				for (int c = 0; c < ColCount; c++)
				{
					foreach (var position in MinePositions)
					{
						if (position.row == r & position.col == c)
						{
							isMine = true;
							break;
						}
					}
					rowList.Add(new CellViewModel
					{
						State = CellVisibleState,
						Row = r,
						Col = c,
						IsMine = isMine
					});
					isMine = false;
				}
				newGridData.Add(rowList);
			}
			GridData = newGridData;
			SetNumberOfNeighbouringMinesForEachCell();
		}

		public void GenerateMinePositionsAsync(int rowCount, int colCount)
		{
			MinePosition newPos;
			MinePositions.Clear();

			while (MinePositions.Count < MineCount)
			{
				var x = _random.Next(0, rowCount);
				var y = _random.Next(0, colCount);
				newPos = new MinePosition { row = x, col = y };

				var exists = MinePositions.Any(p => p.col == newPos.col && p.row == newPos.row);

				if (!exists)
					MinePositions.Add(newPos);
			}
		}

		public void SetNumberOfNeighbouringMinesForEachCell()
		{
			var rows = RowCount;
			var columns = ColCount;

			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < columns; c++)
				{
					var adjacentMineCount = CountNeighbouringMines(r, c, rows, columns);
					GridData[r][c].NeighbouringMines = adjacentMineCount;
				}
			}
		}

		public int CountNeighbouringMines(int r, int c, int rowCount, int colCount)
		{
			var mineCount = 0;

			var topLeftRow = r - 1;
			var topLeftCol = c - 1;

			var leftRow = r;
			var leftCol = c - 1;

			var bottomLeftRow = r + 1;
			var bottomLeftCol = c - 1;

			var topRow = r - 1;
			var topCol = c;

			var bottomRow = r + 1;
			var bottomCol = c;

			var topRightRow = r - 1;
			var topRightCol = c + 1;

			var rightRow = r;
			var rightCol = c + 1;

			var bottomRightRow = r + 1;
			var bottomRightCol = c + 1;


			if (topLeftRow >= 0 && topLeftRow < rowCount && topLeftCol >= 0 && topLeftCol < colCount && GridData[topLeftRow][topLeftCol].IsMine)
			{
				mineCount++;
			}

			if (leftRow >= 0 && leftRow < rowCount && leftCol >= 0 && leftCol < colCount && GridData[leftRow][leftCol].IsMine)
			{
				mineCount++;
			}

			if (bottomLeftRow >= 0 && bottomLeftRow < rowCount && bottomLeftCol >= 0 && bottomLeftCol < colCount && GridData[bottomLeftRow][bottomLeftCol].IsMine)
			{
				mineCount++;
			}

			if (topRow >= 0 && topRow < rowCount && topCol >= 0 && topCol < colCount && GridData[topRow][topCol].IsMine)
			{
				mineCount++;
			}

			if (bottomRow >= 0 && bottomRow < rowCount && bottomCol >= 0 && bottomCol < colCount && GridData[bottomRow][bottomCol].IsMine)
			{
				mineCount++;
			}

			if (topRightRow >= 0 && topRightRow < rowCount && topRightCol >= 0 && topRightCol < colCount && GridData[topRightRow][topRightCol].IsMine)
			{
				mineCount++;
			}

			if (rightRow >= 0 && rightRow < rowCount && rightCol >= 0 && rightCol < colCount && GridData[rightRow][rightCol].IsMine)
			{
				mineCount++;
			}

			if (bottomRightRow >= 0 && bottomRightRow < rowCount && bottomRightCol >= 0 && bottomRightCol < colCount && GridData[bottomRightRow][bottomRightCol].IsMine)
			{
				mineCount++;
			}

			return mineCount;
		}

		public void RevealCell(int row, int col)
		{
			Time.InitializeTimer();

			if (row < 0 || row >= GridData.Count || col < 0 || col >= GridData[row].Count)
			{
				return;
			}

			var cell = GridData[row][col];

			if (cell.State != CellVisibleState.IsUnclicked)
			{
				return;
			}

			if (cell.IsMine)
			{
				cell.State = CellVisibleState.HasMineClicked;
				RevealAllCells();
				IsLost = true;
				return;
			}

			if (cell.NeighbouringMines > 0)
			{
				cell.State = CellVisibleState.ClickedHasNeighbouringMines;
				revealedCellCount++;
				return;
			}

			cell.State = CellVisibleState.ClickedHasNoNeighbouringMines;
			revealedCellCount++;

			for (var rowOffset = -1; rowOffset <= 1; rowOffset++)
			{
				for (var colOffset = -1; colOffset <= 1; colOffset++)
				{
					if (rowOffset == 0 && colOffset == 0)
					{
						continue;
					}
					RevealCell(row + rowOffset, col + colOffset);
				}
			}
		}

		public void RevealAllCells()
		{
			for (int r = 0; r < RowCount; r++)
			{
				for (int c = 0; c < ColCount; c++)
				{
					var cell = GridData[r][c];
					if (cell.IsMine)
					{
						cell.State = CellVisibleState.HasMineClicked;
					}
					else if (cell.State == CellVisibleState.IsUnclicked)
					{
						if (cell.NeighbouringMines > 0)
						{
							cell.State = CellVisibleState.ClickedHasNeighbouringMines;
						}
						else
						{
							cell.State = CellVisibleState.ClickedHasNoNeighbouringMines;
						}
					}
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
