namespace Prn212.Q1
{
    public class Program
    {
        private static void Main(string[] args)
        {
            List<Ticket> items = new List<Ticket>();
            items.Add(new Ticket(1, "A", "X", 10m));
            items.Add(new StandardItem(2, "B", "Y", 20m));
            items.Add(new PremiumItem(3, "C", "X", 20m));
            ExamService service = new ExamService();
            List<Ticket> selected = service.Select(items, 20m);
            Console.Write("Selected IDs:");
            foreach (Ticket t in selected)
            {
                Console.Write(" " + t.Id);
            }
            Console.WriteLine();
            Console.WriteLine("Total: " + service.Total(items).ToString("0.##"));
        }
    }
}
