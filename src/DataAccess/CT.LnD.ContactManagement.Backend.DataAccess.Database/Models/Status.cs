using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }
        public string StatusName { get; set; }

        public static implicit operator Status(string v)
        {
            throw new NotImplementedException();
        }
    }
}