namespace RxSample
{
    static class Program
    {
        public class Market
        {
            public BindingList<float> Prices = new List<float>();

            public void AddPrice(float price)
            {
                Prices.Add(price);
                PriceAdded?.Invoke(this, price);
            }

            public event EventHandler<float> PriceAdded;
        }

        static void Main(string[] args)
        {
            var market = new Market();
            market.Prices.ListChanged += (sender, eventArgs) =>
            {
                if (eventArgs.ListChangedType == ListChangedType.ItemAdded)
                {
                    float price = ((BindingList<float>)sender)[eventArgs.NewIndex];
                    Console.WriteLine($"Binding list got a price of {price}")
                }
            }
            market.AddPrice(123);
        }
    }
}