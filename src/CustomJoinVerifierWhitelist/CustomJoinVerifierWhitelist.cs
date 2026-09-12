using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using FrooxEngine;

using HarmonyLib;

using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Security;

using ResoniteModLoader;

namespace CustomJoinVerifierWhitelist
{
	public sealed class CustomJoinVerifierWhitelist : ResoniteMod
	{
		// UPDATE VERSIONS HERE AND IN GITHUB ACTIONS. DON'T FORGET RELEASE NOTES!
		internal const string VersionConstant = "1.1.1";

		public override string Name => "CustomJoinVerifierWhitelist";

		public override string Author => "Dominion";

		public override string Version => VersionConstant;

		public override string Link => "https://github.com/Cyberboss/CustomJoinVerifierWhitelist";

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> Enabled = new ModConfigurationKey<bool>("Enabled", "Mod Enabled", () => true);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<List<string?>> BypassUserIDs = new ModConfigurationKey<List<string?>>("Whitelist User IDs", "User IDs that may bypass enabled custom join verifiers of hosted sessions", () => []);

		private static ModConfiguration? Config;

		public override void OnEngineInit()
		{
			Config = GetConfiguration()!;

			var bypassUserIds = Config.GetValue(BypassUserIDs);
			if (bypassUserIds == null)
			{
				bypassUserIds = new List<string?>();
				Config.Set(BypassUserIDs, bypassUserIds);
			}

			if (Config.GetValue(Enabled))
			{
				Msg("The following user IDs are configured to bypass all enabled custom join verifiers:");
				foreach (var bypassUserId in bypassUserIds)
				{
					Msg($" - {bypassUserId}");
				}
			}

			Config.Save(true);

			Harmony harmony = new("net.dextraspace.CustomJoinVerifierWhitelist");
			var thisType = typeof(CustomJoinVerifierWhitelist);
			var verifyJoinRequestPrefix = thisType.GetMethod(
				nameof(Prefix),
				BindingFlags.NonPublic | BindingFlags.Static
			);

			var proxyType = typeof(VerifyJoinRequest.Proxy);
			var verifyJoinRequest = proxyType.GetMethod(
				nameof(IUserJoinVerifier.VerifyJoinRequest)
			);

			harmony.Patch(verifyJoinRequest, prefix: new HarmonyMethod(verifyJoinRequestPrefix));
		}

		static bool Prefix(VerifyJoinRequest.Proxy __instance, SessionConnection request, ref Task<JoinGrant?> __result)
		{
			const string NullString = "<NULL>";
			var userString = $"{request?.Username ?? NullString} ({request?.UserID ?? NullString})";
			var config = Config;
			if (config == null)
			{
				Warn($"Missing config during {nameof(IUserJoinVerifier.VerifyJoinRequest)} for user {userString}!");
				return true;
			}

			if (!config.GetValue(Enabled))
			{
				Debug($"Mod is disabled, ignoring join request for user {userString}");
				return true;
			}

			if (config.GetValue(BypassUserIDs)?.Contains(request?.UserID) ?? false)
			{
				Msg($"Configured user {userString} is bypassing the enabled custom join verifier");
				__result = Task.FromResult<JoinGrant?>(JoinGrant.Allow());
				return false;
			}

			Msg($"User {userString} is not in the whitelist, proceeding to {nameof(VerifyJoinRequest)} Protoflux node");
			return true;
		}
	}
};
