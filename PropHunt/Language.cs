using HarmonyLib;
using System.Collections.Generic;

namespace PropHunt
{
    public enum StringKey
    {
        PropHunt,
        HidingTime,
        MaxMisKill,
        Infection,
        Seeker,
        SeekerDescription,
        Prop,
        PropDescription,
        HelloWrods,
        PingMsText,
        RemainingAttempts,
        True,
        False,
        HostSetting
    }
    public static class Language
    {
        public static Dictionary<StringKey, string> langDic = new();

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.Initialize))]
        [HarmonyPostfix]
        public static void Init(TranslationController __instance)
        {
            langDic = GetLang(__instance.currentLanguage.languageID);
        }

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.SetLanguage))]
        [HarmonyPostfix]
        public static void SetLangPatch([HarmonyArgument(0)] TranslatedImageSet lang)
        {
            langDic = GetLang(lang.languageID);
        }

        public static string GetString(StringKey key)
        {
            string result = "";
            try
            {
                result = langDic[key];
            }
            catch
            {
                result = "<Error loading:" + key.ToString() + ">";
            }
            return result;
        }

        private static Dictionary<StringKey, string> GetLang(SupportedLangs lang)
        {
            switch (lang)
            {
                default:
                case SupportedLangs.English:
                    return new()
                    {
                        [StringKey.PropHunt] = "Prop Hunt",
                        [StringKey.HidingTime] = "Hiding Time",
                        [StringKey.MaxMisKill] = "Maximum Missed Kills",
                        [StringKey.Infection] = "Infection Mode",
                        [StringKey.Seeker] = "Seeker",
                        [StringKey.SeekerDescription] = "Find and kill the props\nYour game will be unfrozen after {0} seconds",
                        [StringKey.Prop] = "Prop",
                        [StringKey.PropDescription] = "Turn into props to hide from the seekers",
                        [StringKey.HelloWrods] = "Welcome to <color=#ff6700FF>Prop Hunt </color>v2024.5.29!\n\nBy <color=#ff00ff>ugackMiner53</color> & <color=#00ffff>fangkuai</color>\nYou can use “<b>R</b>”to clone nearest task\nOr you can use “<b>SHIFT</b>”to noclip through walls",
                        [StringKey.PingMsText] = "Ping: {0} ms",
                        [StringKey.RemainingAttempts] = "<color=#FF0000>Remaining Attempts: {0}</color>\nPing: {1} ms\n",
                        [StringKey.True] = "True",
                        [StringKey.False] = "False",
                        [StringKey.HostSetting] = "\n\nHost Settings:\nHiding Time: {0}\nMaximum Missed Kills: {1}\nInfection Mode: {2}\n"
                    };
                case SupportedLangs.SChinese:
                    return new()
                    {
                        [StringKey.PropHunt] = "道具躲猫猫",
                        [StringKey.HidingTime] = "躲藏时间",
                        [StringKey.MaxMisKill] = "最多击杀失误次数",
                        [StringKey.Infection] = "躲藏者死后变为内鬼",
                        [StringKey.Seeker] = "寻找者",
                        [StringKey.SeekerDescription] = "找出躲藏者们\n你在 {0} 秒后才能行动",
                        [StringKey.Prop] = "躲藏者",
                        [StringKey.PropDescription] = "变成道具，戏弄寻找者",
                        [StringKey.HelloWrods] = "欢迎来到 <color=#ff6700FF>道具躲猫猫 </color>v2024.5.29!\n\n由 <color=#ff00ff>ugackMiner53</color> & <color=#00ffff>fangkuai</color>制作\n你可以使用 “<b>R</b>” 按键来变形成附近的物品\n你也可以使用 “<b>SHIFT</b>” 按键穿墙（不建议）\n(有BUG加群805390622反应,不然憋着)",
                        [StringKey.PingMsText] = "延迟: {0} 毫秒",
                        [StringKey.RemainingAttempts] = "<color=#FF0000>剩余错误击杀次数: {0}</color>\n延迟: {1} 毫秒\n",
                        [StringKey.True] = "开启",
                        [StringKey.False] = "关闭",
                        [StringKey.HostSetting] = "\n\n房主设置:\n躲藏时间: {0}\n最大错误击杀次数: {1}\n躲藏者死后变为内鬼: {2}\n"
                    };

                case SupportedLangs.French:
                    return new()
                    {
                        [StringKey.PropHunt] = "Cache-cache avec des objets",
                        [StringKey.HidingTime] = "Temps de cachette",
                        [StringKey.MaxMisKill] = "Nombre maximum d'erreurs de tuerie",
                        [StringKey.Infection] = "Les cacheurs deviennent des imposteurs après leur mort",
                        [StringKey.Seeker] = "Chercheur",
                        [StringKey.SeekerDescription] = "Trouvez les cacheurs\nVous pouvez bouger après {0} secondes",
                        [StringKey.Prop] = "Cacheur",
                        [StringKey.PropDescription] = "Devenez un objet et trompez les chercheurs",
                        [StringKey.HelloWrods] = "Bienvenue dans <color=#ff6700FF>Cache-cache avec des objets</color> v2024.5.29!\n\nCréé par <color=#ff00ff>ugackMiner53</color> & <color=#00ffff>fangkuai</color>\nVous pouvez utiliser la touche “<b>R</b>” pour vous transformer en un objet proche\nVous pouvez aussi utiliser la touche “<b>SHIFT</b>” pour traverser les murs (non recommandé)\n(Si vous rencontrez des bugs, rejoignez le groupe 805390622 pour les signaler, sinon taisez-vous)",
                        [StringKey.PingMsText] = "Latence : {0} millisecondes",
                        [StringKey.RemainingAttempts] = "<color=#FF0000>Nombre d'erreurs de tuerie restantes : {0}</color>\nLatence : {1} millisecondes\n",
                        [StringKey.True] = "Activé",
                        [StringKey.False] = "Désactivé",
                        [StringKey.HostSetting] = "\n\nParamètres de l'hôte:\nTemps de cachette : {0}\nNombre maximum d'erreurs de tuerie : {1}\nLes cacheurs deviennent des imposteurs après leur mort : {2}\n"
                };
            }
        }
    }
}
