using OilTeamProject.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OilTeamProject.Areas.Admin.Controllers.api
{
    public class ProductStocksController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public ProductStocksController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpDelete]
        public IHttpActionResult Delete(string Id)
        {
            var productStock = _context.ProductStocks
                .Single(ps => ps.ProductStockID == Id);

            if (productStock == null)
                return NotFound();

            _context.ProductStocks.Remove(productStock);
            _context.SaveChanges();

            return Ok();
        }

    }
}
