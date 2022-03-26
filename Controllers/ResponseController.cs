using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace WeddingPlanner.Controllers
{
    [Route("response")]
    public class ResponseController : Controller
    {
        private WeddingUser loggedInUser
        {
            get { return dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId")); }
        }
        private WeddingContext dbContext;
        public ResponseController(WeddingContext context)
        {
            dbContext = context;
        }
        [HttpGet("rsvp/{weddingId}/{status}")]
        public IActionResult RSVP(int weddingId, string status)
        {
            if(loggedInUser == null)
                return RedirectToAction("Index", "Home");

            // make sure wedding exists, redirect if not
            if(!dbContext.Weddings.Any(w => w.WeddingId == weddingId))
                return RedirectToAction("Index", "Wedding");

            if(status == "add")
                AddRSVP(weddingId);
            else
                RemoveRSVP(weddingId);

            return RedirectToAction("Index", "Wedding");
        }
        private void AddRSVP(int weddingId)
        {
            Response newResponse = new Response()
            {
                WeddingId = weddingId,
                UserId = loggedInUser.UserId
            };

            dbContext.Responses.Add(newResponse);
            dbContext.SaveChanges();
        }
        private void RemoveRSVP(int weddingId)
        {
            // query for response
            Response rsvp = dbContext.Responses.FirstOrDefault(r => r.UserId == loggedInUser.UserId && r.WeddingId == weddingId);

            if(rsvp != null)
            {
                dbContext.Responses.Remove(rsvp);
                dbContext.SaveChanges();
            }
                
        }
    }

}