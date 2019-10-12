using HNProject.Models;
using HNProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HNProject.Controllers
{
    [RoutePrefix("Shippers")]
    public class ShipperController : BaseController
    {
        [HttpGet, Route("")]
        public IHttpActionResult Get([FromUri]double longtitude, [FromUri]double latitude)
        {
            try
            {
                var sCoord = new GeoCoordinate(longtitude, latitude);
                var address = _context.Accounts.AsEnumerable().Where(x => x.id_role == "1").Select(x => new ShipperVM
                {
                    id = x.id_account,
                    username = x.username,
                    lat = x.GroupAddress.Addresses.FirstOrDefault().lat,
                    @long = x.GroupAddress.Addresses.FirstOrDefault().@long,
                    getDistance = sCoord.GetDistanceTo(new GeoCoordinate(x.GroupAddress.Addresses.FirstOrDefault().@long ?? 0, x.GroupAddress.Addresses.FirstOrDefault().lat ?? 0))
                }).ToList().OrderBy(x => x.getDistance).Take(1);

                foreach (var item in address)
                {
                    if (item.getDistance <= 3000)
                    {
                        return Ok(item);
                    }
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
