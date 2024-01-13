using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace DesignPatterns
{
    public class BankAccount
    {
        private int balance;
        private int overdraftLimit = -500;

        public void Deposit(int amount)
        {
            balance += amount;
            WriteLine($"Deposited ${amount}, balance is now {balance}");
        }

        public void Withdraw(int amount)
        {
            if (balance - amount >= overdraftLimit)
            {
                balance -= amount;
                WriteLine($"Withdrew ${amount}, balance is now {balance}");
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"{nameof(balance)}: {balance}";
        }
    }

    public abstract class Command
    {
        public abstract void Call();
        public abstract void Undo();
        public bool Success;
    }

    // public interface ICommand
    // {
    //     void Call();
    //     void Undo();
    // }

    public class BankAccountCommand : Command
    {
        private BankAccount bankAccount;

        public enum Action
        {
            Deposit, Withdraw
        }

        private Action action;
        private int amount;
        private bool succeeded;

        public BankAccountCommand(BankAccount bankAccount, Action action, int amount)
        {
            this.account = account ?? throw new ArgumentNullException(paramName: nameof(account));
            this.action = action;
            this.amount = amount;
        }

        public void Call()
        {
            switch (action)
            {
                case Action.Deposit:
                    account.Deposit(amount);
                    succeeded = true;
                    break;
                case Action.Withdraw:
                    succeeded = account.Withdraw(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Undo()
        {
            if (!succeeded) return;
            switch (action)
            {
                case Action.Deposit:
                    account.Withdraw(amount);
                    break;
                case Action.Withdraw:
                    account.Deposit(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    abstract class CompositeBankAccountCommand : List<BankAccountCommand>, ICommand
    {
        public virtual void Call()
        {
            ForEach(cmd => cmd.Call());
        }

        public virtual void Undo()
        {
            foreach (var cmd in ((IEnumerable<BankAccountCommand>)this).Reverse())
            {
                cmd.Undo();
            }
        }
    }

    class MoneyTransferCommand : CompositeBankAccountCommand
    {
        public MoneyTransferCommand(BankAccount from, BankAccount to, int amount)
        {
            AddRange(new[]
            {
                new BankAccountCommand(from,
                BankAccountCommand.Action.Withdraw, amount),
                new BankAccountCommand(to,
                BankAccountCommand.Action.Deposit, amount)
            });
        }

        public override void Call()
        {
            bool ok = true;
            foreach (var cmd in this)
            {
                if (ok)
                {
                    cmd.Call();
                    ok = cmd.Success;
                }
                else
                {
                    cmd.Success = false;
                }
            }
        }
    }

    class Demo
    {
        static void Main(string[] args)
        {
            var ba = new BankAccount();
            var cmdDeposit = new BankAccountCommand(ba,
                BankAccountCommand.Action.Deposit, 100);
            var cmdWithdraw = new BankAccountCommand(ba,
                BankAccountCommand.Action.Withdraw, 1000);
            cmdDeposit.Call();
            cmdWithdraw.Call();
            WriteLine(ba);
            cmdWithdraw.Undo();
            cmdDeposit.Undo();
            WriteLine(ba);


            var from = new BankAccount();
            from.Deposit(100);
            var to = new BankAccount();

            var mtc = new MoneyTransferCommand(from, to, 1000);
            mtc.Call();


            // Deposited $100, balance is now 100
            // balance: 100
            // balance: 0

            WriteLine(from);
            WriteLine(to);
        }
    }
}