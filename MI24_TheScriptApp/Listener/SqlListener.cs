using MI24_TheScriptApp.Service;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MI24_TheScriptApp.Listener
{
    public static class SqlListener
    {

        public static void StartEventListener()
        {
            var connection = new SqlConnection("Server=DBKDVW10-HREY;Database=QA_ExpDev_Sec;uid=sa;pwd=Sql.Admin;");
            connection.Open();

            // Listen and process the captured events continuously
            var command = new SqlCommand();

            command.Connection = connection;
            string query = @"
            DROP EVENT SESSION MyEventSession
            ON SERVER

            CREATE EVENT SESSION MyEventSession ON SERVER
            ADD EVENT sqlserver.sql_batch_completed(
                ACTION(sqlserver.database_id)
                WHERE(
                    (sqlserver.sql_text like '%INSERT%'
                            OR sqlserver.sql_text like '%UPDATE%'
                            OR sqlserver.sql_text like '%DELETE%'
                            OR sqlserver.sql_text like '%SELECT%')
                )
            ),
            ADD EVENT sqlserver.rpc_completed(
                ACTION(sqlserver.database_id)
                WHERE(
                    (sqlserver.sql_text like '%INSERT%'
                            OR sqlserver.sql_text like '%UPDATE%'
                            OR sqlserver.sql_text like '%DELETE%'
                            OR sqlserver.sql_text like '%SELECT%')
                )
            ),
            ADD EVENT sqlserver.sql_statement_completed(
                ACTION(sqlserver.database_id)
                WHERE(
                    (sqlserver.sql_text like '%INSERT%'
                            OR sqlserver.sql_text like '%UPDATE%'
                            OR sqlserver.sql_text like '%DELETE%'
                            OR sqlserver.sql_text like '%SELECT%')
                )
            ),
            ADD EVENT sqlserver.error_reported(
                ACTION(sqlserver.database_id)
            )
            ADD TARGET package0.ring_buffer
            WITH(
                MAX_MEMORY = 4096 KB,
                EVENT_RETENTION_MODE = ALLOW_SINGLE_EVENT_LOSS,
                MAX_DISPATCH_LATENCY = 30 SECONDS
            )
            ALTER EVENT SESSION MyEventSession ON SERVER STATE = START";
            command.CommandText = query;
            command.ExecuteNonQuery();
        }

        public static string StopEventListener()
        {
            string connectionString = "Server=DBKDVW10-HREY;Database=QA_ExpDev_Sec;uid=sa;pwd=Sql.Admin;";
            StringBuilder sb = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                // Listen and process the captured events continuously
                using (SqlCommand command = new SqlCommand())
                {

                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM sys.dm_xe_session_targets st JOIN sys.dm_xe_sessions s ON s.address = st.event_session_address WHERE s.name = 'MyEventSession'";

                    var reader = command.ExecuteReader();

                    // Process and log the events as needed
                    var eventData = reader["target_data"].ToString();

                    string xmlData = eventData; // Replace with the provided XML data
                    var ringBufferTarget = XmlParser.ParseXml(xmlData);
                    // Now you can use the 'ringBufferTarget' object to access the parsed data.
                    // For example:

                    Console.WriteLine($"Total Events Processed: {ringBufferTarget.TotalEventsProcessed}");


                    foreach (var evnt in ringBufferTarget.Events)
                    {

                        if (evnt.Name == "sql_statement_completed")
                        {

                            Console.WriteLine($"Event Name: {evnt.Name}");
                            Console.WriteLine($"Timestamp: {evnt.Timestamp}");

                            foreach (var data in evnt.Data)
                            {

                                if (data.Name == "statement")
                                {
                                    sb.Append(data.Value);
                                    Console.WriteLine($"Data: {data.Name} | Type: {data.Type} | Value: {data.Value}");
                                }
                            }

                            Console.WriteLine("------------------------------------------");

                        }
                    }

                }

            }
            return sb.ToString();
        }
    }
}
