using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CheatMenu
{
    internal class HotLoaderInterface
    {
        private static GameObject instance;

        public static void Load()
        {
            instance = new GameObject();
            instance.AddComponent<CheatMenu>();
            GameObject.DontDestroyOnLoad(instance);
        }

        public static void Unload()
        {
            GameObject.Destroy(instance);
        }
    }
}
