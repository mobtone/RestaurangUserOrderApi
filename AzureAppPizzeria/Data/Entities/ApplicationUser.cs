using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using OtherIdentity = Microsoft.Extensions.Identity.Core;

namespace AzureAppPizzeria.Data.Entities
{
    //Core Identity hanteras med basklassen "IdentityUser" - här är username, password, email osv inbyggt så jag behöver ej 
    //definiera detta i min user-klass. 
    public class ApplicationUser : IdentityUser
    {
        public List<Order> Orders { get; set; } = new List<Order>();
        public int BonusPoints { get; set; } = 0;

    }
}
