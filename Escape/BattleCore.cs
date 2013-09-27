using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Escape
{
	static class BattleCore
	{
		#region Declarations
		public static Enemy CurrentEnemy;
		public static string CurrentTurn;

		public static double BaseLuckyRate = 7.5;
		#endregion

		#region Properties
		public static double AttackerHealth
		{
			get
			{
				if (CurrentTurn == "player")
					return Player.Health;
				else
					return CurrentEnemy.Health;
			}
			set
			{
				if (CurrentTurn == "player")
					Player.Health = (int)value;
				else
					CurrentEnemy.Health = (int)value;
			}
		}

		public static double AttackerMaxHealth
		{
			get
			{
				if (CurrentTurn == "player")
					return Player.MaxHealth;
				else
					return CurrentEnemy.MaxHealth;
			}
		}

		public static double DefenderHealth
		{
			get
			{
				if (CurrentTurn == "player")
					return CurrentEnemy.Health;
				else
					return Player.Health;
			}
			set
			{
				if (CurrentTurn == "player")
					CurrentEnemy.Health = (int)value;
				else
					Player.Health = (int)value;
			}
		}

		public static double DefenderMaxHealth
		{
			get
			{
				if (CurrentTurn == "player")
					return CurrentEnemy.MaxHealth;
				else
					return Player.MaxHealth;
			}
		}

		public static double AttackerMagic
		{
			get
			{
				if (CurrentTurn == "player")
					return Player.Magic;
				else
					return CurrentEnemy.Magic;
			}
			set
			{
				if (CurrentTurn == "player")
					Player.Magic = (int)value;
				else
					CurrentEnemy.Magic = (int)value;
			}
		}

		public static double AttackerPower
		{
			get
			{
				if (CurrentTurn == "player")
					return Player.Power;
				else
					return CurrentEnemy.Power;
			}
		}

		public static double DefenderDefense
		{
			get
			{
				if (CurrentTurn == "player")
					return CurrentEnemy.Defense;
				else
					return Player.Defense;
			}
		}
		#endregion

		#region Battle Methods
		public static void StartBattle(int enemy)
		{
			Program.GameState = Program.GameStates.Battle;

			CurrentTurn = "player";
			CurrentEnemy = CloneEnemy(World.Enemies[enemy]);
		}

		public static void NextTurn()
		{
			if (CurrentTurn == "player")
			{
				string temp = Text.SetPrompt("[" + World.Map[Player.Location].Name + "] > ");
				Text.Clear();
				Player.Do(temp);
			}
			else if (CurrentTurn == "enemy")
			{
				Text.SetKeyPrompt("[Press any key to continue!]");
				Text.Clear();

				CurrentEnemy.Attack();
			}
			else
			{
				Program.SetError("Errrr.... wanna fuk?");
			}

			CurrentTurn = (CurrentTurn == "player") ? "enemy" : "player";
		}
		#endregion
		
		#region Public Methods
		public static void BattleHUD()
		{
			Text.WriteColor("`c`/-----------------------------------------------------------------------\\", false);

			List<string> enemyDescription = Text.Limit(CurrentEnemy.Name + " - " + CurrentEnemy.Description, Console.WindowWidth - 4);

			foreach (string line in enemyDescription)
			{
				Text.WriteColor("| `w`" + line + Text.BlankSpaces((Console.WindowWidth - line.Length - 4), true) + "`c` |", false);
			}

			Text.WriteColor(">-----------------v-----------------v-----------------v-----------------<", false);
			Text.WriteColor("|      `w`Stats`c`      |     `w`Attacks`c`     |      `w`Items`c`      |      `w`Enemy`c`      |", false);
			Text.WriteColor(">-----------------#-----------------#-----------------#-----------------<`w`", false);

			int currentY = Console.CursorTop;
			int i;
			int longestList = 0;

			Text.WriteColor("  HP [`r`" + Text.ToBar(Player.Health, Player.MaxHealth, 10) + "`w`]");
			Text.WriteColor("  MP [`g`" + Text.ToBar(Player.Magic, Player.MaxMagic, 10) + "`w`]");

			longestList = (2 > longestList) ? 2 : longestList;
			i = 0;

			Console.SetCursorPosition(18, currentY);

			foreach (int attack in Player.Attacks)
			{
				string name = World.Attacks[attack].Name;
				Text.WriteColor("  " + name);
				i++;
			}

			longestList = (i > longestList) ? i : longestList;
			i = 0;

			Console.SetCursorPosition(36, currentY);

			foreach (int item in Player.ItemsUsableInBattle())
			{
				string name = World.Items[item].Name;
				Text.WriteColor("  " + name);
				i++;
			}

			longestList = (i > longestList) ? i : longestList;
			i = 0;

			Console.SetCursorPosition(54, currentY);

			Text.WriteColor("  HP [`r`" + Text.ToBar(CurrentEnemy.Health, CurrentEnemy.MaxHealth, 10) + "`w`]");
			Text.WriteColor("  MP [`g`" + Text.ToBar(CurrentEnemy.Magic, CurrentEnemy.MaxMagic, 10) + "`w`]");

			Console.SetCursorPosition(0, currentY);

			for (i = 0; i < longestList; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Text.WriteColor("`c`|", false);
					Console.CursorLeft += 17;
				}

				Text.Write("|");
				Console.CursorLeft = 0;
			}

			Text.WriteColor("\\-----------------^-----------------+-----------------^-----------------/`w`", false);
			Text.WriteColor(" `c`\\`w` Lvl.", false);

			if (Player.Level < 10)
			{
				Text.Write(" ");
			}

			Text.WriteColor(Player.Level + " [`g`" + Text.ToBar(Player.Exp, Player.NextLevel, 23) + "`w`] `c`|`w` "
			+ Player.Exp + "/" + Player.NextLevel + "`c` /", false);

			int expLength = Console.CursorLeft;

			Text.WriteLine("", false);

			Text.WriteColor("  \\---------------------------------^", false);

			while (Console.CursorLeft < expLength - 2)
			{
				Text.WriteColor("-", false);
			}

			Text.WriteColor("/`w`", false);

			Text.WriteAt("`c`v`w`", expLength, Console.CursorTop - 2, true, true);

			Text.WriteLine("", false);
			Text.WriteLine("", false);
		}
		#endregion

		#region Helper Methods
		private static T CloneEnemy<T>(T obj)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;

				return (T) formatter.Deserialize(ms);
			}
		}
		#endregion
	}
}