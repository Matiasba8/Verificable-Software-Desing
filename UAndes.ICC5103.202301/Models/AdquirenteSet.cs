//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UAndes.ICC5103._202301.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AdquirenteSet
    {
        public int Id { get; set; }
        public Nullable<decimal> PorcentajeDerechos { get; set; }
        public bool DerechosNoAcreditados { get; set; }
        public int FormularioSetNumeroAtencion { get; set; }
        public string RUT { get; set; }
    
        public virtual FormularioSet FormularioSet { get; set; }
    }
}
