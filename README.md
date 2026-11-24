# StingerReadded for Quest

The Quest version of CGNik's StingerReadded. It brings back Stinger, a melee skill cut from the game, and adds Side Power Hit in front of it on the Melee tree of the Warrior Shrine.

Original: https://github.com/CG-Nik/StingerReadded

## Install

Put `StingerReaddedQuest.dll` in the headset's Mods folder, or take it from the Download board:

```
/sdcard/Android/data/com.Alta.ATownshipTale/files/MelonLoader/com.Alta.ATownshipTale/Mods/
```

The server needs CGNik's PC mods, CustomSkillsAPI and StingerReadded. A server without them never offers the two skills, and this mod changes nothing you can see there.

## What it does

On the headset it does what these three PC client mods by CGNik do, in one DLL:

| PC mod | On the headset |
|---|---|
| CustomSkillsAPI v5 | adds the skill and the two tree slots to your game |
| StingerReadded v10 | Side Power Hit (Power Hit, charged with the hand at your side, more damage), the Stinger slot, its achievement and a visible target orb |
| HandOnSideStructurePatch v2 | Power Hit's charge sounds for the hand on side pose |

PC players keep CGNik's own mods.

## Building

`build.ps1` builds `quest\StingerReaddedQuest.csproj` into `build\`. Put LemonLoader's `net8` folder and its generated `Il2CppAssemblies` in `lemon\`. The slot icons in `res\icons.bin` come from CGNik's asset bundle through `tools\bake.py`.

## Credits

StingerReadded, CustomSkillsAPI and HandOnSideStructurePatch are by CGNik, under the MIT licence. Quest port by Shadow.

This is an unofficial, fan made mod for A Township Tale. It is not affiliated with or endorsed by the owners or developers of A Township Tale.
