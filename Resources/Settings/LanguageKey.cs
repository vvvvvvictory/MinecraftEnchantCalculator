﻿using System.ComponentModel;
using System.Configuration;

namespace MinecraftEnchantCalculator.Resources.Settings
{
  // 通过此类可以处理设置类的特定事件: 
  //  在更改某个设置的值之前将引发 SettingChanging 事件。
  //  在更改某个设置的值之后将引发 PropertyChanged 事件。
  //  在加载设置值之后将引发 SettingsLoaded 事件。
  //  在保存设置值之前将引发 SettingsSaving 事件。
  internal sealed partial class LanguageKey
  {
    private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
    {
      // 在此处添加用于处理 SettingChangingEvent 事件的代码。
    }

    private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
    {
      // 在此处添加用于处理 SettingsSaving 事件的代码。
    }
  }
}