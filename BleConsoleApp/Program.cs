using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;


namespace BleConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.CancelKeyPress += Console_CancelKeyPress;
            App.Start();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            //// If we're waiting for async results, let's abandon the wait
            //if (_notifyCompleteEvent != null)
            //{
            //    _notifyCompleteEvent.Set();
            //    _notifyCompleteEvent = null;
            //    e.Cancel = true;
            //}
            //// If we're waiting for "delay" command, let's abandon the wait
            //else if (_delayEvent != null)
            //{
            //    _delayEvent.Set();
            //    _delayEvent = null;
            //    e.Cancel = true;
            //}
            //// Otherwise, quit the app
            //else
            //{
            //    if (!Console.IsInputRedirected)
            //        Console.WriteLine("\nBLEConsole is terminated");
            //    e.Cancel = false;
            //    _doWork = false;
            //}
        }


        #region Variables
        
        #endregion
    }
}
