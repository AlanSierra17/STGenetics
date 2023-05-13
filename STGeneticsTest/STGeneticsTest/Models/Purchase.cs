namespace STGeneticsTest.Models
{
    public class Purchase
    {
        public int PurchaseId { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal AmountWithDiscount { get; set; }
        public decimal freight { get;}
        public string Discount { get; }

    }
}
