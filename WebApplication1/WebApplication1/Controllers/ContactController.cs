using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ContactController : Controller
    {
        private readonly JuanDbContext _context;
        public ContactController(JuanDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            Setting setting = _context.Settings.FirstOrDefault();
            return View(setting);
        }
    }
}
