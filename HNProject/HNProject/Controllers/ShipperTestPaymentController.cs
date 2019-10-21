using HNProject.Models;
using HNProject.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web;

namespace HNProject.Controllers
{
    [RoutePrefix("TestPayment")]
    public class ShipperTestPaymentController : BaseController
    {
        [HttpGet, Route("id_order")]
        public IHttpActionResult Get(string query,string transaction_info,string order_code,string price,string payment_id,string payment_type,string error_text,string secure_code,[FromUri]double longtitude, [FromUri]double latitude,string id_shipper = null)
        {
            //Get URL from Ngan luong
            //var uri = new Uri(url);
            //var query = HttpUtility.ParseQueryString(uri.Query);
            //var transaction_info = query.Get("transaction_info");
            //var order_code = query.Get("order_code");
            //var price = query.Get("price");
            //var payment_id = query.Get("payment_id");
            //var payment_type = query.Get("payment_type");
            //var error_text = query.Get("error_text");
            //var secure_code = query.Get("secure_code");
            //var return_url = query.Get("return_url");

            //Customer from order
            var customer = _context.Orders.Select(x => x.Account);

            //Shipper from order
            var shipper = _context.Orders.Select(x => x.Account1);
            
            //order từ order_code
            var order_fcm = _context.Orders.Find(order_code);


            try
            { 
                NL_Checkout checkOut = new NL_Checkout();
                bool rs = checkOut.verifyPaymentUrl(transaction_info, order_code, price, payment_id, payment_type, error_text, secure_code);
                byte[] buffer = Guid.NewGuid().ToByteArray();
                var trans_id = BitConverter.ToInt64(buffer, 0).ToString();
                rs = true;
                #region rs:OK Shipper:tìm thấy
                if (rs)
                {
                    _context.Transactions.Add(new Transaction
                    {
                        id_transaction = trans_id,
                        id_order = order_code,
                        money = double.Parse(price),
                        created_time = DateTime.Today

                    });
                    _context.SaveChanges();
                    try
                    {
                        var sCoord = new GeoCoordinate(latitude, longtitude);
                        var address = _context.Accounts.AsEnumerable().Where(x => x.id_role == "1").Select(x => new ShipperVM
                        {
                            id = x.id_account,
                            username = x.username,
                            lat = x.GroupAddress.Addresses.FirstOrDefault().lat,
                            @long = x.GroupAddress.Addresses.FirstOrDefault().@long,
                            levels = x.Levels.Select(xx => xx.point).FirstOrDefault(),
                            fcm_token = x.fcm_token,
                            id_order = order_code,
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
                        list1.Reverse();

                        if(list.Count == 0)
                        {
                            WebRequest ftRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                            ftRequest.Method = "post";
                            //serverKey - Key from Firebase cloud messaging server  
                            ftRequest.Headers.Add(string.Format("Authorization: key={0}", "AIzaSyDN6yImcd7eWPRiN86cn1C7iecVNaF945M"));
                            //Sender Id - From firebase project setting  
                            ftRequest.Headers.Add(string.Format("Sender: id={0}", "462404511537"));
                            ftRequest.ContentType = "application/json";

                            var fcm_ss = "eg8nUQs-lbQ:APA91bHPgVKmMy8d0vnpt-u0UMJPivSvU9kaGPyttY1-ZP-gcu6qkt7VuJ9MG-uDPOPCIkDxpgWJ0TfdP_axotwn-mZEcBOpkEXpn1QfD_HeueUMt-myWeS3ZATw_R11s5D2-N83Sq6U";
                            
                            Account _customer = _context.Accounts.Find(order_fcm.id_customer);
                            fcm_ss = _context.Accounts.AsEnumerable().Where(x => x.id_role == "2" && x.id_account == _customer.id_account).Select(x => x.fcm_token).FirstOrDefault();
                            var payload2 = new
                            {
                                //to = customer.Select(x => x.fcm_token).FirstOrDefault().ToString(),
                                to = fcm_ss,
                                priority = "high",
                                content_available = true,
                                notification = new
                                {
                                    body = "Không tìm thấy Shipper gần Market!",
                                    title = "Test",
                                    badge = 1
                                },

                            };

                            string postbody2 = JsonConvert.SerializeObject(payload2).ToString();
                            Byte[] byteArray2 = Encoding.UTF8.GetBytes(postbody2);
                            ftRequest.ContentLength = byteArray2.Length;
                            using (Stream dataStream = ftRequest.GetRequestStream())
                            {
                                dataStream.Write(byteArray2, 0, byteArray2.Length);
                                using (WebResponse tResponse = ftRequest.GetResponse())
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
                            return NotFound();
                        }


                        WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                        tRequest.Method = "post";
                        //serverKey - Key from Firebase cloud messaging server  
                        tRequest.Headers.Add(string.Format("Authorization: key={0}", "AIzaSyDN6yImcd7eWPRiN86cn1C7iecVNaF945M"));
                        //Sender Id - From firebase project setting  
                        tRequest.Headers.Add(string.Format("Sender: id={0}", "462404511537"));
                        tRequest.ContentType = "application/json";

                      

                        var fcm_s = "eg8nUQs-lbQ:APA91bHPgVKmMy8d0vnpt-u0UMJPivSvU9kaGPyttY1-ZP-gcu6qkt7VuJ9MG-uDPOPCIkDxpgWJ0TfdP_axotwn-mZEcBOpkEXpn1QfD_HeueUMt-myWeS3ZATw_R11s5D2-N83Sq6U";
                       
                        ShipperVM shipperr = list1.First();
                        fcm_s = _context.Accounts.AsEnumerable().Where(x => x.id_role == "1" && x.id_account == shipperr.id).Select(x => x.fcm_token).FirstOrDefault();

                        var payload = new
                        {
                            //Bỏ vô để test cho vui chứ chưa có FCM token nó trả ra lỗi BAD request(400) từ firebase server
                            to = "eg8nUQs-lbQ:APA91bHPgVKmMy8d0vnpt-u0UMJPivSvU9kaGPyttY1-ZP-gcu6qkt7VuJ9MG-uDPOPCIkDxpgWJ0TfdP_axotwn-mZEcBOpkEXpn1QfD_HeueUMt-myWeS3ZATw_R11s5D2-N83Sq6U",
                            priority = "high",
                            content_available = true,
                            notification = new
                            {
                                body = "Bạn có một đơn hàng!",
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

                        if (list1.Count > 0)
                        {
                            order_fcm.id_shipper = list1.First().id;
                            _context.SaveChanges();
                            return Ok(list1);
                        }
                        #endregion

                        #region rs:OK Shipper: không tìm thấy

                        return NotFound();

                    }
                    catch (Exception ex)
                    {
                        return InternalServerError(ex);
                    }
                }

                #endregion

                #region rs:không thành công
                else
                {
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    //serverKey - Key from Firebase cloud messaging server  
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", "AIzaSyDN6yImcd7eWPRiN86cn1C7iecVNaF945M"));
                    //Sender Id - From firebase project setting  
                    tRequest.Headers.Add(string.Format("Sender: id={0}", "462404511537"));
                    tRequest.ContentType = "application/json";

                    var fcm_ss = "eg8nUQs-lbQ:APA91bHPgVKmMy8d0vnpt-u0UMJPivSvU9kaGPyttY1-ZP-gcu6qkt7VuJ9MG-uDPOPCIkDxpgWJ0TfdP_axotwn-mZEcBOpkEXpn1QfD_HeueUMt-myWeS3ZATw_R11s5D2-N83Sq6U";

                    Account _customer = _context.Accounts.FirstOrDefault(x => x.id_account == order_fcm.id_customer);
                    fcm_ss = _context.Accounts.AsEnumerable().Where(x => x.id_role == "2" && x.id_account == _customer.id_account).Select(x => x.fcm_token).FirstOrDefault();

                    var payload3 = new
                    {
                        // to
                        to = fcm_ss,
                        priority = "high",
                        content_available = true,
                        notification = new
                        {
                            body = "Giao dịch thất bại!",
                            title = "Test",
                            badge = 1
                        },

                    };

                    string postbody3 = JsonConvert.SerializeObject(payload3).ToString();
                    Byte[] byteArray3 = Encoding.UTF8.GetBytes(postbody3);
                    tRequest.ContentLength = byteArray3.Length;
                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray3, 0, byteArray3.Length);
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
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            #endregion

        }

    }
}
