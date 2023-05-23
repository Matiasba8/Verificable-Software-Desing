using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Controllers
{
    public class AdquirentesController : Controller
    {
        private DateTime _2019 = new DateTime(2019, 1, 1);
        private InscripcionesBrDbGrupo06Entities db = new InscripcionesBrDbGrupo06Entities();

        // GET: Adquirentes
        public ActionResult Index()
        {
            var adquirenteSet = db.AdquirenteSet.Include(a => a.FormularioSet);
            return View(adquirenteSet.ToList());
        }

        // GET: Adquirentes/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdquirenteSet adquirenteSet = db.AdquirenteSet.Find(id);
            if (adquirenteSet == null)
            {
                return HttpNotFound();
            }
            return View(adquirenteSet);
        }

        // GET: Adquirentes/Create
        public ActionResult Create()
        {
            ViewBag.FormularioSetNumeroAtencion = new SelectList(db.FormularioSet, "NumeroAtencion", "CNE");
            return View();
        }

        // POST: Adquirentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int numeroAtencion, string rut, string porcentajeDerechos, string derechosNoAcreditados)
        {
            decimal porcentajeDerechosParse;
            bool derechosNoAcreditadosParse = false;
            //Se revisa si el input está en formato correcto, si no, no agrega al adquirente y recarga la página
            try
            {
                porcentajeDerechosParse = Decimal.Parse(porcentajeDerechos);
                if (derechosNoAcreditados == "on") { derechosNoAcreditadosParse = true; }
            }
            catch (FormatException) { return RedirectToAction("Edit", "Formularios", new { id = numeroAtencion });}

            //Crea al adquirente
            AdquirenteSet adquirenteSet = new AdquirenteSet();
            adquirenteSet.FormularioSetNumeroAtencion = numeroAtencion;
            adquirenteSet.RUT = rut;
            if (derechosNoAcreditadosParse) {porcentajeDerechosParse = 0;adquirenteSet.PorcentajeDerechos = 0;}
            else if (porcentajeDerechosParse > 0 && porcentajeDerechosParse <= 100) {adquirenteSet.PorcentajeDerechos = porcentajeDerechosParse;}
            else { return RedirectToAction("Edit", "Formularios", new { id = numeroAtencion });}
            adquirenteSet.DerechosNoAcreditados = derechosNoAcreditadosParse;

            //Busca al formulario y revisa si el porcentaje disponible permite la creación de este adquirente
            FormularioSet formularioSet = db.FormularioSet.Find(numeroAtencion);
            if (porcentajeDerechosParse >= 0 && (formularioSet.PorcentajeDisponible - porcentajeDerechosParse >= 0))
            { formularioSet.PorcentajeDisponible -= porcentajeDerechosParse; }
            else { return RedirectToAction("Edit", "Formularios", new { id = numeroAtencion });}

            //Crea el objeto multipropietario que se usará
            MultipropietarioSet multipropietarioSet = new MultipropietarioSet();
            multipropietarioSet.RUT = rut;
            multipropietarioSet.PorcentajeDerechos = porcentajeDerechosParse;
            multipropietarioSet.Fojas = formularioSet.Fojas;
            multipropietarioSet.NumeroInscripcion = formularioSet.NumeroInscripcion;
            multipropietarioSet.FechaInscripcion = formularioSet.FechaInscripcion;
            if (formularioSet.FechaInscripcion <= _2019) { multipropietarioSet.AñoVigenciaInicial = _2019;}
            else {multipropietarioSet.AñoVigenciaInicial = formularioSet.FechaInscripcion;}
            multipropietarioSet.AñoVigenciaFinal = null;
            multipropietarioSet.Comuna = formularioSet.Comuna;
            multipropietarioSet.Manzana = formularioSet.Manzana;
            multipropietarioSet.Predio = formularioSet.Predio;
            multipropietarioSet.FormularioNumeroAtencion = formularioSet.NumeroAtencion;
            multipropietarioSet.DerechosNoAcreditados = derechosNoAcreditadosParse;

            if (ModelState.IsValid)
            {
                db.Entry(formularioSet).State = EntityState.Modified;
                db.AdquirenteSet.Add(adquirenteSet);
                db.MultipropietarioSet.Add(multipropietarioSet);
                db.SaveChanges();
            }
            return RedirectToAction("Edit", "Formularios", new { id = adquirenteSet.FormularioSetNumeroAtencion });
        }

        // GET: Adquirentes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdquirenteSet adquirenteSet = db.AdquirenteSet.Find(id);
            if (adquirenteSet == null)
            {
                return HttpNotFound();
            }
            ViewBag.FormularioSetNumeroAtencion = new SelectList(db.FormularioSet, "NumeroAtencion", "CNE", adquirenteSet.FormularioSetNumeroAtencion);
            return View(adquirenteSet);
        }

        // POST: Adquirentes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RUT,PorcentajeDerechos,DerechosNoAcreditados,FormularioSetNumeroAtencion")] AdquirenteSet adquirenteSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adquirenteSet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FormularioSetNumeroAtencion = new SelectList(db.FormularioSet, "NumeroAtencion", "CNE", adquirenteSet.FormularioSetNumeroAtencion);
            return View(adquirenteSet);
        }

        // GET: Adquirentes/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdquirenteSet adquirenteSet = db.AdquirenteSet.Find(id);
            if (adquirenteSet == null)
            {
                return HttpNotFound();
            }
            return View(adquirenteSet);
        }

        // POST: Adquirentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AdquirenteSet adquirenteSet = db.AdquirenteSet.Find(id);
            db.AdquirenteSet.Remove(adquirenteSet);
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
