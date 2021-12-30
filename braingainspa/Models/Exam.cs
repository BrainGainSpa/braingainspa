using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace braingainspa.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public TimeSpan ReaminTime { get; set; }

    }
}