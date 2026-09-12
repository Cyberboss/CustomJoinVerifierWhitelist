using System.Collections.Generic;
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
		internal const string VersionConstant = "1.0.0";

		public override string Name => "CustomJoinVerifierWhitelist";

		public override string Author => "Dominion";

		public override string Version => VersionConstant;

		public override string Link => "https://github.com/Cyberboss/CustomJoinVerifierWhitelist";

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> Enabled = new ModConfigurationKey<bool>("Enabled", "Mod Enabled", () => true);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<List<string>> BypassUserIDs = new ModConfigurationKey<List<string>>("Whitelist User IDs", "User IDs that may bypass enabled custom join verifiers of hosted sessions", () => []);

		private static ModConfiguration? Config;

		public override void OnEngineInit()
		{
			Config = GetConfiguration()!;
			Config.Save(true);
			Harmony harmony = new("net.dextraspace.CustomJoinVerifierWhitelist");
			harmony.PatchAll();
		}

		[HarmonyPatch(typeof(VerifyJoinRequest.Proxy), nameof(IUserJoinVerifier.VerifyJoinRequest))]
		class VerifyJoinRequestProxy_VerifyJoinRequest_Patch
		{

			public static bool Prefix(VerifyJoinRequest.Proxy __instance, SessionConnection request, ref Task<JoinGrant?> result)
			{
				if ((Config?.GetValue(Enabled) ?? false) && (Config?.GetValue(BypassUserIDs)?.Contains(request.UserID) ?? false))
				{
					result = Task.FromResult<JoinGrant?>(JoinGrant.Allow());
					return false;
				}

				return true;
			}
		}
	}
};
