namespace KS.DTO.AccountsModule
{
    public class AccountsDTO
    {
        public string AccountCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double OpeningBalance { get; set; }
        public double CurrentBalance { get; set; }
        public bool Active { get; set; }
        public int GroupId { get; set; }
        public string Group { get; set; }
    }
}
