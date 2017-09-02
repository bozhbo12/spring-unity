using System;
using System.Collections.Generic;

namespace SystemHelper.Duplicator
{
    /// <summary>
    /// copy objects to target number and keep a reference of those instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DuplicatorBase<T> where T : class
    {
        protected Func<T> generator;
        protected int targetQuantity = 10;
        protected int currentQuantity = 0;
        protected List<T> instances = new List<T>();

        internal DuplicatorBase(int quantity, Func<T> generator)
        {
            this.generator = generator;
            this.targetQuantity = quantity;
        }

        public abstract bool operation();

        public List<T> Instances
        {
            get { return instances; }
        }

        public static DuplicatorBase<T> create(DuplicationStrategyType duplicationStrategy, int quantity, Func<T> generator)
        {
            switch (duplicationStrategy)
            {
                case DuplicationStrategyType.SmoothGeneration:
                    return new SmoothDuplicator<T>(quantity, generator);
                case DuplicationStrategyType.InstantGeneration:
                    return new InstantDuplicator<T>(quantity, generator);
                default: return null;
            }
        }

        public bool IsFinished { get { return currentQuantity == targetQuantity; } }

        public void clear()
        {
            instances.Clear();
            currentQuantity = 0;
        }
    }
}
