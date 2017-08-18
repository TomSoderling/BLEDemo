using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Plugin.BluetoothLE;
using Xamarin.Forms;

namespace BLEDemo
{
    public partial class DeviceListPage : ContentPage
    {
        IDisposable scanner;
        private bool isScanning;

        public ObservableCollection<IScanResult> ScannedDevices { get; set; }

        public DeviceListPage()
        {
            InitializeComponent();

            ScannedDevices = new ObservableCollection<IScanResult>();
            scannedDeviceListView.ItemsSource = ScannedDevices;
            scannedDeviceListView.ItemSelected += ScannedDeviceListView_ItemSelectedAsync;

            // Adapter status is always "Unknown" the first time. Not sure why this is.
            var adapterStatus = CrossBleAdapter.Current.Status;
        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            // check the status again - should be PoweredOn now
            var adapterStatus = CrossBleAdapter.Current.Status;
            if (adapterStatus != AdapterStatus.PoweredOn)
            {
                await DisplayAlert("BLE Adapter", $"BLE adapter status is: {adapterStatus}", "Rats");
                return;
            }

            if (isScanning)
                StopScanning();

            scanner = CrossBleAdapter.Current.Scan().Subscribe(scanResult =>
            {
                Debug.WriteLine($"BLE peripheral found. RSSI: {scanResult.Rssi}, Name: {scanResult.Device.Name}");

                // Add unique devices to collection, based on name
                if (scanResult.Device.Name != null && ScannedDevices.Any(sr => sr.Device.Name == scanResult.Device.Name) == false)
                {
                    ScannedDevices.Add(scanResult);
                }
            });

            isScanning = true;
            scanButton.Text = "Stop scanning";
        }

        private async void ScannedDeviceListView_ItemSelectedAsync(object sender, SelectedItemChangedEventArgs e)
        {
            StopScanning();

            var selectedItem = e.SelectedItem as IScanResult;

            var servicesListPage = new ServicesListPage(selectedItem.Device);

            await Navigation.PushAsync(servicesListPage);
        }

        private void StopScanning()
        {
            scanner.Dispose(); // stop scanning

            isScanning = false;
            scanButton.Text = "Start scanning for BLE peripheral devices";
        }
    }
}
