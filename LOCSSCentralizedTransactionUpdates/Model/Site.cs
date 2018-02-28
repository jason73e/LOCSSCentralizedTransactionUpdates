//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LOCSSCentralizedTransactionUpdates.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Site
    {
        public string Site1 { get; set; }
        public string Description { get; set; }
        public string TaskMapping { get; set; }
        public string ReportOn { get; set; }
        public string MainConnectionString { get; set; }
        public string TrackingConnectionString { get; set; }
        public string SpecConnectionString { get; set; }
        public bool ActiveFlag { get; set; }
        public string Created_By { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Created_IP { get; set; }
        public string Modified_By { get; set; }
        public Nullable<System.DateTime> Modified_On { get; set; }
        public string Modified_IP { get; set; }
        public Nullable<bool> QtyMandate { get; set; }
        public Nullable<bool> DescMandate { get; set; }
        public Nullable<decimal> NoOfRows { get; set; }
        public Nullable<bool> BatchAccessFlag { get; set; }
        public Nullable<bool> NoQtyForL2 { get; set; }
        public string ProdRptServer { get; set; }
        public string ProdRptDBName { get; set; }
        public string TimeZone { get; set; }
        public string TimeZoneOffset { get; set; }
        public string NewSite { get; set; }
        public Nullable<int> TimeZoneID { get; set; }
    }
}