using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility
{
    public class Class3
    {
        private int Counter;

        Task AsyncLoop()
        {
            return AsyncLoopTask().ContinueWith(t =>
                Counter = t.Result,
                TaskScheduler.FromCurrentSynchronizationContext());
        }
        Task<int> AsyncLoopTask()
        {
            var tcs = new TaskCompletionSource<int>();
            DoIteration(tcs);
            return tcs.Task;
        }
        void DoIteration(TaskCompletionSource<int> tcs)
        {
            LoadNextItem().ContinueWith(t => {
                if (t.Exception != null)
                {
                    tcs.TrySetException(t.Exception.InnerException);
                }
                else if (t.Result.Contains("a"))
                {
                    tcs.TrySetResult(t.Result.Length);
                }
                else
                {
                    DoIteration(tcs);
                }
            });
        }

        private Task<string> LoadNextItem()
        {
            throw new NotImplementedException();
        }

        async Task AsyncLoopAsync()
        {
            while (true)
            {
                string result = await LoadNextItem();
                if (result.Contains("target"))
                {
                    Counter = result.Length;
                    break;
                }
            }
        }
    }
}
