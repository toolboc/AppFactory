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
using AppFactory.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using AppFactory.Services;
using Microsoft.Win32;
using System.Windows;

namespace AppFactory.ViewModels
{
    public class BuildViewModel : ViewModel
    {
        #region Member Variables
        private BuildService buildService;
        private BuildSettings buildSettings;
        private OpenFileDialog ofd;
        #endregion // Member Variables

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="BuildViewModel"/> instance.
        /// </summary>
        public BuildViewModel()
        {
            // Obtain services
            buildService = BuildService.Instance;

            // Create Commands
            BuildCommand = new RelayCommand(Build);
            LoadCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
        }
        #endregion // Constructors

        #region Internal Methods
        private void BuildComplete(int returnCode)
        {
            // If there was an error, try to show the log
            if (returnCode != 0)
            {
                if (MessageBox.Show("A build error was detected. View the log?", "Build Error", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (!buildService.ViewLog(BuildSettings))
                    {
                        MessageBox.Show("An error occurred attempting to view the log file.");
                    }
                }
            }
        }
        #endregion // Internal Methods

        #region Public Methods
        /// <summary>
        /// Kicks off a build.
        /// </summary>
        public void Build()
        {
            if (BuildSettings != null)
            {
                try
                {
                    // Save the build settings first
                    buildService.SaveSettings(BuildSettings);

                    // Kick off the build
                    buildService.BeginBuild(BuildSettings, BuildComplete);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Build Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Loads a project from the specified path.
        /// </summary>
        /// <param name="path">
        /// The path to the project file.
        /// </param>
        public void Load(string path)
        {
            try
            {
                // Load the settings
                BuildSettings = buildService.LoadSettings(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Project Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Prompts the user to load a project from disk.
        /// </summary>
        public void Load()
        {
            // Make sure OFD is configured
            if (ofd == null)
            {
                ofd = new OpenFileDialog()
                {
                    CheckFileExists = true,
                    CheckPathExists = true,
                    DefaultExt = BuildSettings.DefaultProjectExtension,
                    Filter = string.Format("AppFactory projects ({0})|*{0}|All files (*.*)|*.*", BuildSettings.DefaultProjectExtension),
                    Multiselect = false,
                    Title = "Open Project"
                };
            }

            // Show the OFD
            if (ofd.ShowDialog() == true)
            {
                Load(ofd.FileName);
            }
        }

        /// <summary>
        /// Saves project settings to disk.
        /// </summary>
        public void Save()
        {
            if (BuildSettings != null)
            {
                buildService.SaveSettings(BuildSettings);
            }
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets a command that can be used to start a build.
        /// </summary>
        public ICommand BuildCommand { get; private set; }

        /// <summary>
        /// Gets or sets the settings that will be used to start the build.
        /// </summary>
        /// <value>
        /// The settings that will be used to start the build.
        /// </value>
        public BuildSettings BuildSettings
        {
            get
            {
                return buildSettings;
            }
            set
            {
                if (buildSettings != value)
                {
                    buildSettings = value;
                    RaisePropertyChanged(() => BuildSettings, buildSettings, value, true);
                }
            }
        }

        /// <summary>
        /// Gets a command that can be used to load a project.
        /// </summary>
        public ICommand LoadCommand { get; private set; }

        /// <summary>
        /// Gets a command that can be used to save project settings.
        /// </summary>
        public ICommand SaveCommand { get; private set; }
        #endregion // Public Properties
    }
}
