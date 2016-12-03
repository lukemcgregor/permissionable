using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jClient;
using Permissionable.Entities;

namespace Permissionable.Tests
{
    [TestClass]
    public class PermissionableTests
	{
		private static IGraphClient _client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "abc123!");
		static PermissionableTests()
		{
			_client.Connect();
		}

		[TestMethod]
		public void GrantPermission()
		{
			var sut = new PermissionableService(_client);
			sut.MakeMember(new ActorKey { Key = "bob", Name = "Bob" }, new NodeKey { Key = "123", Name = "Can Drive Everything" });
			sut.GrantPermission(new NodeKey { Key = "123", Name = "Can Drive Everything" }, new ThingKey { Key = "123-car", Name = "bobs car" }, "Drive");
		}
		
		[TestMethod]
		public void ActorPermissionDirectly()
		{
			var sut = new PermissionableService(_client);
			var actor = new ActorKey { Key = Guid.NewGuid().ToString(), Name = "ActorPermissionDirectly_Actor" };
			var thing = new ThingKey { Key = Guid.NewGuid().ToString(), Name = "ActorPermissionDirectly_Thing" };
			sut.GrantPermission(actor, thing, "ActorPermissionDirectly_Action");

			Assert.IsTrue(sut.CanItAccess(actor, thing, "ActorPermissionDirectly_Action"));
		}

		[TestMethod]
		public void ActorPermissionIndirectly1()
		{
			var sut = new PermissionableService(_client);
			var actor = new ActorKey { Key = Guid.NewGuid().ToString(), Name = "ActorPermissionDirectly_Actor" };
			var node = new NodeKey { Key = Guid.NewGuid().ToString(), Name = "ActorPermissionDirectly_Node" };
			var thing = new ThingKey { Key = Guid.NewGuid().ToString(), Name = "ActorPermissionDirectly_Thing" };
			sut.MakeMember(actor, node);
			sut.GrantPermission(node, thing, "ActorPermissionDirectly_Action");

			Assert.IsTrue(sut.CanItAccess(actor, thing, "ActorPermissionDirectly_Action"));
		}
	}
}
