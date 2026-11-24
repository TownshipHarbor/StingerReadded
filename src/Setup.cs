using UnityEngine;

namespace StingerReadded
{
    static class Setup
    {
        internal const uint PowerHitHash = 51076u;
        internal const uint PowerStingerHash = 60198u;
        internal const uint SidePowerHitHash = 51077u;
        internal const uint StingerSlotHash = 60199u;
        internal const uint SidePowerHitNeeds = 7141u;
        internal const uint HighLevelMeleeAchievement = 7222u;
        internal const float SidePowerHitDamage = 1.8f;

        internal static string Refusal()
        {
            if (Plat.IsServer()) return "This is the client half: a server runs CGNik's StingerReadded and CustomSkillsAPI instead.";
            string other = Plat.OtherCopy();
            if (other != null) return other + " is loaded too and does this job already, so this port stands aside.";
            return null;
        }

        internal static string Missing()
        {
            if (Plat.As<PowerHit>(Plat.Skill(PowerHitHash)) == null) return "Power Hit (" + PowerHitHash + ")";
            if (Plat.As<PowerStinger>(Plat.Skill(PowerStingerHash)) == null) return "Power Stinger (" + PowerStingerHash + ")";
            if (ProfessionSkillTree.GetProfessionTree(ProgressionPath.Melee) == null) return "the Melee skill tree";
            return null;
        }

        internal static void Run()
        {
            PowerHit powerHit = Plat.As<PowerHit>(Plat.Skill(PowerHitHash));
            PowerStinger powerStinger = Plat.As<PowerStinger>(Plat.Skill(PowerStingerHash));
            ProfessionSkillTree melee = ProfessionSkillTree.GetProfessionTree(ProgressionPath.Melee);
            int before = melee.Slots.Count;
            if (Plat.Skill(SidePowerHitHash) != null) throw new System.InvalidOperationException("the game already has a skill " + SidePowerHitHash);

            HandOnSidePatch(powerStinger, powerHit);

            PowerHit side = Plat.Copy(powerHit);
            side.name = "Side Power Hit";
            Plat.SetHash(side, SidePowerHitHash);
            Plat.SetDamage(side, SidePowerHitDamage);
            Plat.SetImpactChannel(side, Plat.ImpactChannel(powerHit));
            Plat.SetInput(side, Plat.Input(powerStinger));
            Plat.CopyHitEffects(side, powerHit);
            Skills.AddProfessionSkill(side, Plat.Glyph(powerStinger, 0), Plat.Glyph(powerHit, 1));
            Skills.AddProgressionSlot(side, "Side Power Hit", 1u, new uint[] { SidePowerHitNeeds }, SidePowerHitHash, new Vector3(0f, 0.1f, 0.1f), ProgressionPath.Melee, Icons.Get("SidePowerHitSlotIcon"));

            GameAchievement achievement = Plat.As<GameAchievement>(Plat.Flag(HighLevelMeleeAchievement));
            if (achievement != null) Plat.SetAchievement(powerStinger, achievement);
            else Mod.Log.Warning("The game has no achievement " + HighLevelMeleeAchievement + ", Stinger keeps its own.");
            string orb = ShowOrb(powerStinger);

            Skills.AddProgressionSlot(powerStinger, "Power Stinger Readded", 1u, new uint[] { SidePowerHitHash }, StingerSlotHash, new Vector3(0f, 0.1f, 0.3f), ProgressionPath.Melee, Icons.Get("StingerSlotIcon"));

            Skills.Commit();
            Report(melee, before, side, powerHit, powerStinger, achievement, orb);
        }

        static void HandOnSidePatch(PowerStinger stinger, PowerHit hit)
        {
            SkillInputStructure handOnSide = stinger.InputStructure;
            SkillInputStructure raiseHand = hit.InputStructure;
            Plat.CopyChargeSounds(handOnSide, raiseHand);
        }

        static string ShowOrb(PowerStinger stinger)
        {
            PooledObjectDefinition definition = Plat.TargetDefinition(stinger);
            SkillHitTarget target = definition == null ? null : Plat.As<SkillHitTarget>(Plat.Prefab(definition));
            if (target == null)
            {
                Mod.Log.Warning("Stinger has no target orb prefab, it stays as the game made it.");
                return "none";
            }
            Transform part = target.transform.Find(Plat.OrbPart);
            if (part == null)
            {
                Mod.Log.Warning("The Stinger orb has no part named " + Plat.OrbPart + ", it stays invisible.");
                return "missing";
            }
            part.gameObject.SetActive(true);
            return part.name;
        }

        static void Report(ProfessionSkillTree melee, int before, PowerHit side, PowerHit powerHit, PowerStinger stinger, GameAchievement achievement, string orb)
        {
            ProgressionSlot sideSlot, stingerSlot;
            bool hasSide = melee.GetSlot(SidePowerHitHash, out sideSlot);
            bool hasStinger = melee.GetSlot(StingerSlotHash, out stingerSlot);
            ProfessionSkill registered = Plat.Skill(SidePowerHitHash);
            Mod.Log.Msg("Side Power Hit: skill " + side.Hash + " registered " + Plat.Same(registered, side) + ", copy of " + powerHit.name + ", damage " + Plat.Damage(side) + " (Power Hit " + Plat.Damage(powerHit) + "), pose " + Name(Plat.Input(side)) + ", glyphs " + Name(Plat.Glyph(side, 0)) + " + " + Name(Plat.Glyph(side, 1)) + ".");
            Mod.Log.Msg("Melee tree: " + before + " slots before, " + melee.Slots.Count + " now.");
            Mod.Log.Msg("Slot " + SidePowerHitHash + ": " + Slot(hasSide, sideSlot, melee));
            Mod.Log.Msg("Slot " + StingerSlotHash + ": " + Slot(hasStinger, stingerSlot, melee));
            Mod.Log.Msg("Stinger: achievement " + (achievement == null ? "unchanged" : achievement.name) + ", orb part shown: " + orb + ", hand on side sounds " + Name(Plat.ChargedSound(stinger.InputStructure)) + ".");
        }

        static string Slot(bool found, ProgressionSlot slot, ProfessionSkillTree tree)
        {
            if (!found || slot == null) return "NOT in the tree";
            return "'" + slot.Name + "' skill " + (slot.Skill == null ? "none" : slot.Skill.name) + ", cost " + slot.Cost + ", needs " + Plat.Join(slot.Dependencies) + ", indent " + slot.GetIndentLevel(tree) + ", at " + Plat.Vec(slot.Position) + ", icon " + (slot.SlotIcon == null ? "none" : slot.SlotIcon.name + " " + slot.SlotIcon.width + "x" + slot.SlotIcon.height) + ", unlocks " + Plat.InheritCount(slot) + " slot(s), path " + slot.Path;
        }

        static string Name(Object o)
        {
            return o == null ? "none" : o.name;
        }

        static string Name(object o)
        {
            return o == null ? "none" : "set";
        }
    }
}
