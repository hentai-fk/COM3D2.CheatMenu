using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        public static void Unload()
        {
            GameObject.Destroy(instance);
            debug = false;
        }
    }
}
