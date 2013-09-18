using System;
using System.Collections.Generic;

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
		public static int AttackerHealth
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
					Player.Health = value;
				else
					CurrentEnemy.Health = value;
			}
		}

		public static int DefenderHealth
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
					CurrentEnemy.Health = value;
				else
					Player.Health = value;
			}
		}

		public static int AttackerMagic
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
					Player.Magic = value;
				else
					CurrentEnemy.Magic = value;
			}
		}

		public static int AttackerPower
		{
			get
			{
				if (CurrentTurn == "player")
					return Player.Power;
				else
					return CurrentEnemy.Power;
			}
		}

		public static int DefenderDefense
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

		#region Start Battle Method
		public static void StartBattle(int enemy)
		{
			CurrentEnemy = World.Enemies[enemy];
		}			
		#endregion
		
		#region Public Methods
		public static void BattleHUD()
		{
			Text.WriteColor("`c`/-----------------------------------------------------------------------\\", false);
			
		}
		#endregion
	}
}