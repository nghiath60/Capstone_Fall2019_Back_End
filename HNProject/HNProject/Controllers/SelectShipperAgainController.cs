using HNProject.Models;
using HNProject.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace HNProject.Controllers
{
    [RoutePrefix("SelectShipper_Again")]
    public class SelectShipperAgainController : BaseController
    {
        [HttpGet, Route("id_order")]
        public IHttpActionResult Get(int count,[FromUri]double latitude, [FromUri]double longtitude, string id_order,string id_shipper = null)
        {
            count++;
            if(count == 4)
            {
                return NotFound();
            }
            try
            {
                //var order = _context.Orders.Find(id_order);
                var sCoord = new GeoCoordinate(latitude, longtitude);
             
                var address = _context.Accounts.AsEnumerable().Where(x => x.id_role == "1").Select(x => new ShipperVM
                {
                    id = x.id_account,
                    username = x.username,
                    lat = x.GroupAddress.Addresses.FirstOrDefault().lat,
                    @long = x.GroupAddress.Addresses.FirstOrDefault().@long,
                    levels = x.Levels.Select(xx => xx.point).FirstOrDefault(),
                    fcm_token = x.fcm_token,
                    id_order = id_order,
                    getDistance = sCoord.GetDistanceTo(new GeoCoordinate(x.GroupAddress.Addresses.FirstOrDefault().lat ?? 0, x.GroupAddress.Addresses.FirstOrDefault().@long ?? 0))
                }).ToList().OrderBy(x => x.getDistance);

                var list = new List<ShipperVM>();
                var list1 = new List<ShipperVM>();
                var list2 = new List<ShipperVM>();
                foreach (var item in address)
                {
                    if (item.getDistance <= 3000)
                    {
                        list.Add(item);
                    }
                }

                foreach (var item in list.OrderBy(x => x.levels).Take(5))
                {
                    list1.Add(item);
                }
                if(id_shipper != null)
                {
                    var temp = list1.FirstOrDefault(x => x.id == id_shipper);
                    if (temp != null)
                    {
                        list1.Remove(temp);
                    }
                }

                //if (id_shipper != null)
                //{
                //    var s = _context.Accounts.FirstOrDefault(x => x.id_account == id_shipper);
                //    var _shipper = new ShipperVM
                //    {
                //        id = s.id_account,
                //        username = s.username,
                //        lat = s.GroupAddress.Addresses.FirstOrDefault().lat,
                //        @long = s.GroupAddress.Addresses.FirstOrDefault().@long,
                //        id_order = id_order,
                //        levels = s.Levels.FirstOrDefault().point,
                //        getDistance = sCoord.GetDistanceTo(new GeoCoordinate(s.GroupAddress.Addresses.FirstOrDefault().@long ?? 0, s.GroupAddress.Addresses.FirstOrDefault().lat ?? 0))
                //    };

                //}
                if(list1.Count ==0)
                {
                    return NotFound();
                }
                else 
                {
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    //serverKey - Key from Firebase cloud messaging server  
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", "AIzaSyDN6yImcd7eWPRiN86cn1C7iecVNaF945M"));
                    //Sender Id - From firebase project setting  
                    tRequest.Headers.Add(string.Format("Sender: id={0}", "462404511537"));
                    tRequest.ContentType = "application/json";


                    var fcm_s = "eg8nUQs-lbQ:APA91bHPgVKmMy8d0vnpt-u0UMJPivSvU9kaGPyttY1-ZP-gcu6qkt7VuJ9MG-uDPOPCIkDxpgWJ0TfdP_axotwn-mZEcBOpkEXpn1QfD_HeueUMt-myWeS3ZATw_R11s5D2-N83Sq6U";

                    ShipperVM shipper = list1.First();

                    fcm_s = _context.Accounts.AsEnumerable().Where(x => x.id_role == "1" && x.id_account == shipper.id).Select(x => x.fcm_token).FirstOrDefault();
                    var payload = new
                    {
                        to = fcm_s,
                        priority = "high",
                        content_available = true,
                        notification = new
                        {
                            body = "Test",
                            title = "Test",
                            badge = 1
                        },

                    };

                    string postbody = JsonConvert.SerializeObject(payload).ToString();
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                    tRequest.ContentLength = byteArray.Length;
                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        String sResponseFromServer = tReader.ReadToEnd();
                                        //result.Response = sResponseFromServer;
                                    }
                            }
                        } 
                    }
                }
                if (list1.Count > 0)
                {
                    var order_fcm = _context.Orders.Find(id_order); 
                    order_fcm.id_shipper = list1[0].id;
                    _context.SaveChanges();
                    return Ok(list1);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

    }
}
