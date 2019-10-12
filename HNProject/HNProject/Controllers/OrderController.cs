using HNProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data.Entity;
using System.Web.Http;
using HNProject.ViewModels;

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
                shipper_id = shipper.Select(x => x.id_account),
                customer_id = customer.Select(x => x.id_account),
                order_state = _.state,
                //shipper_name = shipper.Select(x => x.username),
                //customer_name = customer.Select(x => x.username),
                created_date = _.created_date,
                id_address = _.id_address,
                orderDetails = _.OrderDetails.Select(__ => new
                {
                    __.Product.name,
                    __.Product.price,
                    __.Product.qualify
                })
            }).ToList();

            return Ok(model);
        }

        [HttpGet, Route("id")]
        public IHttpActionResult GetById(string id)
        {
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
                    shipper_name = (order.Account.id_account == order.id_shipper) ? order.Account.username : "",
                    customer_name = (order.Account.id_account == order.id_shipper) ? order.Account.username : "",
                    created_date = order.created_date,
                    id_address = order.id_address,
                    orderDetails = order.OrderDetails.Select(__ => new
                    {
                        __.Product.name,
                        __.Product.price,
                        __.Product.qualify
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

                var order = _context.Orders.Add(new Order
                {
                    id_order = model.id_order,
                    created_date = model.created_date,
                    id_customer = model.id_customer,
                    id_shipper = model.id_shipper,
                    state = 1,
                    id_group_image = model.id_group_image,
                    id_address = model.id_address,
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
                return Ok();
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
                var order = _context.Entry(new Order
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
