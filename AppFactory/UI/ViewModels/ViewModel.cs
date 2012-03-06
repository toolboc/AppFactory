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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using AppFactory.Data;
using AppFactory.Extensions;

namespace AppFactory.ViewModels
{
    public class ViewModel : ViewModelBase
    {
        //private static bool? isInDesignMode;

        //public static bool IsInDesignModeStatic
        //{
        //    get
        //    {
        //        if (!isInDesignMode.HasValue)
        //        {
        //            isInDesignMode = new bool?((bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue);
        //            if (!isInDesignMode.Value && Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
        //            {
        //                isInDesignMode = true;
        //            }
        //        }
        //        return isInDesignMode.Value;
        //    }
        //}

        //public bool IsInDesignMode
        //{
        //    get
        //    {
        //        return IsInDesignModeStatic;
        //    }
        //}

        /// <summary>
        /// Notifies external subscribers that a property has been changed.
        /// </summary>
        /// <param name="propertyExpression">
        /// An expression that indicates the property that has changed.
        /// </param>
        protected void RaisePropertyChanged(Expression<Func<object>> propertyExpression)
        {
            RaisePropertyChanged(propertyExpression.GetMemberName());
        }

        /// <summary>
        /// Notifies external subscribers that a property has been changed.
        /// </summary>
        /// <param name="propertyExpression">
        /// An expression that indicates the property that has changed.
        /// </param>
        /// <param name="oldValue">
        /// The property's value before the change occurred.
        /// </param>
        /// <param name="newValue">
        /// The property's value after the change occurred.
        /// </param>
        /// <param name="broadcast">
        /// If true, a PropertyChangedMessage will be broadcasted. If false, only the event will be raised.
        /// </param>
        protected void RaisePropertyChanged<T>(Expression<Func<object>> propertyExpression, T oldValue, T newValue, bool broadcast)
        {
            RaisePropertyChanged<T>(propertyExpression.GetMemberName(), oldValue, newValue, broadcast);
        }
    }
}
