//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HNProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Address
    {
        public string id_address { get; set; }
        public Nullable<double> @long { get; set; }
        public Nullable<double> lat { get; set; }
        public string address1 { get; set; }
        public string id_group_address { get; set; }
        public string description { get; set; }
        public Nullable<int> priority { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
    
        public virtual GroupAddress GroupAddress { get; set; }
    }
}
