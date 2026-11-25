# StingerReadded for Quest

The Quest version of CGNik's StingerReadded. It brings back Stinger, a melee skill cut from the game, and adds Side Power Hit in front of it on the Melee tree of the Warrior Shrine.

Original: https://github.com/CG-Nik/StingerReadded

## Install

Put these in the headset's Mods folder, or take StingerReadded from the Download board (the other two come with it):

```
/sdcard/Android/data/com.Alta.ATownshipTale/files/MelonLoader/com.Alta.ATownshipTale/Mods/
```

- `StingerReaddedQuest.dll` (this build)
- `CustomSkillsAPI.dll`, the Quest build of CGNik's CustomSkillsAPI (needed)
- `HandOnSideStructurePatch.dll`, the Quest build of CGNik's HandOnSideStructurePatch (the charge sounds; CGNik highly suggests it)

The server needs CGNik's PC mods, CustomSkillsAPI and StingerReadded. A server without them never offers the two skills, and this mod changes nothing you can see there.

## What it does

The same as CGNik's StingerReadded v10 on PC, through CustomSkillsAPI: Side Power Hit (Power Hit, charged with the hand at your side, more damage), the Stinger slot, its achievement and a visible target orb.

## Differences from the PC DLL

| PC v10 | Quest build |
|---|---|
| Loads Side Power Hit from an asset bundle | Copies the game's own Power Hit and sets what CGNik's asset changes (name, hash 51077, glyphs, the hand on side pose, damage 1.8): PC bundles don't load on Android |
| The slot icons come from the bundle | The same pixels, baked into the DLL by `tools\bake.py` |
| Turns on the orb's `Orb windows` part | Turns on `Orb android`, the Quest build's name for it |
| Reflection on private fields | The IL2CPP properties for the same fields |

PC players keep CGNik's own mods.

## Building

`build.ps1` builds `quest\StingerReaddedQuest.csproj` into `build\`. Put LemonLoader's `net8` folder and its generated `Il2CppAssemblies` in `lemon\`, and the Quest `CustomSkillsAPI.dll` in `lib\`. The slot icons in `res\icons.bin` come from CGNik's asset bundle through `tools\bake.py`.

## Credits

StingerReadded is by CGNik, under the MIT licence. Quest port by Shadow.

This is an unofficial, fan made mod for A Township Tale. It is not affiliated with or endorsed by the owners or developers of A Township Tale.
