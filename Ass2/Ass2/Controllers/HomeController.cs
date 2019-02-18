using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ass2.Models;
using System.Data;
using System.IO;
using Ass2.ViewModels;

namespace Ass2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        public ActionResult Advanced()
        {
            AdView vm = new AdView();

            //Retrieve a list of vendors so that it can be used to populate the dropdown on the View
            vm.EmpNames = GetEmployees(0);

            //Set default values for the FROM and TO dates
            vm.DateFrom = new DateTime(2014, 12, 1);
            vm.DateTo = new DateTime(2014, 12, 31);


            return View(vm);


        }
        //This action builds a collection of SelectList items based on the Vendor table in the DB
        //The selected parameter is used to preselect an item in the SelectList. The selected item will be selected by default when the dropdown list on the view loads.
        private SelectList GetEmployees(int selected)
        {
            using (HardwareDBEntities db = new HardwareDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;

                //Create a SelectListItem for each Vendor record in the DB
                //Value is set to the primary key of the record and Text is set to the Name of the vendor
                var Emp = db.lgemployees.Select(x => new SelectListItem
                {
                    Value = x.emp_num.ToString(),
                    Text = x.emp_fname
                }).ToList();

                //If selected pearameter has a value, configure the SelectList so that the apporiate item is preselected
                if (selected == 0)
                    return new SelectList(Emp, "Value", "Text");
                else
                    return new SelectList(Emp, "Value", "Text", selected);
            }
        }
        //This action is used to process the Advanced report criteria entered by the user and to return report data based on that criteria
        [HttpPost]
        public ActionResult Advanced(AdView vm)
        {
            using (HardwareDBEntities db = new HardwareDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;

                //Retrieve a list of vendors so that it can be used to populate the dropdown on the View
                //The ID of the currently selected item is passed through so that the returned list has that item preselected
                vm.EmpNames = GetEmployees(vm.Empid);

                //Get the full details of the selected vendor so that it can be displayed on the view


                vm.empName = db.lgemployees.Where(zz => zz.emp_num == vm.Empid).FirstOrDefault();
                //Get all supplier orders that adheres to the entered criteria
                //For each of the results, load data into a new ReportRecord object
                var list = db.lginvoices.Where(pp => pp.employee_id == vm.empName.emp_num && pp.inv_DATETIME >= vm.DateFrom && pp.inv_DATETIME <= vm.DateTo).ToList().Select(rr => new ReportRecord
                {
                    OrderDate = rr.inv_DATETIME.ToString(),
                    Amount = Convert.ToDouble(rr.inv_total),
                    Employee = db.lgemployees.Where(pp => pp.emp_num == rr.employee_id).Select(x => x.emp_fname + " " + x.emp_lname).FirstOrDefault(),
                    Empid1 = Convert.ToInt32(rr.employee_id)
                });

                //Load the list of ReportRecords returned by the above query into a new list grouped by Shipment Method
                vm.results = list.GroupBy(g => g.Employee).ToList();

                //Load the list of ReportRecords returned by the above query into a new dictionary grouped by Employee
                //This will be used to generate the chart on the View through the MicroSoft Charts helper
                vm.chartData = list.GroupBy(g => g.).ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

                //Store the chartData dictionary in temporary data so that it can be accessed by the EmployeeOrdersChart action resonsible for generating the chart
                TempData["chartData"] = vm.chartData;
                TempData["records"] = list.ToList();
                TempData["vendor"] = vm.empName;
                return View(vm);
            }
        }
             //This action returns the EmployeeOrdersChart partial view, which is used to generate a chart for the Advanced report
        public ActionResult EmployeeOrdersChart()
        {
            //Load the chartData from temporary memory
            var data = TempData["chartData"];

            //Return the EmployeeOrdersChart temporary view, pass through the required chartData
            return View(TempData["chartData"]);
        }

    }
    
}