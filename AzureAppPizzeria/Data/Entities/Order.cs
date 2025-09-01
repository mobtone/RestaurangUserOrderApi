using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int OriginalPrice { get; set; } // <<< NY: Pris före rabatter i ören

        [Required]
        public int DiscountAmount { get; set; } = 0; // <<< NY: Rabattbeloppet i ören

        [Required]
        public int FinalPrice { get; set; } // <<< Omdöpt från TotalPrice, pris efter rabatter i ören

        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public string ApplicationUserId { get; set; } //den inbyggda FK till applicationUser som är en string, genom Core Identity

        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
        public bool DiscountApplied { get; set; } = false;
        public bool FreePizzaClaimed { get; set; } = false;
        public string Status { get; set; } //ex. "Mottagen", "Förbereds", "Levererad", "Avbruten"
    }
}
