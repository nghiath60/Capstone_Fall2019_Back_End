using HNProject.Models;
using HNProject.Service;
using HNProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HNProject.Controllers
{
    [RoutePrefix("Products")]
    public class ProductController : BaseController
    {
        //[HttpGet, Route("")]
        //public IHttpActionResult Get()
        //{
        //    return Ok(_context.Products.Select(x => new
        //    {
        //        x.id_product,
        //        x.name,
        //        groupImages = x.GroupImage.Images.Select(_ => _.url),
        //        x.price,
        //        x.qualify,
        //        x.description,
        //        x.type,
        //        brandName = x.Brands.Select(_ => _.name),
        //        productCategory = x.ProductCategories.Select(_ => _.Category.name)
        //    }));
        //}

        [HttpGet, Route("")]
        public IHttpActionResult GetAll([FromUri]string searchValue, [FromUri]int pageIndex = 1, [FromUri]int pageSize = 10)
        {
            try
            {
                var paginationImp = new PaginationImp();
                var query = _context.Products.Select(x => x)
                                   .Where(x => searchValue == null || x.name.Contains(searchValue))
                                   .Select(x => new
                                   {
                                       id_product = x.id_product,
                                       name = x.name,
                                       groupImages = x.GroupImage.Images.Select(_ => _.url),
                                       price = x.price,
                                       qualify = x.qualify,
                                       description = x.description,
                                       type = x.type,
                                       brandName = x.Brands.Select(_ => _.name),
                                       productCategory = x.ProductCategories.Select(_ => _.Category.name)

                                   });
                return Ok(paginationImp.ToPagedList(pageIndex, pageSize, query));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [HttpGet, Route("idcate")]
        public IHttpActionResult GetProductByCategory(string idcate, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var paginationImp = new PaginationImp();
                var query = _context.ProductCategories.Where(x => x.id_cate == idcate).Select(x => x.Product);
                var model = query.Select(x => new 
                {
                    products = new 
                    {
                        id_product = x.id_product,
                        name = x.name,
                        groupImages = x.GroupImage.Images.Select(_ => _.url),
                        price = x.price,
                        qualify = x.qualify,
                        description = x.description,
                        type = x.type,
                        brandName = x.Brands.Select(_ => _.name),
                        productCategory = x.ProductCategories.Select(_ => _.Category.name)
                    }
                });

                return Ok(paginationImp.ToPagedList(pageIndex, pageSize, model));
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
                var product = _context.Products.Where(x => x.id_product == id).FirstOrDefault();
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    id_product = product.id_product,
                    name = product.name,
                    groupImages = product.GroupImage.Images.Select(_ => _.url),
                    price = product.price,
                    qualify = product.qualify,
                    description = product.description,
                    type = product.type,
                    brandName = product.Brands.Select(_ => _.name),
                    productCategory = product.ProductCategories.Select(_ => _.Category.name)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Post(ProductVM model)
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
                var product = _context.Products.Add(new Product
                {
                    id_product = model.id_product,
                    name = model.name,
                    id_group_image = model.id_group_image,
                    price = model.price,
                    qualify = model.qualify,
                    description = model.description,
                    type = model.type,
                    Brands = model.Brands.Select(x => new Models.Brand
                    {
                        id_brand = x.id_brand
                    }).ToList(),
                    ProductCategories = model.ProductCategories.Select(x => new ProductCategory
                    {
                        id_cate = x.id_cate,
                        id_product = x.id_product,
                        created_date = x.created_date
                    }).ToList(),
                    ProductInMarkets = model.ProductInMarkets.Select(x => new Models.ProductInMarket
                    {
                        id_market = x.id_market,
                        id_product = x.id_product,
                        created_date = x.created_date
                    }).ToList()
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
        public IHttpActionResult Put(string id, ProductVM model)
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
                var product = _context.Entry(new Product
                {
                    id_product = id,
                    name = model.name,
                    id_group_image = model.id_group_image,

                    price = model.price,
                    qualify = model.qualify,
                    description = model.description,
                    type = model.type,
                    Brands = model.Brands.Select(x => new Models.Brand
                    {
                        id_brand = x.id_brand
                    }).ToList(),
                    ProductCategories = model.ProductCategories.Select(x => new ProductCategory
                    {
                        id_cate = x.id_cate,
                        id_product = x.id_product,
                        created_date = x.created_date
                    }).ToList(),
                    ProductInMarkets = model.ProductInMarkets.Select(x => new Models.ProductInMarket
                    {
                        id_market = x.id_market,
                        id_product = x.id_product,
                        created_date = x.created_date
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
                var product = _context.Entry(new Product
                {
                    id_product = id
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
