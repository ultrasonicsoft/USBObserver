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
        private string destinationDirectory = string.Empty;
        private WqlEventQuery insertQuery;
        private ManagementEventWatcher insertWatcher;

        internal void StartUSBCloneEngine(string destinationDirectory)
        {
            this.destinationDirectory = destinationDirectory;

            insertQuery = new WqlEventQuery(UsbDetectionQuery);
            insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            insertWatcher.Start();
        }
        private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            var driveName = e.NewEvent.Properties["DriveName"].Value.ToString();
            CopyEverythingFromUsbDriveToDirectory(driveName, destinationDirectory);
        }
        private void CopyEverythingFromUsbDriveToDirectory(string driveName, string targetDir)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(driveName);

                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + driveName);
                }

                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(targetDir, file.Name);
                    file.CopyTo(temppath, true);
                }

                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(targetDir, subdir.Name);
                    CopyEverythingFromUsbDriveToDirectory(subdir.FullName, temppath);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
