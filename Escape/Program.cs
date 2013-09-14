using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Reflection;

namespace Escape
{
	class Program
	{
		#region Declarations
		private const int Width = 73;
		private const int Height = 30;
		
		private const string saveFile = "save.dat";
		
		public enum GameStates { Start, Playing, Battle, Quit, GameOver };
		public static GameStates GameState = GameStates.Start;
		private static bool run = true;
		
		private static bool isError = false;
		private static List<string> errors = new List<string>();
		
		private static bool isNotification = false;
		private static List<string> notifications = new List<string>();
		
		public static Random Rand = new Random();
		#endregion
		
		#region Main
		public static void Main(string[] args)
		{
			Console.WindowWidth = Width;
			Console.WindowHeight = Height;

			Console.BufferWidth = Width;
			Console.BufferHeight = Height;

			World.Initialize();
			
			while(run)
			{
				if (!isError)
				{
					if (Player.Health <= 0)
					{
						GameState = GameStates.GameOver;
					}
					
					switch (GameState)
					{
						case GameStates.Start:
							StartState();
							break;
							
						case GameStates.Playing:
							PlayingState();
							break;
						
						case GameStates.Battle:
							BattleState();
							break;
							
						case GameStates.Quit:
							QuitState();
							break;
							
						case GameStates.GameOver:
							GameOverState();
							break;
					}
				}
				else
				{
					DisplayError();
				}
			}
		}
		#endregion
		
		#region GameState Methods
		private static void StartState()
		{
			Text.WriteLine("Hello adventurer! What is your name?");
			Player.Name = Text.SetPrompt("> ");
			Text.Clear();
			GameState = GameStates.Playing;
		}
		
		private static void PlayingState()
		{
			if (isNotification)
			{
				DisplayNotification();
			}
			
			World.LocationHUD();
			
			string temp = Text.SetPrompt("[" + Player.Location + "] > ");
			Text.Clear();
			Player.Do(temp);
		}
		
		private static void BattleState()
		{
			if (isNotification)
			{
				DisplayNotification();
			}
			
			BattleCore.BattleHUD();
		}
		
		private static void QuitState()
		{
			Console.Clear();
			Text.WriteColor("`r`/-----------------------------------------------------------------------\\", false);
			Text.WriteColor("|`w`                 Are you sure you want to quit? (y/n)                  `r`|", false);
			Text.WriteColor("\\-----------------------------------------------------------------------/`w`", false);
			
			ConsoleKeyInfo quitKey = Console.ReadKey();
			
			if (quitKey.KeyChar == 'y')
			{
				run = false;
			}
			else
			{
				Text.Clear();
				GameState = GameStates.Playing;
			}
		}
		
		private static void GameOverState()
		{
			Console.Clear();
			Text.WriteColor("`r`/-----------------------------------------------------------------------\\", false);
			Text.WriteColor("|`w`                              Game Over!                               `r`|", false);
			Text.WriteColor("|`w`                           Try again? (y/n)                            `r`|", false);
			Text.WriteColor("\\-----------------------------------------------------------------------/`w`", false);
			
			ConsoleKeyInfo quitKey = Console.ReadKey();
			
			if (quitKey.KeyChar == 'y')
			{
				Process.Start(Assembly.GetExecutingAssembly().Location);
				run = false;
			}
			else
			{
				run = false;
			}
		}
		#endregion
		
		#region Notification Handling
		private static void DisplayNotification()
		{
			Console.CursorTop = Console.WindowHeight - 1;
			Text.WriteColor("`g`/-----------------------------------------------------------------------\\", false);
			
			foreach (string notification in notifications)
			{	
				List<string> notificationLines = Text.Limit(string.Format("`g`Alert: `w`" + notification), Console.WindowWidth - 4);
								
				foreach (string line in notificationLines)
				{
					Text.WriteColor("| `w`" + line + Text.BlankSpaces(Console.WindowWidth - Regex.Replace(line, @"`.`", "").Length - 4, true) + "`g` |", false);
				}
			}
			
			Text.Write("\\-----------------------------------------------------------------------/");
			
			Console.SetCursorPosition(0, 0);
			UnsetNotification();
		}
		
		public static void SetNotification(string message)
		{
			notifications.Add(message);
			isNotification = true;
		}
		
		private static void UnsetNotification()
		{
			notifications.Clear();
			isNotification = false;
		}
		#endregion
		
		#region Error Handling
		private static void DisplayError()
		{
			Console.CursorTop = Console.WindowHeight - 1;
			Text.WriteColor("`r`/-----------------------------------------------------------------------\\", false);
			
			foreach (string error in errors)
			{	
				List<string> errorLines = Text.Limit(string.Format("`r`Error: `w`" + error), Console.WindowWidth - 4);
								
				foreach (string line in errorLines)
				{					
					Text.WriteColor("| `w`" + line + Text.BlankSpaces(Console.WindowWidth - Regex.Replace(line, @"`.`", "").Length - 4, true) + "`r` |", false);
				}
			}
			
			Text.Write("\\-----------------------------------------------------------------------/");
			
			Console.SetCursorPosition(0, 0);
			UnsetError();
		}
		
		public static void SetError(string message)
		{
			errors.Add(message);
			isError = true;
		}
		
		private static void UnsetError()
		{
			errors.Clear();
			isError = false;
		}
		#endregion
		
		#region Save and Load
		public static void Save()
		{
			SaveGame saveGame = new SaveGame();
			
			try
			{
				using (Stream stream = File.Open(saveFile, FileMode.Create))
				{
					BinaryFormatter bin = new BinaryFormatter();
					bin.Serialize(stream, saveGame);
				}
				
				Program.SetNotification("Save Successful!");
			}
			catch (AccessViolationException)
			{
				Program.SetError("Save Failed! File access denied.");
			}
			catch (Exception)
			{
				Program.SetError("Save Failed! An unspecified error occurred.");
			}
		}
		
		public static void Load()
		{
			try
			{
				using (Stream stream = File.Open(saveFile, FileMode.Open))
				{
					BinaryFormatter bin = new BinaryFormatter();
					SaveGame saveGame = (SaveGame)bin.Deserialize(stream);
					saveGame.Load();
				}
				
				Program.SetNotification("Load Successful!");
			}
			catch (FileNotFoundException)
			{
				Program.SetError("No savegame exists!");
			}
			catch (AccessViolationException)
			{
				Program.SetError("Load failed! File access denied.");
			}
			catch (Exception)
			{
				Program.SetError("Load failed! An unspecified error occurred.");
			}
		}
		#endregion
	}
}