using System;

namespace Escape
{
	[Serializable]
	abstract class Entity
	{
		#region Declarations
		public int ID;
		public string Name;
		public string Description;
		#endregion
		
		#region Constructor
		public Entity(int ID, string Name, string Description)
		{
			this.ID = ID;
			this.Name = Name;
			this.Description = Description;
		}
		#endregion
	}
}