using System;

namespace Escape
{
	abstract class Entity
	{
		#region Declarations
		public string Name;
		public string Description;
		#endregion
		
		#region Constructor
		public Entity(string Name, string Description)
		{
			this.Name = Name;
			this.Description = Description;
		}
		#endregion
	}
}