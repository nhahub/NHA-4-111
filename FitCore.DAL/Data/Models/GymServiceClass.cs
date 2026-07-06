using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class GymServiceClass
    {
        public int ServiceId { get; set; }
        public GymService GymService { get; set; } = null!;

        public int ClassId { get; set; }
        public Class Class { get; set; } = null!;

        public int AllowedAttendancesPerMonth { get; set; }
    }
}
