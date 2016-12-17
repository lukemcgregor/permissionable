using System;
using System.Collections.Generic;
using System.Text;

namespace Permissionable
{
    interface IPermissionableService
    {
		void EnsureNode(string key, string name);
		void EnsureActor(string key, string name);
		void EnsureThing(string key, string name);
		void GrantPermission(string grantedKey, string thingKey, string action);
		void MakeMember(string memberKey, string memberOfKey);
	}
}
