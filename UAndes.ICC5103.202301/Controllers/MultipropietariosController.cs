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
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.CSharp;

namespace UAndes.ICC5103._202301.Controllers
{
    public class MultipropietariosController : Controller
    {
        private InscripcionesBrDbGrupo06Entities db = new InscripcionesBrDbGrupo06Entities();

        // GET: Multipropietarios
        public ActionResult Index()
        {
            ViewBag.Comunas = db.Comunas;
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
            ViewBag.Comunas = db.Comunas;
            return View(multipropietarioSet);
        }

        // GET: Multipropietarios/Search/5
        public ActionResult Search()
        {
            ViewBag.Comunas = db.Comunas;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string añoInput, string comuna, string manzana, string predio)
        {
            int añoParsed;
            bool success = int.TryParse(añoInput, out añoParsed);
            Debug.WriteLine(añoParsed);

            if (success)
            {
                DateTime año = new DateTime(añoParsed, 1, 1);

                ViewBag.Comunas = db.Comunas;
                SearchData queryData = new SearchData(año, comuna, manzana, predio);
                string jsonConvertedData = JsonConvert.SerializeObject(queryData);
                return RedirectToAction("SearchResult", "Multipropietarios", new { año = queryData.año, comuna = queryData.comuna, manzana = queryData.manzana, predio = queryData.predio });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Multipropietarios/Create
        public ActionResult SearchResult(DateTime año, string comuna, string manzana, string predio)
        {
            //SearchData queryData = JsonConvert.DeserializeObject<SearchData>(jsonData);

            //DateTime año = queryData.año;
            //string comuna = queryData.comuna;
            //string manzana = queryData.manzana;
            //string predio = queryData.predio;
            ViewBag.Comunas = db.Comunas;
            var queryMultipropietario = db.MultipropietarioSet.Where(multipropietario => (multipropietario.AñoVigenciaInicial.Value.Year <= año.Year && multipropietario.AñoVigenciaFinal.Value.Year >= año.Year && multipropietario.Comuna == comuna && multipropietario.Manzana == manzana && multipropietario.Predio == predio) || (multipropietario.AñoVigenciaInicial.Value.Year <= año.Year && multipropietario.AñoVigenciaFinal == null && multipropietario.Comuna == comuna && multipropietario.Manzana == manzana && multipropietario.Predio == predio));
            
            List<int> listaNumerosInscripcion = new List<int>();
            foreach (MultipropietarioSet multipropietarioSet in queryMultipropietario)
            {
                if (listaNumerosInscripcion.Contains((int)multipropietarioSet.NumeroInscripcion)){}
                else
                {
                    listaNumerosInscripcion.Add((int)multipropietarioSet.NumeroInscripcion);
                }
            }

            listaNumerosInscripcion.Sort();

            listaNumerosInscripcion.Reverse();

            List<MultipropietarioSet> multipropietariosFinales = new List<MultipropietarioSet>();
            foreach(MultipropietarioSet multipropietarioSet in queryMultipropietario)
            {
                if (multipropietarioSet.NumeroInscripcion == listaNumerosInscripcion.First())
                {
                    multipropietariosFinales.Add(multipropietarioSet);
                }
            }

            ViewBag.queryMultipropietario = multipropietariosFinales;
            return View();
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
        public ActionResult Create([Bind(Include = "Id,RUT,PorcentajeDerechos,Fojas,NumeroInscripcion,FechaInscripcion,AñoVigenciaInicial,AñoVigenciaFinal,Comuna,Manzana,Predio")] MultipropietarioSet multipropietarioSet)
        {
            ViewBag.Comunas = db.Comunas;
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
        public ActionResult Edit([Bind(Include = "Id,RUT,PorcentajeDerechos,Fojas,NumeroInscripcion,FechaInscripcion,AñoVigenciaInicial,AñoVigenciaFinal,Comuna,Manzana,Predio")] MultipropietarioSet multipropietarioSet)
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

public class SearchData{
    public DateTime año;
    public string comuna;
    public string manzana;
    public string predio;

    public SearchData(DateTime año, string comuna, string manzana, string predio)
    {
        this.año = año;
        this.comuna = comuna;
        this.manzana = manzana;
        this.predio = predio;
    }
}