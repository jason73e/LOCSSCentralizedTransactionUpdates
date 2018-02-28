using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LOCSSCentralizedTransactionUpdates.Model;

namespace LOCSSCentralizedTransactionUpdates
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime today = DateTime.Today;
            DateTime month = new DateTime(today.Year, today.Month, 1);
            DateTime first = month.AddMonths(-1);
            DateTime last = month.AddDays(-1);
            LOCSSSCWEBEntities lOCSSSCWEBEntities = new LOCSSSCWEBEntities();
            if (lOCSSSCWEBEntities.Sites.Where(x => x.ReportOn == "1" && x.ActiveFlag == true).Any())
            {
                List<Site> ls = lOCSSSCWEBEntities.Sites.Where(x => x.ReportOn == "1" && x.ActiveFlag == true).OrderBy(x => x.Description).ToList();
                foreach(Site s in ls)
                {
                    string connetionString = s.MainConnectionString;
                    SqlConnection cnn;
                    SqlCommand command;
                    int iResult = 0;
                    cnn = new SqlConnection(connetionString);
                    try
                    {
                        cnn.Open();
                        string sql1 = "Insert into LOCSStransact (init,eTimeID,site,sitename,job_nbr,jobdesc,task_code,date_in,start_time,custid,custname,end_time,elapsed,date_out,qty,empname,taskdesc,hours,task_position) " +
                                      "Select t.init,isnull(e.etimeid,e.file_num),t.site,'" + s.Description + "',t.job_nbr,j.descr,t.task_code,t.date_in,t.start_time,t.custid,cust.name, t.end_time,t.elapsed,t.date_out,t.qty, ltrim(lastname + ', ' + firstname + case when isnull(minit,'') = '' then '' else ' ' + isnull(minit,'') end), case when len(ltrim(isnull(js.task_desc,''))) > 0 then js.task_desc else case when len(ltrim(isnull(ct.taskdesc,''))) > 0 then ct.taskdesc else tsk.name end end, case when len(ltrim(isnull(t.task_code2,''))) = 0 then t.elapsed else case when len(ltrim(isnull(t.task_code3,''))) = 0 then t.elapsed/2 else case when len(ltrim(isnull(t.task_code4,''))) = 0 then t.elapsed/3 else t.elapsed/4 end end end, '1' from " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.transact as t left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.job as j on j.job_nbr = t.job_nbr left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.employee as e on t.init = e.init left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.custtask as ct on ct.custid = t.custid and ct.task_code = t.task_code left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.customer as cust on cust.custid = t.custid left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.jobsumm as js on js.job_nbr = t.job_nbr and js.task_code = t.task_code and js.site = t.site left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.task as tsk on tsk.task_code = t.task_code " +
                                      "where t.date_in >= '"+ first.ToString("yyyy/MM/dd") + "' and t.date_in <= '" + last.ToString("yyyy/MM/dd") + "' and elapsed<> 0 and t.job_nbr not in ('in', 'out', 'pto', 'hol') and len(t.task_code) > 0";
                        string sql2 = "Insert into LOCSStransact (init,eTimeID,site,sitename,job_nbr,jobdesc,task_code,date_in,start_time,custid,custname,end_time,elapsed,date_out,qty,empname,taskdesc,hours,task_position) " +
                                      "Select t.init,isnull(e.etimeid,e.file_num),t.site,'" + s.Description + "',t.job_nbr,j.descr,t.task_code2,t.date_in,t.start_time,t.custid,cust.name, t.end_time,t.elapsed,t.date_out,t.qty2, ltrim(lastname + ', ' + firstname + case when isnull(minit,'') = '' then '' else ' ' + isnull(minit,'') end), case when len(ltrim(isnull(js.task_desc,''))) > 0 then js.task_desc else case when len(ltrim(isnull(ct.taskdesc,''))) > 0 then ct.taskdesc else tsk.name end end, case when len(ltrim(isnull(t.task_code2,''))) = 0 then t.elapsed else case when len(ltrim(isnull(t.task_code3,''))) = 0 then t.elapsed/2 else case when len(ltrim(isnull(t.task_code4,''))) = 0 then t.elapsed/3 else t.elapsed/4 end end end, '1' from " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.transact as t left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.job as j on j.job_nbr = t.job_nbr left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.employee as e on t.init = e.init left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.custtask as ct on ct.custid = t.custid and ct.task_code = t.task_code2 left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.customer as cust on cust.custid = t.custid left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.jobsumm as js on js.job_nbr = t.job_nbr and js.task_code = t.task_code2 and js.site = t.site left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.task as tsk on tsk.task_code = t.task_code2 " +
                                      "where t.date_in >= '" + first.ToString("yyyy/MM/dd") + "' and t.date_in <= '" + last.ToString("yyyy/MM/dd") + "' and elapsed<> 0 and t.job_nbr not in ('in', 'out', 'pto', 'hol') and len(t.task_code2) > 0";
                        string sql3 = "Insert into LOCSStransact (init,eTimeID,site,sitename,job_nbr,jobdesc,task_code,date_in,start_time,custid,custname,end_time,elapsed,date_out,qty,empname,taskdesc,hours,task_position) " +
                                      "Select t.init,isnull(e.etimeid,e.file_num),t.site,'" + s.Description + "',t.job_nbr,j.descr,t.task_code3,t.date_in,t.start_time,t.custid,cust.name, t.end_time,t.elapsed,t.date_out,t.qty3, ltrim(lastname + ', ' + firstname + case when isnull(minit,'') = '' then '' else ' ' + isnull(minit,'') end), case when len(ltrim(isnull(js.task_desc,''))) > 0 then js.task_desc else case when len(ltrim(isnull(ct.taskdesc,''))) > 0 then ct.taskdesc else tsk.name end end, case when len(ltrim(isnull(t.task_code2,''))) = 0 then t.elapsed else case when len(ltrim(isnull(t.task_code3,''))) = 0 then t.elapsed/2 else case when len(ltrim(isnull(t.task_code4,''))) = 0 then t.elapsed/3 else t.elapsed/4 end end end, '1' from " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.transact as t left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.job as j on j.job_nbr = t.job_nbr left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.employee as e on t.init = e.init left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.custtask as ct on ct.custid = t.custid and ct.task_code = t.task_code3 left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.customer as cust on cust.custid = t.custid left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.jobsumm as js on js.job_nbr = t.job_nbr and js.task_code = t.task_code3 and js.site = t.site left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.task as tsk on tsk.task_code = t.task_code3 " +
                                      "where t.date_in >= '" + first.ToString("yyyy/MM/dd") + "' and t.date_in <= '" + last.ToString("yyyy/MM/dd") + "' and elapsed<> 0 and t.job_nbr not in ('in', 'out', 'pto', 'hol') and len(t.task_code3) > 0";
                        string sql4 = "Insert into LOCSStransact (init,eTimeID,site,sitename,job_nbr,jobdesc,task_code,date_in,start_time,custid,custname,end_time,elapsed,date_out,qty,empname,taskdesc,hours,task_position) " +
                                      "Select t.init,isnull(e.etimeid,e.file_num),t.site,'" + s.Description + "',t.job_nbr,j.descr,t.task_code4,t.date_in,t.start_time,t.custid,cust.name, t.end_time,t.elapsed,t.date_out,t.qty4, ltrim(lastname + ', ' + firstname + case when isnull(minit,'') = '' then '' else ' ' + isnull(minit,'') end), case when len(ltrim(isnull(js.task_desc,''))) > 0 then js.task_desc else case when len(ltrim(isnull(ct.taskdesc,''))) > 0 then ct.taskdesc else tsk.name end end, case when len(ltrim(isnull(t.task_code2,''))) = 0 then t.elapsed else case when len(ltrim(isnull(t.task_code3,''))) = 0 then t.elapsed/2 else case when len(ltrim(isnull(t.task_code4,''))) = 0 then t.elapsed/3 else t.elapsed/4 end end end, '1' from " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.transact as t left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.job as j on j.job_nbr = t.job_nbr left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.employee as e on t.init = e.init left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.custtask as ct on ct.custid = t.custid and ct.task_code = t.task_code4 left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.customer as cust on cust.custid = t.custid left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.jobsumm as js on js.job_nbr = t.job_nbr and js.task_code = t.task_code4 and js.site = t.site left join " +
                                      s.ProdRptServer + "." + s.ProdRptDBName + ".dbo.task as tsk on tsk.task_code = t.task_code4 " +
                                      "where t.date_in >= '" + first.ToString("yyyy/MM/dd") + "' and t.date_in <= '" + last.ToString("yyyy/MM/dd") + "' and elapsed<> 0 and t.job_nbr not in ('in', 'out', 'pto', 'hol') and len(t.task_code4) > 0";
                        command = new SqlCommand(sql1, cnn);
                        iResult = command.ExecuteNonQuery();
                        command.Dispose();
                        command = new SqlCommand(sql2, cnn);
                        iResult = command.ExecuteNonQuery();
                        command.Dispose();
                        command = new SqlCommand(sql3, cnn);
                        iResult = command.ExecuteNonQuery();
                        command.Dispose();
                        command = new SqlCommand(sql4, cnn);
                        iResult = command.ExecuteNonQuery();
                        command.Dispose();
                        cnn.Close();
                    }
                    catch(Exception e)
                    {

                    }
                }
            }
            string connetionString = ConfigurationManager.ConnectionStrings["LOCSSMGMT"].ToString();
            SqlConnection cnn;
            SqlCommand command;
            int iResult = 0;
            cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
                string sql = "Insert into AllData ([eTimeSite],[eTimeSiteName],[LOCSSSite],[LOCSSSiteName],[eTime_eTimeID],[FileID],[eTime_Name],[LOCSSID],[LOCSS_Name],[LOCSS_eTimeID],[Date],[eTimeHours],[LOCSSHours],[OTHours]) " +
                    "Select e.site,e.sitename, '','',e.etimeid,e.fileid,e.fullname,'','','',e.date,e.hours,0,e.othours From " +
                    "(Select site,isnull(max(s.sitename),'') as SiteName,etimeid,isnull(filenum,'') as FileID,fullname,edate as date,max(regular)+max(overtime) as Hours,max(overtime) as OTHours " +
                    "from [dcap-sql-locs-a].etime.dbo.etime as e left join " +
                    "[dcap-sql-locs-a].etime.dbo.sites as s on s.etimesite = e.site " +
                    "where edate >= '" + first.ToString("yyyy/MM/dd") + "' and edate <= '" + last.ToString("yyyy/MM/dd") + "' " +
                    "group by site,etimeid,filenum,fullname,edate) as e " +
                    "Where cast(e.hours as float) <> 0 group by e.site,e.sitename,e.etimeid,e.fileid, e.fullname,e.date,e.hours,e.othours";
                command = new SqlCommand(sql, cnn);
                iResult = command.ExecuteNonQuery();
                command.Dispose();
                cnn.Close();
            }
            catch (Exception e)
            {

            }

        }
    }
}
