using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

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
                    DeviceListAdd(device.Name + "[" + device.DeviceId + "]");

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
            if (!string.IsNullOrEmpty(args.Advertisement.LocalName) && args.Advertisement.LocalName== "Flower care")
            {

                var device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);

                if (!bluetoothLEDevices.Any(x => x.DeviceId == device.DeviceId))
                {
                    bluetoothLEDevices.Add(device);
                }

            }



        }

        private async void DeviceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string id = DeviceList.GetItemText(DeviceList.SelectedItem);
            var item = bluetoothLEDevices.FirstOrDefault(device => device.Name + "[" + device.DeviceId + "]" == id);
            item =await BluetoothLEDevice.FromIdAsync(item.DeviceId);
            if (item != null)
            {
                //item.DeviceInformation.Pairing.Custom.PairingRequested += Custom_PairingRequested;

                //var result = await item.DeviceInformation.Pairing.Custom.PairAsync(
                //      DevicePairingKinds.ConfirmOnly, DevicePairingProtectionLevel.None);
                //item.DeviceInformation.Pairing.Custom.PairingRequested -= Custom_PairingRequested;

                //if (result.Status == DevicePairingResultStatus.Paired || result.Status == DevicePairingResultStatus.AlreadyPaired)
                //{
                    LogMessage("讀出資料中...");

                    var svcresult = await item.GetGattServicesAsync();
                    var curService = svcresult.Services.FirstOrDefault(x => x.Uuid == Guid.Parse("00001204-0000-1000-8000-00805f9b34fb"));
                    if (curService != null)
                    {

                        var curCharacteristic = await curService.GetCharacteristicsForUuidAsync(Guid.Parse("00001a00-0000-1000-8000-00805f9b34fb"));
                        if (curCharacteristic != null)
                        {
                            var tempCharacteristic = curCharacteristic.Characteristics.SingleOrDefault();
                            if (tempCharacteristic != null)
                            {


                                var communicationStatus = await tempCharacteristic.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(new byte[] { 0xa0, 0x1f }));
                                if (communicationStatus != GattCommunicationStatus.Success)
                                {
                                    LogMessage("開啟讀資料模式失敗");
                                }
                                else
                                {
                                    LogMessage("開啟讀資料模式成功");
                                }

                            }
                        }

                    }
                    if (curService != null)
                    {
                       
                        var curCharacteristic = await curService.GetCharacteristicsForUuidAsync(Guid.Parse("00001a01-0000-1000-8000-00805f9b34fb"));
                        if ( curCharacteristic !=null)
                        {
                            var tempCharacteristic = curCharacteristic.Characteristics.SingleOrDefault();
                            if ( tempCharacteristic != null)
                            {
                                
                                    var readResult = await tempCharacteristic.ReadValueAsync();
                                    var reader = DataReader.FromBuffer(readResult.Value);
                                    var input = new byte[reader.UnconsumedBufferLength];
                                    reader.ReadBytes(input);
                                LogMessage("原始資料："+BitConverter.ToString(input));
                                LogMessage("溫度：" + BitConverter.ToInt16(input.Take(2).ToArray(), 0) / 10f);
                                LogMessage("光照：" + BitConverter.ToInt32(input.Skip(3).Take(4).ToArray(), 0));
                                LogMessage("溼度：" + input[7]); // 只有一個Byte
                                LogMessage("肥力：" + BitConverter.ToInt16(input.Skip(8).Take(2).ToArray(), 0));


                            }
                        }

                    }
                   
                 
                //}
                //else
                //{
                //    LogMessage("請先配對");
                //}

            }

        }

        //private void Custom_PairingRequested(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
        //{
        //    args.Accept();
        //}
    }
}
