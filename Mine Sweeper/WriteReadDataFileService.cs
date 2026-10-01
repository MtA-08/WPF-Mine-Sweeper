using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;

namespace Mine_Sweeper
{
    public class WriteReadDataFileService : INotifyPropertyChanged
    {
		public TimerService Time = new TimerService();
		public GridDataService GridData = new GridDataService();

		public string ResultsFilePath { get; set; }
        public void InitializeAppDataFolder()
        {
			string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string AppFolderPath = Path.Combine(LocalAppData, "Mine Sweeper");
			ResultsFilePath = Path.Combine(AppFolderPath, "results.json");
            
            if(!Directory.Exists(AppFolderPath))
            {
                Directory.CreateDirectory(AppFolderPath);
            }

            if (!File.Exists(ResultsFilePath))
            {
                File.WriteAllText(ResultsFilePath,"[]");
            }
            else
            { 
                WriteFileDataToList();
            }
            
		}
        public ObservableCollection<RunData> Results = new ObservableCollection<RunData>();
		
        public async void WriteFileDataToList()
        {
            var loadedResults = JsonSerializer.Deserialize<List<RunData>>(File.ReadAllText(ResultsFilePath)) ?? new List<RunData>();
            
            Results.Clear();

            foreach (var runData in loadedResults)
            {
                Results.Add(runData);
            }
        }

        public void WriteListDataToFile()
        {
            File.WriteAllText(ResultsFilePath, JsonSerializer.Serialize(Results));
        }

        public void AddCurrentRunDataToFile(int elapsedTime, string result)
        {
			var mainWindow = Application.Current.MainWindow as MainWindow;
			
			var currentRunData = new RunData
			{
				RunId = new RunData().GenerateNewId(Results),
				RunCompletonTime = elapsedTime,
				RunResult = result,
				RunDifficulty = mainWindow.CurrentRunDifficulty,
				RunDate = DateTime.Now.ToShortTimeString()
			};
			Results.Add(currentRunData);
			WriteListDataToFile();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

    public class RunData
    { 
        public int RunId {get ;set;}
        public int RunCompletonTime {get ;set;}
        public string RunResult {get ;set;}
        public RunDifficulty RunDifficulty {get ;set;}
        public String RunDate {get ;set;}

		public int GenerateNewId(ObservableCollection<RunData> results)
        {
            int newId = 1;
            if (results.Any())
            {
                newId = results.Max(id => id.RunId) + 1;
            }
            return newId;
        }
    }

    public enum RunDifficulty {Einfach, Mittel, Schwer}	
}
