using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using Den.Tools;
using HarmonyLib;
using HarmonyLib.Tools;
using SemanticVersioning;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Json;
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

        private static Vector3 explorerSpawnPoint = new Vector3(-2.949118f, 1.192093E-07f, 39.10889f);
        private static Vector3 explorerSpawnPoint2 = new Vector3(179.8053f, 0.05544382f, -73.4415f);
        private static Vector3 hardcoreSpawnPoint = new Vector3(-69f, 0.0f, 10f);
        private static Vector3 hardcoreSpawnPoint2 = new Vector3(-49.10827f, 3.067196f, 14.40517f);
        private static Vector3[] safeSpawnPoints = new Vector3[4];

        [HarmonyPatch(typeof(UI_Gameplay), "Update")]
        [HarmonyPrefix] // call after the original method is called
        public static bool bl_ThirdPersonUserControl_Update(object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            bool isKeyYUp = false;
            bool isKeyF12Up = false;

            try
            {
                isKeyYUp = Input.GetKeyUp(KeyCode.Y);
            }
            catch (Exception ex)
            {
                isKeyYUp = false;
            }

            try
            {
                isKeyF12Up = Input.GetKeyUp(KeyCode.F12);
            }
            catch (Exception ex)
            {
                isKeyF12Up = false;
            }

            if (!isKeyYUp && !isKeyF12Up)
            {
                return true;
            }

            if (isKeyF12Up)
            {
                try
                {
                    Vector3 lastSpawnPoint = Main.Instance.Player.transform.position;
                    //Main.Instance.GameplayMenu.ShowNotification(lastSpawnPoint.x.ToString() + " " + lastSpawnPoint.y.ToString() + " " + lastSpawnPoint.z.ToString());
                    safeSpawnPoints[0] = explorerSpawnPoint;
                    safeSpawnPoints[1] = explorerSpawnPoint2;
                    safeSpawnPoints[2] = hardcoreSpawnPoint;
                    safeSpawnPoints[3] = hardcoreSpawnPoint2;
                    int index = UnityEngine.Random.Range(0, safeSpawnPoints.Length);
                    Vector3 spawnPoint = safeSpawnPoints[index];
                    Main.Instance.Player.transform.position = spawnPoint;
                   // Main.Instance.GameplayMenu.ShowNotification(lastSpawnPoint.x.ToString() + " " + lastSpawnPoint.y.ToString() + " " + lastSpawnPoint.z.ToString());
                }
                catch (Exception ex)
                {
                }
                return true;
            }

            try
            {
                Main.Instance.Player.RunBlockers.Clear();
            }
            catch
            {

                try
                {
                    Main.Instance.Player.RunBlockers = new List<string>();
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                Main.Instance.Player.MoveBlockers.Clear();
            }
            catch
            {

                try
                {
                    Main.Instance.Player.MoveBlockers = new List<string>();
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                Main.Instance.Player.ThisPersonInt.InteractBlockers.Clear();
            }
            catch
            {

                try
                {
                    Main.Instance.Player.ThisPersonInt.InteractBlockers = new List<string>();
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                Main.Instance.Player.CanMove = true;
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.InteractingWith.CanLeave = true;
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.Interacting = false;
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.InCombat = false;
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.CanSaveFlags.Remove("CantMoveNow");
            }
            catch
            {
            }

            try
            {
                Main.Instance.CanSaveFlags = Main.Instance.CanSaveFlags;
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.SleepMenu.SetActive(false);
            }
            catch (Exception e)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.EscMenu.SetActive(false);
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.TextInputMenu.SetActive(false);
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.TraderMenu.SetActive(false);
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.EnableMove();
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.AllowCursor();
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.EndChat();
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.UserControl.UnstuckPlayer();
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.UserControl.m_Character.m_Animator.SetFloat("Forward", 0.5f);
            } catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.UserControl.m_Character.m_Animator.SetFloat("Turn", 0.5f);
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.UserControl.m_Character.m_Rigidbody.velocity = Vector3.one;
            }
            catch (Exception ex)
            {
            }

            try
            {
                if (Main.Instance.PeopleFollowingPlayer.Count > 0)
                {
                    Main.Instance.GameplayMenu.ShowNotification("UNSTUCK ME 2.0 Following Player ");
                    int index = Main.Instance.PeopleFollowingPlayer.Count - 1;
                    if (Main.Instance.Player.transform != null && Main.Instance.PeopleFollowingPlayer[index].transform != null)
                    {
                        Main.Instance.GameplayMenu.ShowNotification("UNSTUCK ME MINI F8 2.0 Following Player ");
                        Main.Instance.PeopleFollowingPlayer[index].transform.position = Main.Instance.Player.transform.position;
                    }
                }
            } catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.ShowNotification("UNSTUCK ME 3.0!");
            }
            catch (Exception ex)
            {
            }

            try
            {
                Logger.LogInfo("UNSTUCK ME REALLLY!");
            }
            catch (Exception ex)
            {
            }

            return true;
        }
    }
}
