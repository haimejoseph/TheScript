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
