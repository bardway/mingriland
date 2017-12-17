// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
#endregion

namespace TLAuto.Base.Async
{
    public abstract class AsyncTaskBase<TResult> : IDisposable
    {
        private readonly int _timeOutMs;
        private readonly WriteOnceBlock<TResult> _wobResult = new WriteOnceBlock<TResult>(i => i);

        private CancellationToken? _cancelToken;

        private volatile bool _invoked;

        protected AsyncTaskBase(CancellationToken? cancelToken, int timeOutMs = 4000)
        {
            _timeOutMs = timeOutMs;
            _cancelToken = cancelToken;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Invoke();

        public async Task<TResult> InvokeAsync()
        {
            if (_invoked)
            {
                throw new InvalidOperationException("Already invoded.");
            }

            _invoked = true;

            try
            {
                var waitTask = _cancelToken != null ? _wobResult.ReceiveAsync(_cancelToken.Value) : _wobResult.ReceiveAsync();

                Invoke();

                var resultTask = await Task.WhenAny(waitTask, Task.Delay(_timeOutMs));
                if (resultTask == waitTask)
                {
                    return waitTask.Result;
                }

                throw new TimeoutException();
            }
            catch (Exception ex)
            {
                Exception(ex);
                return default(TResult);
            }
            finally
            {
                Dispose();
            }
        }

        protected void PostResult(TResult result)
        {
            _wobResult.Post(result);
        }

        protected abstract void Exception(Exception ex);

        protected abstract void Dispose(bool disposing);
    }
}