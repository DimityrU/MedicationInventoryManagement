﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedicationInventoryManagement.Entities;

public partial class Notification
{
    [Key]
    public Guid NotificationId { get; set; }

    public Guid MedicationId { get; set; }

    [Required]
    [StringLength(20)]
    public string NotificationType { get; set; }

    [Required]
    public string NotificationMessage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("MedicationId")]
    [InverseProperty("Notifications")]
    public virtual Medication Medication { get; set; }
}