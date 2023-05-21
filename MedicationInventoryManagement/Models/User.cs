using System;
using System.Collections.Generic;

namespace MedicationInventoryManagement.Models
{
    public partial class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
