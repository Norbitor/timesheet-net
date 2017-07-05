using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using timesheet_net.Models;

namespace timesheet_net.Utils.Security
{
    public class PermissionUtil
    {
        TimesheetDBEntities ctx = new TimesheetDBEntities();
        public bool IsAdministrator(int jobPositionId)
        {
            var jobPositions = from j in ctx.JobPositions
                               where j.JobPositionName == "Administrator" && j.JobPositionID == jobPositionId
                               select j;
            if (jobPositions.Count() > 0) return true;
            return false;
        }

        public bool IsAccountant(int jobPositionId)
        {
            var jobPositions = from j in ctx.JobPositions
                               where j.JobPositionName == "Ksiegowy" && j.JobPositionID == jobPositionId
                               select j;
            if (jobPositions.Count() > 0) return true;
            return false;
        }

        public bool IsHumanResources(int jobPositionId)
        {
            var jobPositions = from j in ctx.JobPositions
                               where j.JobPositionName == "HR" && j.JobPositionID == jobPositionId
                               select j;
            if (jobPositions.Count() > 0) return true;
            return false;
        }
    }
}