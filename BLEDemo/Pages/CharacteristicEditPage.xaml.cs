using System;
using System.Collections.Generic;
using Plugin.BluetoothLE;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BLEDemo
{
    public partial class CharacteristicEditPage : ContentPage
    {
        public string CharacteristicValue { get; set; }
         
        public CharacteristicEditPage(IGattCharacteristic selectedCharacteristic)
        {
            InitializeComponent();

            Title = selectedCharacteristic.Uuid.ToString();
                                          

            if (selectedCharacteristic.CanRead())
            {
                ReadCharacteristicAsync(selectedCharacteristic);
            }

            if (selectedCharacteristic.CanWrite())
            {
                //await Characteristic.Write(bytes);
            }

            if (selectedCharacteristic.CanNotify())
            {
                // TODO: couldn't get this to work for the demo.
                // The docs show this, but SetNotificationValue() doesn't exist in IGattCharacteristic
                //var success = selectedCharacteristic.SetNotificationValue(CharacteristicConfigDescriptorValue.Notify);

                // TODO: this didn't seem to do much
                selectedCharacteristic.SubscribeToNotifications();
                selectedCharacteristic.WhenNotificationReceived().Subscribe(result => 
                {  
                    //result.Data
                });
            }
        }

        private async Task ReadCharacteristicAsync(IGattCharacteristic ch)
        {
            var charResult = await ch.Read();
            var theData = charResult.Data;

            Device.BeginInvokeOnMainThread(() =>
            {
                characteristicValue.Text = theData[0].ToString();
            });
        }
    }
}
