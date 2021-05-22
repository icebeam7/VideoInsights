using System;
using System.Collections.Generic;

#nullable disable

namespace VideoInsights.Api.Models
{
    public partial class Timeperiod
    {
        public string Id { get; set; }
        public string Keyframeid { get; set; }
        public string Starttime { get; set; }
        public string Endtime { get; set; }
    }
}
