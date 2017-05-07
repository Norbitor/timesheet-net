//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace timesheet_net.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tasks
    {
        public int TaskID { get; set; }
        public int TimesheetID { get; set; }
        public string TaskName { get; set; }
        public decimal MondayHours { get; set; }
        public decimal TuesdayHours { get; set; }
        public decimal WednesdayHours { get; set; }
        public decimal ThursdayHours { get; set; }
        public decimal FridayHours { get; set; }
        public decimal SaturdayHours { get; set; }
        public decimal SundayHours { get; set; }
        public string Comment { get; set; }
        public Nullable<int> LastEditedBy { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreationDate { get; set; }
    
        public virtual Employees Employees { get; set; }
        public virtual Employees Employees1 { get; set; }
        public virtual Timesheets Timesheets { get; set; }
    }
}
