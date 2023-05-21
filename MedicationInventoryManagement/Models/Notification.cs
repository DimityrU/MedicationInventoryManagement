using System;
using System.Collections.Generic;

namespace MedicationInventoryManagement.Models
{
    public partial class Notification
    {
        public Guid NotificationId { get; set; }
        public Guid MedicationId { get; set; }
        public string NotificationType { get; set; }
        public string NotificationMessage { get; set; }

        public virtual Medication Medication { get; set; }
    }
}
