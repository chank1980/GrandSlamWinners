using GrandSlamWinners.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;


namespace GrandSlamWinners.Controllers
{
    public class HomeController : Controller
    {
        private PlayerEntities3 db = new PlayerEntities3();

        public ActionResult Index(string searchString, string gender,int? page)
        {
            //keep the search criteron for paging
            ViewBag.name = searchString;
            ViewBag.sex = gender;                
                    
            //create the dropdown list,gender, by setting up an empty list
            List<string> genderList = new List<string>();
            var genderQuery = from g in db.Players
                              orderby (g.M_F)
                              select (g.M_F);
            //set unique choices
            genderList.AddRange(genderQuery.Distinct());
            //Pass items to view screen
            ViewBag.gender = new SelectList(genderList);
            //selecting all records from db
            var players = from m in db.Players
                          select m;
            if (!String.IsNullOrEmpty(gender))
            {
                players = players.Where(x => x.M_F == gender);
            }

            //search by names            
            if (!String.IsNullOrEmpty(searchString))
            {
                players = players.Where(x => x.FirstName.Contains(searchString) || x.Surname.Contains(searchString));

            }

            //Truncate GrandSlam years greater than 15 length long
            foreach (var player in players)
            {
                player.AusOpenYYYY = player.AusOpenYYYY != null && player.AusOpenYYYY.Length > 15 ? player.AusOpenYYYY.Substring(0, 15) + "..." : player.AusOpenYYYY;
                player.FreOpenYYYY = player.FreOpenYYYY != null && player.FreOpenYYYY.Length > 15 ? player.FreOpenYYYY.Substring(0, 15) + "..." : player.FreOpenYYYY;
                player.WimOpenYYYY = player.WimOpenYYYY != null && player.WimOpenYYYY.Length > 15 ? player.WimOpenYYYY.Substring(0, 15) + "..." : player.WimOpenYYYY;
                player.USOpenYYYY = player.USOpenYYYY != null && player.USOpenYYYY.Length > 15 ? player.USOpenYYYY.Substring(0, 15) + "..." : player.USOpenYYYY;
            }

            
            //give sort order as required for pagination
            return View(players.OrderBy(i=>i.Id).ToPagedList(page ?? 1, 3));
        }

        public ActionResult AddPlayer()
        {
            return View();
        }

        [HttpPost]//comm with server to action method 
        public ActionResult AddPlayer(Player player)
        {
            if (player.ImageUrl == null)
            {
                player.ImageUrl = "https://www.edco.com/Content/Images/wimbledon-championship-trophy.jpg";
            }

            if (ModelState.IsValid)//matching to [required]
            {
                db.Players.Add(player);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(player);
        }

        public ActionResult EditPlayer(int? id)
        {
            //int? to null param in order to check ID, alert when no valid ID 
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            //however, if invalid ID passed in and no record found
            if (player==null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        [HttpPost]
        public ActionResult EditPlayer(Player player)
        {
            if (player.ImageUrl == null)
            {
                player.ImageUrl = "https://www.edco.com/Content/Images/wimbledon-championship-trophy.jpg";
            }

            if (ModelState.IsValid)
            {
                db.Entry(player).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(player);
        }

        public ActionResult Details(int? id)
        {
            //int? see 'edit' view for comment
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Player player = db.Players.Find(id);
            //however, if ID is invalid and no record found
            if (player==null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        public ActionResult Delete(int? id)
        {
            //int? see 'Edit' view for comment
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Player player = db.Players.Find(id);
            //if ID is invalid and no record found
            if (player==null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

               
        [HttpPost, ActionName("Delete")]//label the method to DeleteConfirmed
        public ActionResult DeleteConfirmed(int id)
        {
            Player player = db.Players.Find(id);
            db.Players.Remove(player);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    
}