using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hebert.Types
{
    /// <summary> A class for safely managing ongoing task. </summary>
    public class TaskManager
    {
        /// <summary> The currently active tasks. </summary>
        private List<Task> _tasks = new List<Task>();

        /// <summary> The dummy object to allow thread-safe access to tasks. </summary>
        private Object _lock = new Object();


        /// <summary> Add a task to the manager. </summary>
        /// <param name="task"> The task to add. </param>
        public void AddTask(Task task)
        {
            lock (_lock)
            {
                _tasks.Add(task);
            }

            // Add a continuation task to remove the completed task.
            task.ContinueWith(t =>
            {
                lock (_lock)
                {
                    _tasks.Remove(t);
                    Console.WriteLine($"Task {t.Id} completed and removed.");   // TODO - Replace with logger.
                }
            }, TaskContinuationOptions.ExecuteSynchronously); // Execute synchronously to ensure removal before other operations.
        }


        /// <summary> Wait for all the tasks in the manager to complete. </summary>
        public void WaitForAllTasks()
        {
            Task[] currentTasks;
            lock (_lock)
            {
                currentTasks = _tasks.ToArray();
            }
            if (currentTasks.Length > 0)
            {
                Task.WaitAll(currentTasks);
            }
        }


        /// <summary> Get a count of the currently running tasks. </summary>
        /// <returns> How many tasks are currently running in the manager. </returns>
        public Int32 GetRemainingTaskCount()
        {
            lock (_lock)
            {
                return _tasks.Count;
            }
        }
    }
}
