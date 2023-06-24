using Moq;
using NUnit.Framework;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using Tools;
using UAndes.ICC5103._202301.Controllers;
using UAndes.ICC5103._202301.Models;
using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace UAndes.ICC5103.Tests


{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        public bool MultipropComparacion(MultipropietarioSet m1, MultipropietarioSet m2)
        {
            return m1.PorcentajeDerechos == m2.PorcentajeDerechos &&
                   m1.Fojas == m2.Fojas && m1.NumeroInscripcion == m2.NumeroInscripcion &&
                   m1.FechaInscripcion == m2.FechaInscripcion && m1.AñoVigenciaInicial == m2.AñoVigenciaInicial &&
                   m1.AñoVigenciaFinal == m2.AñoVigenciaFinal && m1.Comuna == m2.Comuna && m1.Manzana == m2.Manzana &&
                   m1.Predio == m2.Predio && m1.FormularioNumeroAtencion == m2.FormularioNumeroAtencion &&
                   m1.DerechosNoAcreditados == m2.DerechosNoAcreditados;
        }

        [Test]
        public void IsValidControl1P2_ValidMultiprop_ReturnCorrectMultipropList()
        {

            // Arrange
            var formulariosController = new FormulariosController();
            var adquirentesController = new AdquirentesController();
            var multipropietariosController = new MultipropietariosController();
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockRequest = new Mock<HttpRequestBase>();
            var utils = new Utils();

            // Simulación de metodo POST
            mockRequest.Setup(r => r.HttpMethod).Returns("POST");
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
            formulariosController.ControllerContext = new ControllerContext(mockHttpContext.Object, new RouteData(), formulariosController);

            // Set formulario set data
            var formularioSet = new FormularioSet();
            formularioSet.NumeroInscripcion = 123;
            formularioSet.CNE = "99";
            formularioSet.Manzana = "12";
            formularioSet.Comuna = "LAS CONDES";
            formularioSet.Predio = "9";
            formularioSet.Predio = "9";
            formularioSet.Fojas = "55";
            formularioSet.FechaInscripcion = new DateTime(2022, 06, 01);

            //EDIT POST Formulario Controller
            Console.WriteLine("Edit FormularioSet in Test");
            var form = formulariosController.Create(formularioSet);

            // Adquirentes Controller
            mockRequest.Setup(r => r.HttpMethod).Returns("POST");
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
            adquirentesController.ControllerContext = new ControllerContext(mockHttpContext.Object, new RouteData(), adquirentesController);

            var num_atencion_actual = Utils.obtenerElNumeroDeAtencionActual();

            Console.WriteLine(num_atencion_actual);

            adquirentesController.Create((int)num_atencion_actual, "2-7", "50", "NO");
            adquirentesController.Create((int)num_atencion_actual, "3-5", "15", "NO");
            adquirentesController.Create((int)num_atencion_actual, "2-7", "35", "NO");

            // Multipropietarios Controller
            mockRequest.Setup(r => r.HttpMethod).Returns("GET");
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
            multipropietariosController.ControllerContext = new ControllerContext(mockHttpContext.Object, new RouteData(), multipropietariosController);

            var output = Utils.GetMultipropietarioSets();
            var expected = new List<MultipropietarioSet>();
            var m1 = new MultipropietarioSet();
            var m2 = new MultipropietarioSet();
            var m3 = new MultipropietarioSet();

            // Multirpopietario 1
            m1.NumeroInscripcion = 123;
            m1.Comuna = "LAS CONDES";
            m1.Manzana = "12";
            m1.Predio = "9";
            m1.RUT = "2-7";
            m1.Fojas = "55";
            m1.AñoVigenciaInicial = new DateTime(2022, 1, 01);
            m1.AñoVigenciaFinal = null;
            m1.PorcentajeDerechos = Decimal.Parse("50,000");
            m1.DerechosNoAcreditados = false;
            m1.FechaInscripcion = new DateTime(2022, 06, 01);
            m1.FormularioNumeroAtencion = (int)num_atencion_actual;


            // Multirpopietario 2
            m2.NumeroInscripcion = 123;
            m2.Comuna = "LAS CONDES";
            m2.Manzana = "12";
            m2.Predio = "9";
            m2.Fojas = "55";
            m1.RUT = "3-5";
            m2.AñoVigenciaInicial = new DateTime(2022, 1, 01);
            m2.AñoVigenciaFinal = null;
            m2.PorcentajeDerechos = Decimal.Parse("15,000");
            m2.DerechosNoAcreditados = false;
            m2.FechaInscripcion = new DateTime(2022, 06, 01);
            m2.FormularioNumeroAtencion = (int)num_atencion_actual;


            // Multirpopietario 3
            m3.NumeroInscripcion = 123;
            m3.Comuna = "LAS CONDES";
            m3.Manzana = "12";
            m3.Predio = "9";
            m3.Fojas = "55";
            m1.RUT = "125-2";
            m3.AñoVigenciaInicial = new DateTime(2022, 1, 01);
            m3.AñoVigenciaFinal = null;
            m3.PorcentajeDerechos = Decimal.Parse("50,000");
            m3.DerechosNoAcreditados = false;
            m3.FechaInscripcion = new DateTime(2022, 06, 01);
            m3.FormularioNumeroAtencion = (int)num_atencion_actual;

            expected.Add(m1);
            expected.Add(m2);
            expected.Add(m3);

            
            Console.WriteLine("LISTAS!");
            Console.WriteLine(output);
            Console.WriteLine(expected);


            bool result = Utils.CompareListsOfMultipropUnorderedEquality(output,
                                                                         expected,
                                                                         MultipropComparacion);
            // Código de la prueba
            Assert.AreEqual(true, result);

        }
    }
}