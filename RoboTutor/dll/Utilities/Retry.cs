using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

/// <summary>
/// http://stackoverflow.com/questions/1563191/c-sharp-cleanest-way-to-write-retry-logic
/// Blanket catch statements that simply retry the same call can be dangerous 
/// if used as a general exception handling mechanism. 
/// Here's a lambda-based retry wrapper that you can use with any method. 
/// I chose to factor the number of retries and the retry timeout out as parameters for a bit more flexibility.
/// 
/// You can now use this utility method to perform retry logic:
/// Retry.Do(() => SomeFunctionThatCanFail(), TimeSpan.FromSeconds(1));
///  or
/// Retry.Do(SomeFunctionThatCanFail, TimeSpan.FromSeconds(1));
///  or
/// int result = Retry.Do(SomeFunctionWhichReturnsInt, TimeSpan.FromSeconds(1), 4);
///  Or you could even make an asynchronous overload.
/// </summary>
public static class Retry
{
    public static void Do(Action action, TimeSpan retryInterval, int retryCount = 3)
    {
        try
        {
	        Do<object>(() =>
	        {
	            action();
	            return null;
	        }, retryInterval, retryCount);
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine("Retry: failed - ", ex);
        }
    }

    public static TResult Do<TResult>(Func<TResult> action, TimeSpan retryInterval, int retryCount = 3)
    {
        var exceptions = new List<Exception>();

        for (int retry = 0; retry < retryCount;)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
                Thread.Sleep(retryInterval);

                retry++;
                Debug.WriteLine("Retry {0}...", retry);
            }
        }

        throw new AggregateException(exceptions);
    }

    public static TResult Do<T1, T2, TResult>(Func<T1, T2, TResult> action, T1 arg1, T2 arg2, TimeSpan retryInterval, int retryCount = 3)
    {
        var exceptions = new List<Exception>();

        for (int retry = 0; retry < retryCount; retry++)
        {
            try
            {
                return action(arg1, arg2);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
                Thread.Sleep(retryInterval);
            }
        }

        throw new AggregateException(exceptions);
    }
}

