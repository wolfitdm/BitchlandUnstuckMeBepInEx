using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using Den.Tools;
using HarmonyLib;
using SemanticVersioning;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine;

namespace BitchlandUnstuckMeBepInEx
{
    [BepInPlugin("com.wolfitdm.BitchlandUnstuckMeBepInEx", "BitchlandUnstuckMeBepInEx Plugin", "1.0.0.0")]
    public class BitchlandUnstuckMeBepInEx : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private ConfigEntry<bool> configEnableMe;

        public BitchlandUnstuckMeBepInEx()
        {
        }

        public static Type MyGetType(string originalClassName)
        {
            return Type.GetType(originalClassName + ",Assembly-CSharp");
        }

        private static string pluginKey = "General.Toggles";

        public static bool enableThisMod = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            configEnableMe = Config.Bind(pluginKey,
                                              "EnableThisMod",
                                              true,
                                             "Whether or not you want enable this mod (default true also yes, you want it, and false = no)");


            enableThisMod = configEnableMe.Value;

            Harmony.CreateAndPatchAll(typeof(BitchlandUnstuckMeBepInEx));

            Logger.LogInfo($"Plugin BitchlandUnstuckMeBepInEx BepInEx is loaded!");
        }

        [HarmonyPatch(typeof(Person), "SleepOnFloor")]
        [HarmonyPrefix] // call after the original method is called
        public static bool SleepOnFloor(object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            Main.Instance.Player.RunBlockers = new List<string>();
            Main.Instance.Player.MoveBlockers = new List<string>();
            Main.Instance.Player.ThisPersonInt.InteractBlockers = new List<string>();
            Main.Instance.Player.CanMove = true;
            Main.Instance.Player.InteractingWith.CanLeave = true;
            Main.Instance.Player.Interacting = false;
            Main.Instance.Player.InCombat = false;
            Main.Instance.CanSaveFlags.Remove("CantMoveNow");
            Main.Instance.CanSaveFlags = Main.Instance.CanSaveFlags;
            Main.Instance.GameplayMenu.SleepMenu.SetActive(false);
            Main.Instance.GameplayMenu.EscMenu.SetActive(false);
            Main.Instance.GameplayMenu.TextInputMenu.SetActive(false);
            Main.Instance.GameplayMenu.TraderMenu.SetActive(false);
            Main.Instance.GameplayMenu.EnableMove();
            Main.Instance.GameplayMenu.AllowCursor();

            return true;
        }
    }
}
