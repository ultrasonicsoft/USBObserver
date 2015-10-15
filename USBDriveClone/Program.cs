using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Text;

namespace USBDriveClone
{
    class Program
    {
        static void Main(string[] args)
        {
            var usbCloneManager = new USBDriveCloneManager();
            var destinationDir = @"c:\temp\usb-data";
            usbCloneManager.StartUSBCloneEngine(destinationDir);
            Console.ReadLine();

        }
    }
}
