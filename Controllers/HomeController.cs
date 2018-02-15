using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            ThreadPool.SetMinThreads(100, 100);

            Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);

            //var t = Task.FromResult(new Exception());
            //await t; //.ContinueWith(t => Debug.WriteLine(Thread.CurrentThread.ManagedThreadId));
            //TODO: should check that task is not completed before chaining continuewith

            //var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

            //Task.Run(async () =>
            //{
            //    Debug.WriteLine($"Backgroundtask: {Thread.CurrentThread.ManagedThreadId}");
            //    tcs.SetResult(10);
            //});

            //await tcs.Task;//.ConfigureAwait(false);


            //Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);


            Debug.WriteLine("start, " + new { System.Environment.CurrentManagedThreadId });

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            // test ContinueWith-style continuations (TaskContinuationOptions.ExecuteSynchronously)
            for (int i = 0; i < 50; i++)
            {
                ContinueAsync(i, tcs.Task);
            }

            Task.Run(() =>
            {
                Debug.WriteLine("before SetResult, " + new { System.Environment.CurrentManagedThreadId });
                tcs.TrySetResult(true);
            });

            await Task.Delay(20000);

            return View();
        }

        // log
        static void Continuation(int id)
        {
            Debug.WriteLine(new { continuation = id, System.Environment.CurrentManagedThreadId });
        }

        // await-style continuation
        static async Task ContinueAsync(int id, Task task)
        {
            await task.ConfigureAwait(false);
            Continuation(id);
        }

        // ContinueWith-style continuation
        static Task ContinueWith(int id, Task task)
        {
            return task.ContinueWith(
                t => Continuation(id),
                CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        private async Task<int> Foo()
        {
            //await Task.Run(async () =>
            //{
            //    Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
            //    await Task.Delay(10);
            //});

            return 10;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}