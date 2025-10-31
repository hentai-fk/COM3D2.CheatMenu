using FacilityFlag;
using HarmonyLib;
using System;
using UnityEngine;

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
                //Traverse.Create(GameMain.Instance.FacilityMgr).Field("FacilityCountMax").SetValue(12);
                //foreach (var item in GameMain.Instance.FacilityMgr.GetFacilityArray())
                //{
                //    ExtendCheatMenu.PluginLogger.LogInfo("facilityName: " + ScriptManager.ReplaceCharaName(item.facilityName));
                //    ExtendCheatMenu.PluginLogger.LogInfo("facilityLevel: " + item.facilityLevel);
                //    ExtendCheatMenu.PluginLogger.LogInfo("facilityIncome: " + item.facilityIncome);
                //    ExtendCheatMenu.PluginLogger.LogInfo("facilityExperienceValue: " + item.facilityExperienceValue);
                //    ExtendCheatMenu.PluginLogger.LogInfo("facilityValuation: " + item.facilityValuation);
                //    ExtendCheatMenu.PluginLogger.LogInfo("GetCurrentExp: " + item.expSystem.GetCurrentExp());
                //    ExtendCheatMenu.PluginLogger.LogInfo("GetCurrentLevel: " + item.expSystem.GetCurrentLevel());
                //    ExtendCheatMenu.PluginLogger.LogInfo("GetMaxLevel: " + item.expSystem.GetMaxLevel());
                //    ExtendCheatMenu.PluginLogger.LogInfo("GetMaxLevelNeedExp: " + item.expSystem.GetMaxLevelNeedExp());
                //    item.expSystem.AddExp(100);
                //}
                foreach (var item in FacilityDataTable.GetAllWorkData(false))
                {
                    //Console.WriteLine("设施 " + ScriptManager.ReplaceCharaName(item.Value.name));
                    //Console.WriteLine("item.Key " + item.Key);
                    //var exp = GameMain.Instance.FacilityMgr.GetFacilityExpSystem(item.Key);
                    //Console.WriteLine("GetCurrentLevel " + exp.GetCurrentLevel());
                    //Console.WriteLine("GetCurrentExp " + exp.GetCurrentExp());
                    //Console.WriteLine("GetMaxLevel " + exp.GetMaxLevel());
                    //Console.WriteLine("GetMaxLevelNeedExp " + exp.GetMaxLevelNeedExp());
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
