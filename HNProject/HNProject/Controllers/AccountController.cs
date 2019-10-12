using HNProject.Models;
using HNProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HNProject.Controllers
{
    [RoutePrefix("Accounts")]
    public class AccountController : BaseController
    {
        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_context.Accounts.Select(x => new
            {
                x.id_account,
                x.username,
                x.password,
                x.phone,
                x.name,
                x.dob,
                x.email,
                x.fb_id,
                groupAddress = x.GroupAddress.Addresses,
                groupImages = x.GroupImage.Images.Select(_ => _.url),
                roleName = x.Role.name,
                order = x.Orders,
                favouriteProducts = x.FavouriteProducts.Select(_ => _.Product.name),
                levels = x.Levels.Select(_ => _.point),
                x.state,

            }));
        }

        [HttpGet, Route("idAccount")]
        public IHttpActionResult GetAddressByAccount(string id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var address = _context.Accounts.FirstOrDefault(x => x.id_account == id).GroupAddress.Addresses;
                if (address == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                   address =  address.Select(x => new
                    {
                        x.address1
                    })
                }) ;
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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
                var account = _context.Accounts.Where(x => x.id_account == id).FirstOrDefault();
                if (account == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    id_account = account.id_account,
                    username = account.username,
                    password = account.password,
                    phone = account.phone,
                    name = account.name,
                    dob = account.dob,
                    email = account.email,
                    fb_id = account.fb_id,
                    groupAddress = account.GroupAddress.Addresses,
                    groupImages = account.GroupImage.Images.Select(x => x.url),
                    roleName = account.Role.name,
                    order = account.Orders,
                    favouriteProducts = account.FavouriteProducts.Select(_ => _.Product.name),
                    levels = account.Levels.Select(_ => _.point),
                    account.state,
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost, Route("")]
        public IHttpActionResult Post(AccountVM model)
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
                var product = _context.Accounts.Add(new Account
                {
                    id_account = model.id_account,
                    username = model.username,
                    password = model.password,
                    phone = model.phone,
                    name = model.name,
                    dob = model.dob,
                    email = model.email,
                    fb_id = model.fb_id,
                    id_group_address = model.id_group_address,
                    id_group_image = model.id_group_image,
                    id_role = model.id_role,
                    state = model.state,
                });
                _context.SaveChanges();
                return Ok(product);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("")]
        public IHttpActionResult Put(string id, AccountVM model)
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
                var account = _context.Entry(new Account
                {
                    id_account = id,
                    username = model.username,
                    password = model.password,
                    phone = model.phone,
                    name = model.name,
                    dob = model.dob,
                    email = model.email,
                    fb_id = model.fb_id,
                    id_group_address = model.id_group_address,
                    id_group_image = model.id_group_image,
                    id_role = model.id_role,
                    state = model.state,
                }).State = EntityState.Modified;
                _context.SaveChanges();
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
                var account = _context.Entry(new Account
                {
                    id_account = id
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
