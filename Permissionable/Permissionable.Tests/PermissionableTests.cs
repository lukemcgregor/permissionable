using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jClient;
using Permissionable.Entities;

namespace Permissionable.Tests
{
    [TestClass]
    public class PermissionableTests
    {
        [TestMethod]
        public void GrantPermission()
        {
			var client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "abc123!");
			client.Connect();
			var sut = new PermissionableService(client);
			sut.MakeMember(new ActorKey { Key = "bob", Name = "Bob" }, new NodeKey { Key = "123", Name = "Can Drive Everything" });
			sut.GrantPermission(new NodeKey { Key = "123", Name = "Can Drive Everything" }, new ThingKey { Key = "123-car", Name = "bobs car" }, "Drive");
		}
    }
}
