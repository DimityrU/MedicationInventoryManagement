﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedicationInventoryManagement.Entities;

public partial class Order
{
    [Key]
    public Guid OrderId { get; set; }

    public Guid MedicationId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("MedicationId")]
    [InverseProperty("Orders")]
    public virtual Medication Medication { get; set; }
}