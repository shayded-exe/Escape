using System;

namespace Escape
{
	abstract class Item : Entity
	{
		#region Declarations
		private bool uses;
		public bool CanUseInBattle;
		#endregion
		
		#region Constructor
		public Item(
			string Name,
			string Description,
			bool uses,
			bool CanUseInBattle = false)
		:base(Name, Description)
		{
			this.uses = uses;
			this.CanUseInBattle = CanUseInBattle;
		}
		#endregion
		
		#region Public Methods
		public virtual void Use() { }

		public virtual void UseInBattle() { }

		public void NoUse()
		{
			Program.SetError("There is a time and place for everything, but this is not the place to use that!");
		}
		#endregion
	}

	#region Key
	class Key : Item
	{
		private string targetLocation;
		private string newLocation;

		public Key(
			string Name,
			string Description,
			string targetLocation,
			string newLocation,
			bool uses)
		:base(Name, Description, uses)
		{
			this.targetLocation = targetLocation;
			this.newLocation = newLocation;
		}

		public override void Use()
		{
			if (Player.Location == World.GetLocationIDByName(targetLocation))
			{
				Program.SetNotification("The " + this.Name + " opened the lock!");
				World.Map[World.GetLocationIDByName(targetLocation)].Exits.Add(World.GetLocationIDByName(newLocation));
			}
			else
			{
				NoUse();
				return;
			}
		}
	}
	#endregion

	#region ShinyStone
	class ShinyStone : Item
	{
		public ShinyStone(
			string Name,
			string Description,
			bool uses,
			bool CanUseInBattle)
		: base(Name, Description, uses, CanUseInBattle)
		{
		}

		public override void Use()
		{
			if (Player.Location == World.GetLocationIDByName("secret room"))
			{
				Player.Health += Math.Min(Player.MaxHealth / 10, Player.MaxHealth - Player.Health);
				Program.SetNotification("The magical stone restored your health by 10%!");
			}
			else
			{
				Program.SetNotification("The shiny stone glowed shiny colors!");
			}
		}
	}
	#endregion

	#region Rock
	class Rock : Item
	{
		public Rock(
			string Name,
			string Description,
			bool uses,
			bool CanUseInBattle)
		:base(Name, Description, uses, CanUseInBattle)
		{
		}

		public override void Use()
		{
			Program.SetNotification("You threw the rock at a wall. Nothing happened.");
		}

		public override void UseInBattle()
		{
			Program.SetNotification("The rock hit the enemy in the head! It seems confused...");
		}
	}
	#endregion
}