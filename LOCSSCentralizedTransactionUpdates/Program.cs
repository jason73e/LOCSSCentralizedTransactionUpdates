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
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            DateTime today = DateTime.Today;
            DateTime month = new DateTime(today.Year, today.Month, 1);
            DateTime first = month.AddMonths(-1);
            DateTime last = month.AddDays(-1);
            LOCSSSCWEBEntities lOCSSSCWEBEntities = new LOCSSSCWEBEntities();

            SqlConnection cnn;
            SqlCommand command;
            string connetionString = string.Empty;
            int iResult = 0;
            SqlDataReader r;
            //List<LOCSSTrans> lsExisting = new List<LOCSSTrans>();
            //connetionString = ConfigurationManager.ConnectionStrings["LOCSSMGMT"].ToString();
            //using (cnn = new SqlConnection(connetionString))
            //{
            //    try
            //    {
            //        cnn.Open();
            //        string sql = "select init,etimeid,site,sitename,job_nbr,jobdesc,task_code,date_in,start_time,custid,custname, end_time,elapsed,date_out,qty,empname, taskdesc, hours, task_position " +
            //                     "from LOCSStransact (nolock) " +
            //                     "where date_in between '" + first.ToString("yyyy/MM/dd") + "' and '" + last.ToString("yyyy/MM/dd") + "' " +
            //                     "group by init,etimeid,site,sitename,job_nbr,jobdesc,task_code,date_in,start_time,custid,custname, end_time,elapsed,date_out,qty,empname, taskdesc, hours, task_position";
            //        command = new SqlCommand(sql, cnn);
            //        r = command.ExecuteReader();
            //        lsExisting = FillTransActions(r, lsExisting);
            //        logger.Info("Existing LOCSStransact Records:" + lsExisting.Count.ToString());
            //        r.Close();
            //        command.Dispose();
            //    }
            //    catch(Exception e)
            //    {
            //        logger.Error(e, "Error Getting LOCSStransact Records");
            //    }
            //}

            if (lOCSSSCWEBEntities.Sites.Where(x => x.ReportOn == "1" && x.ActiveFlag == true).Any())
            {
                List<Site> ls = lOCSSSCWEBEntities.Sites.Where(x => x.ReportOn == "1" && x.ActiveFlag == true).OrderBy(x => x.Description).ToList();
                foreach(Site s in ls)
                {
                    connetionString = s.MainConnectionString;//ConfigurationManager.ConnectionStrings["LOCSSMGMT"].ToString();
                    using (cnn = new SqlConnection(connetionString))
                    {
                        try
                        {
                            List<LOCSSTrans> lsTransact = new List<LOCSSTrans>();
                            cnn.Open();
                            string sql1 = "Select t.init,isnull(e.etimeid,e.file_num) as etimeid,t.site,'" + s.Description + "' as sitename,t.job_nbr,j.descr as jobdesc,t.task_code,t.date_in,t.start_time,t.custid,cust.name as custname, t.end_time,t.elapsed,t.date_out,t.qty, ltrim(lastname + ', ' + firstname + case when isnull(minit,'') = '' then '' else ' ' + isnull(minit,'') end) as empname, case when len(ltrim(isnull(js.task_desc,''))) > 0 then js.task_desc else case when len(ltrim(isnull(ct.taskdesc,''))) > 0 then ct.taskdesc else tsk.name end end as taskdesc, case when len(ltrim(isnull(t.task_code2,''))) = 0 then t.elapsed else case when len(ltrim(isnull(t.task_code3,''))) = 0 then t.elapsed/2 else case when len(ltrim(isnull(t.task_code4,''))) = 0 then t.elapsed/3 else t.elapsed/4 end end end as hours, '1' as task_position from " +
                                          s.ProdRptDBName + ".dbo.transact as t left join " +
                                          s.ProdRptDBName + ".dbo.job as j on j.job_nbr = t.job_nbr left join " +
                                          s.ProdRptDBName + ".dbo.employee as e on t.init = e.init left join " +
                                          s.ProdRptDBName + ".dbo.custtask as ct on ct.custid = t.custid and ct.task_code = t.task_code left join " +
                                          s.ProdRptDBName + ".dbo.customer as cust on cust.custid = t.custid left join " +
                                          s.ProdRptDBName + ".dbo.jobsumm as js on js.job_nbr = t.job_nbr and js.task_code = t.task_code and js.site = t.site left join " +
                                          s.ProdRptDBName + ".dbo.task as tsk on tsk.task_code = t.task_code " +
                                          "where t.date_in >= '" + first.ToString("yyyy/MM/dd") + "' and t.date_in <= '" + last.ToString("yyyy/MM/dd") + "' and elapsed<> 0 and t.job_nbr not in ('in', 'out', 'pto', 'hol') and len(t.task_code) > 0";
                            string sql2 = "Select t.init,isnull(e.etimeid,e.file_num) as etimeid,t.site,'" + s.Description + "' as sitename,t.job_nbr,j.descr as jobdesc,t.task_code2 as task_code,t.date_in,t.start_time,t.custid,cust.name as custname, t.end_time,t.elapsed,t.date_out,t.qty2 as qty, ltrim(lastname + ', ' + firstname + case when isnull(minit,'') = '' then '' else ' ' + isnull(minit,'') end) as empname, case when len(ltrim(isnull(js.task_desc,''))) > 0 then js.task_desc else case when len(ltrim(isnull(ct.taskdesc,''))) > 0 then ct.taskdesc else tsk.name end end as taskdesc, case when len(ltrim(isnull(t.task_code2,''))) = 0 then t.elapsed else case when len(ltrim(isnull(t.task_code3,''))) = 0 then t.elapsed/2 else case when len(ltrim(isnull(t.task_code4,''))) = 0 then t.elapsed/3 else t.elapsed/4 end end end as hours, '1' as task_position from " +
                                          s.ProdRptDBName + ".dbo.transact as t left join " +
                                          s.ProdRptDBName + ".dbo.job as j on j.job_nbr = t.job_nbr left join " +
                                          s.ProdRptDBName + ".dbo.employee as e on t.init = e.init left join " +
                                          s.ProdRptDBName + ".dbo.custtask as ct on ct.custid = t.custid and ct.task_code = t.task_code2 left join " +
                                          s.ProdRptDBName + ".dbo.customer as cust on cust.custid = t.custid left join " +
                                          s.ProdRptDBName + ".dbo.jobsumm as js on js.job_nbr = t.job_nbr and js.task_code = t.task_code2 and js.site = t.site left join " +
                                          s.ProdRptDBName + ".dbo.task as tsk on tsk.task_code = t.task_code2 " +
                                          "where t.date_in >= '" + first.ToString("yyyy/MM/dd") + "' and t.date_in <= '" + last.ToString("yyyy/MM/dd") + "' and elapsed<> 0 and t.job_nbr not in ('in', 'out', 'pto', 'hol') and len(t.task_code2) > 0";
                            string sql3 = "Select t.init,isnull(e.etimeid,e.file_num) as etimeid,t.site,'" + s.Description + "' as sitename,t.job_nbr,j.descr as jobdesc,t.task_code3 as task_code,t.date_in,t.start_time,t.custid,cust.name as custname, t.end_time,t.elapsed,t.date_out,t.qty3 as qty, ltrim(lastname + ', ' + firstname + case when isnull(minit,'') = '' then '' else ' ' + isnull(minit,'') end) as empname, case when len(ltrim(isnull(js.task_desc,''))) > 0 then js.task_desc else case when len(ltrim(isnull(ct.taskdesc,''))) > 0 then ct.taskdesc else tsk.name end end as taskdesc, case when len(ltrim(isnull(t.task_code2,''))) = 0 then t.elapsed else case when len(ltrim(isnull(t.task_code3,''))) = 0 then t.elapsed/2 else case when len(ltrim(isnull(t.task_code4,''))) = 0 then t.elapsed/3 else t.elapsed/4 end end end as hours, '1' as task_position from " +
                                          s.ProdRptDBName + ".dbo.transact as t left join " +
                                          s.ProdRptDBName + ".dbo.job as j on j.job_nbr = t.job_nbr left join " +
                                          s.ProdRptDBName + ".dbo.employee as e on t.init = e.init left join " +
                                          s.ProdRptDBName + ".dbo.custtask as ct on ct.custid = t.custid and ct.task_code = t.task_code3 left join " +
                                          s.ProdRptDBName + ".dbo.customer as cust on cust.custid = t.custid left join " +
                                          s.ProdRptDBName + ".dbo.jobsumm as js on js.job_nbr = t.job_nbr and js.task_code = t.task_code3 and js.site = t.site left join " +
                                          s.ProdRptDBName + ".dbo.task as tsk on tsk.task_code = t.task_code3 " +
                                          "where t.date_in >= '" + first.ToString("yyyy/MM/dd") + "' and t.date_in <= '" + last.ToString("yyyy/MM/dd") + "' and elapsed<> 0 and t.job_nbr not in ('in', 'out', 'pto', 'hol') and len(t.task_code3) > 0";
                            string sql4 = "Select t.init,isnull(e.etimeid,e.file_num) as etimeid,t.site,'" + s.Description + "' as sitename,t.job_nbr,j.descr as jobdesc,t.task_code4 as task_code,t.date_in,t.start_time,t.custid,cust.name as custname, t.end_time,t.elapsed,t.date_out,t.qty4 as qty, ltrim(lastname + ', ' + firstname + case when isnull(minit,'') = '' then '' else ' ' + isnull(minit,'') end) as empname, case when len(ltrim(isnull(js.task_desc,''))) > 0 then js.task_desc else case when len(ltrim(isnull(ct.taskdesc,''))) > 0 then ct.taskdesc else tsk.name end end as taskdesc, case when len(ltrim(isnull(t.task_code2,''))) = 0 then t.elapsed else case when len(ltrim(isnull(t.task_code3,''))) = 0 then t.elapsed/2 else case when len(ltrim(isnull(t.task_code4,''))) = 0 then t.elapsed/3 else t.elapsed/4 end end end as hours, '1' as task_position from " +
                                          s.ProdRptDBName + ".dbo.transact as t left join " +
                                          s.ProdRptDBName + ".dbo.job as j on j.job_nbr = t.job_nbr left join " +
                                          s.ProdRptDBName + ".dbo.employee as e on t.init = e.init left join " +
                                          s.ProdRptDBName + ".dbo.custtask as ct on ct.custid = t.custid and ct.task_code = t.task_code4 left join " +
                                          s.ProdRptDBName + ".dbo.customer as cust on cust.custid = t.custid left join " +
                                          s.ProdRptDBName + ".dbo.jobsumm as js on js.job_nbr = t.job_nbr and js.task_code = t.task_code4 and js.site = t.site left join " +
                                          s.ProdRptDBName + ".dbo.task as tsk on tsk.task_code = t.task_code4 " +
                                          "where t.date_in >= '" + first.ToString("yyyy/MM/dd") + "' and t.date_in <= '" + last.ToString("yyyy/MM/dd") + "' and elapsed<> 0 and t.job_nbr not in ('in', 'out', 'pto', 'hol') and len(t.task_code4) > 0";
                            logger.Info("Getting Transactions for Site: " + s.Site1);
                            command = new SqlCommand(sql1, cnn);
                            r = command.ExecuteReader();
                            lsTransact = FillTransActions(r, lsTransact);
                            r.Close();
                            command.Dispose();
                            command = new SqlCommand(sql2, cnn);
                            r = command.ExecuteReader();
                            lsTransact = FillTransActions(r, lsTransact);
                            r.Close();
                            command.Dispose();
                            command = new SqlCommand(sql3, cnn);
                            r = command.ExecuteReader();
                            lsTransact = FillTransActions(r, lsTransact);
                            r.Close();
                            command.Dispose();
                            command = new SqlCommand(sql4, cnn);
                            r = command.ExecuteReader();
                            lsTransact = FillTransActions(r, lsTransact);
                            r.Close();
                            command.Dispose();
                            cnn.Close();
                            logger.Info("Got " + lsTransact.Count.ToString() + " Transactions for Site: " + s.Site1);
                            InsertLOCSSTrans(lsTransact);//,lsExisting);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e, "Error Getting Site Records for Site:" + s.Site1);
                            return;
                        }
                    }
                }
            }
            connetionString = ConfigurationManager.ConnectionStrings["LOCSSMGMT"].ToString();
            using (cnn = new SqlConnection(connetionString))
            {
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
                    logger.Error(e, "Error Inserting AllData fro etime");
                    return;
                }
            }
            using (cnn = new SqlConnection(connetionString))
            {
                try
                {
                    string sql = "Select t.site,t.sitename,t.init,isnull(t.empname,'') as empname,isnull(t.etimeid,'') as etimeid,t.date_in,sum(t.hours) as hours " +
                                 "From LOCSStransact as t " +
                                 "where date_in >= '" + first.ToString("yyyy/MM/dd") + "' and date_in <= '" + last.ToString("yyyy/MM/dd") + "' " +
                                 "Group by t.site,t.sitename,t.init,t.empname,t.etimeid,t.date_in";
                    command = new SqlCommand(sql, cnn);
                    cnn.Open();
                    List<LOCSSTrans> lsTrans = new List<LOCSSTrans>();
                    r = command.ExecuteReader();
                    while(r.Read())
                    {
                        LOCSSTrans t = new LOCSSTrans();
                        t.date_in = Convert.ToDateTime(r["date_in"].ToString());
                        t.empname = r["empname"].ToString();
                        t.etimeid = r["etimeid"].ToString();
                        t.hours = Convert.ToDouble(r["hours"].ToString());
                        t.init = r["init"].ToString();
                        t.site = r["site"].ToString();
                        t.sitename = r["sitename"].ToString();
                        lsTrans.Add(t);
                    }
                    r.Close();
                    command.Dispose();
                    foreach(LOCSSTrans t in lsTrans)
                    {
                        if(t.etimeid==string.Empty)
                        {
                            string sInsertAll = "Insert into alldata(LOCSSSite, LOCSSSiteName, LOCSSID, LOCSS_Name, LOCSS_eTimeID, Date, eTimeHours, LOCSSHours, OTHours, eTimeSite, eTimeSiteName, eTime_eTimeID, FileID, eTime_Name) " +
                                                 "Values('" + t.site +"','" + t.sitename + "','" + t.init + "','"+ t.empname + "','" + t.etimeid +"','" + t.date_in.ToString() + "', 0," + t.hours.ToString() + ", 0,’’,’’,’’,’’,’’)";
                            command = new SqlCommand(sInsertAll, cnn);
                            iResult = command.ExecuteNonQuery();
                        }
                        else
                        {

                        }
                    }

                }
                catch (Exception e)
                {
                    logger.Error(e, "Error Reconciling LOCSStransact Records");
                    return;
                }
            }
            NLog.LogManager.Shutdown();
        }

        private static void InsertLOCSSTrans(List<LOCSSTrans> lsTransact)//, List<LOCSSTrans> lsExisting)
        {
            if (lsTransact.Count > 0)
            {
                //logger.Info("Starting Compare Site Transactions with existing LOCSStransact Records. Trans:" + lsTransact.Count.ToString() + " Existing:" + lsExisting.Count.ToString());
                //List<LOCSSTrans> lsResults = lsTransact.Where(l1 => !lsExisting.Any(l2 => l2.custid == l1.custid & l2.custname == l1.custname & l2.date_in == l1.date_in & l2.date_out == l1.date_out & l2.elapsed == l1.elapsed & l2.empname == l1.empname & l2.end_time == l1.end_time & l2.etimeid == l1.etimeid & l2.hours == l1.hours & l2.init == l1.init & l2.jobdesc == l1.jobdesc & l2.job_nbr == l1.job_nbr & l2.qty == l1.qty & l2.site == l1.site & l2.sitename == l1.sitename & l2.start_time == l1.start_time & l2.taskdesc == l1.taskdesc & l2.task_code == l1.task_code & l2.task_position == l1.task_position)).ToList();
                //logger.Info("Compare Found " + lsResults.Count.ToString() + " records to insert.");
                string connetionString = ConfigurationManager.ConnectionStrings["LOCSSMGMT"].ToString();
                //if (lsResults.Count > 0)
                //{
                    using (SqlConnection cnn = new SqlConnection(connetionString))
                    {
                        cnn.Open();
                        logger.Info("Starting to insert Records.");
                        string sErrorOutput = string.Empty;
                        foreach (LOCSSTrans t in lsTransact)
                        {
                            try
                            {
                                string sInsertAll = "if(not exists(select init,etimeid,site,sitename,job_nbr,jobdesc,task_code,date_in,start_time,custid,custname, end_time,elapsed,date_out,qty,empname, taskdesc, hours, task_position " +
                                                    "from LOCSStransact (nolock) " +
                                                    "where init = @init and etimeid = @etimeid and site = @site and sitename = @sitename and job_nbr = @job_nbr and jobdesc = @jobdesc and task_code = @task_code and date_in = @date_in and start_time = @start_time and custid = @custid and custname = @custname and end_time = @end_time and elapsed = @elapsed and date_out = @date_out and qty = @qty and empname = @empname and taskdesc = @taskdesc and hours = @hours and task_position = @task_position ))begin " +
                                                    "Insert into LOCSStransact (init,eTimeID,site,sitename,job_nbr,jobdesc,task_code,date_in,start_time,custid,custname,end_time,elapsed,date_out,qty,empname,taskdesc,hours,task_position) " +
                                                     "Values(@init,@etimeid,@site,@sitename,@job_nbr,@jobdesc,@task_code,@date_in,@start_time,@custid,@custname,@end_time,@elapsed,@date_out,@qty,@empname,@taskdesc,@hours,@task_position) end";
                                SqlCommand command = new SqlCommand(sInsertAll, cnn);
                                command.Parameters.AddWithValue("@init", t.init);
                                command.Parameters.AddWithValue("@etimeid", t.etimeid);
                                command.Parameters.AddWithValue("@site", t.site);
                                command.Parameters.AddWithValue("@sitename", t.sitename);
                                command.Parameters.AddWithValue("@job_nbr", t.job_nbr);
                                command.Parameters.AddWithValue("@jobdesc", t.jobdesc);
                                command.Parameters.AddWithValue("@task_code", t.task_code);
                                command.Parameters.AddWithValue("@date_in", t.date_in);
                                command.Parameters.AddWithValue("@start_time", t.start_time);
                                command.Parameters.AddWithValue("@custid", t.custid);
                                command.Parameters.AddWithValue("@custname", t.custname);
                                command.Parameters.AddWithValue("@end_time", t.end_time);
                                command.Parameters.AddWithValue("@elapsed", t.elapsed);
                                command.Parameters.AddWithValue("@date_out", t.date_out);
                                command.Parameters.AddWithValue("@qty", t.qty);
                                command.Parameters.AddWithValue("@empname", t.empname);
                                command.Parameters.AddWithValue("@taskdesc", t.taskdesc);
                                command.Parameters.AddWithValue("@hours", t.hours);
                                command.Parameters.AddWithValue("@task_position", t.task_position);
                                foreach(SqlParameter sp in command.Parameters)
                                {
                                    sErrorOutput = sErrorOutput + sp.ParameterName + ": " + sp.Value + "|| ";
                                }
                                int iResult = command.ExecuteNonQuery();
                                command.Dispose();
                            }
                            catch (Exception e)
                            {
                                logger.Error(e, "Insert Failed: " + sErrorOutput);
                            }
                        }
                        logger.Info("insert Records complete.");

                    }

                //}
            }
        }

        private static List<LOCSSTrans> FillTransActions(SqlDataReader r, List<LOCSSTrans> lsTransact)
        {
            try
            {
                while (r.Read())
                {
                    LOCSSTrans t = new LOCSSTrans();
                    t.init = r["init"].ToString();
                    t.etimeid = r["eTimeID"].ToString();
                    t.site = r["site"].ToString();
                    t.sitename = r["sitename"].ToString();
                    t.job_nbr = r["job_nbr"].ToString();
                    t.jobdesc = r["jobdesc"].ToString();
                    t.task_code = r["task_code"].ToString();
                    t.date_in = Convert.ToDateTime(r["date_in"].ToString());
                    t.start_time = r["start_time"].ToString();
                    t.custid = r["custid"].ToString();
                    t.custname = r["custname"].ToString();
                    t.end_time = r["end_time"].ToString();
                    t.elapsed = Convert.ToDouble(r["elapsed"].ToString());
                    t.date_out = Convert.ToDateTime(r["date_out"].ToString());
                    t.qty = Convert.ToDouble(r["qty"].ToString());
                    t.empname = r["empname"].ToString();
                    t.taskdesc = r["taskdesc"].ToString();
                    t.hours = Convert.ToDouble(r["hours"].ToString());
                    t.task_position = r["task_position"].ToString();
                    lsTransact.Add(t);
                }
                return lsTransact;
            }
            catch(Exception e)
            {
                logger.Error(e, "Create Trans Object Failed");
                throw e;
            }
        }
    }
}
