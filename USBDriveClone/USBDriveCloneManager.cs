using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Text;

namespace USBDriveClone
{
    internal class USBDriveCloneManager
    {
        private const string UsbDetectionQuery = "SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2";
        private const string DriveNamePropertyName = "DriveName";

        private string destinationDirectory = string.Empty;
        private WqlEventQuery insertQuery;
        private ManagementEventWatcher insertWatcher;
        private IOHelper ioHelper;

        internal void StartUSBCloneEngine(string destinationDirectory)
        {
            this.destinationDirectory = destinationDirectory;
            ioHelper = new IOHelper();

            insertQuery = new WqlEventQuery(UsbDetectionQuery);
            insertWatcher = new ManagementEventWatcher(insertQuery);

            insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);

            insertWatcher.Start();
        }
        private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            try
            {
                var driveName = e.NewEvent.Properties[DriveNamePropertyName].Value.ToString();
                ioHelper.CopyDirectory(driveName, destinationDirectory);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
