namespace Prn212.Q1
{
    public class StandardItem : Ticket
    {
        public StandardItem(int id, string name, string category, decimal value)
            : base(id, name, category, value)
        {
        }

        public override decimal Evaluate()
        {
            return Value;
        }
    }
}
