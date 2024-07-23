namespace BookStore.Api.Dtos
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public int? CouponId { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; }
    }
}
