using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BleConsoleApp
{
    public class BleManager
    {
        private static readonly string TAG = typeof(BleManager).ToString();
        private LogManager Log => LogManager.Instance;

        #region LAZY
        private static readonly Lazy<BleManager> _lazy = new Lazy<BleManager>(() => new BleManager());
        public static BleManager Instance => _lazy.Value;
        #endregion

        #region Constructor
        private BleManager()
        {
            Mode = Constants.BLE_MODE_ADVERTISING;
            IsConnected = false;

            InitWatcher();
            InitServiceProvider();
        }
        #endregion

        #region Property
        
        public int Mode { get; private set; }
        public bool IsConnected { get; set; }
        public DeviceWatcher Watcher { get; private set; }
        public GattServiceProvider ServiceProvider { get; private set; }
        public GattLocalCharacteristic ReadCharacteristic { get; private set; }
        public GattLocalCharacteristic WriteCharacteristic { get; private set; }
        public GattLocalCharacteristic NotifyCharacteristic { get; private set; }
        #endregion


        #region Func
        


        /// <summary>
        /// Scan Peripheral
        /// </summary>
        #region Gatt Client
        private void InitWatcher()
        {
            // Query for extra properties you want returned
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
            Watcher = DeviceInformation.CreateWatcher(
                                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                                requestedProperties,
                                DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            Watcher.Added += DeviceWatcher_Added;
            Watcher.Updated += DeviceWatcher_Updated;
            Watcher.Removed += DeviceWatcher_Removed;

            // EnumerationCompleted and Stopped are optional to implement.
            Watcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            Watcher.Stopped += DeviceWatcher_Stopped;
        }
        public void StartWatcher()
        {
            // Start the watcher.
            Watcher.Start();
        }

        public void StopWatcher()
        {
            Watcher.Stop();
        }

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            Log.Add(TAG, "DeviceWatcher_Stopped");
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            Log.Add(TAG, "DeviceWatcher_EnumerationCompleted");
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            Log.Add(TAG, $"DeviceWatcher_Removed : {args.Id}");
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            Log.Add(TAG, $"DeviceWatcher_Updated : {args.Id}");
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            Log.Add(TAG, $"DeviceWatcher_Added : {args.Id}");
        }


        private async void Test()
        {
            await Task.Delay(1000);
        }
        #endregion


        #region GATT Server
        private async void InitServiceProvider()
        {
            GattServiceProviderResult result = await GattServiceProvider.CreateAsync(Constants.BLE_SERVICE_GUID);

            if (result.Error == BluetoothError.Success)
            {
                ServiceProvider = result.ServiceProvider;
            }
            byte[] value = new byte[] { 0x21 };
            var ReadParameters = new GattLocalCharacteristicParameters()
            {
                CharacteristicProperties = (GattCharacteristicProperties.Read),
                StaticValue = value.AsBuffer(),
                ReadProtectionLevel = GattProtectionLevel.Plain,
            };
            GattLocalCharacteristicResult characteristicResult = await ServiceProvider.Service.CreateCharacteristicAsync(Constants.BLE_READ_CHAR_GUID, ReadParameters);
            if (characteristicResult.Error != BluetoothError.Success)
            {
                // An error occurred.
                return;
            }
            ReadCharacteristic = characteristicResult.Characteristic;
            ReadCharacteristic.ReadRequested += ReadCharacteristic_ReadRequested;
            var WriteParameters = new GattLocalCharacteristicParameters()
            {
                CharacteristicProperties = GattCharacteristicProperties.Write,
                StaticValue = value.AsBuffer(),
                ReadProtectionLevel = GattProtectionLevel.Plain,
            };
            characteristicResult = await ServiceProvider.Service.CreateCharacteristicAsync(Constants.BLE_READ_CHAR_GUID, WriteParameters);
            if (characteristicResult.Error != BluetoothError.Success)
            {
                // An error occurred.
                return;
            }
            WriteCharacteristic = characteristicResult.Characteristic;
            WriteCharacteristic.WriteRequested += WriteCharacteristic_WriteRequested;

            var NotifyParameters = new GattLocalCharacteristicParameters()
            {
                CharacteristicProperties = GattCharacteristicProperties.Notify,
                StaticValue = value.AsBuffer(),
                ReadProtectionLevel = GattProtectionLevel.Plain,
            };
            characteristicResult = await ServiceProvider.Service.CreateCharacteristicAsync(Constants.BLE_NOTIFY_CHAR_GUID, NotifyParameters);
            if (characteristicResult.Error != BluetoothError.Success)
            {
                // An error occurred.
                return;
            }
            NotifyCharacteristic = characteristicResult.Characteristic;
            NotifyCharacteristic.SubscribedClientsChanged += SubscribedClientsChanged;
        }


        private void SubscribedClientsChanged(GattLocalCharacteristic sender, object args)
        {
            /*
             * 새 장치에서 알림을 구독할 때 SubscribedClientsChanged 이벤트가 호출 됩니다.
             */
            Log.Add(TAG, "SubscribedClientsChanged");

            var clients = sender.SubscribedClients;
            // Diff the new list of clients from a previously saved one 
            // to get which device has subscribed for notifications. 

            // You can also just validate that the list of clients is expected for this app.
        }

        private async void WriteCharacteristic_WriteRequested(GattLocalCharacteristic sender, GattWriteRequestedEventArgs args)
        {
            /*
             * 원격 장치에서 특성에 값을 쓰려고 하면 원격 장치에 대 한 세부 정보를 포함 하는 WriteRequested 이벤트가 호출 됩니다. 
             * 여기에는 쓸 특성 및 값 자체에 대 한 정보가 포함 됩니다.
             */
            Log.Add(TAG, "WriteCharacteristic_WriteRequested");

            var deferral = args.GetDeferral();

            var request = await args.GetRequestAsync();
            // var reader = DataReader.FromBuffer(request.Value);
            // Parse data as necessary. 
            var data = request.Value.ToArray();
            Log.Add(TAG, $"data : {Utils.ToHex(data)}");

            if (request.Option == GattWriteOption.WriteWithResponse)
            {
                request.Respond();
            }

            deferral.Complete();
        }

        private async void ReadCharacteristic_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            /*
             * 원격 장치가 특성에서 값을 읽으려고 할 때 (상수 값이 아닌 경우) ReadRequested 된 이벤트가 호출 됩니다. 
             * 읽기가 호출 된 특성 및 인수 (원격 장치에 대 한 정보 포함)는 대리자에 게 전달 됩니다
             */
            Log.Add(TAG, "ReadCharacteristic_ReadRequested");
            var deferral = args.GetDeferral();

            // Our familiar friend - DataWriter.
            var writer = new DataWriter();
            // populate writer w/ some data. 
            // ... 
            writer.WriteBoolean(true);

            var request = await args.GetRequestAsync();
            request.RespondWithValue(writer.DetachBuffer());

            deferral.Complete();
        }


        public void StartAdvertising()
        {
            GattServiceProviderAdvertisingParameters advParameters = new GattServiceProviderAdvertisingParameters
            {
                IsDiscoverable = true,
                IsConnectable = true
            };

            ServiceProvider.StartAdvertising(advParameters);
            //BluetoothLEAdvertisementPublisher publisher = new BluetoothLEAdvertisementPublisher();
            //BluetoothLEAdvertisement data = new BluetoothLEAdvertisement();
            //data.Flags = BluetoothLEAdvertisementFlags.GeneralDiscoverableMode;
            //data.LocalName = "KeyPlusT";
            //data.ServiceUuids.Add(Constants.BLE_SERVICE_GUID);
        }

        public void SttopAdvertising()
        {
            ServiceProvider.StopAdvertising();
        }

        public async void Notify(byte[] data)
        {
            /*
             * GATT 서버 작업의 가장 빈번한 알림은 원격 장치로 데이터를 푸시하는 중요 한 기능을 수행 합니다.
             * 경우에 따라 구독 되는 모든 클라이언트에 게 알리고, 새 값을 보낼 장치를 선택 하는 것이 좋습니다.
             */
            var writer = new DataWriter();
            // Populate writer with data
            // ...
            writer.WriteBytes(data);

            await NotifyCharacteristic.NotifyValueAsync(writer.DetachBuffer());
        }
        #endregion

        #endregion
    }
}
