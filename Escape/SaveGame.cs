using System;
using System.Collections.Generic;

namespace Escape
{
	[Serializable]
	class SaveGame
	{
		#region Declarations
		public string Player_Name = Player.Name;
		public string Player_Location = Player.Location;
		
		public int Player_MaxHealth = Player.MaxHealth;
		public int Player_Health = Player.Health;
		
		public int Player_MaxMagic = Player.MaxMagic;
		public int Player_Magic = Player.Magic;
		
		public List<string> Player_Inventory = Player.Inventory;
		
		public List<Location> World_Map = World.Map;
		#endregion
		
		#region Loading Method
		public void Load()
		{
			Player.Name = this.Player_Name;
			Player.Location = this.Player_Location;
			
			Player.MaxHealth = this.Player_MaxHealth;
			Player.Health = this.Player_Health;
			
			Player.MaxMagic = this.Player_MaxMagic;
			Player.Magic = this.Player_Magic;
			
			Player.Inventory = this.Player_Inventory;
			
			World.Map = this.World_Map;
		}
		#endregion
	}
}
