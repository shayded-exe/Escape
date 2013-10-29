using System;
using System.Collections.Generic;

namespace Escape
{
	[Serializable]
	class SaveGame
	{
		#region Declarations
		public string Player_Name = Player.Name;
		public int Player_Location = Player.Location;
		public int Player_Health = Player.Health;
		public int Player_Magic = Player.Magic;
		public int Player_Level = Player.Level;
		public int Player_Exp = Player.Exp;
		
		public List<int> Player_Inventory = Player.Inventory;
		
		public List<Location> World_Map = World.Map;
		#endregion
		
		#region Loading Method
		public void Load()
		{
			Player.Name = this.Player_Name;
			Player.Location = this.Player_Location;
			Player.Health = this.Player_Health;
			Player.Magic = this.Player_Magic;
			Player.Level = this.Player_Level;
			Player.Exp = this.Player_Exp;
			
			Player.Inventory = this.Player_Inventory;
			
			World.Map = this.World_Map;
		}
		#endregion
	}
}
