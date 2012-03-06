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
using System.Text.RegularExpressions;
using System.Data.OleDb;
using Microsoft.Build.Framework;
using System.Globalization;

namespace AppFactory.Tasks
{
    public class OleDbExecute : Task
    {
        // Fields
        private int commandTimeout = 30;
        private const string ExecuteRawReaderTaskAction = "ExecuteRawReader";
        private const string ExecuteReaderTaskAction = "ExecuteReader";
        private const string ExecuteScalarTaskAction = "ExecuteScalar";
        private const string ExecuteTaskAction = "Execute";
        // private ScriptExecutionEventHandler ScriptFileExecuted;
        // private static readonly Regex splitter = new Regex(@"^\s*GO\s+", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        // private bool stripMultiLineComments = true;
        private DateTime timer;

        // Methods
        private OleDbConnection CreateConnection(string connectionString)
        {
            OleDbConnection connection = null;
            try
            {
                connection = new OleDbConnection(connectionString);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return connection;
        }

        private void ExecuteText()
        {
            using (OleDbConnection connection = CreateConnection(ConnectionString))
            {
                using (OleDbCommand command = new OleDbCommand(Sql, connection))
                {
                    command.CommandTimeout = CommandTimeout;
                    Log.LogMessage(MessageImportance.High, string.Format(CultureInfo.CurrentCulture, "Execute: {0}", new object[] { command.CommandText }));
                    connection.Open();
                    
                    OleDbTransaction transaction = null;
                    try
                    {
                        if (this.UseTransaction)
                        {
                            transaction = connection.BeginTransaction();
                            command.Transaction = transaction;
                        }


                        switch (TaskAction)
                        {
                            case ExecuteTaskAction:
                                command.ExecuteNonQuery();
                                break;

                            case ExecuteScalarTaskAction:
                                this.ScalarResult = command.ExecuteScalar().ToString();
                                break;

                            case ExecuteReaderTaskAction:
                                List<ITaskItem> itemList = new List<ITaskItem>();
                                using (OleDbDataReader reader = command.ExecuteReader())
                                {
                                    if (reader != null)
                                    {
                                        while (reader.Read())
                                        {
                                            ITaskItem item = new TaskItem(reader[0].ToString());
                                            for (int j = 0; j < reader.FieldCount; j++)
                                            {
                                                item.SetMetadata(reader.GetName(j), reader[j].ToString());
                                            }
                                            itemList.Add(item);
                                        }
                                    }
                                }
                                ReaderResult = itemList.ToArray();
                                break;

                            case ExecuteRawReaderTaskAction:
                                using (OleDbDataReader reader2 = command.ExecuteReader())
                                {
                                    RawReaderResult = string.Empty;
                                    if (reader2 != null)
                                    {
                                        while (reader2.Read())
                                        {
                                            string str = string.Empty;
                                            for (int k = 0; k < reader2.FieldCount; k++)
                                            {
                                                str = str + reader2[k] + " ";
                                            }
                                            RawReaderResult = RawReaderResult + str + Environment.NewLine;
                                        }
                                    }
                                }
                                break;
                        }

                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                        TimeSpan span = (TimeSpan)(DateTime.Now - this.timer);
                        Log.LogMessage(MessageImportance.Low, string.Format(CultureInfo.CurrentCulture, "Execution Time: {0} seconds", new object[] { span.TotalSeconds }));
                        this.timer = DateTime.Now;
                    }
                    catch
                    {
                        if (transaction != null)
                        {
                            transaction.Rollback();
                        }
                        throw;
                    }
                }
            }
        }

        public override bool Execute()
        {
            switch (TaskAction)
            {
                case ExecuteTaskAction:
                case ExecuteScalarTaskAction:
                case ExecuteReaderTaskAction:
                case ExecuteRawReaderTaskAction:
                    ExecuteText();
                    return true;

                default:
                    Log.LogError(string.Format(CultureInfo.CurrentCulture, "Invalid TaskAction passed: {0}", new object[] { this.TaskAction }), new object[0]);
                    return false;
            }
        }




        // Properties
        public int CommandTimeout
        {
            get
            {
                return this.commandTimeout;
            }
            set
            {
                this.commandTimeout = value;
            }
        }

        [Required]
        public string TaskAction { get; set; }

        [Required]
        public string ConnectionString { get; set; }

        [Required]
        public string Sql { get; set; }

        public bool UseTransaction { get; set; }

        [Output]
        public string RawReaderResult { get; set; }

        [Output]
        public ITaskItem[] ReaderResult { get; set; }

        [Output]
        public string ScalarResult { get; set; }
    }
}
