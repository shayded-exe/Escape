using System;
using System.Collections.Generic;
using System.Linq;

namespace Escape
{
    [Serializable]
    class SaveGame
    {
        #region Declarations
        public string Player_Name;
        public Location Player_Location;
        public int Player_Health;
        public int Player_Magic;
        public int Player_Level;
        public int Player_Exp;

        public List<Item> Player_Inventory;
        #endregion

        public SaveGame(Player player)
        {
            Player_Name = player.Name;
            Player_Location = player.Location;
            Player_Health = player.Health;
            Player_Magic = player.Magic;
            Player_Level = player.Level;
            Player_Exp = player.Exp;

            Player_Inventory = new List<Item>(player.Inventory);
        }

        #region Loading Method
        public Player Load()
        {
            Player player = new Player();
            player.Name = this.Player_Name;
            player.Location = this.Player_Location;
            player.Health = this.Player_Health;
            player.Magic = this.Player_Magic;
            player.Level = this.Player_Level;
            player.Exp = this.Player_Exp;

            player.Inventory = this.Player_Inventory;

            return player;
        }
        #endregion
    }
}
