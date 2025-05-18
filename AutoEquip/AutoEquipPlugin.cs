// Decompiled with JetBrains decompiler
// Type: AutoEquipMod.AutoEquipPlugin
// Assembly: AutoEquip2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F73D428-F346-47B3-B2E3-303491308768
// Assembly location: P:\SteamLibrary\steamapps\common\Valheim\BepInEx\plugins\AutoEquip.dll

using AdventureBackpacks.API;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace AutoEquipMod
{
  [BepInPlugin("com.tuplugin.autoequip", "AutoEquip", "1.1.0")]
  public class AutoEquipPlugin : BaseUnityPlugin
  {
    private static ConfigEntry<string> swordOption;
    private static ConfigEntry<string> maceOption;
    private static ConfigEntry<string> axeOption;
    private static ConfigEntry<string> knifeOption;
    private static bool isProcessing;
    private static ItemDrop.ItemData lastAutoEquippedItem;

    private void Awake()
    {
      AutoEquipPlugin.swordOption = this.Config.Bind<string>("Equip", "Sword", "Shield", "Opciones: SameWeapon, Shield, None");
      AutoEquipPlugin.maceOption = this.Config.Bind<string>("Equip", "Mace", "Shield", "Opciones: SameWeapon, Shield, None");
      AutoEquipPlugin.axeOption = this.Config.Bind<string>("Equip", "Axe", "SameWeapon", "Opciones: SameWeapon, Shield, None");
      AutoEquipPlugin.knifeOption = this.Config.Bind<string>("Equip", "Knife", "None", "Opciones: SameWeapon, Shield, None");
      Harmony harmony = new Harmony("com.tuplugin.autoequip");
      harmony.Patch((MethodBase) AccessTools.Method(typeof (Player), "EquipItem", (System.Type[]) null, (System.Type[]) null), (HarmonyMethod) null, new HarmonyMethod(typeof (AutoEquipPlugin.Player_Patches), "Postfix_EquipItem", (System.Type[]) null), (HarmonyMethod) null, (HarmonyMethod) null, (HarmonyMethod) null);
      harmony.Patch((MethodBase) AccessTools.Method(typeof (Player), "UnequipItem", (System.Type[]) null, (System.Type[]) null), new HarmonyMethod(typeof (AutoEquipPlugin.Player_Patches), "Prefix_UnequipItem", (System.Type[]) null), (HarmonyMethod) null, (HarmonyMethod) null, (HarmonyMethod) null, (HarmonyMethod) null);
    }

    public static class Player_Patches
    {
      public static void Postfix_EquipItem(Player __instance, ItemDrop.ItemData item)
      {
        if (AutoEquipPlugin.isProcessing || item == null || !AutoEquipPlugin.Player_Patches.IsValidWeapon(item))
          return;
        AutoEquipPlugin.isProcessing = true;
        string optionForWeaponType = AutoEquipPlugin.Player_Patches.GetOptionForWeaponType(item.m_shared.m_skillType);
        List<ItemDrop.ItemData> allPlayerItems = AutoEquipPlugin.Player_Patches.GetAllPlayerItems(__instance);
        ItemDrop.ItemData itemData = (ItemDrop.ItemData) null;
        switch (optionForWeaponType)
        {
          case "sameweapon":
            string baseName = AutoEquipPlugin.Player_Patches.GetBaseWeaponName(item.m_shared.m_name);
            itemData = allPlayerItems.FirstOrDefault<ItemDrop.ItemData>((Func<ItemDrop.ItemData, bool>) (i => i != item && i.m_shared.m_itemType == item.m_shared.m_itemType && AutoEquipPlugin.Player_Patches.GetBaseWeaponName(i.m_shared.m_name) == baseName));
            break;
          case "shield":
            itemData = allPlayerItems.FirstOrDefault<ItemDrop.ItemData>((Func<ItemDrop.ItemData, bool>) (i => i.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Shield));
            break;
        }
        if (itemData != null && !AutoEquipPlugin.Player_Patches.IsItemCurrentlyEquipped(__instance, itemData))
        {
          __instance.EquipItem(itemData);
          AutoEquipPlugin.lastAutoEquippedItem = itemData;
        }
        AutoEquipPlugin.isProcessing = false;
      }

      public static bool Prefix_UnequipItem(Player __instance, ItemDrop.ItemData item)
      {
        if (AutoEquipPlugin.isProcessing || item == null)
          return true;
        ItemDrop.ItemData itemData = typeof (Humanoid).GetField("m_rightItem", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue((object) __instance) as ItemDrop.ItemData;
        if (item == itemData && AutoEquipPlugin.lastAutoEquippedItem != null && AutoEquipPlugin.Player_Patches.IsItemCurrentlyEquipped(__instance, AutoEquipPlugin.lastAutoEquippedItem))
        {
          AutoEquipPlugin.isProcessing = true;
          __instance.UnequipItem(AutoEquipPlugin.lastAutoEquippedItem);
          AutoEquipPlugin.lastAutoEquippedItem = (ItemDrop.ItemData) null;
          AutoEquipPlugin.isProcessing = false;
        }
        return true;
      }

      private static bool IsValidWeapon(ItemDrop.ItemData item)
      {
        ItemDrop.ItemData.ItemType itemType = item.m_shared.m_itemType;
        return itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon || itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon;
      }

      private static string GetBaseWeaponName(string name)
      {
        if (string.IsNullOrEmpty(name))
          return name;
        string[] source = name.Split(' ');
        return source.Length > 1 ? ((IEnumerable<string>) source).Last<string>() : name;
      }

      private static bool IsItemCurrentlyEquipped(Player player, ItemDrop.ItemData item)
      {
        return player.GetInventory().GetEquippedItems().Contains(item);
      }

      private static List<ItemDrop.ItemData> GetAllPlayerItems(Player player)
      {
        List<ItemDrop.ItemData> allPlayerItems = new List<ItemDrop.ItemData>();
        allPlayerItems.AddRange((IEnumerable<ItemDrop.ItemData>) player.GetInventory().GetAllItems());
        if (ABAPI.IsBackpackEquipped(player))
        {
          ABAPI.Backpack? equippedBackpack = ABAPI.GetEquippedBackpack(player);
          if (equippedBackpack.HasValue && equippedBackpack.GetType().GetField("Inventory", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue((object) equippedBackpack) is Inventory inventory)
            allPlayerItems.AddRange((IEnumerable<ItemDrop.ItemData>) inventory.GetAllItems());
        }
        return allPlayerItems;
      }

      private static string GetOptionForWeaponType(Skills.SkillType skillType)
      {
        switch (skillType)
        {
          case Skills.SkillType.Swords:
            return AutoEquipPlugin.swordOption.Value.ToLowerInvariant();
          case Skills.SkillType.Knives:
            return AutoEquipPlugin.knifeOption.Value.ToLowerInvariant();
          case Skills.SkillType.Clubs:
            return AutoEquipPlugin.maceOption.Value.ToLowerInvariant();
          case Skills.SkillType.Axes:
            return AutoEquipPlugin.axeOption.Value.ToLowerInvariant();
          default:
            return "none";
        }
      }
    }
  }
}
