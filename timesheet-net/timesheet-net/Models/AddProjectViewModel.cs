using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace timesheet_net.Models
{
    public class AddProjectViewModel
    {
        public string Name { get; set; }
        public System.DateTime Start { get; set; }
        public Nullable<System.DateTime> Finish { get; set; }
        public int SuperiorID { get; set; }
        public int[] ProjectMembers { get; set; }
    }
}