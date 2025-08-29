namespace CardholderApi.DTOs
{
    public class CreateCardholderDTO
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int TransactionCount { get; set; }
    }
}
