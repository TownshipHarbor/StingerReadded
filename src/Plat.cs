using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Alta;
using Alta.Impact;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;
using GameList = Il2CppSystem.Collections.Generic;

namespace StingerReadded
{
    static class Plat
    {
        static readonly string[] OtherMods = { "StingerReadded", "CustomSkillsAPI" };

        internal const string Name = "Quest";
        internal const string OrbPart = "Orb android";

        internal static bool IsServer()
        {
            return false;
        }

        internal static T As<T>(object value) where T : Il2CppObjectBase
        {
            Il2CppObjectBase proxy = value as Il2CppObjectBase;
            return proxy == null ? null : proxy.TryCast<T>();
        }

        internal static bool Same(Il2CppObjectBase a, Il2CppObjectBase b)
        {
            return a != null && b != null && a.Pointer != IntPtr.Zero && a.Pointer == b.Pointer;
        }

        static GameList.Dictionary<uint, T> Items<T>() where T : HashedGeneralValue<T>
        {
            HashedGeneralValue<T>.CheckItems();
            return HashedGeneralValue<T>.items;
        }

        internal static T Copy<T>(T asset) where T : Object
        {
            Object made = Object.Instantiate((Object)asset);
            T copy = made == null ? null : made.TryCast<T>();
            if (copy == null) throw new InvalidOperationException("could not copy " + asset.name);
            copy.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return copy;
        }

        internal static void SetHash(HashedGeneralValue value, uint hash) { value.hash = (int)hash; }
        internal static void SetDamage(PowerHit skill, float damage) { skill.damageModifierPerLevel = new Il2CppStructArray<float>(new float[] { damage }); }
        internal static string Damage(PowerHit skill)
        {
            Il2CppStructArray<float> values = skill.damageModifierPerLevel;
            if (values == null) return Floats(null);
            float[] copy = new float[values.Length];
            for (int i = 0; i < copy.Length; i++) copy[i] = values[i];
            return Floats(copy);
        }

        internal static ImpactChannel ImpactChannel(PowerHit skill) { return skill.targetImpactChannel; }
        internal static void SetImpactChannel(PowerHit skill, ImpactChannel channel) { skill.targetImpactChannel = channel; }
        internal static SkillInputStructure Input(PickupBasedSkill skill) { return skill.inputStructure; }
        internal static void SetInput(PickupBasedSkill skill, SkillInputStructure input) { skill.inputStructure = input; }

        internal static void CopyHitEffects(PickupBasedSkill to, PickupBasedSkill from)
        {
            to.hitPositionEffect = from.hitPositionEffect;
            to.hitPositionAndBladeAlignedEffect = from.hitPositionAndBladeAlignedEffect;
        }

        internal static Sprite Glyph(ProfessionSkill skill, int index)
        {
            Il2CppReferenceArray<Sprite> glyphs = skill.practiceGlyphSprites;
            return glyphs == null || index >= glyphs.Length ? null : glyphs[index];
        }

        internal static void SetGlyphs(ProfessionSkill skill, Sprite first, Sprite second)
        {
            Il2CppSystem.Array made = Il2CppSystem.Array.CreateInstance(Il2CppType.Of<Sprite>(), 2);
            Il2CppReferenceArray<Sprite> glyphs = new Il2CppReferenceArray<Sprite>(made.Pointer);
            glyphs[0] = first;
            glyphs[1] = second;
            skill.practiceGlyphSprites = glyphs;
        }

        internal static void SetAchievement(ProfessionSkill skill, GameAchievement achievement) { skill.targetAchievement = achievement; }
        internal static PooledObjectDefinition TargetDefinition(PowerStinger skill) { return skill.targetDefinition; }
        internal static PooledObject Prefab(PooledObjectDefinition definition) { return definition.prefab; }
        internal static TransformEvent ChargedSound(SkillInputStructure input) { return input == null ? null : input.chargedDefaultEffect; }

        internal static void CopyChargeSounds(SkillInputStructure to, SkillInputStructure from)
        {
            to.chargedDefaultEffect = from.chargedDefaultEffect;
            to.executedDefaultEffect = from.executedDefaultEffect;
        }

