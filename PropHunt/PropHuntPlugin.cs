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
using Il2CppSystem.Web.Util;
using PropHunt.CustomOption;

namespace PropHunt;

[BepInPlugin("com.ugackminer.amongus.prophunt", "Prop Hunt", Version)]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class PropHuntPlugin : BasePlugin
{
    // Backend Variables
    public Harmony Harmony { get; } = new("com.ugackminer.amongus.prophunt");
    public const string Version = "2024.11.3";
    // Gameplay Variables
    public static int missedKills = 0;

    public static PropHuntPlugin Instance;

    public static Dictionary<PlayerControl, ulong> PlayerVersion = new();

    public static Sprite TeamLogo;
    public override void Load()
    {
        Instance = this;
        Instance = PluginSingleton<PropHuntPlugin>.Instance;

        Harmony.PatchAll();
        Harmony.PatchAll(typeof(PicturesLoad));
        Harmony.PatchAll(typeof(PingTracker_Update));
        Harmony.PatchAll(typeof(Language));
        Harmony.PatchAll(typeof(Patches));
        Harmony.PatchAll(typeof(CustomOptionType));
        Harmony.PatchAll(typeof(CustomHeaderOption));
        Harmony.PatchAll(typeof(CustomNumberOption));
        Harmony.PatchAll(typeof(CustomToggleOption));
        Harmony.PatchAll(typeof(CustomOption.CustomOption));
        Harmony.PatchAll(typeof(CustomOption.CustumOptions));
        Harmony.PatchAll(typeof(CustomOption.Patches));
        CustomOption.CustumOptions.Load();
    }
    public static Sprite GetTeamLogo()
    {
        if (TeamLogo) return TeamLogo;
        return TeamLogo = PicturesLoad.loadSpriteFromResources("PropHunt.Resources.TeamLogo.png", 150f);
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
            yield return new WaitForSeconds(CustomGameOptions.hidingTime);
            PlayerControl.LocalPlayer.moveable = true;
            Object.Destroy(__instance.gameObject);
        }
    }
}
