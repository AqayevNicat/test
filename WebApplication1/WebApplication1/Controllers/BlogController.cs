using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BlogController : Controller
    {
        private readonly JuanDbContext _context;

        public BlogController(JuanDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();

            IEnumerable<Blog> blogs = await _context.Blogs
               .OrderByDescending(c => c.CreatedAt)
               .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)blogs.Count() / 5);
            return View(blogs.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Detail(int? id)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();

            ViewBag.Blogs = await _context.Blogs.OrderByDescending(b=>b.CreatedAt).Take(4).ToListAsync();

            if (id == null) return BadRequest();
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(p => p.Id == id);
            if (blog == null) return NotFound();
            return View(blog);
        }
    }
}
