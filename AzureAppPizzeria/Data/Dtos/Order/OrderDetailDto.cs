namespace AzureAppPizzeria.Data.Dtos.Order
{
    public class OrderDetailDto
    { //dto för en enskild orderrad i en order
        public int MealId { get; set; }
        public string? MealName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerItem { get; set; } // Pris per styck i kronor
        public decimal TotalPriceForItem { get; set; } // Totalt för denna rad i kronor (Quantity * PricePerItem)
    }
}

