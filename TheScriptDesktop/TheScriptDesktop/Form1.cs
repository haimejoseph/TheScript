using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheScriptDesktop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void start_btn_Click(object sender, EventArgs e)
        {
            //ServiceController controller = new ServiceController("TheManWhoCantBeMoved");
            //if (controller.Status == ServiceControllerStatus.Running)
            //    controller.Stop();

            //if (controller.Status == ServiceControllerStatus.Stopped)
            //    controller.Start();

            // Write a log entry to the Windows Event Log
            while (true)
            {
                string message = "Sample Windows Service Log Entry at " + DateTime.Now;
                //EventLog.WriteEntry("SampleService", message, EventLogEntryType.Information);

                // Replace with your SQL Server connection string
                string connectionString = "Server=DBKDVW10-HREY;Database=QA_ExpDev_Sec;uid=sa;pwd=Sql.Admin;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a new Extended Events session with event filters
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"
                        CREATE EVENT SESSION MyEventSession ON SERVER 
                        ADD EVENT sqlserver.sql_batch_completed(ACTION(sqlserver.database_id))
                        ADD EVENT sqlserver.rpc_completed(ACTION(sqlserver.database_id))
                        ADD EVENT sqlserver.sql_statement_completed(ACTION(sqlserver.database_id))
                        ADD EVENT sqlserver.error_reported(ACTION(sqlserver.database_id))
                        WHERE (
                            sqlserver.database_id = DB_ID('Your_Database') -- Replace 'Your_Database' with your database name
                            AND (
                                sqlserver.sql_text like '%INSERT%'
                                OR sqlserver.sql_text like '%UPDATE%'
                                OR sqlserver.sql_text like '%DELETE%'
                                OR severity >= 16 -- Capturing errors with severity level 16 or higher
                            )
                        )
                        ADD TARGET package0.ring_buffer";

                        command.ExecuteNonQuery();
                    }

                    // Start the Extended Events session
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "ALTER EVENT SESSION MyEventSession ON SERVER STATE = START";

                        command.ExecuteNonQuery();
                    }



                    // Listen and process the captured events continuously
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM sys.dm_xe_session_targets st JOIN sys.dm_xe_sessions s ON s.address = st.event_session_address WHERE s.name = 'MyEventSession'";

                        var reader = command.ExecuteReader();

                        // Continuously read captured events
                        while (reader.Read())
                        {
                            // Process and log the events as needed
                            var eventData = reader["target_data"].ToString();
                            EventLog.WriteEntry("Service1", eventData, EventLogEntryType.Information);
                        }
                    }
                }
            }
            
        }
    }
}
