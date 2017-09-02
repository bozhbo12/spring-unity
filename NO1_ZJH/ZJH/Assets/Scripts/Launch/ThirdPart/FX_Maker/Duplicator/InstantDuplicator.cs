using System;
using System.Collections.Generic;

namespace SystemHelper.Duplicator
{
    internal class InstantDuplicator<T> : DuplicatorBase<T> where T : class
    {
        internal InstantDuplicator(int quantity, Func<T> generator)
            : base(quantity, generator)
        {
        }

        public override bool operation()
        {
            while( currentQuantity < targetQuantity)
            {
                T instance = generator();
                if (instance != null && !instance.Equals(null))
                {
                    instances.Add(instance);
                    currentQuantity++;
                }
                else
                    return false;
            }
            return true;
        }
    }
}
