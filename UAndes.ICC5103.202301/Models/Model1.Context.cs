﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UAndes.ICC5103._202301.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class InscripcionesBrDbGrupo06Entities : DbContext
    {
        public InscripcionesBrDbGrupo06Entities()
            : base("name=InscripcionesBrDbGrupo06Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AdquirenteSet> AdquirenteSet { get; set; }
        public virtual DbSet<Comunas> Comunas { get; set; }
        public virtual DbSet<EnajenanteSet> EnajenanteSet { get; set; }
        public virtual DbSet<FormularioSet> FormularioSet { get; set; }
        public virtual DbSet<MultipropietarioSet> MultipropietarioSet { get; set; }
        public virtual DbSet<Persona> Persona { get; set; }
    }
}
