using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UAndes.ICC5103._202301.Models;

namespace Tools
{
    public class Utils
    {
        private static InscripcionesBrDbGrupo06Entities db = new InscripcionesBrDbGrupo06Entities();

        // Constructores (opcionales)
        public Utils()
        {
        }

        public static void dropDB()
        {
        }

        public static void mostrarValoresDeUnaListaDeMultiprop(List<MultipropietarioSet> list)
        {
            foreach (MultipropietarioSet obj in list)
            {
                Type objectType = obj.GetType();
                PropertyInfo[] properties = objectType.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    string propertyName = property.Name;
                    object propertyValue = property.GetValue(obj);

                    Console.WriteLine(propertyName + ": " + propertyValue);
                }
                Console.WriteLine();
            }
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }

        public static List<MultipropietarioSet> GetMultipropietarioSets()
        {
            return db.MultipropietarioSet.ToList();
        }

        // Métodos
        public static long? obtenerElNumeroDeAtencionActual()
        {
            long? num_atencion_actual = db.FormularioSet.OrderByDescending(f => f.NumeroAtencion)
                                          .FirstOrDefault().NumeroAtencion;
            return num_atencion_actual;
        }

        

        public static bool CompareListsOfMultipropUnorderedEquality(List<MultipropietarioSet> list1, List<MultipropietarioSet> list2, Func<MultipropietarioSet, MultipropietarioSet, bool> MultipropComparacion)
        {
            if (list1.Count != list2.Count)
                return false;

            HashSet<MultipropietarioSet> set1 = new HashSet<MultipropietarioSet>();
            HashSet<MultipropietarioSet> set2 = new HashSet<MultipropietarioSet>();

            mostrarValoresDeUnaListaDeMultiprop(list1);
            mostrarValoresDeUnaListaDeMultiprop(list2);

            foreach(MultipropietarioSet obj in list1)
            {
                set1.Add(obj);
            }

            foreach (MultipropietarioSet obj in list2)
            {
                set2.Add(obj);
            }

            Console.WriteLine(set1);
            Console.WriteLine(set2);

            return set1.SetEquals(set2);
        }
    }
}
