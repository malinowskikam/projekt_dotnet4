﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspMvc.Models;

namespace AspMvc.Controllers
{
    public class ToolsController : Controller
    {
        private IRepository db;

        public ToolsController()
        {
            db = null;
        }

        public ToolsController(IRepository context)
        {
            db = context;
        }
        // GET: Tools
        [Authorize(Roles = "Admin, Moderator, User")]
        public ActionResult Index()
        {
            var tools = db.Tools.Include(t => t.Manufacturer);
            return View(tools.ToList());
        }

        // GET: Tools/Details/5
        [Authorize(Roles = "Admin, Moderator, User")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = db.FindToolById((long)id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        // GET: Tools/Create
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Create()
        {
            ViewBag.ManufacturerId = new SelectList(db.Manufacturers, "Id", "Name");
            return View();
        }

        // POST: Tools/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Create([Bind(Include = "Id,Name,ProductionDate,Price,Rating,ManufacturerId")] Tool tool)
        {
            if (ModelState.IsValid)
            {
                db.Add(tool);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ManufacturerId = new SelectList(db.Manufacturers, "Id", "Name", tool.ManufacturerId);
            return View(tool);
        }
        
        // GET: Tools/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = db.FindToolById((long)id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        // POST: Tools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(long id)
        {
            Tool tool = db.FindToolById(id);
            db.Delete(tool);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ValidateProductionDate(DateTime? ProductionDate, long ManufacturerId)
        {
            var manufacturer = db.Manufacturers.Single(m => m.Id == ManufacturerId);
            if (ProductionDate >= manufacturer.CreationDate)
                return Json(true);
            else
                return Json("Narzędzie nie mogło zostać wyprodukowane przed powstaniem producenta");
        }
    }
}
