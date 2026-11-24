using System;
using System.Collections.Generic;
using UnityEngine;

namespace StingerReadded
{
    static class Skills
    {
        static readonly List<ProgressionSlot> slots = new List<ProgressionSlot>();

        internal static void AddProfessionSkill(ProfessionSkill skill, Sprite first, Sprite second)
        {
            Plat.SetGlyphs(skill, first, second);
            if (!Plat.Register(skill)) throw new InvalidOperationException("the game's skill list already holds " + skill.Hash + " or never loaded");
        }

        internal static ProgressionSlot AddProgressionSlot(ProfessionSkill skill, string name, uint cost, uint[] dependencies, uint hash, Vector3 position, ProgressionPath path, Texture2D icon)
        {
            ProfessionSkillTree tree = ProfessionSkillTree.GetProfessionTree(path);
            ProgressionSlot slot = Plat.NewSlot(tree, skill);
            Plat.FillSlot(slot, name, cost, dependencies, hash, position, icon);
            slot.Validate(tree);
            slots.Add(slot);
            return slot;
        }

        internal static void Commit()
        {
            foreach (ProgressionSlot slot in slots)
            {
                ProfessionSkillTree tree = ProfessionSkillTree.GetProfessionTree(slot.Path);
                Plat.MapSlot(tree, slot);
                tree.Slots.Add(slot);
            }
            foreach (ProgressionSlot slot in slots)
            {
                slot.AddInherit(ProfessionSkillTree.GetProfessionTree(slot.Path));
            }
        }
    }
}
