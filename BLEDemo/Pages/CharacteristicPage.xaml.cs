using System;
using System.Collections.Generic;
using Plugin.BluetoothLE;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Text;
using System.Collections.ObjectModel;

namespace BLEDemo
{
    public partial class CharacteristicPage : ContentPage
    {
        public ObservableCollection<CharacteristicWrapper> Characteristics { get; set; }

        public CharacteristicPage(IGattService selectedService)
        {
            InitializeComponent();

            Title = selectedService.Description;

            Characteristics = new ObservableCollection<CharacteristicWrapper>();
            characteristicListView.ItemsSource = Characteristics;
            characteristicListView.ItemSelected += CharacteristicListView_ItemSelected;

            selectedService.WhenCharacteristicDiscovered().Subscribe((ch) =>
            {
                var characteristic = ch as IGattCharacteristic;
                var charProperties = string.Empty;

                if (characteristic.CanRead())
                    charProperties += "read ";
                if (characteristic.CanWrite())
                    charProperties += "write ";
                if (characteristic.CanNotify())
                    charProperties += "notify";

                Debug.WriteLine($"Characteristic discovered. UUID: {characteristic.Uuid }, Desc: {characteristic.Description}, Properties: {charProperties}");

                Characteristics.Add(new CharacteristicWrapper(characteristic.Uuid.ToString(), charProperties, characteristic));
            });
        }

        private async void CharacteristicListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedCharacteristic = e.SelectedItem as CharacteristicWrapper;

            var page = new CharacteristicEditPage(selectedCharacteristic.Characteristic);

            await Navigation.PushAsync(page);
        }


        // Nested class to help display values in ListView
        public class CharacteristicWrapper
        {
            public CharacteristicWrapper(string uuid, string properties, IGattCharacteristic characteristic)
            {
                UUID = uuid;
                Properties = properties;
                Characteristic = characteristic;
            }

            public string UUID { get; set; }
            public string Properties { get; set; }
            public IGattCharacteristic Characteristic { get; set; }
        }
    }
}
