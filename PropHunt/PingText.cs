using HarmonyLib;
using UnityEngine;
using xCloud;

namespace PropHunt;
[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update
{
    public static string HostSetting = "";
    public static string InfectionSetting = "";

    private static GameObject teamLogo;
    static void Prefix(PingTracker __instance)
    {
        if (teamLogo == null)
        {
            teamLogo = new GameObject("TeamLogo");
            var rend = teamLogo.AddComponent<SpriteRenderer>();
            rend.sprite = PropHuntPlugin.GetTeamLogo();
            rend.color = new Color(1, 1, 1, 0.5f);
            teamLogo.transform.parent = __instance.transform.parent;
            teamLogo.transform.localScale *= 0.6f;
        }
        float offset = (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) ? 0.75f : 0f;
        teamLogo.transform.position = HudManager.Instance.MapButton.transform.position + Vector3.down * offset;
    }

    /*[HarmonyPostfix]
    public static void Postfix(PingTracker __instance)
    {
        __instance.text.text = $"<size=120%><color=#ff6600>Prop Hunt</color> v{PropHuntPlugin.Version}\n</size>By ugackMiner53 & <color=#00ffff>fangkuai</color>\nPING:{AmongUsClient.Instance.Ping} MS\n";
    }*/
}