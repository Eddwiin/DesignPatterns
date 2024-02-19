using static System.Console;
using System.Collections.Generic;

namespace DotNetDesignPatternDemos
{
    public class Memento
    {
        public int Balance { get; }

        public Memento(int balance)
        {
            Balance = balance;
        }
    }

    public class BankAccount
    {
        private int balance;
        private List<Memento> changes = new List<Memento>();
        private int current;

        public BankAccount(int balance)
        {
            this.balance = balance;
            changes.Add(new Memento(balance));
        }

        public Memento Deposit(int amount)
        {
            balance += amount;
            var m = new Memento(balance);
            changes.Add(m);
            ++current;
            return m;
        }

        public Memento Restore(Memento m)
        {
            if (m != null)
            {
                balance = m.Balance;
                changes.Add(m);
                return m;
            }
            return null;
        }

        public Memento Undo()
        {
            if (current > 0)
            {
                var m = changes[--current];
                balance = m.Balance;
                return m;
            }

            return null;
        }

        public Memento Redo()
        {
            if (current + 1 < changes.Count)
            {
                var m = changes[++current];
                balance = m.Balance;
                return m;
            }

            return null;
        }

        public override string ToString()
        {
            return $"{nameof(balance)}: {balance}";
        }
    }


    public class Demo
    {
        static void Main(string[] args)
        {
            // var bank = new BankAccount(100);
            // var m1 = bank.Deposit(50);
            // var m2 = bank.Deposit(25);
            // var m3 = bank.Deposit(10);
            // WriteLine(bank);

            // bank.Restore(m1);
            // WriteLine(bank);

            // bank.Restore(m2);
            // WriteLine(bank);

            // bank.Restore(m3);
            // WriteLine(bank);

            var bank = new BankAccount(100);
            bank.Deposit(50);
            bank.Deposit(25);
            WriteLine(bank);

            bank.Undo();
            WriteLine($"Undo 1: {bank}");
            bank.Undo();
            WriteLine($"Undo 2: {bank}");
            bank.Redo();
            WriteLine($"Redo: {bank}");
        }
    }
}