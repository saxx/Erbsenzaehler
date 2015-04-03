namespace Erbsenzaehler.Models
{
    public class Rule
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }


        public string Regex { get; set; }

        public string ChangeCategoryTo { get; set; }
        public bool? ChangeIgnoreTo { get; set; }
        public ChangeDateToOption? ChangeDateTo { get; set; }


        public enum ChangeDateToOption
        {
            FirstOfCurrentMonth,
            LastOfCurrentMonth,
            NearestFirstOfMonth,
            NearestLastOfMonth
        }
    }
}