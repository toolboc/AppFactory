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
using AppFactory.Extensions;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Globalization;

namespace AppFactory.Services
{
    /// <summary>
    /// A service that can process build requests.
    /// </summary>
    public class BuildService
    {
        static internal class SettingNames
        {
            public const string AppSelectMethod = "AppSelectMethod";
            public const string CustomQuery = "CustomQuery";
            public const string DateQuery = "DateQuery";
            public const string MSBuildPath = "MSBuildPath";
            public const string Verbosity = "Verbosity";
            public const string WhereQuery = "WhereQuery";
        }

        #region Static Version
        #region Constants
        // The root folder where all .NET frameworks reside.
        private const string FrameworkPath = @"Microsoft.NET\Framework";
        private const string FrameworkFolderMask = "v*";
        private const string MSBuildName = "MSBuild.exe";
        private const string SettingsXPath = "//b:PropertyGroup[@Label='UISettings']";
        #endregion // Constants

        #region Member Variables
        static private BuildService instance;
        #endregion // Member Variables

        #region Internal Methods
        /// <summary>
        /// Finds the latest .NET framework path.
        /// </summary>
        /// <returns>
        /// The latest .NET framework path.
        /// </returns>
        static private string FindLatestFramework()
        {
            // Get the Windows directory
            string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            // Combine to get framework directory
            string frameworkDir = Path.Combine(winDir, FrameworkPath);

            // Get all .NET Framework directories, sort them by name, then grab the last one.
            return (from folder in Directory.GetDirectories(frameworkDir, FrameworkFolderMask)
                    orderby folder
                    select folder).LastOrDefault();
        }

        /// <summary>
        /// Finds the path to the latest version of MSBuild.exe
        /// </summary>
        /// <returns>
        /// The path to the latest version of MSBuild.exe, if found; otherwise <c>null</c>.
        /// </returns>
        static private string FindLatestMSBuild()
        {
            // Get the latest framework path
            string latestFramework = FindLatestFramework();

            // Make sure we got the path
            if (string.IsNullOrEmpty(latestFramework)) return null;

            // Combine paths
            string msBuildPath = Path.Combine(latestFramework, MSBuildName);

            // Make sure the file exists
            if (File.Exists(msBuildPath))
            {
                return msBuildPath;
            }
            else
            {
                return null;
            }
        }
        #endregion // Internal Methods

