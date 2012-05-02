using System;
using System.Collections.Generic;

namespace Escape
{
	class Location : Entity
	{
		#region Declarations
		public List<int> Exits;
		public List<int> Items;
		#endregion
		
		#region Constructors	
		public Location(string Name, string Description, List<int> Exits, List<int> Items)
			:base(Name, Description)
		{
			this.Exits = Exits;
			this.Items = Items;
		}
		
		public Location(string Name, string Description, List<int> Exits)
			:base(Name, Description)
		{
			this.Exits = Exits;
			this.Items = new List<int>();
		}
		#endregion
		
		#region Public Methods		
		public bool ContainsExit(int aExit)
		{
			if (Exits.Contains(aExit))
				return true;
			else
				return false;
		}
		
		public bool ContainsItem(int aItem)
		{
			if (Items.Contains(aItem))
				return true;
			else
				return false;
		}
		#endregion
	}
}