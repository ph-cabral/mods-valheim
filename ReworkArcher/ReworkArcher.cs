// Decompiled with JetBrains decompiler
// Type: ReworkArcher.ReworkArcher
// Assembly: ReworkArcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3468E56E-7664-4488-B8C5-57E8A7E725A8
// Assembly location: P:\SteamLibrary\steamapps\common\Valheim\BepInEx\plugins\ReworkArcher.dll

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using UnityEngine;

#nullable disable
namespace ReworkArcher
{
  [BepInPlugin("pablocabral.mods.reworkarcher", "ReworkArcher", "1.0.0")]
  public class ReworkArcher : BaseUnityPlugin
  {
    private const string ModName = "ReworkArcher";
    private const string ModVersion = "1.0.0";
    private const string ModGUID = "pablocabral.mods.reworkarcher";
    private readonly Harmony _harmony = new Harmony("pablocabral.mods.reworkarcher");
    public static ReworkArcher.ReworkArcher Instance;
    public static ConfigEntry<bool> EnableTripleDamage;
    public static ConfigEntry<float> DamageMultiplier;

    private void Awake()
    {
      ReworkArcher.ReworkArcher.Instance = this;
      ReworkArcher.ReworkArcher.EnableTripleDamage = this.Config.Bind<bool>("General", "EnableTripleDamage", true, "Activa o desactiva el multiplicador de daño con arcos.");
      ReworkArcher.ReworkArcher.DamageMultiplier = this.Config.Bind<float>("General", "DamageMultiplier", 3f, "Multiplicador de daño para flechas (por defecto 3).");
      ReworkArcher.ReworkArcher.EnableTripleDamage.SettingChanged += (EventHandler) ((_param1, _param2) => Debug.Log((object) ("[ReworkArcher] Triple daño: " + (ReworkArcher.ReworkArcher.EnableTripleDamage.Value ? "ACTIVADO" : "DESACTIVADO"))));
      ReworkArcher.ReworkArcher.DamageMultiplier.SettingChanged += (EventHandler) ((_param1, _param2) => Debug.Log((object) string.Format("[ReworkArcher] Nuevo multiplicador de daño: {0}", (object) ReworkArcher.ReworkArcher.DamageMultiplier.Value)));
      this._harmony.PatchAll();
    }
  }
}
