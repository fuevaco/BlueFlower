using InTheHand.Net.Bluetooth;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using System.Linq;
using System.Collections.Generic;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Devices.Enumeration;

namespace BlueFlower
{

    public partial class BlueAPP : Form
    {
        BluetoothLEAdvertisementWatcher BleWatcher;
        delegate void MessageDelegate(string text);
        ObservableCollection<BluetoothLEDevice> bluetoothLEDevices;
       
        public BlueAPP()
        {
            InitializeComponent();
            bluetoothLEDevices = new ObservableCollection<BluetoothLEDevice>();
            bluetoothLEDevices.CollectionChanged += BluetoothLEDevices_CollectionChanged;


        }

        private void BluetoothLEDevices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (BluetoothLEDevice device in e.NewItems)
                {
                    DeviceListAdd(device.Name + "[" + device.DeviceId+ "]");
                
                }
            }
        }

        private void BluePair_Click(object sender, EventArgs e)
        {


        }


        private void LogMessage(string message)
        {
            if (Log.InvokeRequired)
            {
                MessageDelegate messageDelegate = new MessageDelegate(LogMessage);
                this.Invoke(messageDelegate, new object[] { message });
            }
            else
            {
                Log.AppendText(message + Environment.NewLine);
            }
        }
        private void DeviceListAdd(string deviceID)
        {
            if (DeviceList.InvokeRequired)
            {
                MessageDelegate messageDelegate = new MessageDelegate(DeviceListAdd);
                this.Invoke(messageDelegate, new object[] { deviceID });
            }
            else
            {
                DeviceList.Items.Add(deviceID);
            }
        }
        private void DeviceListRemove(string deviceID)
        {
            if (DeviceList.InvokeRequired)
            {
                MessageDelegate messageDelegate = new MessageDelegate(DeviceListRemove);
                this.Invoke(messageDelegate, new object[] { deviceID });
            }
            else
            {
                DeviceList.Items.Remove(deviceID);
            }
        }
        private void LocalAdapterInfo(BluetoothRadio bluetoothRadio)
        {
            LogMessage("ClassOfDevice: " + bluetoothRadio.ClassOfDevice);
            LogMessage("HardwareStatus: " + bluetoothRadio.HardwareStatus);
            LogMessage("HciRevision: " + bluetoothRadio.HciRevision);
            LogMessage("HciVersion: " + bluetoothRadio.HciVersion);
            LogMessage("LmpSubversion: " + bluetoothRadio.LmpSubversion);
            LogMessage("LmpVersion: " + bluetoothRadio.LmpVersion);
            LogMessage("LocalAddress: " + bluetoothRadio.LocalAddress);
            LogMessage("Manufacturer: " + bluetoothRadio.Manufacturer);
            LogMessage("Mode: " + bluetoothRadio.Mode);
            LogMessage("Name: " + bluetoothRadio.Name);
            LogMessage("Remote:" + bluetoothRadio.Remote);
            LogMessage("SoftwareManufacturer: " + bluetoothRadio.SoftwareManufacturer);
            LogMessage("StackFactory: " + bluetoothRadio.StackFactory);
        }

        private void Info_Click(object sender, EventArgs e)
        {

            BluetoothRadio bluetoothRadio = BluetoothRadio.PrimaryRadio;


            if (bluetoothRadio == null)
            {
                LogMessage("本機沒有藍芽設備");
            }
            else
            {
                LocalAdapterInfo(bluetoothRadio);

            }
        }

        private void FindBLE_Click(object sender, EventArgs e)
        {
            if (FindBLE.Text != "搜尋中..")
            {

                BleWatcher = new BluetoothLEAdvertisementWatcher
                {
                    ScanningMode = BluetoothLEScanningMode.Active
                };
                BleWatcher.Received += BleWatcher_Received;

                BleWatcher.Start();

                FindBLE.Text = "搜尋中..";
            }
            else
            {

                FindBLE.Text = "尋找附近藍芽";



            }

        }

        private async void BleWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Advertisement.LocalName) && args.Advertisement.LocalName == "Flower care")
            {
                
                var device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
                
                if (!bluetoothLEDevices.Any(x=>x.DeviceId == device.DeviceId))
                {
                    bluetoothLEDevices.Add(device);
                }

            }



        }

        private async void DeviceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string id = DeviceList.GetItemText(DeviceList.SelectedItem);
            var item = bluetoothLEDevices.FirstOrDefault(device => device.Name + "[" + device.DeviceId + "]" == id);
            if(item != null)
            {
               
                if (!item.DeviceInformation.Pairing.IsPaired)
                {

                    var prslt = await item.DeviceInformation.Pairing.Custom.PairAsync(DevicePairingKinds.ProvidePin, DevicePairingProtectionLevel.None);
                    LogMessage(prslt.Status.ToString());
                }
               
            }

        }

    }
}
