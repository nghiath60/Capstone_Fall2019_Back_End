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
    
    public partial class FavouriteProduct
    {
        public string id_product { get; set; }
        public string id_customer { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Account Account { get; set; }
    }
}
