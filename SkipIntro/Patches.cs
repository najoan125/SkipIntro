using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkipIntro
{
    public static class Patches
    {
        public static bool ChangeDeath = false;
        [HarmonyPatch(typeof(scrConductor), "StartMusic")]
        public static class StartMusicPatch
        {
            public static void Prefix()
            {
                if (scrController.deaths == 0 && Main.isplaying)
                {
                    scrController.deaths = 1;
                    ChangeDeath = true;
                }
            }
        }

        [HarmonyPatch(typeof(CustomLevel),"Play")]
        public static class CusotmLevelPlayPatch
        {
            public static void Prefix()
            {
                if (scrController.deaths == 0)
                {
                    scrController.deaths = 1;
                    ChangeDeath = true;
                }
            }
        }

        [HarmonyPatch(typeof(scrCountdown),"Update")]
        public static class ShowGetReadyPatch
        {
            public static void Postfix(scrCountdown __instance)
            {
                if (ChangeDeath && __instance.controller.goShown)
                {
                    scrController.deaths = 0;
                    ChangeDeath = false;
                    Main.Logger.Log("deaths = 0");
                }
            }
        }
    }
}
