namespace Application.Models
{
    public class ReceiptModel
    {
        public string? StoreName { get; set; }
        public string? Date { get; set; }
        public List<ReceiptItem>? Items { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ReceiptItem
    {
        public string? Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
