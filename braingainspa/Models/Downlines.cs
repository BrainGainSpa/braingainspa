using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace braingainspa.Models
{
    public class Downlines
    {
        //Person Id  
        public int ID { get; set; }

        //Person Name  
        public string Name { get; set; }

        //Cat Description  
        public string Description { get; set; }

        //represnts Parent ID and it's nullable  
        public int? Pid { get; set; }
        [ForeignKey("Pid")]

        public virtual Downlines Parent { get; set; }
        public virtual ICollection<Downlines> Childs { get; set; }
    }
}