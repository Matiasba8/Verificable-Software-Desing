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
using System.Collections.Specialized;

namespace UAndes.ICC5103._202301.Controllers
{
    public class MultipropietariosController : Controller
    {
        DateTime fechaBase = new DateTime(2019, 1, 1);
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
            ViewBag.Comunas = db.Comunas;
            var queryMultipropietario = db.MultipropietarioSet.Where
                (multipropietario => (multipropietario.AñoVigenciaInicial.Value.Year <= año.Year && 
                multipropietario.AñoVigenciaFinal.Value.Year >= año.Year && 
                multipropietario.Comuna == comuna && 
                multipropietario.Manzana == manzana && 
                multipropietario.Predio == predio) || 
                (multipropietario.AñoVigenciaInicial.Value.Year <= año.Year && 
                multipropietario.AñoVigenciaFinal == null && 
                multipropietario.Comuna == comuna && 
                multipropietario.Manzana == manzana && 
                multipropietario.Predio == predio));

            List<MultipropietarioSet> multipropietariosFinales = new List<MultipropietarioSet>();
            foreach (MultipropietarioSet multipropietario in queryMultipropietario)
            {
                multipropietariosFinales.Add(multipropietario);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFromAdquirentesController(int numeroAtencion)
        {
            FormularioSet formularioSet = db.FormularioSet.Find(numeroAtencion);
            var adquirentes =
                        db.AdquirenteSet.Where(adquirentesSet =>
                                               adquirentesSet.FormularioSetNumeroAtencion == formularioSet.NumeroAtencion);
            var enajenantes =
                db.EnajenanteSet.Where(enajenantesSet =>
                                       enajenantesSet.FormularioSetNumeroAtencion == formularioSet.NumeroAtencion);

            CreateMultipropsBase(formularioSet, adquirentes);

            switch (formularioSet.CNE)
            {
                case "Regularización de Patrimonio":
                    RegularizacionDePatrimonio(formularioSet);
                    break;

                case "Compraventa":
                    OpcionesCompraventa opcionesCompraventa = SelectOpcionCompraventa(formularioSet);
                    //0 = Aun no se pueden crear multipropietarios
                    //1 = Transferencia total con o sin enajenantes fantasma
                    //2 = Derechos
                    //3 = Dominios
                    //4 = Derechos con enajenantes fantasma
                    //5 = Dominions con enajenantes fantasma
                    switch (opcionesCompraventa)
                    {
                        case OpcionesCompraventa.None:
                            break;
                        case OpcionesCompraventa.TransTotal:
                            CompraventaTransferenciaTotal(enajenantes, adquirentes, formularioSet);
                            AjustePorcentajes(formularioSet);
                            break;
                        case OpcionesCompraventa.Derechos:
                            CompraventaDerechos(enajenantes, adquirentes, formularioSet);
                            AjustePorcentajes(formularioSet);
                            break;
                        case OpcionesCompraventa.Dominios:
                            CompraventaDominios(enajenantes, formularioSet);
                            AjustePorcentajes(formularioSet);
                            AjusteDuplicados(formularioSet);
                            break;
                        case OpcionesCompraventa.DerechosFantasma:
                            CompraventaDerechosFantasma(enajenantes, formularioSet);
                            AjustePorcentajes(formularioSet);
                            break;
                        case OpcionesCompraventa.DominiosFantasma:
                            CompraventaDominiosFantasma(enajenantes, formularioSet);
                            AjustePorcentajes(formularioSet);
                            AjusteDuplicados(formularioSet);
                            break;
                    }
                    break;
            }

            return RedirectToAction("Index", "Formularios");
        }

        private void RegularizacionDePatrimonio(FormularioSet formularioSet)
        {
            //CREACION Y AJUSTE DE MULTIPROPIETARIOS
            var multipropietariosPreviosQuery =
                db.MultipropietarioSet.Where(multipropPreviosSet =>
                                             multipropPreviosSet.Comuna == formularioSet.Comuna &&
                                             multipropPreviosSet.Manzana == formularioSet.Manzana &&
                                             multipropPreviosSet.Predio == formularioSet.Predio);

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
            var adquirentesNoAcreditados =
                db.AdquirenteSet.Where(adquiSet =>
                                       adquiSet.DerechosNoAcreditados == true &&
                                       adquiSet.FormularioSetNumeroAtencion == formularioSet.NumeroAtencion);
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

                    var multipropietariosNoAcreditados = db.MultipropietarioSet.Where(multipropSet =>
                                                                                      multipropSet.FormularioNumeroAtencion == formularioSet.NumeroAtencion);
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

        private OpcionesCompraventa SelectOpcionCompraventa(FormularioSet formularioSet)
        {
            var adquirentes =
                        db.AdquirenteSet.Where(adquirentesSet =>
                                               adquirentesSet.FormularioSetNumeroAtencion == formularioSet.NumeroAtencion);
            var enajenantes =
                db.AdquirenteSet.Where(enajenantesSet =>
                                       enajenantesSet.FormularioSetNumeroAtencion == formularioSet.NumeroAtencion);

            var multipropsPasados =
                db.MultipropietarioSet.Where(multipropsPasadosSet =>
                                             multipropsPasadosSet.Manzana == formularioSet.Manzana &&
                                             multipropsPasadosSet.Comuna == formularioSet.Comuna &&
                                             multipropsPasadosSet.Predio == formularioSet.Predio &&
                                             multipropsPasadosSet.AñoVigenciaFinal == null &&
                                             multipropsPasadosSet.FormularioNumeroAtencion != formularioSet.NumeroAtencion);

            var enajenanteFantasma = false;
            foreach (var enajenante in enajenantes)
            {
                string rutEnajenante = enajenante.RUT;
                bool isRutInMultiprop = false;
                foreach (var multipropPasado in multipropsPasados)
                {
                    if (rutEnajenante == multipropPasado.RUT)
                    {
                        isRutInMultiprop = true;
                        break;
                    }
                }

                if (!isRutInMultiprop)
                {
                    enajenanteFantasma = true;
                }
            }
            
            
            decimal? porcentajeTotalAdquirentes = 0;
            foreach (var adquirente in adquirentes)
            {
                porcentajeTotalAdquirentes += adquirente.PorcentajeDerechos;
            }

            if (!enajenantes.Any() || !adquirentes.Any())
            {
                return OpcionesCompraventa.None;
            }
            else if (enajenantes.Any() && porcentajeTotalAdquirentes == 100 && !enajenanteFantasma)
            {
                return OpcionesCompraventa.TransTotal;
            }
            else if (enajenantes.Count() == 1 && adquirentes.Count() == 1 && !enajenanteFantasma)
            {
                return OpcionesCompraventa.Derechos;
            }
            else if (!enajenanteFantasma)
            {
                return OpcionesCompraventa.Dominios;
            }
            else if (enajenantes.Any() && porcentajeTotalAdquirentes == 100 && enajenanteFantasma)
            {
                return OpcionesCompraventa.TransTotal;
            }
            else if (enajenantes.Count() == 1 && adquirentes.Count() == 1 && enajenanteFantasma)
            {
                return OpcionesCompraventa.DerechosFantasma;
            }
            else if (enajenanteFantasma)
            {
                return OpcionesCompraventa.DominiosFantasma;
            }
            else { return OpcionesCompraventa.None; }
        }

        private void CompraventaTransferenciaTotal(IQueryable<EnajenanteSet> enajenantes, IQueryable<AdquirenteSet> adquirentes, FormularioSet formularioSet)
        {
            //Se calcula el porcentaje a repartir
            decimal? porcentajeTotalEnajenantes = 0;
            List<string> rutsEnajenantes = new List<string>();
            foreach (var enajenante in enajenantes)
            {
                rutsEnajenantes.Add(enajenante.RUT);
            }

            //Buscar los enajenantes en multiprop para cerrarlos,
            //sin año de vigencia final y por RUT
            var multipropietariosPreviosQuery =
                db.MultipropietarioSet.Where(multipropPreviosSet =>
                                     multipropPreviosSet.Comuna == formularioSet.Comuna &&
                                     multipropPreviosSet.Manzana == formularioSet.Manzana &&
                                     multipropPreviosSet.Predio == formularioSet.Predio &&
                                     multipropPreviosSet.FormularioNumeroAtencion != formularioSet.NumeroAtencion);

            foreach (var multipropietario in multipropietariosPreviosQuery)
            {
                if (multipropietario.AñoVigenciaFinal == null && rutsEnajenantes.Contains(multipropietario.RUT))
                {
                    multipropietario.AñoVigenciaFinal = new DateTime(formularioSet.FechaInscripcion.Value.Year - 1, 1, 1);
                    db.Entry(multipropietario).State = EntityState.Modified;

                    porcentajeTotalEnajenantes += multipropietario.PorcentajeDerechos;
                }
                else if (multipropietario.AñoVigenciaFinal == null && !rutsEnajenantes.Contains(multipropietario.RUT))
                {
                    multipropietario.AñoVigenciaFinal = new DateTime(formularioSet.FechaInscripcion.Value.Year - 1, 1, 1);
                    db.Entry(multipropietario).State = EntityState.Modified;

                    MultipropietarioSet multipropietarioSet = new MultipropietarioSet();
                    multipropietarioSet.RUT = multipropietario.RUT;
                    multipropietarioSet.PorcentajeDerechos = multipropietario.PorcentajeDerechos;
                    multipropietarioSet.Fojas = multipropietario.Fojas;
                    multipropietarioSet.NumeroInscripcion = multipropietario.NumeroInscripcion;
                    multipropietarioSet.FechaInscripcion = multipropietario.FechaInscripcion;
                    multipropietarioSet.AñoVigenciaInicial = formularioSet.FechaInscripcion;
                    multipropietarioSet.AñoVigenciaFinal = null;
                    multipropietarioSet.Comuna = multipropietario.Comuna;
                    multipropietarioSet.Manzana = multipropietario.Manzana;
                    multipropietarioSet.Predio = multipropietario.Predio;
                    multipropietarioSet.FormularioNumeroAtencion = multipropietario.FormularioNumeroAtencion;
                    multipropietarioSet.DerechosNoAcreditados = multipropietario.DerechosNoAcreditados;

                    db.MultipropietarioSet.Add(multipropietarioSet);
                }
            }
            db.SaveChanges();

            //Ajustar multiprops
            List<string> adquirentesRuts = new List<string>();

            foreach (var adquirente in adquirentes)
            {
                adquirentesRuts.Add(adquirente.RUT);
            }

            for (int i = 0; i < adquirentesRuts.Count(); i++)
            {
                string rutABuscar = adquirentesRuts[i];
                var multipropietariosQueryAdquirentes = db.MultipropietarioSet.Where(multipropPreviosSet =>
                                     multipropPreviosSet.Comuna == formularioSet.Comuna &&
                                     multipropPreviosSet.Manzana == formularioSet.Manzana &&
                                     multipropPreviosSet.Predio == formularioSet.Predio &&
                                     multipropPreviosSet.FormularioNumeroAtencion == formularioSet.NumeroAtencion &&
                                     multipropPreviosSet.RUT == rutABuscar);

                foreach (var multipropietario in multipropietariosQueryAdquirentes)
                {
                    decimal? porcentajeCorrespondiente = multipropietario.PorcentajeDerechos;
                    if (porcentajeTotalEnajenantes != 0)
                    {
                        multipropietario.PorcentajeDerechos = (porcentajeCorrespondiente * porcentajeTotalEnajenantes) / 100;
                    }
                    db.Entry(multipropietario).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
        }

        private void CompraventaDerechos(IQueryable<EnajenanteSet> enajenantes, IQueryable<AdquirenteSet> adquirentes, FormularioSet formularioSet)
        {
            decimal? porcentajeEnajenante = enajenantes.First().PorcentajeDerechos;
            decimal? porcentajeAdquirente = adquirentes.First().PorcentajeDerechos;

            string rutEnajenante = enajenantes.First().RUT;
            string rutAdquirente = adquirentes.First().RUT;

            var multipropietarioPrevioEnajenante =
                db.MultipropietarioSet.Where(multipropPreviosSet =>
                                     multipropPreviosSet.Comuna == formularioSet.Comuna &&
                                     multipropPreviosSet.Manzana == formularioSet.Manzana &&
                                     multipropPreviosSet.Predio == formularioSet.Predio &&
                                     multipropPreviosSet.AñoVigenciaFinal == null &&
                                     multipropPreviosSet.RUT == rutEnajenante &&
                                     multipropPreviosSet.FormularioNumeroAtencion != formularioSet.NumeroAtencion);

            decimal? porcentajeOriginalEnajenante = multipropietarioPrevioEnajenante.First().PorcentajeDerechos;

            multipropietarioPrevioEnajenante.First().PorcentajeDerechos = porcentajeOriginalEnajenante - (porcentajeEnajenante * porcentajeOriginalEnajenante / 100);
            db.Entry(multipropietarioPrevioEnajenante.First()).State = EntityState.Modified;

            var multipropietarioAdquirente =
                db.MultipropietarioSet.Where(multipropAdquirente =>
                                     multipropAdquirente.Comuna == formularioSet.Comuna &&
                                     multipropAdquirente.Manzana == formularioSet.Manzana &&
                                     multipropAdquirente.Predio == formularioSet.Predio &&
                                     multipropAdquirente.AñoVigenciaFinal == null &&
                                     multipropAdquirente.RUT == rutAdquirente &&
                                     multipropAdquirente.FormularioNumeroAtencion == formularioSet.NumeroAtencion);

            multipropietarioAdquirente.First().PorcentajeDerechos = (porcentajeAdquirente * porcentajeOriginalEnajenante) / 100;
            db.Entry(multipropietarioAdquirente.First()).State = EntityState.Modified;

            db.SaveChanges();
        }

        private void CompraventaDominios(IQueryable<EnajenanteSet> enajenantes, FormularioSet formularioSet)
        {
            Dictionary<string, decimal?> enajenantesRutsYPorcentajes = new Dictionary<string, decimal?>();

            foreach (var enajenante in enajenantes)
            {
                enajenantesRutsYPorcentajes.Add(enajenante.RUT, enajenante.PorcentajeDerechos);
            }

            foreach (KeyValuePair<string, decimal?> enajenante in enajenantesRutsYPorcentajes)
            {
                var multipropietariosQueryPrevios = db.MultipropietarioSet.Where(multipropPreviosSet =>
                                     multipropPreviosSet.Comuna == formularioSet.Comuna &&
                                     multipropPreviosSet.Manzana == formularioSet.Manzana &&
                                     multipropPreviosSet.Predio == formularioSet.Predio &&
                                     multipropPreviosSet.AñoVigenciaFinal == null &&
                                     multipropPreviosSet.RUT == enajenante.Key && 
                                     multipropPreviosSet.FormularioNumeroAtencion != formularioSet.NumeroAtencion);

                foreach (var multipropietario in multipropietariosQueryPrevios)
                {
                    if (multipropietario.AñoVigenciaInicial.Value.Year < formularioSet.FechaInscripcion.Value.Year)
                    {
                        multipropietario.AñoVigenciaFinal = new DateTime(formularioSet.FechaInscripcion.Value.Year - 1, 1, 1);
                    }
                    else
                    {
                        multipropietario.AñoVigenciaFinal = multipropietario.AñoVigenciaInicial;
                    }
                    db.Entry(multipropietario).State = EntityState.Modified;

                    MultipropietarioSet multipropietarioSet = new MultipropietarioSet();
                    multipropietarioSet.RUT = multipropietario.RUT;
                    multipropietarioSet.PorcentajeDerechos = multipropietario.PorcentajeDerechos - enajenantesRutsYPorcentajes[multipropietario.RUT];
                    if (multipropietarioSet.PorcentajeDerechos < 0) { multipropietarioSet.PorcentajeDerechos = 0; }
                    multipropietarioSet.Fojas = multipropietario.Fojas;
                    multipropietarioSet.NumeroInscripcion = multipropietario.NumeroInscripcion;
                    multipropietarioSet.FechaInscripcion = multipropietario.FechaInscripcion;
                    multipropietarioSet.AñoVigenciaInicial = formularioSet.FechaInscripcion;
                    multipropietarioSet.AñoVigenciaFinal = null;
                    multipropietarioSet.Comuna = multipropietario.Comuna;
                    multipropietarioSet.Manzana = multipropietario.Manzana;
                    multipropietarioSet.Predio = multipropietario.Predio;
                    multipropietarioSet.FormularioNumeroAtencion = multipropietario.FormularioNumeroAtencion;
                    multipropietarioSet.DerechosNoAcreditados = multipropietario.DerechosNoAcreditados;

                    db.MultipropietarioSet.Add(multipropietarioSet);
                }
                
            }
            db.SaveChanges();
        }

        private void CompraventaDerechosFantasma(IQueryable<EnajenanteSet> enajenantes, FormularioSet formularioSet)
        {
            DateTime _2019 = new DateTime(2019, 1, 1);
            //Crea el objeto multipropietario que se usará
            MultipropietarioSet multipropietarioSet = new MultipropietarioSet();
            multipropietarioSet.RUT = enajenantes.First().RUT;
            multipropietarioSet.PorcentajeDerechos = 0;
            multipropietarioSet.Fojas = null;
            multipropietarioSet.NumeroInscripcion = null;
            multipropietarioSet.FechaInscripcion = null;
            if (formularioSet.FechaInscripcion <= _2019) { multipropietarioSet.AñoVigenciaInicial = _2019; }
            else { multipropietarioSet.AñoVigenciaInicial = formularioSet.FechaInscripcion; }
            multipropietarioSet.AñoVigenciaFinal = null;
            multipropietarioSet.Comuna = formularioSet.Comuna;
            multipropietarioSet.Manzana = formularioSet.Manzana;
            multipropietarioSet.Predio = formularioSet.Predio;
            multipropietarioSet.FormularioNumeroAtencion = formularioSet.NumeroAtencion;
            multipropietarioSet.DerechosNoAcreditados = enajenantes.First().DerechosNoAcreditados;

            db.MultipropietarioSet.Add(multipropietarioSet);
            db.SaveChanges();
        }

        private void CompraventaDominiosFantasma(IQueryable<EnajenanteSet> enajenantes, FormularioSet formularioSet)
        {
            var multipropsPasados =
                db.MultipropietarioSet.Where(multipropsPasadosSet =>
                                             multipropsPasadosSet.Manzana == formularioSet.Manzana &&
                                             multipropsPasadosSet.Comuna == formularioSet.Comuna &&
                                             multipropsPasadosSet.Predio == formularioSet.Predio &&
                                             multipropsPasadosSet.AñoVigenciaFinal == null &&
                                             multipropsPasadosSet.FormularioNumeroAtencion != formularioSet.NumeroAtencion);

            List<string> enajenantesFantasmaRUTS = new List<string>();
            foreach (var enajenante in enajenantes)
            {
                string rutEnajenante = enajenante.RUT;
                bool isRutInMultiprop = false;
                foreach (var multipropPasado in multipropsPasados)
                {
                    if (rutEnajenante == multipropPasado.RUT)
                    {
                        isRutInMultiprop = true;
                        break;
                    }
                }

                if (!isRutInMultiprop)
                {
                    enajenantesFantasmaRUTS.Add(rutEnajenante);
                    //Crea el objeto multipropietario que se usará
                    MultipropietarioSet multipropietarioSet = new MultipropietarioSet();
                    multipropietarioSet.RUT = rutEnajenante;
                    multipropietarioSet.PorcentajeDerechos = 0;
                    multipropietarioSet.Fojas = null;
                    multipropietarioSet.NumeroInscripcion = null;
                    multipropietarioSet.FechaInscripcion = null;
                    if (formularioSet.FechaInscripcion <= fechaBase) { multipropietarioSet.AñoVigenciaInicial = fechaBase; }
                    else { multipropietarioSet.AñoVigenciaInicial = formularioSet.FechaInscripcion; }
                    multipropietarioSet.AñoVigenciaFinal = null;
                    multipropietarioSet.Comuna = formularioSet.Comuna;
                    multipropietarioSet.Manzana = formularioSet.Manzana;
                    multipropietarioSet.Predio = formularioSet.Predio;
                    multipropietarioSet.FormularioNumeroAtencion = formularioSet.NumeroAtencion;
                    multipropietarioSet.DerechosNoAcreditados = false;

                    db.MultipropietarioSet.Add(multipropietarioSet);
                }
            }
            db.SaveChanges();

            Dictionary<string, decimal?> enajenantesRutsYPorcentajes = new Dictionary<string, decimal?>();

            foreach (var enajenante in enajenantes)
            {
                if (!enajenantesFantasmaRUTS.Contains(enajenante.RUT))
                {
                    enajenantesRutsYPorcentajes.Add(enajenante.RUT, enajenante.PorcentajeDerechos);
                }
            }

            foreach (KeyValuePair<string, decimal?> enajenante in enajenantesRutsYPorcentajes)
            {
                var multipropietariosQueryPrevios = db.MultipropietarioSet.Where(multipropPreviosSet =>
                                     multipropPreviosSet.Comuna == formularioSet.Comuna &&
                                     multipropPreviosSet.Manzana == formularioSet.Manzana &&
                                     multipropPreviosSet.Predio == formularioSet.Predio &&
                                     multipropPreviosSet.AñoVigenciaFinal == null &&
                                     multipropPreviosSet.RUT == enajenante.Key &&
                                     multipropPreviosSet.FormularioNumeroAtencion != formularioSet.NumeroAtencion);

                foreach (var multipropietario in multipropietariosQueryPrevios)
                {
                    if (multipropietario.AñoVigenciaInicial.Value.Year < formularioSet.FechaInscripcion.Value.Year)
                    {
                        multipropietario.AñoVigenciaFinal = new DateTime(formularioSet.FechaInscripcion.Value.Year - 1, 1, 1);
                    }
                    else
                    {
                        multipropietario.AñoVigenciaFinal = multipropietario.AñoVigenciaInicial;
                    }
                    db.Entry(multipropietario).State = EntityState.Modified;

                    MultipropietarioSet multipropietarioSet = new MultipropietarioSet();
                    multipropietarioSet.RUT = multipropietario.RUT;
                    multipropietarioSet.PorcentajeDerechos = multipropietario.PorcentajeDerechos - enajenantesRutsYPorcentajes[multipropietario.RUT];
                    if (multipropietarioSet.PorcentajeDerechos < 0) { multipropietarioSet.PorcentajeDerechos = 0; }
                    multipropietarioSet.Fojas = multipropietario.Fojas;
                    multipropietarioSet.NumeroInscripcion = multipropietario.NumeroInscripcion;
                    multipropietarioSet.FechaInscripcion = multipropietario.FechaInscripcion;
                    multipropietarioSet.AñoVigenciaInicial = formularioSet.FechaInscripcion;
                    multipropietarioSet.AñoVigenciaFinal = null;
                    multipropietarioSet.Comuna = multipropietario.Comuna;
                    multipropietarioSet.Manzana = multipropietario.Manzana;
                    multipropietarioSet.Predio = multipropietario.Predio;
                    multipropietarioSet.FormularioNumeroAtencion = multipropietario.FormularioNumeroAtencion;
                    multipropietarioSet.DerechosNoAcreditados = multipropietario.DerechosNoAcreditados;

                    db.MultipropietarioSet.Add(multipropietarioSet);
                }

            }
            db.SaveChanges();
        }

        private void AjustePorcentajes(FormularioSet formularioSet)
        {
            var multipropietariosQuery =
                db.MultipropietarioSet.Where(multipropsSet =>
                                     multipropsSet.Comuna == formularioSet.Comuna &&
                                     multipropsSet.Manzana == formularioSet.Manzana &&
                                     multipropsSet.Predio == formularioSet.Predio &&
                                     multipropsSet.AñoVigenciaFinal == null);

            decimal? sumaTotalPorcentajes = 0;
            foreach (var multipropietario in multipropietariosQuery)
            {
                sumaTotalPorcentajes += multipropietario.PorcentajeDerechos;
            }

            if (sumaTotalPorcentajes > 100)
            {
                foreach (var multipropietario in multipropietariosQuery)
                {
                    decimal? porcentajeOriginal = multipropietario.PorcentajeDerechos;
                    multipropietario.PorcentajeDerechos = (porcentajeOriginal * 100) / sumaTotalPorcentajes;
                    db.Entry(multipropietario).State = EntityState.Modified;
                }
            }
            else if (sumaTotalPorcentajes < 100)
            {
                decimal? totalARepartir = 100 - sumaTotalPorcentajes;

                int contadorDeCerosMultipropietarios = 0;
                foreach(MultipropietarioSet multipropietario in multipropietariosQuery)
                {
                    if (multipropietario.PorcentajeDerechos == 0)
                    {
                        contadorDeCerosMultipropietarios += 1;
                    }
                }

                foreach (MultipropietarioSet multipropietario in multipropietariosQuery)
                {
                    if (multipropietario.PorcentajeDerechos == 0 && contadorDeCerosMultipropietarios > 0)
                    {
                        multipropietario.PorcentajeDerechos += totalARepartir / contadorDeCerosMultipropietarios;
                        db.Entry(multipropietario).State = EntityState.Modified;
                    }
                }
            }
            db.SaveChanges();
        }

        private void AjusteDuplicados(FormularioSet formularioSet)
        {
            var multipropietariosQuery =
                db.MultipropietarioSet.Where(multipropsSet =>
                                     multipropsSet.Comuna == formularioSet.Comuna &&
                                     multipropsSet.Manzana == formularioSet.Manzana &&
                                     multipropsSet.Predio == formularioSet.Predio &&
                                     multipropsSet.AñoVigenciaFinal == null);

            List<string> rutsRepetidos = new List<string>();
            Dictionary<string, int> multipropsRutYRepeticiones = new Dictionary<string, int>();
            foreach (MultipropietarioSet multipropietario in multipropietariosQuery)
            {
                if (multipropsRutYRepeticiones.ContainsKey(multipropietario.RUT))
                {
                    multipropsRutYRepeticiones[multipropietario.RUT] += 1;
                    rutsRepetidos.Add(multipropietario.RUT);
                }
                else
                {
                    multipropsRutYRepeticiones[multipropietario.RUT] = 1;
                }
            }

            if (!rutsRepetidos.Any())
            {
                return;
            }

            foreach (string rut in rutsRepetidos)
            {
                var multipropietariosRepetidosQuery =
                                    db.MultipropietarioSet.Where(multipropsSet =>
                                                         multipropsSet.Comuna == formularioSet.Comuna &&
                                                         multipropsSet.Manzana == formularioSet.Manzana &&
                                                         multipropsSet.Predio == formularioSet.Predio &&
                                                         multipropsSet.RUT == rut &&
                                                         multipropsSet.AñoVigenciaFinal == null);

                List<MultipropietarioSet> multipropsRepetidos = new List<MultipropietarioSet>();
                foreach (MultipropietarioSet multipropRepetido in multipropietariosRepetidosQuery)
                {
                    multipropsRepetidos.Add(multipropRepetido);
                }

                if (multipropsRepetidos[0].AñoVigenciaInicial < multipropsRepetidos[1].AñoVigenciaInicial)
                {
                    multipropsRepetidos[0].AñoVigenciaFinal = new DateTime(multipropsRepetidos[1].AñoVigenciaInicial.Value.Year - 1, 1, 1);
                    multipropsRepetidos[1].PorcentajeDerechos += multipropsRepetidos[0].PorcentajeDerechos;
                }
                else
                {
                    multipropsRepetidos[1].AñoVigenciaFinal = new DateTime(multipropsRepetidos[0].AñoVigenciaInicial.Value.Year - 1, 1, 1);
                    multipropsRepetidos[0].PorcentajeDerechos += multipropsRepetidos[1].PorcentajeDerechos;
                }

                db.Entry(multipropsRepetidos[0]).State = EntityState.Modified;
                db.Entry(multipropsRepetidos[1]).State = EntityState.Modified;
            }
            db.SaveChanges();
        }

        private void CreateMultipropsBase(FormularioSet formularioSet, IQueryable<AdquirenteSet> adquirentes)
        {
            foreach (AdquirenteSet adquirente in adquirentes)
            {
                //Crea el objeto multipropietario que se usará
                MultipropietarioSet multipropietarioSet = new MultipropietarioSet();
                multipropietarioSet.RUT = adquirente.RUT;
                multipropietarioSet.PorcentajeDerechos = adquirente.PorcentajeDerechos;
                multipropietarioSet.Fojas = formularioSet.Fojas;
                multipropietarioSet.NumeroInscripcion = formularioSet.NumeroInscripcion;
                multipropietarioSet.FechaInscripcion = formularioSet.FechaInscripcion;
                if (formularioSet.FechaInscripcion <= fechaBase) { multipropietarioSet.AñoVigenciaInicial = fechaBase; }
                else { multipropietarioSet.AñoVigenciaInicial = formularioSet.FechaInscripcion; }
                multipropietarioSet.AñoVigenciaFinal = null;
                multipropietarioSet.Comuna = formularioSet.Comuna;
                multipropietarioSet.Manzana = formularioSet.Manzana;
                multipropietarioSet.Predio = formularioSet.Predio;
                multipropietarioSet.FormularioNumeroAtencion = formularioSet.NumeroAtencion;
                multipropietarioSet.DerechosNoAcreditados = adquirente.DerechosNoAcreditados;

                db.MultipropietarioSet.Add(multipropietarioSet);
            }
            db.SaveChanges();
        }

        enum OpcionesCompraventa
        {
            None,
            TransTotal,
            Derechos,
            Dominios,
            DerechosFantasma,
            DominiosFantasma
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