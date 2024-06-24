# Mod Support - Skill
## Access Levels

- **Public Access**: All Skill References
- **Private Access**: Skill Setter (Only for Serializer)

## Required Feature

<b>Modding Support<br/>(Add new skill at runtime or load from external file)</b>

### How to Implement?

The external file should contain the following data structure:

1. **Skill Script (C#)**: Inherits SkillBase
2. **Skill Data**: Name, IconSprite, MaxLevel, etc.<br/>
   (Should be referenced by the Skill Script)
3. **Action Script (C#)**: Inherits ActionBase.<br/>
   (Should be referenced by the Skill Script)
4. **Action Data**: Name, Description, Duration, etc.<br/>
   (Should be referenced by the Action Script)
5. **SkillCategory Data**:<br/>
   Almost everything is included in the Skill Script.<br/>
   Some miscellaneous data like Order priority, etc.

## Modding Support

For Modding Support, the external file should be placed in the specific folder. The game should load the file at runtime, and register the skill to the Player.

### Things to Consider

1. **How to load/Register the file?**<br/>
   Use a Custom Loader in Serializer.
2. **How to reference the scripts, especially in WebGL?**<br/>
   ``` C#
   Application.ExternalEval or Application.ExternalCall
   ```
   Need to think more...