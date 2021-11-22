using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMSTask.Models
{
    public class Customer
    {
        [Key]
        public int CustomerCode { get; set; }

        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }
}
