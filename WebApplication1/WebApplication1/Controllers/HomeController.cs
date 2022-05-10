using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels.Basket;
using WebApplication1.ViewModels.Home;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly JuanDbContext _context;
        public HomeController(JuanDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<HomeSlider> homeSliders = _context.HomeSliders.Where(s => !s.IsDeleted).ToList();
            List<Product> products = _context.Products
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Color)
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Size)
                .Where(s => !s.IsDeleted).ToList();
            HomeVM homeVM = new HomeVM()
            {
                HomeSliders = homeSliders,
                Products = products,
                Blogs = _context.Blogs.ToList()
            };
            return View(homeVM);
        }
        public async Task<IActionResult> AddToBasket(int? id, int count = 1, int colorid = 1, int sizeid = 1)
        {
            if (id == null) return BadRequest();
            Product dBproduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (dBproduct == null) return NotFound();

            //List<Product> products = null;
            List<BasketVM> basketVMs = null;

            string cookie = HttpContext.Request.Cookies["basket"];

            if(cookie != "" && cookie != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
                if (basketVMs.Any(b => b.ProductId == id && b.Color == colorid && b.Size == sizeid))
                {
                    basketVMs.Find(b => b.ProductId == id).Count += count;
                }
                else
                {
                    basketVMs.Add(new BasketVM
                    {
                        ProductId = (int)id,
                        Count = count,
                        Color = colorid,
                        Size = sizeid
                    });
                }
            }
            else
            {
                basketVMs = new List<BasketVM>();

                basketVMs.Add(new BasketVM()
                {
                    ProductId = (int)id,
                    Count = count,
                    Color = colorid,
                    Size = sizeid
                });
            }


            HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketVMs));

            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.DiscountPrice > 0 ? dbProduct.DiscountPrice : dbProduct.Price;
                basketVM.Name = dbProduct.Name;
            }

            return PartialView("_BasketPartial", basketVMs);
        }

        public async Task<IActionResult> DetailModal(int? id,int? color = 1,int? size=1,int count = 0)
        {
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();

            ViewBag.Colorid = color;
            ViewBag.Sizeid = size;
            ViewBag.Count = count;

            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Color)
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Size)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null) return NotFound();

            return PartialView("_ProductModalPartial", product);
        }
    }
}
