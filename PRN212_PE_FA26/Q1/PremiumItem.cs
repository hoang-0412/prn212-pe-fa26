namespace Prn212.Q1
{
    public class PremiumItem : Ticket
    {
        public PremiumItem(int id, string name, string category, decimal value)
            : base(id, name, category, value)
        {
        }

        public override decimal Evaluate()
        {
            return Value * 0.9m;
        }
    }
}
