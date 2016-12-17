using Neo4jClient;
using Permissionable.Entities;
using System;
using Xunit;

namespace Permissionable.Tests
{
    public class PermissionableTests
    {
        private static IGraphClient _client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "neo4j");
        static PermissionableTests()
        {
            _client.Connect();
        }

        [Fact]
        public void GrantPermission()
        {
            var sut = new PermissionableService(_client);
            sut.MakeMember(new ActorKey { Key = "bob", Name = "Bob" }, new NodeKey { Key = "123", Name = "Can Drive Everything" });
            sut.GrantPermission(new NodeKey { Key = "123", Name = "Can Drive Everything" }, new ThingKey { Key = "123-car", Name = "bobs car" }, "Drive");
        }

        /// <summary>
        /// Build graphs with different depths and ensure that there is no limit to group nesting
        /// </summary>
        /// <param name="depth">the depth of the graph to create</param>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(20)]
        public void ActorPermissionByDepth(int depth)
        {
            var sut = new PermissionableService(_client);
            var actor = new ActorKey { Key = Guid.NewGuid().ToString(), Name = $"{GetMethodName()}_Actor" };
            ICanJoinKey next = actor;
            for (int i = 0; i < depth; i++)
            {
                var node = new NodeKey { Key = Guid.NewGuid().ToString(), Name = $"{GetMethodName()}_Node_" + depth };
                sut.MakeMember(next, node);
                next = node;
            }
            var thing = new ThingKey { Key = Guid.NewGuid().ToString(), Name = $"{GetMethodName()}_Thing" };
            sut.GrantPermission(next, thing, $"{GetMethodName()}_Action");

            Assert.True(sut.CanItAccess(actor, thing, $"{GetMethodName()}_Action"));
        }

        /// <summary>
        /// Build graphs with different depths and multiple paths
        /// </summary>
        /// <param name="depth">the depth of the graph to create</param>
        [Theory]
        [InlineData(1)]
        public void ActorPermissionMultiplePath(int depth)
        {
            var s = GetMethodName();
            var sut = new PermissionableService(_client);
            var actor = new ActorKey { Key = Guid.NewGuid().ToString(), Name = $"{GetMethodName()}_Actor" };
            ICanJoinKey next = actor;
            ICanJoinKey next2 = actor;
            for (int i = 0; i < depth; i++)
            {
                {
                    var node = new NodeKey { Key = Guid.NewGuid().ToString(), Name = $"{GetMethodName()}_Node_Path1_" + depth };
                    sut.MakeMember(next, node);
                    next = node;
                }

                {
                    var node = new NodeKey { Key = Guid.NewGuid().ToString(), Name = $"{GetMethodName()}_Node_Path2_" + depth };
                    sut.MakeMember(next2, node);
                    next2 = node;
                }
            }
            var thing = new ThingKey { Key = Guid.NewGuid().ToString(), Name = $"{GetMethodName()}_Thing" };
            sut.GrantPermission(next, thing, $"{GetMethodName()}_Action");
            sut.GrantPermission(next2, thing, $"{GetMethodName()}_Action");

            Assert.True(sut.CanItAccess(actor, thing, $"{GetMethodName()}_Action"));
        }

        /// <summary>
        /// If there is a loop in the permissions graph make sure that it doesnt crash the lookup.
        /// </summary>
        [Fact]
        public void ActorPermissionLoop()
        {
            var sut = new PermissionableService(_client);
            var actor = new ActorKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Actor" };
            var node = new NodeKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Node" };
            var node2 = new NodeKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Node2" };
            var thing = new ThingKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Thing" };
            sut.MakeMember(actor, node);
            sut.MakeMember(node, node2);
            sut.MakeMember(node2, node);
            sut.GrantPermission(node2, thing,  $"{GetMethodName()}_Action");

            Assert.True(sut.CanItAccess(actor, thing,  $"{GetMethodName()}_Action"));
        }


        /// <summary>
        /// If there is a loop in the permissions graph make sure that it doesnt crash the lookup.
        /// </summary>
        [Fact]
        public void ActorPermissionDeleted()
        {
            var sut = new PermissionableService(_client);
            var actor = new ActorKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Actor" };
            var node = new NodeKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Node" };
            var node2 = new NodeKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Node2" };
            var thing = new ThingKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Thing" };
            sut.MakeMember(actor, node);
            sut.MakeMember(node, node2);
            sut.MakeMember(node2, node);
            sut.GrantPermission(node2, thing,  $"{GetMethodName()}_Action");

            sut.RemoveMember(node,node2);

            Assert.False(sut.CanItAccess(actor, thing,  $"{GetMethodName()}_Action"));
        }

         /// <summary>
        /// If there is a loop in the permissions graph make sure that it doesnt crash the lookup.
        /// </summary>
        [Fact]
        public void ActorPermissionDeletingANonExistantPermission()
        {
            var sut = new PermissionableService(_client);
            var actor = new ActorKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Actor" };
            var node = new NodeKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Node" };
            var node2 = new NodeKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Node2" };
            var thing = new ThingKey { Key = Guid.NewGuid().ToString(), Name =  $"{GetMethodName()}_Thing" };
            sut.MakeMember(actor, node);
            sut.GrantPermission(node2, thing,  $"{GetMethodName()}_Action");

            sut.RemoveMember(node,node2);

            Assert.False(sut.CanItAccess(actor, thing,  $"{GetMethodName()}_Action"));
        }

        //see http://stackoverflow.com/a/41112496/1070291
        public string GetMethodName([System.Runtime.CompilerServices.CallerMemberName]string methodName = "")
        {
            return methodName;
        }
    }
}
