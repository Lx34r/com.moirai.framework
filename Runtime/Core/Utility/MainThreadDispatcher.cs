using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Moirai.Atropos
{
    /// <summary>
    /// 将 <see cref="IEnumerator"/> 函数调度到 Unity 中的主线程的线程安全方法。
    /// 它包含一个队列，其中包含要在下一个 <see cref="Update"/> 方法上执行的操作。
    /// 它可用于调用主线程，以执行 Unity 中的 UI操控等操作。
    /// </summary>
    public class MainThreadDispatcher : SingletonMono_Persistent<MainThreadDispatcher>
    {
        private static readonly Queue<Action> s_RequestsQueue = new Queue<Action>(); // 线程同步请求队列
        private const int MAX_BATCH_SIZE = 10; // 每帧最多处理任务数
        private static int s_MainThreadId;

        public static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == s_MainThreadId;

        protected override void OnInit()
        {
           s_MainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        private void Update()
        {
            lock (s_RequestsQueue)
            {
                // 批量处理优化。避免单帧卡顿，适合高频小操作场景
                int batchSize = Mathf.Min(s_RequestsQueue.Count, MAX_BATCH_SIZE);
                for (int i = 0; i < batchSize; i++)
                {
                    Action action = s_RequestsQueue.Dequeue();
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"MainThreadDispatcher action execution failed: {ex}");
                    }
                }
            }
        }

        #region 下次主线程更新时执行 [Enqueue/OnNextUpdate]

        /// <summary>
        /// 锁定队列并将 <see cref="IEnumerator"/> 添加到队列中
        /// </summary>
        /// <param name="coroutine">将从主线程执行的 <see cref="IEnumerator"/> 函数。</param>
        /// <remarks>加入到更新队列，下次主线程更新时执行。</remarks>
        public void Enqueue(IEnumerator coroutine)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));
            Enqueue(() => StartCoroutine(coroutine));
        }

        /// <summary>
        /// 锁定队列并将操作添加到队列中
        /// </summary>
        /// <param name="action">将从主线程执行的函数。</param>
        /// <remarks>加入到更新队列，下次主线程更新时执行。</remarks>
        public void Enqueue(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            lock (s_RequestsQueue)
            {
                s_RequestsQueue.Enqueue(action);
            }
        }

        /// <summary>
        /// 锁定队列并将操作添加到队列中，返回一个任务，该任务在操作完成时完成
        /// </summary>
        /// <param name="action">将从主线程执行的函数。</param>
        /// <returns>在操作完成之前可以等待的任务</returns>
        /// <remarks>加入到更新队列，下次主线程更新时执行。</remarks>
        public Task EnqueueAsync(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            var tcs = new TaskCompletionSource<bool>();

            Enqueue(WrappedAction);
            return tcs.Task;

            void WrappedAction()
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }
        }

        /// <summary>
        /// 将一个函数入队，使其在主线程上异步执行，并返回该函数的执行结果。
        /// </summary>
        /// <typeparam name="T">函数的返回值类型。</typeparam>
        /// <param name="func">要执行的函数。</param>
        /// <returns>以该函数执行结果完成的任务（Task）。</returns>
        public Task<T> EnqueueAsync<T>(Func<T> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            var tcs = new TaskCompletionSource<T>();
            Enqueue(() =>
            {
                try
                {
                    T result = func();
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// 将一个异步函数入队，使其在主线程上执行，并返回该函数的执行结果。
        /// </summary>
        /// <param name="func">要执行的异步函数。</param>
        public Task EnqueueAsync(Func<Task> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            var tcs = new TaskCompletionSource<bool>();
            Enqueue(async () =>
            {
                try
                {
                    await func().ConfigureAwait(false);
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// 将一个带返回值的异步函数入队，使其在主线程上执行。
        /// </summary>
        /// <typeparam name="T">函数的返回值类型。</typeparam>
        /// <param name="func">要执行的异步函数。</param>
        public Task<T> EnqueueAsync<T>(Func<Task<T>> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            var tcs = new TaskCompletionSource<T>();
            Enqueue(async () =>
            {
                try
                {
                    T result = await func().ConfigureAwait(false);
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });
            return tcs.Task;
        }

        #endregion

        #region 同步请求到主线程 [Dispatch/SyncRequest]

        /// <summary>
        /// 在主线程上执行函数。
        /// </summary>
        /// <param name="action">将从主线程执行的 <see cref="IEnumerator"/> 函数。</param>
        /// <remarks>如果在主线程时立即执行，否则下次主线程更新时执行。</remarks>
        public void Dispatch(IEnumerator action)
        {
            if (IsMainThread) StartCoroutine(action);
            else Enqueue(action);
        }

        /// <summary>
        /// 在主线程上执行函数。
        /// </summary>
        /// <param name="action">将在主线程上执行的函数。</param>
        /// <remarks>如果在主线程时立即执行，否则下次主线程更新时执行。</remarks>
        public void Dispatch(Action action)
        {
            if (IsMainThread) action();
            else Enqueue(action);
        }

        /// <summary>
        /// 锁定队列并将 <see cref="Action"/> 添加到队列中，返回一个 <see cref="Task"/>，该 <see cref="Task"/> 在 <see cref="Action"/> 完成时完成
        /// </summary>
        /// <param name="action">将从主线程执行的函数。</param>
        /// <returns>可以等待到操作完成的 <see cref="Task"/></returns>
        /// <remarks>如果在主线程时立即执行，否则下次主线程更新时执行。</remarks>
        public Task DispatchAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            if (IsMainThread)
            {
                WrappedAction();
            }
            else
            {
                Enqueue(WrappedAction);
            }

            return tcs.Task;

            void WrappedAction()
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }
        }

        /// <summary>
        /// 锁定队列并将 <see cref="Action"/> 添加到队列中，返回一个 <see cref="Task"/>，该 <see cref="Task"/> 在 <see cref="Action"/> 完成时完成
        /// </summary>
        /// <param name="func">将从主线程执行的函数。</param>
        /// <returns>可以等待到操作完成的 <see cref="Task"/></returns>
        /// <remarks>如果在主线程时立即执行，否则下次主线程更新时执行。</remarks>
        public Task<T> DispatchAsync<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();

            if (IsMainThread)
            {
                WrappedAction();
            }
            else
            {
                Enqueue(WrappedAction);
            }

            return tcs.Task;

            void WrappedAction()
            {
                try
                {
                    tcs.TrySetResult(func());
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }
        }

        #endregion
    }
}