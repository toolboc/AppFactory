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

namespace AppFactory.Data
{
    /// <summary>
    /// Defines the methods for determining which applications to build.
    /// </summary>
    public enum AppSelectMethod
    {
        /// <summary>
        /// All applications will be built.
        /// </summary>
        All,

        /// <summary>
        /// A custom query will be used to determine which applications to build.
        /// </summary>
        CustomQuery,

        /// <summary>
        /// Applications not built since a specified date and time will be built.
        /// </summary>
        NotBuiltSince,

        /// <summary>
        /// The specified 'where' portion of a query will be used to determine which applications to build.
        /// </summary>
        WhereQuery,
    }

    /// <summary>
    /// Defines the verbosity levels possible during the build process.
    /// </summary>
    public enum BuildVerbosity
    {
        Minimal,
        Quiet,
        Normal,
        Detailed,
        Diagnostic,
    }

    /// <summary>
    /// Settings to be used when starting an AppFactory build process.
    /// </summary>
    public class BuildSettings : NotificationObject
    {
        #region Member Variables
        private string customQuery;
        private DateTime dateQuery = DateTime.Now;
        private string msBuildPath;
        private string projectPath;
        private AppSelectMethod appSelectMethod;
        private string whereQuery;
        #endregion // Member Variables

        #region Public Properties
        /// <summary>
        /// Gets or sets the method used to select which applications will be built.
        /// </summary>
        /// <value>
        /// The method used to select which applications will be built.
        /// </value>
        public AppSelectMethod AppSelectMethod
        {
            get
            {
                return appSelectMethod;
            }
            set
            {
                if (appSelectMethod != value)
                {
                    appSelectMethod = value;
                    RaisePropertyChanged(() => AppSelectMethod);
                }
            }
        }

        /// <summary>
        /// Gets or sets the full query statement used to select applications.
        /// </summary>
        /// <value>
        /// The full query statement used to select applications.
        /// </value>
        public string CustomQuery
        {
            get
            {
                return customQuery;
            }
            set
            {
                if (customQuery != value)
                {
                    customQuery = value;
                    RaisePropertyChanged(() => CustomQuery);
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="DateTime"/> value that is used to determine which applications to build.
        /// </summary>
        /// <value>
        /// A <see cref="DateTime"/> value that is used to determine which applications to build.
        /// </value>
        public DateTime DateQuery
        {
            get
            {
                return dateQuery;
            }
            set
            {
                if (dateQuery != value)
                {
                    dateQuery = value;
                    RaisePropertyChanged(() => DateQuery);
                }
            }
        }

        /// <summary>
        /// Gets the default file extension for AppFactory projects.
        /// </summary>
        public const string DefaultProjectExtension = ".afproj";

        /// <summary>
        /// Gets or sets the path to the version of MSBuild.exe that should be used for this build.
        /// </summary>
        /// <value>
        /// The path to the version of MSBuild.exe that should be used for this build, or <c>null</c> to use the latest version.
        /// </value>
        /// <remarks>
        /// If this property is null the latest version of MSBuild.exe will be used.
        /// </remarks>
        public string MSBuildPath
        {
            get
            {
                return msBuildPath;
            }
            set
            {
                if (msBuildPath != value)
                {
                    msBuildPath = value;
                    RaisePropertyChanged(() => MSBuildPath);
                }
            }
        }


        /// <summary>
        /// Gets or sets the path of the project file to be built.
        /// </summary>
        /// <value>
        /// The path of the project file to be built.
        /// </value>
        public string ProjectPath
        {
            get
            {
                return projectPath;
            }
            set
            {
                if (projectPath != value)
                {
                    projectPath = value;
                    RaisePropertyChanged(() => ProjectPath);
                }
            }
        }

        private BuildVerbosity verbosity;
        /// <summary>
        /// Gets or sets a value that indicates how verbose the build logging will be.
        /// </summary>
        /// <value>
        /// A value that indicates how verbose the build logging will be.
        /// </value>
        public BuildVerbosity Verbosity
        {
            get
            {
                return verbosity;
            }
            set
            {
                if (verbosity != value)
                {
                    verbosity = value;
                    RaisePropertyChanged(() => Verbosity);
                }
            }
        }

        /// <summary>
        /// Gets or sets the 'WHERE' clause used to select applications.
        /// </summary>
        /// <value>
        /// The 'WHERE' clause used to select applications.
        /// </value>
        public string WhereQuery
        {
            get
            {
                return whereQuery;
            }
            set
            {
                if (whereQuery != value)
                {
                    whereQuery = value;
                    RaisePropertyChanged(() => WhereQuery);
                }
            }
        }
        #endregion // Public Properties
    }
}
