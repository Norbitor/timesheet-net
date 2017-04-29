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
    
    public partial class Timesheets
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Timesheets()
        {
            this.Tasks = new HashSet<Tasks>();
        }
    
        public int TimesheetID { get; set; }
        public System.DateTime Start { get; set; }
        public System.DateTime Finish { get; set; }
        public int ProjectMemberID { get; set; }
        public int TimesheetStateID { get; set; }
        public Nullable<int> LastEditedBy { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
        public string Comment { get; set; }
    
        public virtual Employees Employees { get; set; }
        public virtual ProjectMembers ProjectMembers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tasks> Tasks { get; set; }
        public virtual TimesheetStates TimesheetStates { get; set; }
    }
}
