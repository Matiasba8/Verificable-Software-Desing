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
    public class EnajenantesController : Controller
    {
        private InscripcionesBrDbGrupo06Entities db = new InscripcionesBrDbGrupo06Entities();

        // GET: Enajenantes
        public ActionResult Index()
        {
            var enajenanteSet = db.EnajenanteSet.Include(e => e.FormularioSet);
            return View(enajenanteSet.ToList());
        }

        // GET: Enajenantes/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EnajenanteSet enajenanteSet = db.EnajenanteSet.Find(id);
            if (enajenanteSet == null)
            {
                return HttpNotFound();
            }
            return View(enajenanteSet);
        }

        // GET: Enajenantes/Create
        public ActionResult Create()
        {
            ViewBag.FormularioNumeroAtencion = new SelectList(db.FormularioSet, "NumeroAtencion", "CNE");
            return View();
        }

        // POST: Enajenantes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int NumeroAtencion, string rut, string porcentajeDerechos, string derechosNoAcreditados)
        {
            decimal porcDerechos;
            bool derNoAcreditados = false;

            try
            {
                porcDerechos = Decimal.Parse(porcentajeDerechos);
                if (derechosNoAcreditados == "on") { derNoAcreditados = true; }

                EnajenanteSet enajenanteSet = new EnajenanteSet();
                enajenanteSet.FormularioSetNumeroAtencion = NumeroAtencion;
                enajenanteSet.RUT = rut;
                enajenanteSet.PorcentajeDerechos = porcDerechos;
                enajenanteSet.DerechosNoAcreditados = derNoAcreditados;

                if (ModelState.IsValid)
                {
                    db.EnajenanteSet.Add(enajenanteSet);
                    db.SaveChanges();
                }
                return RedirectToAction("Edit", "Formularios", new { id = enajenanteSet.FormularioSetNumeroAtencion });
            }
            catch (FormatException)
            {
                return RedirectToAction("Edit", "Formularios", new { id = NumeroAtencion });
            }
        }

        // GET: Enajenantes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EnajenanteSet enajenanteSet = db.EnajenanteSet.Find(id);
            if (enajenanteSet == null)
            {
                return HttpNotFound();
            }
            ViewBag.FormularioNumeroAtencion = new SelectList(db.FormularioSet, "NumeroAtencion", "CNE", enajenanteSet.FormularioSetNumeroAtencion);
            return View(enajenanteSet);
        }

        // POST: Enajenantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RUT,PorcentajeDerechos,DerechosNoAcreditados,FormularioNumeroAtencion")] EnajenanteSet enajenanteSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enajenanteSet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FormularioNumeroAtencion = new SelectList(db.FormularioSet, "NumeroAtencion", "CNE", enajenanteSet.FormularioSetNumeroAtencion);
            return View(enajenanteSet);
        }

        // GET: Enajenantes/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EnajenanteSet enajenanteSet = db.EnajenanteSet.Find(id);
            if (enajenanteSet == null)
            {
                return HttpNotFound();
            }
            return View(enajenanteSet);
        }

        // POST: Enajenantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            EnajenanteSet enajenanteSet = db.EnajenanteSet.Find(id);
            db.EnajenanteSet.Remove(enajenanteSet);
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
