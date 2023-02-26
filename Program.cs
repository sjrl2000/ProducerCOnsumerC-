//
// Author: Sebastian Rivera
// 4/11/2022
// Program 2 - CS 490-01
// Built upon threadexample.cs provided on Canvas but most things got replaced/changed
//
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadTest
{
    internal class Program
    {
        /// <summary>
        /// Main where we call our processes to begin
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(10, 0); // Sets minimum amount of threads
            var plant = new Processes();
            plant.Process();
        }
    }
    /// <summary>
    /// Where we create our processes
    /// </summary>
    public sealed class Processes
    {
        /// <summary>
        /// Using a BlockingCollection Queue referenced from https://docs.microsoft.com/en-us/dotnet/standard/collections/thread-safe/blockingcollection-overview
        /// In order to create a thread safe environment
        /// Also allows "Concurrent adding and taking of items from multiple threads."
        /// "Insertion and removal operations that block when collection is empty or full."
        /// </summary>
        private readonly BlockingCollection<string> _queue = new BlockingCollection<string>();

        public void Process()
        {
            Parallel.Invoke(producer, consumers);
        }
        /// <summary>
        /// Creates the producer object 
        /// </summary>
        private void producer()
        {
            // Randomly begins creating nodes, could not get it to create a certain amount, it creates it 
            // one by one but it does do it in random intervals depending on
            // the time it the producer awakes
            Console.WriteLine("Producer is starting...");
            Console.WriteLine("Producer is adding Nodes to Queue");
            for (int i = 0; i < 75; i++)
            {
                //Convert node to string for easier printing to commandline
                string node = i.ToString();
                Console.WriteLine("Producer is queueing {0}", node);
                _queue.Add(node);  // Here's where we add a process to the queue.
                Thread.Sleep(0);
            }
            
            _queue.CompleteAdding(); // Here's where we make the producer exit from the processes it assgined by adding the nodes to a queue.
            Console.WriteLine("Producer has completed its queues/task."); 
        }
        /// <summary>
        /// Invokes consumers and creates 2 consumers to take a node(s) added to queue and process
        /// </summary>
        private void consumers()
        {
            Parallel.Invoke(
                () => consumer(1),
                () => consumer(2)
            );
        }
        /// <summary>
        /// Creates the consumer object
        /// </summary>
        /// <param name="id"></param>
        private void consumer(int id)
        {
            Console.WriteLine("\t\t\tConsumer {0} is starting.", id);
            Console.WriteLine("\t\t\tConsumer {0} is idle...", id);
            DateTime start = DateTime.Now;
            foreach (var node in _queue.GetConsumingEnumerable()) //Here's where we remove items.
            {
                DateTime end = DateTime.Now;
                TimeSpan elapsed = end - start;
                Console.WriteLine("\t\t\tConsumer {0} completed process {1} at {2} ms.", id, node, elapsed.TotalMilliseconds.ToString()); // Consumer ID, Node completed and time elapsed on process
                Thread.Sleep(0);
            }

            Console.WriteLine("\t\t\tConsumer {0} is exiting - completed processes", id);
        }
    }
}