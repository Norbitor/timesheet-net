using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace timesheet_net.Models
{
    public class ShowReportViewModel
    {
        public string ProjectName { get; set; }
        public int TimesheetID { get; set; }
        public System.DateTime Start { get; set; }
        public System.DateTime Finish { get; set; }
        public int ProjectMemberID { get; set; }
        public int TimesheetStateID { get; set; }
        public string Comment { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
        public decimal MondaySum { get; set; }
        public decimal TuesdaySum { get; set; }
        public decimal WednesdaySum { get; set; }
        public decimal ThursdaySum { get; set; }
        public decimal FridaySum { get; set; }
        public decimal SaturdaySum { get; set; }
        public decimal SundaySum { get; set; }
    }
}