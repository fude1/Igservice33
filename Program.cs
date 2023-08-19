using System.ServiceProcess;

namespace IGService3
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
            new IGService3()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
