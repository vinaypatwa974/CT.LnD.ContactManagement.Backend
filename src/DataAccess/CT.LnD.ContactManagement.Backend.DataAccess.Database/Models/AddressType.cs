using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Models
{
    public class AddressType
    {
        [Key]
        public int Id { get; set; }
        public string TypeName { get; set; }
    }
}