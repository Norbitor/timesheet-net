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
    using System.ComponentModel.DataAnnotations;

    public partial class Employees
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employees()
        {
            this.ProjectMembers = new HashSet<ProjectMembers>();
            this.Projects = new HashSet<Projects>();
            this.Projects1 = new HashSet<Projects>();
            this.Projects2 = new HashSet<Projects>();
            this.Tasks = new HashSet<Tasks>();
            this.Tasks1 = new HashSet<Tasks>();
            this.Timesheets = new HashSet<Timesheets>();
        }

        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
        public string EMail { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane!")]   
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
        [RegularExpression(@"^[0-9]{7,15}", ErrorMessage = "Podaj poprawny numer telefonu!")]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
        public int JobPositionID { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public byte LoginNo { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane!")]
        public byte EmployeeStateID { get; set; }

        public virtual EmployeeState EmployeeState { get; set; }
        public virtual JobPositions JobPositions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProjectMembers> ProjectMembers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Projects> Projects { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Projects> Projects1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Projects> Projects2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tasks> Tasks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tasks> Tasks1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Timesheets> Timesheets { get; set; }
    }
}
