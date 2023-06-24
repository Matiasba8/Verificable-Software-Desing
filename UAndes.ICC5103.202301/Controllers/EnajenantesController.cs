using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
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
            decimal porcentajeDerechosParse;
            bool derechosNoAcreditadosParse = false;

            try
            {
                porcentajeDerechosParse = Decimal.Parse(porcentajeDerechos);
                if (derechosNoAcreditados == "on") { derechosNoAcreditadosParse = true; }

                if (!IsRUTValido(rut))
                {
                    return RedirectToAction("Edit", "Formularios", new { id = NumeroAtencion });
                }

                EnajenanteSet enajenanteSet = new EnajenanteSet();
                enajenanteSet.FormularioSetNumeroAtencion = NumeroAtencion;
                enajenanteSet.RUT = rut;
                enajenanteSet.PorcentajeDerechos = porcentajeDerechosParse;
                enajenanteSet.DerechosNoAcreditados = derechosNoAcreditadosParse;

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

        private bool IsRUTValido(string rut)
        {
            string rutInput = rut;
            rutInput = rutInput.Replace(".", "").ToUpper();

            //Expresion obtenida de https://gist.github.com/donpandix/045f865c3bf800893036
            Regex rutExpression = new Regex("^([0-9]+-[0-9K])$");
            if (!rutExpression.IsMatch(rutInput))
            {
                return false;
            }

            rutInput = rutInput.Replace("-", "");

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

                totalSumCharacters += digitoInt * multiplierRut;

                multiplierRut++;
                if (multiplierRut > 7)
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
