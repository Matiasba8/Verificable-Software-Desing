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
    public class MultipropietariosController : Controller
    {
        private InscripcionesBrDbGrupo06Entities db = new InscripcionesBrDbGrupo06Entities();

        // GET: Multipropietarios
        public ActionResult Index()
        {
            return View(db.MultipropietarioSet.ToList());
        }

        // GET: Multipropietarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MultipropietarioSet multipropietarioSet = db.MultipropietarioSet.Find(id);
            if (multipropietarioSet == null)
            {
                return HttpNotFound();
            }
            return View(multipropietarioSet);
        }

        // GET: Multipropietarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Multipropietarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RUT,PorcentajeDerechos,Fojas,NumeroInscripcion,FechaInscripcion,AñoVigenciaInicial,AñoVigenciaFinal,ComunaManzanaPredio")] MultipropietarioSet multipropietarioSet)
        {
            if (ModelState.IsValid)
            {
                db.MultipropietarioSet.Add(multipropietarioSet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(multipropietarioSet);
        }

        // GET: Multipropietarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MultipropietarioSet multipropietarioSet = db.MultipropietarioSet.Find(id);
            if (multipropietarioSet == null)
            {
                return HttpNotFound();
            }
            return View(multipropietarioSet);
        }

        // POST: Multipropietarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RUT,PorcentajeDerechos,Fojas,NumeroInscripcion,FechaInscripcion,AñoVigenciaInicial,AñoVigenciaFinal,ComunaManzanaPredio")] MultipropietarioSet multipropietarioSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(multipropietarioSet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(multipropietarioSet);
        }

        // GET: Multipropietarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MultipropietarioSet multipropietarioSet = db.MultipropietarioSet.Find(id);
            if (multipropietarioSet == null)
            {
                return HttpNotFound();
            }
            return View(multipropietarioSet);
        }

        // POST: Multipropietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            MultipropietarioSet multipropietarioSet = db.MultipropietarioSet.Find(id);
            db.MultipropietarioSet.Remove(multipropietarioSet);
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
