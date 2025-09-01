namespace AzureAppPizzeria.Data.Dtos.Order
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal OriginalTotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalTotalPrice { get; set; }
        public string? Status { get; set; }
        public string? UserName { get; set; }
        public bool DiscountApplied { get; set; }
        public bool FreePizzaClaimed { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
    }
}