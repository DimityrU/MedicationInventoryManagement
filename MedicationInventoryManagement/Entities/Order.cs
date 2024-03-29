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

    [Required]
    [StringLength(50)]
    public string OrderName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderMedication> OrderMedications { get; set; } = new List<OrderMedication>();
}