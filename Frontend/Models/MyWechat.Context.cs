﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CSharpSDK.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WechatEntities : DbContext
    {
        public WechatEntities()
            : base("name=WechatEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ShareInfo> ShareInfo { get; set; }
        public virtual DbSet<Pageinexinfo> Pageinexinfo { get; set; }
        public virtual DbSet<ShareLog> ShareLogs { get; set; }
    }
}
