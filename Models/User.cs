using System.Collections.Generic;

namespace AutoPartsPOS.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public int? RoleId { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }

        public ICollection<SalesInvoice> SalesInvoices { get; set; }

    }
}
