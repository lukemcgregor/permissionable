using Neo4jClient;
using Neo4jClient.Cypher;
using Permissionable.Entities;
using System;

namespace Permissionable
{
	public class PermissionableService
	{
		private readonly IGraphClient _client;
		public PermissionableService(IGraphClient graphClient)
		{
			_client = graphClient;
		}
		
		public void GrantPermission(ICanJoinKey grantedToKey, ICanBeGrantedKey grantedKey, string action)
		{
			_client.Cypher
				.MergeKey(grantedToKey, "grantedToKey")
				.MergeKey(grantedKey, "grantedKey")
				.Merge("(grantedToKey) - [:GRANT {action: {action}}] -> (grantedKey)")
				.WithParam("action", action)
				.ExecuteWithoutResults();
		}
		public void MakeMember(ICanJoinKey memberKey, ICanBeJoinedKey memberOfKey)
		{
			_client.Cypher
				.MergeKey(memberKey, "memberKey")
				.MergeKey(memberOfKey, "memberOfKey")
				.Merge("(memberKey) - [:MEMBER] -> (memberOfKey)")
				.ExecuteWithoutResults();
		}

	}
	internal static class MergeExtensions
	{
		internal static ICypherFluentQuery MergeKey(this ICypherFluentQuery q, IKey key, string valueLabel)
		{
			if (key is ActorKey)
			{
				return q
					.Merge($"({valueLabel}:Actor:CanJoin {{ key: {{{valueLabel}Key}}, name: {{{valueLabel}Name}} }})")
					.WithParam($"{valueLabel}Key", key.Key)
					.WithParam($"{valueLabel}Name", key.Name);
			}
			if (key is ThingKey)
			{
				return q
					.Merge($"({valueLabel}:Thing:CanBeGranted {{ key: {{{valueLabel}Key}}, name: {{{valueLabel}Name}} }})")
					.WithParam($"{valueLabel}Key", key.Key)
					.WithParam($"{valueLabel}Name", key.Name);
			}
			if (key is NodeKey)
			{
				return q
					.Merge($"({valueLabel}:Node:CanBeJoined:CanJoin {{ key: {{{valueLabel}Key}}, name: {{{valueLabel}Name}} }})")
					.WithParam($"{valueLabel}Key", key.Key)
					.WithParam($"{valueLabel}Name", key.Name);
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
