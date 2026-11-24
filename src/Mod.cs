using System;
using MelonLoader;

[assembly: MelonInfo(typeof(StingerReadded.Mod), "StingerReaddedQuest", StingerReadded.Mod.Version, "CGNik, ported by Shadow")]
[assembly: MelonGame("Alta", "Att_Quest")]

namespace StingerReadded
{
    public class Mod : MelonMod
    {
        public const string Version = "0.1.0";
        const int RetryEvery = 60;
        const int GiveUpAfter = 3600;
        internal static MelonLogger.Instance Log;
        static bool finished;
        static int frames;
        static string waitingFor;

        public override void OnInitializeMelon()
        {
            Log = LoggerInstance;
            Log.Msg("StingerReadded " + Version + " for " + Plat.Name + ": CGNik's StingerReadded v10, CustomSkillsAPI v5 and HandOnSideStructurePatch v2 in one client mod.");
        }

        public override void OnLateInitializeMelon()
        {
            Attempt("late start");
        }

        public override void OnUpdate()
        {
            if (finished) return;
            frames++;
            if (frames % RetryEvery != 0) return;
            if (frames >= GiveUpAfter)
            {
                finished = true;
                Log.Error("Gave up after " + frames + " frames: the game never loaded " + waitingFor + ". Side Power Hit and Stinger are not on this client.");
                return;
            }
            Attempt("frame " + frames);
        }

        static void Attempt(string when)
        {
            if (finished) return;
            string refusal = Setup.Refusal();
            if (refusal != null)
            {
                finished = true;
                Log.Warning(refusal + " Nothing was changed.");
                return;
            }
            string missing = Setup.Missing();
            if (missing != null)
            {
                if (missing != waitingFor) Log.Msg("Waiting for " + missing + " (" + when + ").");
                waitingFor = missing;
                return;
            }
            finished = true;
            DateTime start = DateTime.UtcNow;
            try
            {
                Setup.Run();
                Log.Msg("Ready at " + when + " in " + (int)(DateTime.UtcNow - start).TotalMilliseconds + " ms.");
            }
            catch (Exception e)
            {
                Log.Error("Setup failed at " + when + ": " + e);
            }
        }
    }
}
