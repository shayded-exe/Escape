using System;
using System.Collections.Generic;

namespace Escape
{
	static class BattleCore
	{
		#region Declarations
		public static Enemy CurrentEnemy;

		public static double BaseLuckyRate = 7.5;
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