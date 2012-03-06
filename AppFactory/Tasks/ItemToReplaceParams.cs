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

namespace AppFactory.Tasks
{
    /// <summary>
    /// Reads the properties off a MSBuild ItemGroup Item and creates an ItemGroup that can be used with the 
    /// <see href="http://msbuildextensionpack.com/help/4.0.4.0/html/348d3976-920f-9aca-da50-380d11ee7cf5.htm">Detokenize</see> task.
    /// </summary>
    public class ItemToReplaceParams : Task
    {
        private Collection<TaskItem> items = new Collection<TaskItem>();

        public override bool Execute()
        {
            // Clear in case the task is reused
            items.Clear();

            // Read all the properties on the source item
            foreach (string metaName in SourceItem.MetadataNames)
            {
                // Prefix?
                string token = (string.IsNullOrEmpty(Prefix) ? metaName : Prefix + metaName);

                // Create a new item. Use the token as the ItemSpec ("Include")
                TaskItem replaceItem = new TaskItem(token);

                // Set the "Replace" parameter
                replaceItem.SetMetadata("Replacement", SourceItem.GetMetadata(metaName));

                // Add to the collection
                items.Add(replaceItem);
            }

            return true;
        }

        /// <summary>
        /// The optional prefix to be prepended to the token name.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// The source item where the replacement items will come from.
        /// </summary>
        [Required]
        public ITaskItem SourceItem { get; set; }

        /// <summary>
        /// The replacement items to output.
        /// </summary>
        [Output]
        public ITaskItem[] ReplacementValues
        {
            get
            {
                return items.ToArray();
            }
        }
    }
}
