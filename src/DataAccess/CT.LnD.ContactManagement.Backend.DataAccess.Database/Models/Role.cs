
using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string RoleName { get; set; }

        public static implicit operator Role(string v)
        {
            throw new NotImplementedException();
        }
    }
}
