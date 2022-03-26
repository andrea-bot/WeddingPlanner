using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    [Route("weddings")]
    public class WeddingController : Controller
    {
        private WeddingUser loggedInUser
        {
            get { return dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId")); }
        } 
        private WeddingContext dbContext;
        public WeddingController(WeddingContext context)
        {
            dbContext = context;
        }
        // localhost:5000/weddings
        [HttpGet("")]
        public IActionResult Index()
        {
            // TODO: Check session for userid
            // if no key in session, redirect back to Home->Index
            if(loggedInUser == null)
                return RedirectToAction("Index", "Home");

            var weddings = dbContext.Weddings
                .Include(w => w.Responses)
                .OrderByDescending(w => w.Date);

            ViewBag.UserId = loggedInUser.UserId;

            // weddings moose has responded to
            var responsed = weddings.Where(w => w.Responses.Any(r => r.UserId == 1));


            return View(weddings.ToList());
        }
        [HttpGet("{weddingId}")]
        public IActionResult Show(int weddingId)
        {
            if(loggedInUser == null)
                return RedirectToAction("Index", "Home");
            
            Wedding viewModel = dbContext.Weddings
                .Include(w => w.Responses)
                .ThenInclude(r => r.Guest)
                .FirstOrDefault(w => w.WeddingId == weddingId);

            return View(viewModel);

        }
        [HttpGet("new")]
        public IActionResult New()
        {
            if(loggedInUser == null)
                return RedirectToAction("Index", "Home");

            ViewBag.UserId = loggedInUser.UserId;
            return View();
        }
        [HttpPost("create")]
        public IActionResult Create(Wedding newWedding)
        {
            if(ModelState.IsValid)
            {
                newWedding.UserId = loggedInUser.UserId;
                dbContext.Weddings.Add(newWedding);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = loggedInUser.UserId;
            return View("New");
        }
        [HttpGet("remove/{weddingId}")]
        public IActionResult Remove(int weddingId)
        {
            if(loggedInUser == null)
                return RedirectToAction("Index", "Home");

            // query for wedding
            Wedding toDelete = dbContext.Weddings.FirstOrDefault(w => w.WeddingId == weddingId
                && w.UserId == loggedInUser.UserId);
            
            // if null, redirect
            if(toDelete == null)
                return RedirectToAction("Index");
            
            dbContext.Weddings.Remove(toDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}