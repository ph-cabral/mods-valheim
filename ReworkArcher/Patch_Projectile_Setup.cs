// Decompiled with JetBrains decompiler
// Type: ReworkArcher.Patch_Projectile_Setup
// Assembly: ReworkArcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3468E56E-7664-4488-B8C5-57E8A7E725A8
// Assembly location: P:\SteamLibrary\steamapps\common\Valheim\BepInEx\plugins\ReworkArcher.dll

using HarmonyLib;
using UnityEngine;

#nullable disable
namespace ReworkArcher
{
  [HarmonyPatch(typeof (Projectile), "Setup")]
  public static class Patch_Projectile_Setup
  {
    private static void Postfix(
      Projectile __instance,
      ItemDrop.ItemData item,
      ItemDrop.ItemData ammo)
    {
      if (!ReworkArcher.ReworkArcher.EnableTripleDamage.Value || item == null || item.m_shared.m_skillType != Skills.SkillType.Bows)
        return;
      __instance.m_damage.Modify(ReworkArcher.ReworkArcher.DamageMultiplier.Value);
      Debug.Log((object) string.Format("[ReworkArcher] Daño de flecha multiplicado por {0}", (object) ReworkArcher.ReworkArcher.DamageMultiplier.Value));
    }
  }
}
