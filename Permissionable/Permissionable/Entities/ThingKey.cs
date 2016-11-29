using System;
using System.Collections.Generic;
using System.Text;

namespace Permissionable.Entities
{
	public class ThingKey : ICanBeGrantedKey
	{
		public string Key { get; set; }
		public string Name { get; set; }
	}
}
