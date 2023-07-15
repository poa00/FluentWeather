﻿using FluidWeather.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FluidWeather.Helpers;
using FluidWeather.ViewModels;

namespace FluidWeather.Dialogs
{
    public sealed partial class FirstRunDialog : ContentDialog
    {
        private static HttpClient sharedClient = new()
        {
            BaseAddress = new Uri("https://api.weather.com/v3/"),
        };

        private AppViewModel AppViewModel =  AppViewModelHolder.GetViewModel();


        public FirstRunDialog()
        {
            // TODO: Update the contents of this dialog with any important information you want to show when the app is used for the first time.
            RequestedTheme = ((FrameworkElement) Window.Current.Content).RequestedTheme;

            this.Closing += DialogClosingEvent;

            /*var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;



            this.Resources["ContentDialogMaxWidth"] = bounds.Width*scaleFactor;
            this.Resources["ContentDialogMaxHeight"] = bounds.Height*scaleFactor;*/

            //make the primary button colored
            this.DefaultButton = ContentDialogButton.Primary;

            //disable primary button until user selects a location
            this.IsPrimaryButtonEnabled = false;

            //FullSizeDesired = true;

            InitializeComponent();

            /*MainGrid.Width = bounds.Width * scaleFactor;
            MainGrid.Height = bounds.Height*scaleFactor;*/
        }


        //prevent dialog dismiss by escape key
        private void DialogClosingEvent(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            // This mean user does click on Primary or Secondary button
            if (args.Result == ContentDialogResult.None)
            {
                args.Cancel = true;
            }
            else
            {
                AppViewModel.UpdateUI();
            }
        }


        private async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Since selecting an item will also change the text,
            // only listen to changes caused by user entering text.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput
                && !string.IsNullOrEmpty(sender.Text))
            {
                var language = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];

                var response = await sharedClient.GetAsync("location/searchflat?query=" + sender.Text + "&language=" +
                                                           language +
                                                           "&apiKey=793db2b6128c4bc2bdb2b6128c0bc230&format=json");

                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                else
                {

                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    var myDeserializedClass = JsonConvert.DeserializeObject<SearchLocationResponse>(jsonResponse);

                    foreach (var location in myDeserializedClass.location)
                    {
                        Debug.WriteLine(location.address);
                    }


                    List<SearchedLocation> finalitems = myDeserializedClass.location.Select(x => x).ToList();
                    // the select statement above is the same as the foreach below


                    sender.ItemsSource = finalitems;

                }
            }
        }

        private async void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender,
            AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var selectedPlaceId = ((SearchedLocation) args.SelectedItem).placeId;

            //save location to settings
            await ApplicationData.Current.LocalSettings.SaveAsync("lastPlaceId", selectedPlaceId);

            this.IsPrimaryButtonEnabled = true;
        }
    }
}
