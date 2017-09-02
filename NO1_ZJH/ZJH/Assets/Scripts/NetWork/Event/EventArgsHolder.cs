using System;

/**
 * An EventArgs meant to hold a single object.
 * 
 * To wrap an int in an EventArgs:
 * EventArgs arg = new EventArgsHolder<int>(42);
 * or
 * EventArgs arg = EventArgsHolder.Construct(42);
 * 
 * An EventArgsHolder<T> can be implicitly casted to a T.
 */
public class EventArgsHolder<T> : EventArgs
{
    public EventArgsHolder(T held)
    {
        m_Held = held;
    }

    public T Get()
    {
        return m_Held;
    }

    public override string ToString()
    {
        return m_Held.ToString();
    }

    public static implicit operator T(EventArgsHolder<T> obj)
    {
        return obj.Get();
    }

    public static implicit operator EventArgsHolder<T>(T obj)
    {
        return new EventArgsHolder<T>(obj);
    }

    private T m_Held;
}

public static class EventArgsHolder
{
    /**
     * Helper function (constructor) that deduces the type.
     */
    public static EventArgsHolder<T> Construct<T>(T argument)
    {
        return new EventArgsHolder<T>(argument);
    }

    /**
     * Helper function to pull the value out of an EventArgs.
     */
    public static T Get<T>(EventArgs argument)
    {
        return (EventArgsHolder<T>)argument;
    }
}