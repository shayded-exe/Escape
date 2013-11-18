using System;
using System.Collections.Generic;
using System.Linq;

namespace Escape
{
    [Serializable]
    class SaveGame
    {
        #region Declarations
        public string Player_Name = Player.Name;
        public Location Player_Location = Player.Location;
        public int Player_Health = Player.Health;
        public int Player_Magic = Player.Magic;
        public int Player_Level = Player.Level;
        public int Player_Exp = Player.Exp;
        public World Player_World = Player.World;

        public List<Item> Player_Inventory = Player.Inventory;
        #endregion

        public SaveGame()
        {
            Player_Name = Player.Name;
            Player_Location = Player.Location;
            Player_Health = Player.Health;
            Player_Magic = Player.Magic;
            Player_Level = Player.Level;
            Player_Exp = Player.Exp;
            Player_World = Player.World;
        }

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

            // Replace world instead of locations
            // I think this is better as there won't be any version mix (as long as loading succeeds).
            Player.World = this.Player_World;
        }
        #endregion
    }
}
