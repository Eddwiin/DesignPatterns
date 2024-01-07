using System;
using System.Collections.Generic;
using DotNetDesignPatternDemos.Behavioral.ChainOfResponsibility.MethodChain;
using static System.Console;

namespace DotNetDesignPatternDemos.Behavioral.ChainOfResponsibility.ModifierChain.BrokerChain
{
    public class Query
    {
        public string CreatureName;

        public enum Argument
        {
            Attack, Defense
        }

        public Argument WhatToQuery;

        public int Value;

        public Query(string creatureName, Argument WhatToQuery, int value)
        {
            CreatureName = creatureName ?? throw new ArgumentNullException(paramName: nameof(creatureName));
            WhatToQuery = WhatToQuery;
            Value = value;
        }

    }

    public class Game
    {
        public event EventHandler<Query> Queries;

        public void PerformQuery(object sender, Query q)
        {
            Queries?.Invoke(sender, q)
        }
    }

    public class Creature
    {
        private Game game;
        private string Name;
        private int attack, defense;

        public Creature(Game game, string name, int attack, int defense)
        {
            this.game = game ?? throw new ArgumentNullException(paramName: nameof(game));
            this.Name = name ?? throw new ArgumentNullException(paramName: nameof(name));
            this.attack = attack;
            this.defense = defense;
        }

        public int Attack
        {
            get
            {
                var q = new Query(Name, Query.Argument.Attack, attack);
                game.PerformQuery(this, q);
                return q.Value;
            }
        }

        public int Defense
        {
            get
            {
                var q = new Query(Name, Query.Argument.Defense, defense);
                game.PerformQuery(this, q);
                return q.Value;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(attack)}: {Attack}, {nameof(defense)}: {Defense}";
            //                                                 ^^^^^^^^ using a property    ^^^^^^^^^
        }
    }

    public abstract class CreatureModifier : IDisposable
    {
        protected Game game;
        protected Creature creature;

        protected CreatureModifier(Game game, Creature creature)
        {
            this.game = game;
            this.creature = creature;
            game.Queries += Handle;
        }

        protected abstract void Handle(object sender, Query q);

        public void Dispose()
        {
            game.Queries -= Handle;
        }
    }

    public class DoubleAttackModifier : CreatureModifier
    {
        public DoubleAttackModifier(Game game, Creature creature) : base(game, creature)
        {

        }

        protected override void Handle(object sender, Query q)
        {
            if (q.CreatureName == creature.Name &&
                q.WhatToQuery == Query.Argument.Attack)
                q.Value *= 2;
        }
    }

    public class IncreaseDefenseModifier : CreatureModifier
    {
        public IncreaseDefenseModifier(Game game, Creature creature) : base(game, creature)
        {
        }

        protected override void Handle(object sender, Query q)
        {
            if (q.CreatureName == creature.Name &&
                q.WhatToQuery == Query.Argument.Defense)
                q.Value += 2;
        }
    }

    public class Demo
    {
        public static void Main()
        {
            var game = new Game();
            var goblin = new Creature(game, "Strong Goblin", 3, 3);
            WriteLine(goblin);

            using (new DoubleAttackModifier(game, goblin))
            {
                WriteLine(goblin);
                using (new IncreaseDefenseModifier(game, goblin))
                {
                    WriteLine(goblin);
                }
            }

            WriteLine(goblin);
        }
    }
}