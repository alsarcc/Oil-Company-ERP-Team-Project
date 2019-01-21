using OilTeamProject.Persistence;
using OilTeamProject.ViewModels;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using OilTeamProject.Models.Products;
using System.Data.Entity.Migrations;

namespace OilTeamProject.Areas.Admin.Controllers
{
    public class ProductStocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductStocksController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Admin/ProductStock
        public ActionResult Index(string sortOrder)
        {
            //include everything
            var productStocks = _context.ProductStocks
                .Include(p => p.Product)
                .Include(p => p.Sector)
                .Include(p => p.Bottling);

            // run through the products in database and assign the expiration date to the method
            foreach (var product in productStocks)
            {
                product.CalculateExpirationDate();
                product.CheckQuantity();
            }



            // sorting 
            ViewBag.DescriptionSortParam = string.IsNullOrEmpty(sortOrder) ? "descr_desc" : "";
            ViewBag.AvailQuantityParam = sortOrder == "avail" ? "avail_desc" : "avail";

            ViewBag.CurrentSort = sortOrder;

            switch (sortOrder)
            {
                case "descr_desc":
                    productStocks = productStocks.OrderByDescending(ps => ps.Product.Description);
                    break;
                case "avail_desc":
                    productStocks = productStocks.OrderByDescending(ps => ps.AvailableQuantity);
                    break;
                case "avail":
                    productStocks = productStocks.OrderBy(ps => ps.AvailableQuantity);
                    break;
                default:
                    productStocks = productStocks.OrderBy(ps => ps.Product.Description);
                    break;
            }

            return View(productStocks.ToList());
        }


        public ActionResult Create()
        {
            var viewmodel = new ProductStockFromViewModel()
            {
                Bottlings = _context.Bottlings,
                Products = _context.Products,
                Sectors = _context.Sectors

            };

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductStockFromViewModel viewmodel)
        {

            if (!ModelState.IsValid)
            {
                viewmodel.Bottlings = _context.Bottlings;
                viewmodel.Products = _context.Products;
                viewmodel.Sectors = _context.Sectors;
                return View(viewmodel);
            }

            viewmodel.Bottling = _context.Bottlings.Find(viewmodel.BottlingID);
            viewmodel.CalculateExpirationDate();
            viewmodel.CheckQuantity();
            var productStock = new ProductStock(viewmodel);

            _context.ProductStocks.Add(productStock);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(string Id)
        {
            if (String.IsNullOrEmpty(Id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var productStock = _context.ProductStocks
                .Include(ps => ps.Bottling)
                .Include(ps => ps.Product)
                .Include(ps => ps.Sector)
                .SingleOrDefault(ps => ps.ProductStockID == Id);

            if (productStock == null)
                return HttpNotFound();

            var viewmodel = new ProductStockFromViewModel()
            {
                Id = productStock.ProductStockID,
                AvailableQuantity = productStock.AvailableQuantity,
                ActualQuantity = productStock.ActualQuantity,
                BottlingID = productStock.BottlingID,
                ProductID = productStock.ProductID,
                SectorID = productStock.SectorID,
                ExpirationDate = productStock.CalculateExpirationDate(),
                Name = productStock.Product.Name,
                IsLow = productStock.CheckQuantity()
            };


            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductStockFromViewModel viewmodel)
        {
            if (!ModelState.IsValid)
                return View();

            var productStock = _context.ProductStocks
                .Include(ps => ps.Product)
                .Include(ps => ps.Bottling)
                .Include(ps => ps.Sector)
                .FirstOrDefault(ps => ps.ProductStockID == viewmodel.Id);

            productStock.Modify(viewmodel);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public ActionResult OrderFromProduction(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var productStock = _context.ProductStocks
                .Include(ps => ps.Bottling)
                .Include(ps => ps.Product)
                .Include(ps => ps.Sector)
                .SingleOrDefault(ps => ps.ProductStockID == Id);

            if (productStock == null)
                return HttpNotFound();

            var viewmodel = new ProductStockFromViewModel()
            {
                Id = productStock.ProductStockID,
                AvailableQuantity = productStock.AvailableQuantity,
                ActualQuantity = productStock.ActualQuantity,
                BottlingID = productStock.BottlingID,
                ProductID = productStock.ProductID,
                SectorID = productStock.SectorID,
                ExpirationDate = productStock.CalculateExpirationDate(),
                Name = productStock.Product.Name,
                IsLow = productStock.CheckQuantity()
            };

            return View("Order",viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderFromProduction(ProductStockFromViewModel viewmodel,int orderAmount)
        {
            if (!ModelState.IsValid)
                return View("Order");

            var productStock = _context.ProductStocks
                .Include(ps => ps.Product)
                .Include(ps => ps.Bottling)
                .Include(ps => ps.Sector)
                .FirstOrDefault(ps => ps.ProductStockID == viewmodel.Id);

            productStock.OrderFromProduction(orderAmount);

            _context.Set<ProductStock>().AddOrUpdate(productStock);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        //// POST: Admin/Products/DeleteProduct/5
        //public void DeleteProductStock(string id)
        //{
        //     ProductStock productStock = _context.ProductStocks.Find(id);
        //    _context.ProductStocks.Remove(productStock);
        //    _context.SaveChanges();
        //}
    }
}