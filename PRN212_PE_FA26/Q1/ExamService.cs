namespace Prn212.Q1
{
    public class ExamService
    {
        public ExamService()
        {
        }

        public List<Ticket> Select(List<Ticket> items, decimal min)
        {
            List<Ticket> result = items
                .Where(t => t.Value >= min)
                .OrderByDescending(t => t.Value)
                .ThenBy(t => t.Id)
                .ToList();
            return result;
        }

        public decimal Total(List<Ticket> items)
        {
            decimal total = 0m;
            foreach (Ticket item in items)
            {
                total += item.Evaluate();
            }
            return total;
        }
    }
}
