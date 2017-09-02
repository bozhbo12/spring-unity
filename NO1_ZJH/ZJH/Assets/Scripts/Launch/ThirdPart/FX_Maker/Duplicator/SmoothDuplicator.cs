
using System;
namespace SystemHelper.Duplicator
{
    internal class SmoothDuplicator<T> : DuplicatorBase<T> where T : class
    {
        public SmoothDuplicator(int quantity, Func<T> generator)
            : base(quantity, generator)
        {

        }

        public override bool operation()
        {
            if (currentQuantity < targetQuantity)
            {
                T instance = generator();
                if (instance != null && !instance.Equals(null))
                {
                    instances.Add(instance);
                    currentQuantity++;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
    }
}
