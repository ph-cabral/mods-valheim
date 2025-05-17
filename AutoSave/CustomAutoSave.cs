// Decompiled with JetBrains decompiler
// Type: CustomAutoSave
// Assembly: AutoSave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5F3ADC15-AD8E-4726-A76E-3261AB342CF8
// Assembly location: P:\SteamLibrary\steamapps\common\Valheim\BepInEx\plugins\AutoSave.dll

using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections;
using UnityEngine;

#nullable disable
[BepInPlugin("pablo.customautosave", "Auto Save", "1.0.1")]
public class CustomAutoSave : BaseUnityPlugin
{
  private ConfigEntry<int> autoSaveInterval;

  private void Awake()
  {
    this.autoSaveInterval = this.Config.Bind<int>("General", "autoSaveInterval", 300, "Tiempo en segundos entre cada auto-guardado");
    this.Logger.LogInfo((object) string.Format("[CustomAutoSave] Activado. Guardará cada {0} segundos.", (object) this.autoSaveInterval.Value));
    ((MonoBehaviour) this).StartCoroutine(this.AutoSaveRoutine());
  }

  private IEnumerator AutoSaveRoutine()
  {
    while (true)
    {
      yield return (object) new WaitForSeconds((float) this.autoSaveInterval.Value);
      this.Logger.LogInfo((object) string.Format("[CustomAutoSave] Intentando guardar a las {0}", (object) DateTime.Now));
      if ((UnityEngine.Object) ZNet.instance != (UnityEngine.Object) null)
      {
        this.Logger.LogInfo((object) string.Format("[CustomAutoSave] ZNet.instance.IsServer(): {0}", (object) ZNet.instance.IsServer()));
        if (ZNet.instance.IsServer())
        {
          try
          {
            Game.instance.SavePlayerProfile(true);
            this.Logger.LogInfo((object) "[CustomAutoSave] Perfil del jugador guardado.");
            ZNet.instance.Save(true);
            this.Logger.LogInfo((object) "[CustomAutoSave] Mundo guardado forzadamente.");
          }
          catch (Exception ex)
          {
            this.Logger.LogError((object) ("[CustomAutoSave] Error al guardar: " + ex.Message));
          }
        }
        else
          this.Logger.LogWarning((object) "[CustomAutoSave] No es servidor, no se guardará.");
      }
      else
        this.Logger.LogWarning((object) "[CustomAutoSave] ZNet.instance es null, no se puede guardar.");
    }
  }
}
