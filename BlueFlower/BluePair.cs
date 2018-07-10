using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
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
            if (item != null)
            {
                item.DeviceInformation.Pairing.Custom.PairingRequested += Custom_PairingRequested;

                var result = await item.DeviceInformation.Pairing.Custom.PairAsync(
                      DevicePairingKinds.ConfirmOnly, DevicePairingProtectionLevel.None);
                item.DeviceInformation.Pairing.Custom.PairingRequested -= Custom_PairingRequested;

                if (result.Status == DevicePairingResultStatus.Paired || result.Status == DevicePairingResultStatus.AlreadyPaired)
                {
                    LogMessage("讀出資料中...");

                    var svcresult = await item.GetGattServicesAsync();
                   
                    
                    if (svcresult.Status == Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
                    {
                        
                        // 查詢所有服務
                        foreach (var service in svcresult.Services)
                        {
                            LogMessage("服務:"+service.Uuid.ToString());
                            var chrresult = await service.GetCharacteristicsAsync();
                            
                            // 查詢所有特徵
                            foreach (var chr in chrresult.Characteristics)
                            {
                                LogMessage("  特徵:" + chr.UserDescription + "[" + chr.Uuid + "]");
                                if (chr.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                                {

                                    // 支援讀
                                    var valueResult = await chr.ReadValueAsync();

                                    var reader = DataReader.FromBuffer(valueResult.Value);
                                    var input = new byte[reader.UnconsumedBufferLength];
                                    reader.ReadBytes(input);
                                    LogMessage("    " + chr.AttributeHandle.ToString() + "支援讀(值：" + BitConverter.ToString(input) + ")");
                                }
                                if (chr.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                                {
                                    // 支援訂閱
                                    var valueResult = await chr.ReadValueAsync();

                                    var reader = DataReader.FromBuffer(valueResult.Value);
                                    var input = new byte[reader.UnconsumedBufferLength];
                                    reader.ReadBytes(input);
                                    LogMessage("    " + chr.AttributeHandle.ToString() + "支援訂閱(值：" + BitConverter.ToString(input) + ")");


                                }
                                if (chr.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write))
                                {
                                    LogMessage("    " + chr.AttributeHandle.ToString() + "支援寫");
                                }
                            }

                        }
                    }




                }
                else
                {
                    LogMessage("請先配對");
                }

            }

        }

        private void Custom_PairingRequested(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
        {
            args.Accept();
        }
    }
}
