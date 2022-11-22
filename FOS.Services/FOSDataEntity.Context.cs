﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq;

public partial class FOSDataEntity : DbContext
{
    public FOSDataEntity()
        : base("name=FOSDataEntity")
    {
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }

    public virtual DbSet<Area> Areas { get; set; }
    public virtual DbSet<City> Cities { get; set; }
    public virtual DbSet<Dealer> Dealers { get; set; }
    public virtual DbSet<Job> Jobs { get; set; }
    public virtual DbSet<LoginHistory> LoginHistories { get; set; }
    public virtual DbSet<Page> Pages { get; set; }
    public virtual DbSet<RegionalHeadRegion> RegionalHeadRegions { get; set; }
    public virtual DbSet<RegionalHead> RegionalHeads { get; set; }
    public virtual DbSet<Region> Regions { get; set; }
    public virtual DbSet<Retailer> Retailers { get; set; }
    public virtual DbSet<RolePage> RolePages { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<SaleOfficer> SaleOfficers { get; set; }
    public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
    public virtual DbSet<User> Users { get; set; }

    public virtual ObjectResult<GetDealersBySo_Result> GetDealersBySo(Nullable<int> salesOfficerId)
    {
        var salesOfficerIdParameter = salesOfficerId.HasValue ?
            new ObjectParameter("salesOfficerId", salesOfficerId) :
            new ObjectParameter("salesOfficerId", typeof(int));

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetDealersBySo_Result>("GetDealersBySo", salesOfficerIdParameter);
    }
}