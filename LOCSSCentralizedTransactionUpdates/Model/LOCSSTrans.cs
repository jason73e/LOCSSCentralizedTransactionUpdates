using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOCSSCentralizedTransactionUpdates.Model
{
    public class LOCSSTrans
    {
        public string site { get; set; }
        public string sitename { get; set; }
        public string init { get; set; }
        public string empname { get; set; }
        public string etimeid { get; set; }
        public DateTime date_in { get; set; }
        public double hours { get; set; }
        public string job_nbr { get; set; }
        public string jobdesc { get; set; }
        public string task_code { get; set; }
        public string start_time { get; set; }
        public string custid { get; set; }
        public string custname { get; set; }
        public string end_time { get; set; }
        public string taskdesc { get; set; }
        public string task_position { get; set; }
        public double elapsed { get; set; }
        public double qty { get; set; }
        public DateTime date_out { get; set; }
    }
}
