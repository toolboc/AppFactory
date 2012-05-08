#region License
/******************************************************************************
 * COPYRIGHT © MICROSOFT CORP. 
 * MICROSOFT LIMITED PERMISSIVE LICENSE (MS-LPL)
 * This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
 * 1. Definitions
 * The terms “reproduce,” “reproduction,” “derivative works,” and “distribution” have the same meaning here as under U.S. copyright law.
 * A “contribution” is the original software, or any additions or changes to the software.
 * A “contributor” is any person that distributes its contribution under this license.
 * “Licensed patents” are a contributor’s patent claims that read directly on its contribution.
 * 2. Grant of Rights
 * (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 * (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
 * 3. Conditions and Limitations
 * (A) No Trademark License- This license does not grant you rights to use any contributors’ name, logo, or trademarks.
 * (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
 * (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
 * (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
 * (E) The software is licensed “as-is.” You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
 * (F) Platform Limitation- The licenses granted in sections 2(A) & 2(B) extend only to the software or derivative works that you create that run on a Microsoft Windows operating system product.
 ******************************************************************************/
#endregion // License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Globalization;
using Microsoft.SmartDevice.Connectivity;

namespace AppFactory.Tasks.WindowsPhone
{
    public enum WPDeviceAction
    {
        InstallApp,
        UninstallApp,
        LaunchApp,
        TerminateApp,
    }

    /// <summary>
    /// Communicates with a Windows Phone device or emulator.
    /// </summary>
    public class WPDevice : Task
    {
        private Device device = null;
        private Guid prodID;

        public WPDevice()
        {
            AppGenre = "apps.normal";
            UseEmulator = true;
        }

        private void EnsureDevice()
        {
            // If we already have a device just bail
            if (device != null) { return; }

            // Get CoreCon WP7 SDK
            DatastoreManager dsmgrObj = new DatastoreManager(1033);

            Platform WP7SDK = dsmgrObj.GetPlatforms().Single(p => p.Name == "Windows Phone 7");

            // Get Emulator / Device
            bool useEmulator = true;

            if (useEmulator)
            {
                Log.LogMessage(MessageImportance.High, "Connecting to Emulator...");
                device = WP7SDK.GetDevices().Single(d => d.Name == "Windows Phone Emulator");
            }
            else
            {
                Log.LogMessage(MessageImportance.High, "Connecting to Device...");
                device = WP7SDK.GetDevices().Single(d => d.Name == "Windows Phone Device");
            }

            device.Connect();

            Log.LogMessage("Emulator / Device Connected...");
        }

        private void EnsureProdID()
        {
            if (prodID == Guid.Empty)
            {
                if (string.IsNullOrEmpty(ProductID)) throw new ArgumentException("ProductID");
                prodID = Guid.Parse(ProductID);
            }
        }

        private bool Install()
        {
            // Ensure we have a product ID
            EnsureProdID();

            // Validate other params
            if (string.IsNullOrEmpty(AppGenre)) throw new ArgumentException("AppGenre");
            if (string.IsNullOrEmpty(IconPath)) throw new ArgumentException("IconPath");
            if (string.IsNullOrEmpty(XapPath)) throw new ArgumentException("XapPath");

            // Ensure we're connected
            EnsureDevice();

            // If this is a clean install try uninstalling first
            if (CleanInstall)
            {
                Uninstall();
            }

            // Only install if not installed
            if (device.IsApplicationInstalled(prodID))
            {
                Log.LogMessage(MessageImportance.High, string.Format("Application '{0}' already installed; not installing again. Set option CleanInstall to true to force uninstallation of the application.", prodID));
                return false;
            }
            else
            {
                Log.LogMessage(MessageImportance.High, string.Format("Installing application '{0}'...", prodID));
                device.InstallApplication(prodID, prodID, AppGenre, IconPath, XapPath);
                return true;
            }
        }

        private bool Launch()
        {
            // Ensure we have a product ID
            EnsureProdID();

            // Ensure we're connected
            EnsureDevice();

            // Get the app
            RemoteApplication app = device.GetApplication(prodID);

            // Unable to test if the app is running because app.IsRunning throws an exception. Will just have to launch blind.
            Log.LogMessage(MessageImportance.High, string.Format("Launching application '{0}'...", prodID));
            app.Launch();
            return true;
        }

        private bool Terminate()
        {
            // Ensure we have a product ID
            EnsureProdID();

            // Ensure we're connected
            EnsureDevice();

            // Get the app
            RemoteApplication app = device.GetApplication(prodID);

            // Unable to test if the app is running because app.IsRunning throws an exception. Will just have to terminate blind.
            Log.LogMessage(MessageImportance.High, string.Format("Terminating application '{0}'...", prodID));
            app.TerminateRunningInstances();

            return true;
        }

        private bool Uninstall()
        {
            // Ensure we have a product ID
            EnsureProdID();

            // Ensure we're connected
            EnsureDevice();

            // Only uninstall if installed
            if (!device.IsApplicationInstalled(prodID))
            {
                Log.LogMessage(MessageImportance.High, string.Format("Application '{0}' not installed, nothing to uninstall.", prodID));
            }
            else
            {
                Log.LogMessage(MessageImportance.High, string.Format("Uninstalling application '{0}'...", prodID));
                RemoteApplication app = device.GetApplication(prodID);
                app.Uninstall();
            }

            return true;
        }

        public override bool Execute()
        {
            // Get the action
            WPDeviceAction action = (WPDeviceAction)Enum.Parse(typeof(WPDeviceAction), TaskAction);

            switch (action)
            {
                case WPDeviceAction.InstallApp:
                    return Install();
                case WPDeviceAction.LaunchApp:
                    return Launch();
                case WPDeviceAction.TerminateApp:
                    return Terminate();
                case WPDeviceAction.UninstallApp:
                    return Uninstall();
                default:
                    throw new InvalidOperationException(string.Format("Unexpected Branch: {0}", action));
            }
        }

        /// <summary>
        /// The application genre to be used when installing.
        /// </summary>
        public string AppGenre { get; set; }

        /// <summary>
        /// <c>true</c> to uninstall the application, if found, before installing it. The default is <c>false</c>.
        /// </summary>
        public bool CleanInstall { get; set; }

        /// <summary>
        /// The path of the icon file to be used when installing.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// The GUID of the application to install, launch, terminate or uninstall.
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// The action to perform.
        /// </summary>
        [Required]
        public string TaskAction { get; set; }

        /// <summary>
        /// <c>true</c> to use the emulator; <c>false</c> to use the actual device. The default is <c>true</c>.
        /// </summary>
        public bool UseEmulator { get; set; }

        /// <summary>
        /// The path of the XAP file to be used when installing.
        /// </summary>
        public string XapPath { get; set; }
    }
}
