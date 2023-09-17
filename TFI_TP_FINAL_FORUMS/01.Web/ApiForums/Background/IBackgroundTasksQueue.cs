namespace ApiForums.Background
{
    /// <summary>
    /// Representa una cola de tareas que deben resolverse en segundo plano
    /// </summary>
    public interface IBackgroundTasksQueue
    {
        /// <summary>
        /// Añade una nueva tarea
        /// </summary>
        /// <param name="backgroundTask">Representación de la tarea, con
        /// los datos necesarios para llevar a cabo la misma</param>
        void AddTask(BackgroundTask backgroundTask);

        /// <summary>
        /// Extrae la primer tarea de lista
        /// </summary>
        /// <returns>Una tarea, o null si no hay tareas pendientes</returns>
        BackgroundTask GetNextTask();
    }
}
