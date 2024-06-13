// Core Script of PropHuntPlugin
// Copyright (C) 2022  ugackMiner
global using static PropHunt.Language;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Utilities;
using Reactor.Networking.Rpc;
using Reactor.Networking.Attributes;
using UnityEngine;
using System.Collections.Generic;
using AmongUsSpecimen;

namespace PropHunt;

[BepInPlugin("com.ugackminer.amongus.prophunt", "Prop Hunt", Version)]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(Specimen.Guid)]
[CustomRegion("PropHunt AU Server(MEU)", "au-eu.duikbo.at", "https://au-eu.duikbo.at", color: "#ff0000")]
public partial class PropHuntPlugin : BasePlugin
{
    // Backend Variables
    public Harmony Harmony { get; } = new("com.ugackminer.amongus.prophunt");
    public ConfigEntry<float> HidingTime { get; private set; }
    public ConfigEntry<int> MaxMissedKills { get; private set; }
    public ConfigEntry<bool> Infection { get; private set; }
    public ConfigEntry<bool> EnableInvisible {get; private set;}
     public ConfigEntry<bool> EnableSpeed {get; private set;}
    public const string Version = "2024.5.29";
    // Gameplay Variables
    public static float hidingTime = 30f;
    public static int maxMissedKills = 3;
    public static bool infection = true;

    public static int missedKills = 0;

    public static PropHuntPlugin Instance;

    public static Dictionary<PlayerControl, ulong> PlayerVersion = new();

    public static Sprite TeamLogo;
    public override void Load()
    {
        HidingTime = Config.Bind("Prop Hunt", "Hiding Time", 30f);
        MaxMissedKills = Config.Bind("Prop Hunt", "Max Misses", 3);
        Infection = Config.Bind("Prop Hunt", "Infection", true);
         EnableInvisible = Config.Bind("Prop Hunt", "Should The Player Be Invisible instead of disgusing", false);
         EnableInvisible = Config.Bind("Prop Hunt", "Should The Player Speed Get 2X instead of disgusing (cool with invisible)", false);

        Instance = this;
        Instance = PluginSingleton<PropHuntPlugin>.Instance;

        Harmony.PatchAll();
        Harmony.PatchAll(typeof(NewGameSettingsTabPatch));
        Harmony.PatchAll(typeof(PicturesLoad));
        Harmony.PatchAll(typeof(PingTracker_Update));
        Harmony.PatchAll(typeof(Language));
        Harmony.PatchAll(typeof(Patches));
        Harmony.PatchAll(typeof(CustomRoleSettings));
    }
    public static Sprite GetTeamLogo()
    {
        if (TeamLogo) return TeamLogo;
        return TeamLogo = PicturesLoad.loadSpriteFromResources("PropHunt.Resources.TeamLogo.png", 150f);
    }

    public enum RPC
    {
        PropSync,
        SettingSync
    }

    public static class RPCHandler
    {
        // static MethodRpc rpc = new MethodRpc(PropHuntPlugin.Instance, Type.GetMethod("RPCPropSync"), RPC.PropSync, Hazel.SendOption.Reliable, RpcLocalHandling.None, true);
        [Rpc]
        public static void RPCPropSync(PlayerControl __sender, string propIndex)
        {
            GameObject prop = ShipStatus.Instance.AllConsoles[int.Parse(propIndex)].gameObject;
            Logger<PropHuntPlugin>.Info($"{__sender.Data.PlayerName} changed their sprite to: {prop.name}");
            __sender.GetComponent<SpriteRenderer>().sprite = prop.GetComponent<SpriteRenderer>().sprite;
            __sender.transform.localScale = prop.transform.lossyScale;
            __sender.Visible = false;
        }

        [Rpc]
        public static void RPCSettingSync(PlayerControl __sender, float _hidingTime, int _missedKills, bool _infection, bool HaveInvisibleAbility, bool HaveSpeedAbility)
        {
            hidingTime = _hidingTime;
            maxMissedKills = _missedKills;
            infection = _infection;
            Logger<PropHuntPlugin>.Info("H: " + PropHuntPlugin.hidingTime + ", M: " + PropHuntPlugin.maxMissedKills + ", I: " + PropHuntPlugin.infection);
            if (__sender == PlayerControl.LocalPlayer && (hidingTime != Instance.HidingTime.Value || maxMissedKills != Instance.MaxMissedKills.Value || infection != Instance.Infection.Value || HaveInvisibleAbility != Instance.EnableInvisible.Value || HaveSpeedAbility != Instance.EnableSpeed.Value))
            {
                Instance.HidingTime.Value = hidingTime;
                Instance.MaxMissedKills.Value = maxMissedKills;
                Instance.Infection.Value = infection;
                Instance.EnableInvisible.Value = NewGameSettingsTabPatch.EnableInvisible;
                Instance.EnableSpeed.Value = NewGameSettingsTabPatch.EnableSpeed;
                Instance.Config.Save();
            }
        }
    }


    public static class Utility
    {
        public static GameObject FindClosestConsole(GameObject origin, float radius)
        {
            Collider2D bestCollider = null;
            float bestDist = 9999;
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(origin.transform.position, radius))
            {
                if (collider.GetComponent<Console>() != null)
                {
                    float dist = Vector2.Distance(origin.transform.position, collider.transform.position);
                    if (dist < bestDist)
                    {
                        bestCollider = collider;
                        bestDist = dist;
                    }
                }
            }
            return bestCollider.gameObject;
        }

        public static System.Collections.IEnumerator KillConsoleAnimation()
        {
            if (Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.PlaySound(ShipStatus.Instance.SabotageSound, false, 0.8f);
                HudManager.Instance.FullScreen.color = new Color(1f, 0f, 0f, 0.372549027f);
                HudManager.Instance.FullScreen.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                HudManager.Instance.FullScreen.gameObject.SetActive(false);
            }
            yield break;
        }

        public static System.Collections.IEnumerator IntroCutsceneHidePatch(IntroCutscene __instance)
        {
            PlayerControl.LocalPlayer.moveable = false;
            yield return new WaitForSeconds(PropHuntPlugin.hidingTime);
            PlayerControl.LocalPlayer.moveable = true;
            Object.Destroy(__instance.gameObject);
        }
    }
}
