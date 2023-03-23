using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Controllers
{
    public class MultipropietarioController : Controller
    {
        private InscripcionesBrDbGrupo06Entities db = new InscripcionesBrDbGrupo06Entities();

        // GET: Multipropietario
        public ActionResult Index()
        {
            return View(db.Multipropietario1.ToList());
        }

        // GET: Multipropietario/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multipropietario1 multipropietario1 = db.Multipropietario1.Find(id);
            if (multipropietario1 == null)
            {
                return HttpNotFound();
            }
            return View(multipropietario1);
        }

        // GET: Multipropietario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Multipropietario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NumeroAtencion,Comuna,Manzana,Predio,Fojas,ano_inscripcion,numero_de_inscripcion")] Multipropietario1 multipropietario1)
        {
            if (ModelState.IsValid)
            {
                db.Multipropietario1.Add(multipropietario1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(multipropietario1);
        }

        // GET: Multipropietario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multipropietario1 multipropietario1 = db.Multipropietario1.Find(id);
            if (multipropietario1 == null)
            {
                return HttpNotFound();
            }
            return View(multipropietario1);
        }

        // POST: Multipropietario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NumeroAtencion,Comuna,Manzana,Predio,Fojas,ano_inscripcion,numero_de_inscripcion")] Multipropietario1 multipropietario1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(multipropietario1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(multipropietario1);
        }

        // GET: Multipropietario/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multipropietario1 multipropietario1 = db.Multipropietario1.Find(id);
            if (multipropietario1 == null)
            {
                return HttpNotFound();
            }
            return View(multipropietario1);
        }

        // POST: Multipropietario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Multipropietario1 multipropietario1 = db.Multipropietario1.Find(id);
            db.Multipropietario1.Remove(multipropietario1);
            db.SaveChanges();
            return RedirectToAction("Index");
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
