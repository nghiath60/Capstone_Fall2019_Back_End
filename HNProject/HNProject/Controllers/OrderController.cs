using HNProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data.Entity;
using System.Web.Http;
using HNProject.ViewModels;
using System.Device.Location;

namespace HNProject.Controllers
{
    [RoutePrefix("Orders")]
    public class OrderController : BaseController
    {
            

        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            var customer = _context.Orders.Select(x => x.Account);

            var shipper = _context.Orders.Select(x => x.Account1); ;

            var model = _context.Orders.Select(_ => new
            {
                id_order = _.id_order,
                shipper_id = _.id_shipper,
                customer_id = _.id_customer,
                order_state = _.state,
                point = _.point,
                shipping_cost = _.shipping_cost,
                time_to_ship = _.time_to_ship,
                customer_address_id = _.customer_address_id,
                dis_cus_to_market = _.dis_cus_to_market,
                //shipper_name = shipper.Select(x => x.username),
                //customer_name = customer.Select(x => x.username),
                created_date = _.created_date,
                id_address = _.id_address,
                phone = _.phone,
                name = _.name,
                taking_time = _.taking_time,
                done_time = _.done_time,
                order_code = _.order_code,
                customer_comment = _.customer_comment,
                system_cost = _.system_cost,
                orderDetails = _.OrderDetails.Select(__ => new
                {
                    __.Product.name,
                    Price = __.Product.price*__.quanlity,
                    __.Product.qualify,
                    market_name = __.SMarket.name,
                   
                })
            }).ToList().OrderBy(x => x.done_time).ThenBy(x => x.taking_time).ThenBy(x =>x.time_to_ship).Reverse();

            return Ok(model);
        }

   

