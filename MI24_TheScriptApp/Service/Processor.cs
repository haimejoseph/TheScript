using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MI24_TheScriptApp.Service
{
    public static class Processor
    {

        public static async void Process(string compareA, string compareB)
        {

            string comparisonResult = await Gpt35TextComparison.CompareTexts(compareA, compareB);
            // Split the comparison result to identify added, deleted, and unchanged tokens
            string[] tokens = comparisonResult.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


            //Step 3: Generate an HTML file with the query results
            //string htmlFilePath = "C:\\query_comparison.html"; // Modify the path if needed
            //using (StreamWriter sw = new StreamWriter(htmlFilePath))
            //{
            //    sw.WriteLine("<html>");
            //    sw.WriteLine("<head>");
            //    sw.WriteLine("<title>Query Comparison Results</title>");
            //    sw.WriteLine("<style>");
            //    sw.WriteLine(".table-container { display: flex; justify-content: space-around; }");
            //    sw.WriteLine(".table-container table { border: 1px solid #ddd; padding: 20px;width: 500px;border-radius: 10px;}");
            //    sw.WriteLine(".table-container table th, .table-container table td { padding: 5px; }");
            //    sw.WriteLine(".table-container table tr.unmatched { background-color: #f5b7b1; }"); // Unmatched rows background color (light red)
            //    sw.WriteLine("</style>");
            //    sw.WriteLine("</head>");
            //    sw.WriteLine("<body>");

            //    sw.WriteLine("<div class='table-container'>");
            //    sw.WriteLine("<div>");
            //    sw.WriteLine("<h1>Query 1 Results:</h1>");
            //    WriteDataTableToHTMLVertical(sw, compareA, compareB);
            //    sw.WriteLine("</div>");

            //    sw.WriteLine("<div>");
            //    sw.WriteLine("<h1>Query 2 Results:</h1>");
            //    WriteDataTableToHTMLVertical(sw, compareA, compareB);
            //    sw.WriteLine("</div>");
            //    sw.WriteLine("</div>");

            //    sw.WriteLine("<h2>Results Comparison:</h2>");
            //    sw.WriteLine("<p>Are the results equal? " + areEqual.ToString() + "</p>");

            //    sw.WriteLine("</body>");
            //    sw.WriteLine("</html>");
            //}
            //Console.WriteLine("HTML file generated successfully!");
            //Console.ReadLine();
        }

        public static async void ProcessCompare(string compareA, string compareB)
        {
            string comparisonResult = await Gpt35TextComparison.CompareTexts(compareA, compareB);

            // Split the comparison result to identify added, deleted, and unchanged tokens
            string[] tokens = comparisonResult.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            string htmlFilePath = "C:\\query_comparison.html"; // Modify the path if needed
            using (StreamWriter sw = new StreamWriter(htmlFilePath))
            {
                sw.WriteLine("<html>");
                sw.WriteLine("<head>");
                sw.WriteLine("<style>");
                sw.WriteLine(".added { background-color: #d6ffd6; }");
                sw.WriteLine(".deleted { background-color: #ffd6d6; }");
                sw.WriteLine(".unchanged { background-color: #f0f0f0; }");
                sw.WriteLine("table { width: 100%; border-collapse: collapse; }");
                sw.WriteLine("td { padding: 5px; border: 1px solid #ccc; }");
                sw.WriteLine("</style>");
                sw.WriteLine("</head>");
                sw.WriteLine("<body>");

                // Start the table
                sw.WriteLine("<table>");

                // Split text1 and text2 into tokens
                string[] text1Tokens = compareA.Split(' ');
                string[] text2Tokens = compareB.Split(' ');

                // Create two rows in the table for text1 and text2
                sw.WriteLine("<tr>");
                sw.WriteLine("<td>");
                foreach (var token in text1Tokens)
                {
                    bool isDifferent = tokens.Any(t => t.Contains(token) && (t.Contains(":added") || t.Contains(":deleted")));
                    string spanClass = isDifferent ? "added" : "unchanged";
                    sw.WriteLine($"<span class='{spanClass}'>{token}</span> ");
                }
                sw.WriteLine("</td>");

                sw.WriteLine("<td>");
                foreach (var token in text2Tokens)
                {
                    bool isDifferent = tokens.Any(t => t.Contains(token) && (t.Contains(":added") || t.Contains(":deleted")));
                    string spanClass = isDifferent ? "deleted" : "unchanged";
                    sw.WriteLine($"<span class='{spanClass}'>{token}</span> ");
                }
                sw.WriteLine("</td>");

                sw.WriteLine("</tr>");

                // End the table
                sw.WriteLine("</table>");

                sw.WriteLine("</body>");
                sw.WriteLine("</html>");
            }

            Console.WriteLine("HTML file generated successfully!");
            Console.ReadLine();
        }

        public static void WriteDataTableToHTMLVertical(StreamWriter sw, DataTable dt, DataTable compareDt)
        {

            sw.WriteLine("<table>");
            foreach (DataRow row in dt.Rows)
            {
                //sw.WriteLine("<tr>");

                bool unmatched = false;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sw.WriteLine("<tr>");
                    if (!row[i].Equals(compareDt.Rows[dt.Rows.IndexOf(row)][i]))
                    {
                        unmatched = true;
                        sw.WriteLine("<tr class='{0}'>", unmatched ? "unmatched" : ""); // Apply 'unmatched' class if unmatched is true
                    }
                    sw.Write("<td>{0}</td>", row[i]);
                    sw.WriteLine("</tr>");
                }
            }
        }
     }
}
