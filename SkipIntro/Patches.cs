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
        public static int seq = 0;
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
            public static void Prefix(int seqID)
            {
                seq = seqID;
                if (scrController.deaths == 0)
                {
                    scrController.deaths = 1;
                    ChangeDeath = true;
                }
                if (RDC.auto && seqID == 0 && Persistence.GetSkipIntroAfterFirstTry() && scrConductor.instance.addoffset > GCS.longIntroThresholdSec)
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
                if (ChangeDeath && AudioSettings.dspTime - (double)scrConductor.calibration_i > __instance.conductor.GetCountdownTime(0))
                {
                    scrController.deaths = 0;
                    ChangeDeath = false;
                }
                if (auto && AudioSettings.dspTime - (double)scrConductor.calibration_i > __instance.conductor.GetCountdownTime(0))
                {
                    RDC.auto = true;
                    auto = false;
                }
                if ((auto || RDC.auto) && seq == 0)
                {
                    __instance.CancelGo();
                    scrConductor.instance.fastTakeoff = true;
                    scrController.instance.forceNoCountdown = true;
                }
            }
        }
    }
}
