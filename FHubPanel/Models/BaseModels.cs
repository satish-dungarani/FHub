using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class BaseModels
    {
        public Nullable<int> InsUser { get; set; }
        public Nullable<DateTime> InsDate { get; set; }
        public string InsTerminal { get; set; }
        public Nullable<int> UpdUser { get; set; }
        public Nullable<DateTime> UpdDate { get; set; }
        public string UpdTerminal { get; set; }
    }
}