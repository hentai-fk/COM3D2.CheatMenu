using FacilityFlag;
using HarmonyLib;
using Schedule;
using System;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using static VRFaceShortcutConfig;

namespace CheatMenu
{
    internal class HotLoadDebug
    {
        public static bool debug;

        private static GameObject instance;

        public static void Load()
        {
            debug = true;
            instance = new GameObject();
            instance.AddComponent<ExtendCheatMenu>();
            GameObject.DontDestroyOnLoad(instance);


            try
            {
                foreach (var item in ScheduleCSVData.AllData)
                {
                    if (item.Value.type == ScheduleTaskCtrl.TaskType.Work)
                    {
                        Console.WriteLine($"Key " + item.Key);
                        Console.WriteLine($"name " + ScriptManager.ReplaceCharaName(item.Value.name));
                        Console.WriteLine($"isCommu " + item.Value.isCommu);
                        Console.WriteLine($"IsCommon " + item.Value.IsCommon);
                        Console.WriteLine($"information " + item.Value.information);
                        Console.WriteLine($"IsLegacy " + item.Value.IsLegacy);
                        Console.WriteLine($"isNewBodyBlock " + item.Value.isNewBodyBlock);
                        Console.WriteLine($"mode " + item.Value.mode);
                    }
                }
            }
            catch (Exception ex)
            {
                ExtendCheatMenu.PluginLogger.LogError("HotLoadDebug: Error logging load message: " + ex);
            }
        }

        public static void Unload()
        {
            GameObject.Destroy(instance);
            debug = false;
        }
    }
}
