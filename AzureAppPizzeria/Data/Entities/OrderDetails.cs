using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Entities
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailId { get; set; }
        [Required]
        public int Quantity { get; set; } // antal av maträtten i ordern

        [Required]
        public int PricePerItem { get; set; } // priset per maträtt vid beställningstillfället
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int MealId { get; set; }
        public virtual Meal Meal { get; set; }

    }
}
