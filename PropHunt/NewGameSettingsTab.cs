using System;
using UnityEngine;
using HarmonyLib;
using BepInEx;
using AmongUsSpecimen.ModOptions;
using Reactor.Utilities;


namespace PropHunt
{
    public static class NewGameSettingsTabPatch
    {
       public static ModOptionTab PropHuntSpecialTab;
       public static ModBoolOption EnableInvisible;   

       public static ModBoolOption EnableSpeed;  
       public static void AddNewOptions()
       {
         PropHuntSpecialTab =  new ModOptionTab("1", "PropHunt-Special Tab", PicturesLoad.loadSpriteFromResources("PropHunt.Resources.tabicon.png", 100f));
         EnableInvisible = new ModBoolOption(PropHuntSpecialTab, "Should The Prop be Invisible Instead of disguising", false, null);
         EnableSpeed = new ModBoolOption(PropHuntSpecialTab, "Should The Prop Speed Get 2X Instead of disguising (cool with invisible)", false, null);
         Logger<PropHuntPlugin>.Instance.LogMessage("Options Added !");
       }
    }
}