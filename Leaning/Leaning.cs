using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace AetharNet.Mods.ZumbiBlocks2.Leaning;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class Leaning : BaseUnityPlugin
{
    public const string PluginGUID = "AetharNet.Mods.ZumbiBlocks2.Leaning";
    public const string PluginAuthor = "wowi";
    public const string PluginName = "Leaning";
    public const string PluginVersion = "0.1.0";

    internal new static ManualLogSource Logger;

    public static KeyCode KeyLeanLeft => configKeyLeanLeft.Value;
    public static KeyCode KeyLeanRight => configKeyLeanRight.Value;

    private static ConfigEntry<KeyCode> configKeyLeanLeft;
    private static ConfigEntry<KeyCode> configKeyLeanRight;

    private void Awake()
    {
        Logger = base.Logger;

        configKeyLeanLeft = Config.Bind(
            "KeyBinds",
            "LeanLeft",
            KeyCode.Z,
            "Key to use to lean camera left");

        configKeyLeanRight = Config.Bind(
            "KeyBinds",
            "LeanRight",
            KeyCode.X,
            "Key to use to lean camera right");

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginGUID);
    }
}
