using System;
using System.Collections.Generic;
using System.Text;

namespace Permissionable.Entities
{
    public interface IKey
    {
		string Key { get; set; }
		string Name { get; set; }
	}
}
