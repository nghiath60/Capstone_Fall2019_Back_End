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
    [RoutePrefix("OrdersGetFull")]
    public class OrdersGetFullController : BaseController
    {


        [HttpGet, Route("")]
        public IHttpActionResult GetFull()
        {
            var customer = _context.Orders.Select(x => x.Account);

            var shipper = _context.Orders.Select(x => x.Account1); ;

            var model = _context.Orders.Select(_ => new
            {
                id_order = _.id_order,
                shipper = new
                {
                   id_account =  shipper.Select(x => x.id_account),
                    username =  shipper.Select(x => x.username),
                    password = shipper.Select(x => x.password),
                    phone = shipper.Select(x => x.phone),
                    dob =  shipper.Select(x => x.dob),
                    email = shipper.Select(x => x.email),
                    fb_id =  shipper.Select(x => x.fb_id),
                    id_group_address =  shipper.Select(x => x.id_group_address),
                    id_group_image =  shipper.Select(x => x.id_group_image),
                    id_role = shipper.Select(x => x.id_role),
                    state = shipper.Select(x => x.state)
                },
                customer = new
                {
                    id_account = customer.Select(x => x.id_account),
                    username = customer.Select(x => x.username),
                    password = customer.Select(x => x.password),
                    phone = customer.Select(x => x.phone),
                    dob = customer.Select(x => x.dob),
                    email = customer.Select(x => x.email),
                    fb_id = customer.Select(x => x.fb_id),
                    id_group_address = customer.Select(x => x.id_group_address),
                    id_group_image = customer.Select(x => x.id_group_image),
                    id_role = customer.Select(x => x.id_role),
                    state = customer.Select(x => x.state)
                },
                //_customer = customer.Where(x => x.id_account == customer_id),

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

    }    
}
