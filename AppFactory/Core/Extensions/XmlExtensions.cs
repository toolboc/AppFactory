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
using System.Xml;
using System.Xml.Linq;

namespace AppFactory.Extensions
{
    static public class XmlExtensions
    {
        private const string RootNS = "xmlns";

        /// <summary>
        /// Creates a <see cref="XmlNamespaceManager"/> for the specified document and populates the namespaces.
        /// </summary>
        /// <param name="doc">
        /// The <see cref="XDocument"/> to create the namespace manager for.
        /// </param>
        /// <param name="rootPrefix">
        /// The prefix to be used in xpath queries when querying for elements in the root namespace (xmlns).
        /// </param>
        /// <returns>
        /// The namespace manager.
        /// </returns>
        static public XmlNamespaceManager CreateNamespaceManager(this XDocument doc, string rootPrefix)
        {
            // Create the manager
            XmlNamespaceManager mgr = new XmlNamespaceManager(new NameTable());

            // Find all namespace declarations
            var namespaces = (from attr in doc.Root.Attributes()
                          where attr.IsNamespaceDeclaration
                          select attr).ToList();

            // Convert each namespace into a declaration
            foreach (XAttribute attr in namespaces)
            {
                // Which prefix
                string prefix = (attr.Name.LocalName == RootNS ? rootPrefix : attr.Name.LocalName);

                // Add the namespace to the manager
                mgr.AddNamespace(prefix, attr.Value);
            }

            // Return the manager
            return mgr;
        }

        /// <summary>
        /// Attempts to find the named child element.
        /// </summary>
        /// <param name="parent">
        /// The parent XElement to search in.
        /// </param>
        /// <param name="name">
        /// The XName of the element to find.
        /// </param>
        /// <returns>
        /// The XElement if found; otherwise <c>null</c>.
        /// </returns>
        static public XElement FindElement(this XElement parent, XName name)
        {
            // Validate
            if (parent == null) throw new ArgumentNullException("parent");

            // Look for the element by name
            return parent.Descendants().Where(n => n.Name == name).FirstOrDefault();
        }


        /// <summary>
        /// Attempts to find the named child element.
        /// </summary>
        /// <param name="parent">
        /// The parent XElement to search in.
        /// </param>
        /// <param name="name">
        /// The XName of the element to find in the default namespace.
        /// </param>
        /// <returns>
        /// The XElement if found; otherwise <c>null</c>.
        /// </returns>
        static public XElement FindElement(this XElement parent, string name)
        {
            // Validate
            if (parent == null) throw new ArgumentNullException("parent");

            // Create XName using default namespace
            XName xName = XName.Get(name, parent.GetDefaultNamespace().NamespaceName);

            // Use the override
            return FindElement(parent, xName);
        }


        /// <summary>
        /// Attempts to find the named child element and creates it if it cannot be found.
        /// </summary>
        /// <param name="parent">
        /// The parent XElement to search in.
        /// </param>
        /// <param name="name">
        /// The XName of the element to find or create.
        /// </param>
        /// <returns>
        /// The XElement that was found or created.
        /// </returns>
        static public XElement FindOrCreateElement(this XElement parent, XName name)
        {
            // Try to find the existing element
            XElement ele = FindElement(parent, name);

            // If not found, create and add it
            if (ele == null)
            {
                ele = new XElement(name);
                parent.Add(ele);
            }

            // Return the element
            return ele;
        }

        /// <summary>
        /// Attempts to find the named child element and creates it if it cannot be found.
        /// </summary>
        /// <param name="parent">
        /// The parent XElement to search in.
        /// </param>
        /// <param name="name">
        /// The name of the element to find or create in the default namespace.
        /// </param>
        /// <returns>
        /// The XElement that was found or created.
        /// </returns>
        static public XElement FindOrCreateElement(this XElement parent, string name)
        {
            // Validate
            if (parent == null) throw new ArgumentNullException("parent");
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("name");

            // Create XName using default namespace
            XName xName = XName.Get(name, parent.GetDefaultNamespace().NamespaceName);

            // Use override
            return FindOrCreateElement(parent, xName);
        }
    }
}
