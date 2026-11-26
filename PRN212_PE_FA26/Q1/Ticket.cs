namespace Prn212.Q1
{
    public class Ticket : IValued
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Value { get; set; }

        public Ticket(int id, string name, string category, decimal value)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException("id");
            }
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value");
            }
            Id = id;
            Name = name;
            Category = category;
            Value = value;
        }

        public virtual decimal Evaluate()
        {
            return Value;
        }
    }
}
