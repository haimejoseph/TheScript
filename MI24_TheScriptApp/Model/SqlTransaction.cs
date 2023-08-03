using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MI24_TheScriptApp.Model
{
    public class SqlTransaction
    {
        public int SPID { get; set; }
        public string SQLScript { get; set; }
        public string Status { get; set; } // "Session Started"
    }
}
