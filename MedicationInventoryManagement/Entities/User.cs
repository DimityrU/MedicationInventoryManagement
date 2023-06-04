using System;
using System.Collections.Generic;

namespace MedicationInventoryManagement.Entities
{
    public partial class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
