using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BleConsoleApp
{
    public static class App
    {
        public static void Start()
        {
            IsRunning = true;
            MainAsync().Wait();
        }

        public static void Exit()
        {
            IsRunning = false;
            // Cancel work
        }


        #region Property
        public static bool IsRunning { get; private set; }
        private static BleManager Ble => BleManager.Instance;
        #endregion

        static async Task MainAsync()
        {
            // Scan Peripheral
            // Ble.StartWatcher();
            Ble.StartAdvertising();
            while (IsRunning)
            {
                // test code
                Thread.Sleep(1000);
            }
        }
    }
}
