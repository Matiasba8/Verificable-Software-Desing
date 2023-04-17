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
        public ActionResult Create(int NumeroAtencion, string rut, string porcentajeDerechos, string derechosNoAcreditados)
        {
            decimal porcentajeDerechosParse;
            bool derechosNoAcreditadosParse = false;

            try
            {
                //Se revisa si el input está en formato correcto y se crean los adquirentes y
                //los multipropietarios.
                porcentajeDerechosParse = Decimal.Parse(porcentajeDerechos);
                if (derechosNoAcreditados == "on") { derechosNoAcreditadosParse = true; }

                AdquirenteSet adquirenteSet = new AdquirenteSet();
                adquirenteSet.FormularioSetNumeroAtencion = NumeroAtencion;
                adquirenteSet.RUT = rut;
                if (derechosNoAcreditadosParse)
                {
                    porcentajeDerechosParse = 0;
                    adquirenteSet.PorcentajeDerechos = 0;
                }
                else if (porcentajeDerechosParse > 0 && porcentajeDerechosParse <=100)
                {
                    adquirenteSet.PorcentajeDerechos = porcentajeDerechosParse;
                }
                else
                {
                    return RedirectToAction("Edit", "Formularios", new { id = NumeroAtencion });
                }
                adquirenteSet.DerechosNoAcreditados = derechosNoAcreditadosParse;

                
                FormularioSet formularioSet = db.FormularioSet.Find(NumeroAtencion);
                if (porcentajeDerechosParse >= 0 && (formularioSet.PorcentajeDisponible - porcentajeDerechosParse >= 0))
                {
                    formularioSet.PorcentajeDisponible -= porcentajeDerechosParse;
                }
                else
                {
                    return RedirectToAction("Edit", "Formularios", new { id = NumeroAtencion });
                }

                MultipropietarioSet multipropietarioSet = new MultipropietarioSet();
                multipropietarioSet.RUT = rut;
                multipropietarioSet.PorcentajeDerechos = porcentajeDerechosParse;
                multipropietarioSet.Fojas = formularioSet.Fojas;
                multipropietarioSet.NumeroInscripcion = formularioSet.NumeroInscripcion;
                multipropietarioSet.FechaInscripcion = formularioSet.FechaInscripcion;
                if (formularioSet.FechaInscripcion <= new DateTime(2019, 1, 1))
                {
                    multipropietarioSet.AñoVigenciaInicial = new DateTime(2019, 1, 1);
                }
                else
                {
                    multipropietarioSet.AñoVigenciaInicial = formularioSet.FechaInscripcion;
                }

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

                    //CREACION Y AJUSTE DE MULTIPROPIETARIOS
                    var multipropietariosPreviosQuery = db.MultipropietarioSet.Where(multipropietariosPreviosSet => multipropietariosPreviosSet.Comuna == formularioSet.Comuna && multipropietariosPreviosSet.Manzana == formularioSet.Manzana && multipropietariosPreviosSet.Predio == formularioSet.Predio);
                    if (multipropietariosPreviosQuery.Any())
                    {
                        List<MultipropietarioSet> multipropietariosPrevios = multipropietariosPreviosQuery.ToList();

                        Dictionary<int, List<MultipropietarioSet>> dictMultipropietariosOrdenados = new Dictionary<int, List<MultipropietarioSet>>();

                        foreach (MultipropietarioSet multipropietarioSetTemp in multipropietariosPrevios)
                        {
                            if (dictMultipropietariosOrdenados.ContainsKey(multipropietarioSetTemp.AñoVigenciaInicial.Value.Year))
                            {
                                dictMultipropietariosOrdenados[multipropietarioSetTemp.AñoVigenciaInicial.Value.Year].Add(multipropietarioSetTemp);
                            }
                            else
                            {
                                dictMultipropietariosOrdenados.Add(multipropietarioSetTemp.AñoVigenciaInicial.Value.Year, new List<MultipropietarioSet>());
                                dictMultipropietariosOrdenados[multipropietarioSetTemp.AñoVigenciaInicial.Value.Year].Add(multipropietarioSetTemp);
                            }
                        }

                        List<int> listaAños = dictMultipropietariosOrdenados.Keys.ToList();
                        listaAños.Sort();

                        for (int i = 0; i < listaAños.Count; i++)
                        {
                            if (listaAños[i] != listaAños.Last())
                            {
                                foreach (MultipropietarioSet multipropietario in dictMultipropietariosOrdenados[listaAños[i]])
                                {
                                    multipropietario.AñoVigenciaFinal = new DateTime(listaAños[i + 1] - 1, 1, 1);
                                    db.Entry(multipropietario).State = EntityState.Modified;
                                }
                            }
                            else
                            {
                                List<int> listaNumeroInscripcion = new List<int>();
                                foreach (MultipropietarioSet multipropietario in dictMultipropietariosOrdenados[listaAños[i]])
                                {
                                    listaNumeroInscripcion.Add((int)(multipropietario.NumeroInscripcion));
                                }

                                listaNumeroInscripcion.Sort();

                                for (int j = 0; j < listaNumeroInscripcion.Count; j++)
                                {
                                    if (listaNumeroInscripcion[j] != listaNumeroInscripcion.Last())
                                    {
                                        foreach (MultipropietarioSet multipropietario in dictMultipropietariosOrdenados[listaAños[i]])
                                        {
                                            if (multipropietario.NumeroInscripcion == listaNumeroInscripcion[j])
                                            {
                                                multipropietario.AñoVigenciaFinal = multipropietario.AñoVigenciaInicial;
                                                db.Entry(multipropietario).State = EntityState.Modified;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    db.SaveChanges();

                    //AJUSTE DE % DE NO ACREDITADOS
                    var adquirentesNoAcreditados = db.AdquirenteSet.Where(adquiSet => adquiSet.DerechosNoAcreditados == true && adquiSet.FormularioSetNumeroAtencion == formularioSet.NumeroAtencion);
                    if (adquirentesNoAcreditados.Any())
                    {
                        decimal porcentajeDisponible = (decimal)formularioSet.PorcentajeDisponible;
                        if (porcentajeDisponible >= 0)
                        {
                            decimal porcentajePorRepartir = porcentajeDisponible / adquirentesNoAcreditados.Count();

                            foreach (AdquirenteSet adquirenteSetUpdate in adquirentesNoAcreditados)
                            {
                                adquirenteSetUpdate.PorcentajeDerechos = porcentajePorRepartir;
                                db.Entry(adquirenteSetUpdate).State = EntityState.Modified;
                            }

                            var multipropietariosNoAcreditados = db.MultipropietarioSet.Where(multipropSet => multipropSet.FormularioNumeroAtencion == formularioSet.NumeroAtencion);
                            if (multipropietariosNoAcreditados.Any())
                            {
                                foreach (MultipropietarioSet multipropietarioSetUpdate in multipropietariosNoAcreditados)
                                {
                                    if (multipropietarioSetUpdate.DerechosNoAcreditados)
                                    {
                                        multipropietarioSetUpdate.PorcentajeDerechos = porcentajePorRepartir;
                                        db.Entry(multipropietarioSetUpdate).State = EntityState.Modified;
                                    }
                                }
                            }

                        }
                    }
                    db.SaveChanges();

                }

                return RedirectToAction("Edit", "Formularios", new { id = adquirenteSet.FormularioSetNumeroAtencion });
            }
            catch(FormatException)
            {
                return RedirectToAction("Edit", "Formularios", new { id = NumeroAtencion });
            }
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
