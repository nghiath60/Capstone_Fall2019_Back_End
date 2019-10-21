using HNProject.Controllers;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoSignalR.Hubs
{
    public class HubSSmarket : Hub
    {
        //goi 1 thang   
        public void Hub_NotifyShipper(string connectionIdCuaThangClientDo, string id_order, int count, string msg, string id_shipper, double lat, double @long)
        {
            string thamSoGiDoCuaHamGiDoDuoiClient2 = msg;

            ////goi tat ca
            //Clients.All.tenHamNaoDoDuoiClient2("goi tat ca o day: " + 
            //    thamSoGiDoCuaHamGiDoDuoiClient2);
            //goi chi dinh t` nao day
            Clients.Client(connectionIdCuaThangClientDo).NotifyShipper(id_order,count,msg,id_shipper,lat,@long);
        }

        public void NotifyCustomer(string connectionIdCuaThangClientDo,string msg)
        {
            Clients.Client(connectionIdCuaThangClientDo).NotifyCustomer(msg);
        }

        public void InRaGiDo(string gido, string connectionID)
        {
            gido = "Tim thay mot shipper!";
            string thamSoGiDoCuaHamGiDoDuoiClient2 = gido;


            string connectionID_shipper = connectionID;
            ////goi tat ca
            //Clients.All.tenHamNaoDoDuoiClient2("goi tat ca o day: " + 
            //    thamSoGiDoCuaHamGiDoDuoiClient2);
            //goi chi dinh t` nao day
            Clients.Client(connectionID_shipper).tenHamNaoDoDuoiClient2(new Random().Next() + ":" + thamSoGiDoCuaHamGiDoDuoiClient2);
        }
    }
}