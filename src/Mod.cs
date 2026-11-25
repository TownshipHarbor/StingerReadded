using System;
using MelonLoader;

[assembly: MelonInfo(typeof(StingerReadded.Mod), "StingerReaddedQuest", StingerReadded.Mod.Version, "CGNik, ported by Shadow")]
[assembly: MelonGame("Alta", "Att_Quest")]
[assembly: MelonAdditionalDependencies("CustomSkillsAPI")]

namespace StingerReadded
{
    public class Mod : MelonMod
    {
        public const string Version = "0.2.0";
        internal static MelonLogger.Instance Log;
        static bool set;

        public override void OnInitializeMelon()
        {
            Log = LoggerInstance;
            string refusal = Setup.Refusal();
            if (refusal != null)
            {
                Log.Warning(refusal + " Nothing was changed.");
                return;
            }
            Log.Msg("StingerReadded " + Version + " for " + Plat.Name + ": CGNik's StingerReadded v10 on CustomSkillsAPI.");
            CustomSkillsAPI.Core.SetUpProgressionSlots += SetUpProgressionSlots;
            CustomSkillsAPI.Core.PostAddInherits += PostAddInherits;
        }

        static void SetUpProgressionSlots()
        {
            string missing = Setup.Missing();
            if (missing != null)
            {
                Log.Error("The game has no " + missing + ". Side Power Hit and Stinger are not on this client.");
                return;
            }
            DateTime start = DateTime.UtcNow;
            Setup.Run();
            set = true;
            Log.Msg("Skills and slots handed to CustomSkillsAPI in " + (int)(DateTime.UtcNow - start).TotalMilliseconds + " ms.");
        }

        static void PostAddInherits()
        {
            if (set) Setup.Report();
        }
    }
}
