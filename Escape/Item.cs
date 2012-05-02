using System;

namespace Escape
{
	class Item : Entity
	{
		#region Declarations
		public bool SingleUse;
		#endregion
		
		#region Constructor
		public Item(string Name, string Description, bool SingleUse = false)
			:base(Name, Description)
		{
			this.SingleUse = SingleUse;
		}
		#endregion
		
		#region Item Using Methods
		public void Use()
		{
			switch (this.Name)
			{
				case "Brass Key":
					if (Player.Location == 2)
					{
						Program.SetNotification("The key opened the lock!");
						World.Map[2].Exits.Add(3);
					}
					else
					{
						NoUse();
						return;
					}
					break;
					
				case "Shiny Stone":
					Program.SetNotification("The shiny orb glowed shiny colors!");
					break;
					
				default:
					NoUse();
					break;
			}
			
			if (this.SingleUse)
			{
				Player.RemoveItemFromInventory(World.GetItemIdByName(this.Name));
			}
		}
		#endregion
		
		#region Private Methods
		private void NoUse()
		{
			Program.SetError("There is a time and place for everything, but this is not the place to use that!");
		}
		#endregion
	}
}