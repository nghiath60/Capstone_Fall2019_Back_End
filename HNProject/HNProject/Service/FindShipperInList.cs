using HNProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.Service
{
    public class FindShipperInList
    {
         public ShipperVM FindShipper(List<ShipperVM> list, string id)
        {
            foreach (var item in list)
            {
                if(item.id == id)
                {
                    return item;
                }
            }
            return null;
        }
    }
}