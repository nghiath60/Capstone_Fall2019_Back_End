using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.ViewModels
{
    public class OrderVM
    {
        public string id_order { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string id_customer { get; set; }
        public string id_shipper { get; set; }
        public Nullable<int> state { get; set; }
        public string id_group_image { get; set; }
        public string id_address { get; set; }
        public virtual ICollection<OrderDetailVM> OrderDetails { get; set; }
    }

    public class OrderDetailVM
    {
        public string id_orderdetail { get; set; }
        public string id_order { get; set; }
        public string id_product { get; set; }
        public string id_market { get; set; }
        public Nullable<double> price { get; set; }
        public Nullable<double> quanlity { get; set; }
        public Nullable<int> priority { get; set; }
    }
}