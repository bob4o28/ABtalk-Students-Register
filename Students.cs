using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABtalk_Students_Register
{
    class Students
    {
        public int idStudents { get; set; }
        public string FirstName { get; set; }
        public string MidName { get; set; }
        public string LastName { get; set; }
        public string School { get; set; }
        public string Class { get; set; }
        public string ClassLetter { get; set; }
        public DateTime RegTime { get; set; }
        public DateTime? LastTime { get; set; }
        public DateTime? PauseTime { get; set; }
        public DateTime? PauseLast { get; set; }
        public string Status { get; set; }
    }
}