        #region Public Properties
        /// <summary>
        /// Gets the <see cref="BuildService"/> singleton instance.
        /// </summary>
        static public BuildService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BuildService();
                }
                return instance;
            }
        }
        #endregion // Public Properties
        #endregion // Static Version




        #region Instance Version
        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="BuildService"/> instance.
        /// </summary>
        private BuildService()
        {
        }
        #endregion // Constructors

        #region Public Methods
        /// <summary>
        /// Starts an asynchronous build using the specified settings.
        /// </summary>
        /// <param name="settings">
        /// The settings used to begin the build.
        /// </param>
        /// <param name="completed">
        /// A method that will handle the result of the build.
        /// </param>
        public void BeginBuild(BuildSettings settings, Action<int> completed)
        {
            // Validate
            if (settings == null) throw new ArgumentNullException("settings");
            if (!File.Exists(settings.ProjectPath)) { throw new InvalidOperationException(string.Format("Project file '{0}' was not found.", settings.ProjectPath)); }

            // Parse local values
            string workingDirectory = Path.GetDirectoryName(settings.ProjectPath);
            string projectFile = Path.GetFileName(settings.ProjectPath);
            string msbVerbosity = settings.Verbosity.ToString().ToLower();
            string msBuildPath = settings.MSBuildPath;
            string logFileName = Path.GetFileNameWithoutExtension(settings.ProjectPath) + ".log";

            // Do we need to find MSBuild?
            if (string.IsNullOrEmpty(msBuildPath))
            {
                // Try to find latest
                msBuildPath = FindLatestMSBuild();

                // Ensure we found it
                if (string.IsNullOrEmpty(msBuildPath)) throw new InvalidOperationException("MSBuild.exe could not be located.");
            }

            // Create our process
            Process process = new Process();

            // Determine the working path
            process.StartInfo.WorkingDirectory = workingDirectory;

            // Use determined MSBuild path
            process.StartInfo.FileName = msBuildPath;

            // Pass project, verbosity and log parameters
            process.StartInfo.Arguments = string.Format("{0} /verbosity:{1} /fileLogger /fileLoggerParameters:LogFile={2}", projectFile, msbVerbosity, logFileName);

            // Does the caller want to know when the build completes?
            if (completed != null)
            {
                process.EnableRaisingEvents = true;
                process.Exited += ((s, e) =>
                    {
                        completed(process.ExitCode);
                    });
            }

            // Start the build
            process.Start();
        }

        /// <summary>
        /// Attempts to view the log for the specified build settings.
        /// </summary>
        /// <param name="settings">
        /// The settings to view the log for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the log could be viewed; otherwise <c>false</c>.
        /// </returns>
        public bool ViewLog(BuildSettings settings)
        {
            // Validate
            if (settings == null) throw new ArgumentNullException("settings");

            // Only proceed if we have enough data
            if (string.IsNullOrEmpty(settings.ProjectPath)) return false;

            // Determine the path
            string workingDirectory = Path.GetDirectoryName(settings.ProjectPath);
            string logFileName = Path.GetFileNameWithoutExtension(settings.ProjectPath) + ".log";
            string logPath = Path.Combine(workingDirectory, logFileName);

            // Define process info
            Process viewLog = new Process();
            viewLog.StartInfo.WorkingDirectory = workingDirectory;
            viewLog.StartInfo.FileName = logPath;

            // Open the log
            return viewLog.Start();
        }

        /// <summary>
        /// Loads the build settings that are stored in the specified project.
        /// </summary>
        /// <param name="projectPath">
        /// The path to the project file where the settings are stored.
        /// </param>
        /// <returns>
        /// The loaded build settings.
        /// </returns>
        public BuildSettings LoadSettings(string projectPath)
        {
            // Load the project into an XDocument
            XDocument doc = XDocument.Load(projectPath, LoadOptions.PreserveWhitespace);

            // Create a NamespaceManager needed for XPath Queries
            XmlNamespaceManager mgr = doc.CreateNamespaceManager("b");

            // Create the BuildSettings instance to be returned
            BuildSettings settings = new BuildSettings();

            // We know the project path
            settings.ProjectPath = projectPath;

            // Look for the 'UI Settings' node in the project file
            XElement settingsElement = doc.XPathSelectElement(SettingsXPath, mgr);

            // If we found the settings node, try to load individual settings
            if (settingsElement != null)
            {
                // App Select Method
                XElement selMethod = settingsElement.FindElement(SettingNames.AppSelectMethod);
                if (selMethod != null)
                {
                    settings.AppSelectMethod = (AppSelectMethod)Enum.Parse(typeof(AppSelectMethod), selMethod.Value);
                }

                // Custom query
                XElement customQuery = settingsElement.FindElement(SettingNames.CustomQuery);
                if (customQuery != null)
                {
                    settings.CustomQuery = customQuery.Value;
                }

                // Date query
                XElement dateQuery = settingsElement.FindElement(SettingNames.DateQuery);
                if (dateQuery != null)
                {
                    settings.DateQuery = DateTime.Parse(dateQuery.Value);
                }

                // MSBuild Path
                XElement msbuildPath = settingsElement.FindElement(SettingNames.MSBuildPath);
                if (msbuildPath != null)
                {
                    settings.MSBuildPath = msbuildPath.Value;
                }

                // Verbosity
                XElement verbosity = settingsElement.FindElement(SettingNames.Verbosity);
                if (verbosity != null)
                {
                    settings.Verbosity = (BuildVerbosity)Enum.Parse(typeof(BuildVerbosity), verbosity.Value);
                }

                // Custom query
                XElement whereQuery = settingsElement.FindElement(SettingNames.WhereQuery);
                if (whereQuery != null)
                {
                    settings.WhereQuery = whereQuery.Value;
                }
            }

            // Return the loaded settings
            return settings;
        }

        /// <summary>
        /// Saves the build settings into the specified project file.
        /// </summary>
        /// <param name="settings">
        /// The settings to save.
        /// </param>
        public void SaveSettings(BuildSettings settings)
        {
            // Validate
            if (settings == null) throw new ArgumentNullException("settings");
            if (settings.ProjectPath == null) throw new ArgumentNullException("settings.ProjectPath");

            // Load the project into an XDocument
            XDocument doc = XDocument.Load(settings.ProjectPath, LoadOptions.PreserveWhitespace);

            // Create a NamespaceManager needed for XPath Queries
            XmlNamespaceManager mgr = doc.CreateNamespaceManager("b");

            // Look for the 'UI Settings' node in the project file
            XElement settingsElement = doc.XPathSelectElement(SettingsXPath, mgr);

            // If the settings element isn't found, create it
            if (settingsElement == null)
            {
                settingsElement = new XElement(XName.Get("PropertyGroup"));
                settingsElement.SetAttributeValue(XName.Get("Label"), "UISettings");
                doc.Root.AddFirst(settingsElement);
            }

            // App Select Method
            XElement selMethod = settingsElement.FindOrCreateElement(SettingNames.AppSelectMethod);
            selMethod.Value = settings.AppSelectMethod.ToString();

            // Custom query
            if (!string.IsNullOrEmpty(settings.CustomQuery))
            {
                XElement customQuery = settingsElement.FindOrCreateElement(SettingNames.CustomQuery);
                customQuery.Value = settings.CustomQuery;
            }

            // Date query
            if (settings.DateQuery != null)
            {
                XElement dateQuery = settingsElement.FindOrCreateElement(SettingNames.DateQuery);
                dateQuery.Value = settings.DateQuery.ToUniversalTime().ToString("s", DateTimeFormatInfo.InvariantInfo) + "Z";
            }

            // MSBuild Path
            if (!string.IsNullOrEmpty(settings.MSBuildPath))
            {
                XElement msbuildPath = settingsElement.FindOrCreateElement(SettingNames.MSBuildPath);
                msbuildPath.Value = settings.MSBuildPath;
            }

            // Verbosity
            XElement verbosity = settingsElement.FindOrCreateElement(SettingNames.Verbosity);
            verbosity.Value = settings.Verbosity.ToString();

            // Where query
            if (!string.IsNullOrEmpty(settings.WhereQuery))
            {
                XElement whereQuery = settingsElement.FindOrCreateElement(SettingNames.WhereQuery);
                whereQuery.Value = settings.WhereQuery;
            }

            // Save
            doc.Save(settings.ProjectPath);
        }
        #endregion // Public Methods
        #endregion // Instance Version
    }
}
