using System;
using System.Collections.Generic;

namespace MedicationInventoryManagement.Models
{
    public partial class Order
    {
        public Guid OrderId { get; set; }
        public Guid MedicationId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }

        public virtual Medication Medication { get; set; }
    }
}
