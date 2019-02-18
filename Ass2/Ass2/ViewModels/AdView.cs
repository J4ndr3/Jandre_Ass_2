using Ass2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Ass2.ViewModels
{
    public class AdView
    {
        //Fields for report criteria
        public IEnumerable<SelectListItem> EmpNames { get; set; }
        public int Empid { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        //Fields for report data
        public lgemployee empName { get; set; }
        public List<IGrouping<string, ReportRecord>> results { get; set; }
        public Dictionary<string, double> chartData { get; set; }
    }
    public class ReportRecord
    {
        public string OrderDate { get; set; }
        public double Amount { get; set; }
        public string ShipMethod { get; set; }
        public string Employee { get; set; }
        public int Empid1 { get; set; }
    }
}