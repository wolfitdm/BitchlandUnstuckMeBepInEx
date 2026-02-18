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
using UMA.Examples;
using UnityEngine;

namespace BitchlandUnstuckMeBepInEx
{
    [BepInPlugin("com.wolfitdm.BitchlandUnstuckMeBepInEx", "BitchlandUnstuckMeBepInEx Plugin", "1.0.0.0")]
    public class BitchlandUnstuckMeBepInEx : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private ConfigEntry<bool> configEnableMe;

        public static void BL_CloseEscMenu()
        {
            Main.Instance.GameplayMenu.OpenEscMenu();
            Main.Instance.GameplayMenu.CloseEscMenu();
        }
        public static void BL_CloseJournal()
        {
            Main.Instance.GameplayMenu.OpenJournal();
            Main.Instance.GameplayMenu.CloseJournal();
        }

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

            PatchAllHarmonyMethods();

            Logger.LogInfo($"Plugin BitchlandUnstuckMeBepInEx BepInEx is loaded!");
        }

        public static void PatchAllHarmonyMethods()
        {
            if (!enableThisMod)
            {
                return;
            }

            try
            {
                PatchHarmonyMethodUnity(typeof(UI_Gameplay), "Update", "bl_ThirdPersonUserControl_Update", true, false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        public static void PatchHarmonyMethodUnity(Type originalClass, string originalMethodName, string patchedMethodName, bool usePrefix, bool usePostfix, Type[] parameters = null)
        {
            string uniqueId = "com.wolfitdm.BitchlandUnstuckMeBepInEx";
            Type uniqueType = typeof(BitchlandUnstuckMeBepInEx);

            // Create a new Harmony instance with a unique ID
            var harmony = new Harmony(uniqueId);

            if (originalClass == null)
            {
                Logger.LogInfo($"GetType originalClass == null");
                return;
            }

            MethodInfo patched = null;

            try
            {
                patched = AccessTools.Method(uniqueType, patchedMethodName);
            }
            catch (Exception ex)
            {
                patched = null;
            }

            if (patched == null)
            {
                Logger.LogInfo($"AccessTool.Method patched {patchedMethodName} == null");
                return;

            }

            // Or apply patches manually
            MethodInfo original = null;

            try
            {
                if (parameters == null)
                {
                    original = AccessTools.Method(originalClass, originalMethodName);
                }
                else
                {
                    original = AccessTools.Method(originalClass, originalMethodName, parameters);
                }
            }
            catch (AmbiguousMatchException ex)
            {
                Type[] nullParameters = new Type[] { };
                try
                {
                    if (patched == null)
                    {
                        parameters = nullParameters;
                    }

                    ParameterInfo[] parameterInfos = patched.GetParameters();

                    if (parameterInfos == null || parameterInfos.Length == 0)
                    {
                        parameters = nullParameters;
                    }

                    List<Type> parametersN = new List<Type>();

                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        ParameterInfo parameterInfo = parameterInfos[i];

                        if (parameterInfo == null)
                        {
                            continue;
                        }

                        if (parameterInfo.Name == null)
                        {
                            continue;
                        }

                        if (parameterInfo.Name.StartsWith("__"))
                        {
                            continue;
                        }

                        Type type = parameterInfos[i].ParameterType;

                        if (type == null)
                        {
                            continue;
                        }

                        parametersN.Add(type);
                    }

                    parameters = parametersN.ToArray();
                }
                catch (Exception ex2)
                {
                    parameters = nullParameters;
                }

                try
                {
                    original = AccessTools.Method(originalClass, originalMethodName, parameters);
                }
                catch (Exception ex2)
                {
                    original = null;
                }
            }
            catch (Exception ex)
            {
                original = null;
            }

            if (original == null)
            {
                Logger.LogInfo($"AccessTool.Method original {originalMethodName} == null");
                return;
            }

            HarmonyMethod patchedMethod = new HarmonyMethod(patched);
            var prefixMethod = usePrefix ? patchedMethod : null;
            var postfixMethod = usePostfix ? patchedMethod : null;

            harmony.Patch(original,
                prefix: prefixMethod,
                postfix: postfixMethod);
        }

        public static GameObject getInteract()
        {
            try
            {
                if (Main.Instance.Player == null || Main.Instance.Player.WeaponInv == null || Main.Instance.Player.WeaponInv.IntLookingAt == null)
                {
                    return null;
                }

                Interactible la = Main.Instance.Player.WeaponInv.IntLookingAt;

                if (la != null)
                {
                    GameObject ga = la.gameObject;
                    return ga;
                }
            }
            catch (Exception e)
            {
            }
            return null;
        }

        public static GameObject getPersonInteract()
        {
            GameObject ga = getInteract();

            if (ga == null)
            {
                return null;
            }

            Interactible la = ga.GetComponent<Interactible>();

            if (la != null)
            {
                if (la is int_Person)
                {
                    int_Person int_thisPerson = (int_Person)la;
                    if (int_thisPerson.ThisPerson != null)
                    {
                        Person thisPerson = int_thisPerson.ThisPerson;
                        return thisPerson.gameObject;
                    }
                }
            }

            return null;
        }

        private static Vector3 explorerSpawnPoint = new Vector3(-2.949118f, 1.192093E-07f, 39.10889f);
        private static Vector3 explorerSpawnPoint2 = new Vector3(179.8053f, 0.05544382f, -73.4415f);
        private static Vector3 hardcoreSpawnPoint = new Vector3(-69f, 0.0f, 10f);
        private static Vector3 hardcoreSpawnPoint2 = new Vector3(-49.10827f, 3.067196f, 14.40517f);
        private static Vector3[] safeSpawnPoints = new Vector3[4];
        public static bool bl_ThirdPersonUserControl_Update(object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            bool isKeyYUp = false;
            bool isKeyF12Up = false;
            bool isKeyZUp = false;

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
                isKeyZUp = Input.GetKeyUp(KeyCode.Z);
            }
            catch (Exception ex)
            {
                isKeyZUp = false;
            }

            try
            {
                isKeyF12Up = Input.GetKeyUp(KeyCode.F12);
            }
            catch (Exception ex)
            {
                isKeyF12Up = false;
            }

            if (!isKeyYUp && !isKeyF12Up && !isKeyZUp)
            {
                return true;
            }

            if (isKeyZUp) {
                try
                {
                    Main.Instance.GameplayMenu.ShowNotification("UNSTUCK ME 4.0 Save to autosave, everytime...It's not guaranteed that this is safe...");
                    Main.Instance.SaveGame(true);
                } catch (Exception ex) {
                }
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
                GameObject personGa = getPersonInteract();
                if (personGa != null)
                {
                    Main.Instance.GameplayMenu.ShowNotification("unstuck me from the chat");
                    Person person = personGa.GetComponent<Person>();
                    if (person != null)
                    {
                        Main.Instance.GameplayMenu.ShowNotification("unstuck me from the chat really");
                        int_Person personInt = person.ThisPersonInt;
                        if (personInt != null)
                        {
                            Main.Instance.GameplayMenu.ShowNotification("unstuck me from the chat really really");
                            personInt.EndTheChat();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.UserControl.enabled = true;
            }
            catch
            {
            }

            try
            {
                Main.Instance.Player.UserControl.ThirdCamPositionType = Main.Instance.Player.UserControl.ThirdCamPositionTypeOnSettings;
            }
            catch
            {
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
            }
            catch (Exception ex)
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
                UI_Gameplay _this = (UI_Gameplay)Main.Instance.GameplayMenu;
                try
                {
                    _this.CloseStorage();
                }
                catch (Exception ex)
                {
                }

                try
                {
                    BL_CloseEscMenu();
                }
                catch (Exception ex)
                {
                }

                try
                {
                    BL_CloseJournal();
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                if (Main.Instance.PeopleFollowingPlayer.Count > 0)
                {
                    Main.Instance.GameplayMenu.ShowNotification("UNSTUCK ME 2.0 Following Player ");
                    for (int i = 0; i < Main.Instance.PeopleFollowingPlayer.Count; i++)
                    {
                        if (Main.Instance.Player.transform != null && Main.Instance.PeopleFollowingPlayer[i].transform != null)
                        {
                            Main.Instance.GameplayMenu.ShowNotification("UNSTUCK ME MINI F8 2.0 Following Player ");
                            Main.Instance.PeopleFollowingPlayer[i].transform.position = Main.Instance.Player.transform.position;
                        }
                    }
                }
            }
            catch (Exception ex)
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
