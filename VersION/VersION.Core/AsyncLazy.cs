using System.Runtime.CompilerServices;

namespace VersION.Core;

// Thanks to Stephen Toub for this snippet (https://devblogs.microsoft.com/pfxteam/asynclazyt/)

///-------------------------------------------------------------------------------------------------
/// <summary>   An asynchronous lazy.   </summary>
///
/// <typeparam name="T">    Generic type parameter. </typeparam>
///
/// <seealso cref="Lazy{System.Threading.Tasks.Task{T}}"/>
///-------------------------------------------------------------------------------------------------
public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<T> valueFactory) : base(() => Task.Factory.StartNew(valueFactory))
    { }

    public AsyncLazy(Func<Task<T>> taskFactory) : base(() => Task.Factory.StartNew(taskFactory).Unwrap())
    { }

    public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
}