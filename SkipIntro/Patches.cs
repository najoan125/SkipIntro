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
        public static bool auto = false;
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
            public static void Prefix(int seqID, scrConductor __instance)
            {
                if (scrController.deaths == 0)
                {
                    scrController.deaths = 1;
                    ChangeDeath = true;
                }
                if (RDC.auto && seqID == 0 && Persistence.GetSkipIntroAfterFirstTry())
                {
                    RDC.auto = false;
                    auto = true;
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
                }
                if (auto && AudioSettings.dspTime - (double)scrConductor.calibration_i > __instance.conductor.GetCountdownTime(0))
                {
                    auto = false;
                    RDC.auto = true;
                }
            }
        }
    }
}