        [HttpGet, Route("id")]
        public IHttpActionResult GetById(string id)
        {
            var customer = _context.Orders.Select(x => x.Account);

            var shipper = _context.Orders.Select(x => x.Account1); ;
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var order = _context.Orders.Find(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    id_order = order.id_order,
                    //shipper_name = (order.Account.id_account == order.id_shipper) ? order.Account.username : "",
                    //customer_name = (order.Account.id_account == order.id_customer) ? order.Account.username : "",
                    shipper_id = order.id_shipper,
                    customer_name = order.id_customer,
                    order_state = order.state,
                    created_date = order.created_date,
                    id_address = order.id_address,
                    point = order.point,
                    dis_cus_to_market = order.dis_cus_to_market,
                    shipping_cost = order.shipping_cost,
                    time_to_ship = order.time_to_ship,
                    customer_address_id = order.customer_address_id,
                    phone = order.phone,
                    name = order.name,
                    taking_time = order.taking_time,
                    done_time = order.done_time,
                    order_code = order.order_code,
                    customer_comment = order.customer_comment,
                    system_cost = order.system_cost,
                    orderDetails = order.OrderDetails.Select(__ => new
                    {
                        __.Product.name,
                        Price = __.Product.price * __.quanlity,
                        __.quanlity,
                        __.Product.qualify,
                        market_name = __.SMarket.name,
                        img = __.Product.GroupImage.Images.Select(i => i.url).Take(1),
                    })
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Post(OrderVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (model == null)
                {
                    return NotFound();
                }

                var address_market = _context.Addresses.FirstOrDefault(x => x.id_address == model.id_address);
                var address_customer = _context.Addresses.FirstOrDefault(s => s.id_address == model.customer_address_id);
                GeoCoordinate sCoor = new GeoCoordinate(address_market.lat?? 0,address_market.@long?? 0);
                GeoCoordinate cCoor = new GeoCoordinate(address_customer.lat ?? 0, address_customer.@long ?? 0);
                var distance_s_to_m = sCoor.GetDistanceTo(cCoor);
                double ship_cost = 0;
                double _point = 0;
                if (distance_s_to_m > 1000)
                {
                    ship_cost = distance_s_to_m * 10;
                }
                else
                {
                    ship_cost = 10000;
                }
                var stime = new TimeSpan(17, 00, 00);
                var etime = new TimeSpan(19, 00, 00);
                if((stime.CompareTo(model.time_to_ship?? new TimeSpan(00,00,00)) <0) && (etime.CompareTo(model.time_to_ship?? new TimeSpan(00,00,00)) > 0))
                {
                    ship_cost = ship_cost + 10000;
                    _point += 2;
                }
                byte[] buffer = Guid.NewGuid().ToByteArray();
                var order_code = BitConverter.ToInt64(buffer, 0).ToString().Substring(9);


                if (distance_s_to_m > 1000)
                {
                    _point += distance_s_to_m / 100;
                }
                else
                {
                    _point += 10;
                }

                ship_cost = (int)ship_cost;
                int nguyen = (int) ship_cost / 1000;
                int du = (int) ship_cost % 1000;
                if (du < 500 && du > 0)
                {
                    ship_cost = (nguyen * 1000) + 500;
                }
                else
                {
                    ship_cost = (nguyen * 1000) + 1000;
                }

                distance_s_to_m = distance_s_to_m / 1000;
                 distance_s_to_m = double.Parse(distance_s_to_m.ToString("0.0"));

                DateTime dt = DateTime.Now;
                var order = _context.Orders.Add(new Models.Order
                {
                    id_order = model.id_order,
                    created_date = dt,
                    id_customer = model.id_customer,
                    id_shipper = model.id_shipper,
                    state = 1,
                    id_group_image = model.id_group_image,
                    dis_cus_to_market = distance_s_to_m,
                    id_address = model.id_address,
                    point = (int) _point,
                    shipping_cost = ship_cost,
                    time_to_ship = model.time_to_ship,
                    customer_address_id = model.customer_address_id,
                    phone = model.phone,
                    name = model.name,
                    taking_time = dt,
                    done_time = dt,
                    order_code = order_code,
                    customer_comment = model.customer_comment,
                    system_cost = 10000,
                    OrderDetails = model.OrderDetails.Select(x => new OrderDetail
                    {
                        id_orderdetail = x.id_orderdetail,
                        id_order = x.id_order,
                        id_product = x.id_product,
                        id_market = x.id_market,
                        price = x.price,
                        quanlity = x.quanlity,
                        priority = x.priority

                    }).ToList()
                });
                _context.SaveChanges();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

       [HttpPut,Route("Id_Shipper_Add_to_Order")]
       public IHttpActionResult Put(string order_id,string id_shipper)
        {
            try
            {
                var order = _context.Orders.Find(order_id);
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}
                if (order == null)
                {
                    return NotFound();
                }

                order.id_shipper = id_shipper;
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("")]
        public IHttpActionResult Put(string id,OrderVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (model == null)
                {
                    return NotFound();
                }

            
                
                var order = _context.Entry(new Order
                {
                    id_order = id,
                    created_date = model.created_date,
                    id_customer = model.id_customer,
                    id_shipper = model.id_shipper,
                    state = model.state,
                    id_group_image = model.id_group_image,
                    id_address = model.id_address,
                    point = model.point,
                    shipping_cost = model.shipping_cost,
                    time_to_ship = model.time_to_ship,
                    customer_address_id = model.customer_address_id,
                    dis_cus_to_market = model.dis_cus_to_market,
                    phone = model.phone,
                    name = model.name,
                    
                    taking_time = model.taking_time,
                    done_time = model.done_time,
                    order_code = model.order_code,
                    customer_comment = model.customer_comment,
                    system_cost = model.system_cost,
                    OrderDetails = model.OrderDetails.Select(x => new OrderDetail
                    {
                        id_orderdetail = x.id_orderdetail,
                        id_order = x.id_order,
                        id_product = x.id_product,
                        id_market = x.id_market,
                        price = x.price,
                        quanlity = x.quanlity,
                        priority = x.priority

                    }).ToList()
                }).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route("")]
        public IHttpActionResult Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var order = _context.Entry(new Models.Order
                {
                    id_order = id
                }).State = EntityState.Deleted;
                _context.SaveChanges();
                return Ok();
                
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
