using System;
using System.Collections.Generic;

namespace MedicationInventoryManagement.Entities
{
    public partial class Medication
    {
        public Medication()
        {
            Notifications = new HashSet<Notification>();
            Orders = new HashSet<Order>();
        }

        public Guid MedicationId { get; set; }
        public string MedicationName { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpirationDate { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
