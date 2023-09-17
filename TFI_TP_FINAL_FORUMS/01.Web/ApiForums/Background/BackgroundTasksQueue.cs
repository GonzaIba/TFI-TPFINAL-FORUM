using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiForums.Background
{
    public class BackgroundTasksQueue : IBackgroundTasksQueue
    {
        private readonly Queue<BackgroundTask> _backgroundTasks = new Queue<BackgroundTask>();

        public void AddTask(BackgroundTask backgroundTask)
        {
            _backgroundTasks.Enqueue(backgroundTask);
        }

        public BackgroundTask GetNextTask()
        {
            try
            {
                if (_backgroundTasks.Any())
                {
                    return _backgroundTasks.Dequeue();
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            return null;
        }
    }
}
