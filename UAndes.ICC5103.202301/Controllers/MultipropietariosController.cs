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

        // GET: Multipropietarios/Search/5
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(DateTime año, string comuna, string manzana, string predio)
        {
            SearchData queryData = new SearchData(año, comuna, manzana, predio);
            string jsonConvertedData = JsonConvert.SerializeObject(queryData);
            return RedirectToAction("SearchResult", "Multipropietarios", new { año = queryData.año , comuna = queryData.comuna , manzana = queryData.manzana, predio = queryData.predio });
        }

        // GET: Multipropietarios/Create
        public ActionResult SearchResult(DateTime año, string comuna, string manzana, string predio)
        {
            //SearchData queryData = JsonConvert.DeserializeObject<SearchData>(jsonData);

            //DateTime año = queryData.año;
            //string comuna = queryData.comuna;
            //string manzana = queryData.manzana;
            //string predio = queryData.predio;

            var queryMultipropietario = db.MultipropietarioSet.Where(multipropietario => multipropietario.AñoVigenciaInicial <= año && multipropietario.AñoVigenciaFinal >= año && multipropietario.Comuna == comuna && multipropietario.Manzana == manzana && multipropietario.Predio == predio);
            var queryMultipropietarioNull = db.MultipropietarioSet.Where(multipropietario => multipropietario.AñoVigenciaInicial <= año && (multipropietario.AñoVigenciaFinal == null || multipropietario.AñoVigenciaFinal < multipropietario.AñoVigenciaInicial) && multipropietario.Comuna == comuna && multipropietario.Manzana == manzana && multipropietario.Predio == predio);

            ViewBag.queryMultipropietario = queryMultipropietario;
            ViewBag.queryMultipropietarioNull = queryMultipropietarioNull;
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