using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Controllers
{
    public class AdquirentesController : Controller
    {
<<<<<<< Updated upstream
        private InscripcionesBrDbGrupo06Entities db = new InscripcionesBrDbGrupo06Entities();
=======
        private DateTime _2019 = new DateTime(2019, 1, 1);
        public InscripcionesBrDbGrupo06Entities db = new InscripcionesBrDbGrupo06Entities();

        
>>>>>>> Stashed changes

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

            if (!IsRUTValido(rut)) { return RedirectToAction("Edit", "Formularios", new { id = numeroAtencion }); }

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

            if (ModelState.IsValid)
            {
                db.Entry(formularioSet).State = EntityState.Added;
                db.AdquirenteSet.Add(adquirenteSet);
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

        private bool IsRUTValido(string rut)
        {
            string rutInput = rut;
            rutInput = rutInput.Replace(".","").ToUpper();

            //Expresion obtenida de https://gist.github.com/donpandix/045f865c3bf800893036
            Regex rutExpression = new Regex("^([0-9]+-[0-9K])$");
            if (!rutExpression.IsMatch(rutInput))
            {
                return false;
            }
            
            rutInput = rutInput.Replace("-","");

            List<string> rutCharsList = new List<string>();
            foreach (char digito in rutInput)
            {
                rutCharsList.Insert(0, digito.ToString());
            }
            string verificadorInput = rutCharsList[0];
            rutCharsList.RemoveAt(0);

            int multiplierRut = 2;
            int totalSumCharacters = 0;
            foreach (string digito in rutCharsList)
            {
                int digitoInt = Int32.Parse(digito);

                totalSumCharacters += digitoInt* multiplierRut;

                multiplierRut++;
                if(multiplierRut > 7)
                {
                    multiplierRut = 2;
                }
            }

            int paso1 = totalSumCharacters / 11;
            int paso2 = paso1 * 11;
            int paso3 = totalSumCharacters - paso2;
            int paso4 = 11 - paso3;

            string verificadorCalculado = paso4.ToString();
            if (verificadorCalculado == "10")
            {
                verificadorCalculado = "K";
            }
            else if (verificadorCalculado == "11")
            {
                verificadorCalculado = "0";
            }
            
            if (verificadorCalculado == verificadorInput) { return true; }
            else { return false; }
        }
    }
}
