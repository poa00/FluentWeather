﻿
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FluidWeather.ViewModels
{
    public class AppViewModel : ObservableObject
    {
        //delegate to request ui update
        //public delegate void RequestUiUpdateDelegate();
        //public event RequestUiUpdateDelegate RequestUiUpdate;

        public event Action UpdateUIAction;

        //https://stackoverflow.com/a/4378380/9008381
        public void UpdateUI()
        {
            UpdateUIAction?.Invoke();
        }
    }
}
