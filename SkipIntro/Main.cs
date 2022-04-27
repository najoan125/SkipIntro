using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;

namespace SkipIntro
{
    public static class Main
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static Harmony harmony;
        public static bool IsEnabled = false;
        public static bool isplaying = false;

        public static void Setup(UnityModManager.ModEntry modEntry)
        {
            Logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnUpdate = OnUpdate;
        }

        private static void OnUpdate(UnityModManager.ModEntry modentry, float deltaTime)
        {
            if (!scrController.instance || !scrConductor.instance)
            {
                return;
            }
            isplaying = !scrController.instance.paused && scrConductor.instance.isGameWorld;
            if(!isplaying && Patches.auto)
            {
                Patches.auto = false;
                RDC.auto = true;
            }
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            IsEnabled = value;

            if (value)
            {
                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            else
            {
                harmony.UnpatchAll(modEntry.Info.Id);
            }
            return true;
        }
    }
}
