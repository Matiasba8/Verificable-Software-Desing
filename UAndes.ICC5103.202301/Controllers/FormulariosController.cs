using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Controllers
{
    public class FormulariosController : Controller
    {
        private InscripcionesBrDbGrupo06Entities db = new InscripcionesBrDbGrupo06Entities();

        // GET: Formularios
        public ActionResult Index()
        {
            return View(db.FormularioSet.ToList());
        }

        // GET: Formularios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormularioSet formularioSet = db.FormularioSet.Find(id);
            var adquirentes = db.AdquirenteSet.Where(adquiSet => adquiSet.FormularioSetNumeroAtencion == id);
            var enajenantes = db.EnajenanteSet.Where(enajenanteSet => enajenanteSet.FormularioSetNumeroAtencion == id);

            if (formularioSet == null)
            {
                return HttpNotFound();
            }

            ViewBag.adquirentes = adquirentes;
            ViewBag.enajenantes = enajenantes;
            return View(formularioSet);
        }

        // POST: Formularios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        public ActionResult New(int? Id)
        {
            ViewBag.Comunas = db.Comunas;
            if (Id != null)
            {
                FormularioSet formularioSet = db.FormularioSet.Find(Id);

                ViewBag.adquirentes = db.AdquirenteSet.Where(adquirente => adquirente.FormularioSetNumeroAtencion == Id);
                ViewBag.enajenantes = db.EnajenanteSet.Where(enajenante => enajenante.FormularioSetNumeroAtencion == Id);
                return View(formularioSet);
            }
            else
            {
                FormularioSet formularioSet = new FormularioSet();
                formularioSet.CNE = "";
                formularioSet.Comuna = "";
                formularioSet.Manzana = "";
                formularioSet.Predio = "";
                formularioSet.Fojas = "";
                formularioSet.FechaInscripcion = new DateTime(2023, 1, 1);
                formularioSet.NumeroInscripcion = 0;
                formularioSet.PorcentajeDisponible = new Decimal(100.000);

                if (ModelState.IsValid)
                {
                    db.FormularioSet.Add(formularioSet);
                    db.SaveChanges();
                }

                return View(formularioSet);
            }
        }

        // GET: Formularios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormularioSet formularioSet = db.FormularioSet.Find(id);
            var adquirentes = db.AdquirenteSet.Where(adquiSet => adquiSet.FormularioSetNumeroAtencion == id);
            var enajenantes = db.EnajenanteSet.Where(enajenanteSet => enajenanteSet.FormularioSetNumeroAtencion == id);

            if (formularioSet == null)
            {
                return HttpNotFound();
            }

            ViewBag.adquirentes = adquirentes;
            ViewBag.enajenantes = enajenantes;
            ViewBag.formularioSet = formularioSet;
            ViewBag.Comunas = db.Comunas;
            return View(formularioSet);
        }

        // POST: Formularios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NumeroAtencion,CNE,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumeroInscripcion")] FormularioSet formularioSet)
        {   
            if (ModelState.IsValid)
            {

             
                var adquirentes = db.AdquirenteSet.Where(adquiSet => adquiSet.FormularioSetNumeroAtencion == formularioSet.NumeroAtencion);
                var enajenantes = db.EnajenanteSet.Where(enajenanteSet => enajenanteSet.FormularioSetNumeroAtencion == formularioSet.NumeroAtencion);

                ViewBag.adquirentes = adquirentes;
                ViewBag.enajenantes = enajenantes;
                ViewBag.formularioSet = formularioSet;
                ViewBag.Comunas = db.Comunas;

                var formsPrevios = db.FormularioSet.Where(formsPreviosSet => formsPreviosSet.Comuna == formularioSet.Comuna && formsPreviosSet.Manzana == formularioSet.Manzana && formsPreviosSet.Predio == formularioSet.Predio);
                if (formsPrevios.Any())
                {
                    if (formsPrevios.First<FormularioSet>() != null)
                    {
                        FormularioSet formPrevio = formsPrevios.First<FormularioSet>();
                        var multipropietariosPrevios = db.MultipropietarioSet.Where(multipropietariosPreviosSet => multipropietariosPreviosSet.NumeroInscripcion == formPrevio.NumeroInscripcion);
                        foreach (MultipropietarioSet multipropietario in multipropietariosPrevios)
                        {
                            multipropietario.AñoVigenciaFinal = new DateTime(formularioSet.FechaInscripcion.Value.Year - 1, 1, 1);
                            db.Entry(multipropietario).State = EntityState.Modified;
                        }
                    }
                }
                db.Entry(formularioSet).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");
            }
            return View(formularioSet);
        }

        // GET: Formularios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormularioSet formularioSet = db.FormularioSet.Find(id);
            if (formularioSet == null)
            {
                return HttpNotFound();
            }
            return View(formularioSet);
        }

        // POST: Formularios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            FormularioSet formularioSet = db.FormularioSet.Find(id);
            db.FormularioSet.Remove(formularioSet);
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
