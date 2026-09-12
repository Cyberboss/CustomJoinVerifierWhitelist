# CustomJoinVerifierWhitelist

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that allows a configured list of user IDs to skip running an enabled custom join verifier for hosted sessions.

This can be useful for Headless hosts to ensure they always have access to their sessions without rebooting, despite tomfoolery (I have locked myself out in the past).

## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
1. Place [CustomJoinVerifierWhitelist.dll](https://github.com/Cyberboss/CustomJoinVerifierWhitelist/releases/latest/download/CustomJoinVerifierWhitelist.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default Windows install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create this folder for you.
1. Start the game. If you want to verify that the mod is working you can check your Resonite logs.

## Configuration

At the time of writing, the mod must be configured manually. Here is an example:

```json
{
  "version": "1.0.0",
  "values": {
    "Enabled": true,
    "Whitelist User IDs": [
      "U-UserID1",
      "U-UserID2"
    ]
  }
}
```
