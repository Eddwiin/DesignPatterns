using System;

namespace DesignPatterns
{
    public class FallsIllEventArgs
    {
        public string Address { get; set; } = string.Empty;
    }

    public class Person
    {
        public void CatchACold()
        {
            FallsIll?.Invoke(this,
                new FallsIllEventArgs { Address = "123 Paris London" });
        }

        public event EventHandler<FallsIllEventArgs> FallsIll;
    }

    public class Demo
    {
        static void Main(string[] args)
        {
            var person = new Person();

            person.FallsIll += CallDoctor;

            person.CatchACold();
        }

        private static void CallDoctor(object sender, FallsIllEventArgs eventArgs)
        {
            Console.WriteLine($"A doctor has been called to {eventArgs.Address}");
        }
    }
}