using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Plugin.BluetoothLE;
using Xamarin.Forms;

namespace BLEDemo
{
    public partial class ServicesListPage : ContentPage
    {
        public ObservableCollection<IGattService> Services { get; set; }

        public ServicesListPage(IDevice selectedDevice)
        {
            InitializeComponent();

            Title = selectedDevice.Name;

            Services = new ObservableCollection<IGattService>();
            serviceListView.ItemsSource = Services;
            serviceListView.ItemSelected += ServiceListView_ItemSelectedAsync;

            // Connect to the device
            Connect(selectedDevice);

            // Subscribe to changes in device connectivity
            selectedDevice.WhenStatusChanged().Subscribe((obj) =>
            {
                if (selectedDevice.Status == ConnectionStatus.Connected)
                {
                    selectedDevice.WhenServiceDiscovered().Subscribe((service) =>
                    {
                        Debug.WriteLine($"Service discovered. UUID: {service.Uuid}, Description: {service.Description}");

                        //Services.Add(new ServiceWrapper(service.Description, service.Uuid.ToString(), service));
                        Services.Add(service);
                    });
                }
            });

        }

        private async Task Connect(IDevice selectedDevice)
        {
            await selectedDevice.Connect(new GattConnectionConfig
            {
                NotifyOnConnect = true,
                NotifyOnNotification = true,
                NotifyOnDisconnect = true,
                IsPersistent = true
            });

        }

        private async void ServiceListView_ItemSelectedAsync(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedService = e.SelectedItem as IGattService;

            var page = new CharacteristicPage(selectedService);

            await Navigation.PushAsync(page);
        }
    }
}