        internal static ProgressionSlot NewSlot(ProfessionSkillTree tree, ProfessionSkill skill)
        {
            GameList.List<ProgressionSlot> list = tree.Slots;
            ProgressionSlot like = list != null && list.Count > 0 ? list[0] : null;
            if (like == null) throw new InvalidOperationException("the " + tree.name + " tree has no slot to copy");
            Il2CppSystem.Object copy = like.MemberwiseClone();
            ProgressionSlot slot = copy == null ? null : copy.TryCast<ProgressionSlot>();
            if (slot == null) throw new InvalidOperationException("could not copy a slot of the " + tree.name + " tree");
            slot.Path = default(ProgressionPath);
            slot.skill = skill;
            slot.name = skill.name;
            slot.cost = 1u;
            slot.dependencies = new GameList.List<uint>();
            slot.hash = 0u;
            slot.indentLevel = -1;
            slot.position = Vector3.zero;
            slot.slotIcon = null;
            slot.inheritSlots = new GameList.List<ProgressionSlot>();
            return slot;
        }

        internal static void FillSlot(ProgressionSlot slot, string name, uint cost, uint[] dependencies, uint hash, Vector3 position, Texture2D icon)
        {
            GameList.List<uint> list = new GameList.List<uint>();
            foreach (uint d in dependencies) list.Add(d);
            slot.name = name;
            slot.cost = cost;
            slot.dependencies = list;
            slot.hash = hash;
            slot.position = position;
            if (icon != null) slot.slotIcon = icon;
        }

        internal static void MapSlot(ProfessionSkillTree tree, ProgressionSlot slot) { tree.slotMap[slot.Hash] = slot; }
        internal static int InheritCount(ProgressionSlot slot) { return slot.inheritSlots == null ? 0 : slot.inheritSlots.Count; }

        internal static string Join(GameList.List<uint> list)
        {
            List<string> parts = new List<string>();
            if (list != null) for (int i = 0; i < list.Count; i++) parts.Add(list[i].ToString(CultureInfo.InvariantCulture));
            return parts.Count == 0 ? "nothing" : string.Join(", ", parts.ToArray());
        }

        internal static Texture2D NewTexture(string name, int width, int height, int mips, FilterMode filter, TextureWrapMode wrap, int aniso, byte[] raw)
        {
            Texture2D t = new Texture2D(width, height, TextureFormat.RGBA32, mips > 1, false);
            if (t.mipmapCount != mips) throw new InvalidOperationException(name + " got " + t.mipmapCount + " mip levels, the pack has " + mips);
            t.name = name;
            t.filterMode = filter;
            t.wrapMode = wrap;
            t.anisoLevel = aniso;
            t.LoadRawTextureData(new Il2CppStructArray<byte>(raw));
            t.Apply(false, true);
            t.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return t;
        }

        internal static ProfessionSkill Skill(uint hash)
        {
            var items = Items<ProfessionSkill>();
            return items != null && items.ContainsKey(hash) ? items[hash] : null;
        }

        internal static PlayerProgressFlag Flag(uint hash)
        {
            var items = Items<PlayerProgressFlag>();
            return items != null && items.ContainsKey(hash) ? items[hash] : null;
        }

        internal static bool Register(ProfessionSkill skill)
        {
            var items = Items<ProfessionSkill>();
            if (items == null || items.Count == 0 || items.ContainsKey(skill.Hash)) return false;
            items.Add(skill.Hash, skill);
            return true;
        }

        internal static string OtherCopy()
        {
            foreach (MelonMod m in MelonMod.RegisteredMelons)
            {
                if (m == null || m.Info == null) continue;
                foreach (string other in OtherMods)
                {
                    if (m.Info.Name == other) return "CGNik's " + other;
                }
            }
            return null;
        }

        internal static string Vec(Vector3 v)
        {
            return "(" + v.x.ToString("0.00", CultureInfo.InvariantCulture) + ", " + v.y.ToString("0.00", CultureInfo.InvariantCulture) + ", " + v.z.ToString("0.00", CultureInfo.InvariantCulture) + ")";
        }

        static string Floats(float[] values)
        {
            if (values == null) return "none";
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0) b.Append('/');
                b.Append(values[i].ToString("0.###", CultureInfo.InvariantCulture));
            }
            return b.ToString();
        }
    }
}
