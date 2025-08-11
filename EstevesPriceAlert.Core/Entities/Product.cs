namespace EstevesPriceAlert.Core.Entities
{
    public class Product
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        public string ProductName { get; set; } = default!;

        public string? Url { get; set; }

        public string Title { get; set; } = default!;
        public string Currency { get; set; } = "BRL";

        //[BsonRepresentation(BsonType.Decimal128)]
        public decimal CurrentPrice { get; set; }

        public DateTime LastCheckedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
