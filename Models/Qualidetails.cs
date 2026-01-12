using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    [MetadataType(typeof(tblQualiDetailsMetaData))]
    public partial class tblQualiDetails
    {
        public object BitIsSlected { get; internal set; }
    }
}