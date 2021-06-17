using System;
using System.Collections.Generic;
using System.Text;

namespace BleConsoleApp
{
    public static class Constants
    {
        public static int BLE_MODE_ADVERTISING = 0;
        public static int BLE_MODE_GATT = 1;

        public static readonly Guid BLE_SERVICE_GUID = Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
        public static readonly Guid BLE_READ_CHAR_GUID = Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e");
        public static readonly Guid BLE_WRITE_CHAR_GUID = Guid.Parse("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
        public static readonly Guid BLE_NOTIFY_CHAR_GUID = Guid.Parse("6e400004-b5a3-f393-e0a9-e50e24dcca9e");
    }
}
