﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CapStoneProject.Models
{
    public class BidRequest
    {
        public int BidRequestID { get; set; }

        [Required]
        public int UserID { get; set; }

        public string ProjectDescription { get; set; }

        public string IdealTimeFrame { get; set; }

        public DateTime RequestedStartDate { get; set; }

        public string ProjectLocation { get; set; }

        public bool? NewBuild { get; set; }

        public bool? Remodel { get; set; }

        public bool? Concrete { get; set; }

        public bool? WoodWork { get; set; }
    }
}
