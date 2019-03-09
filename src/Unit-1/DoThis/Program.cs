using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail
{
    #region Program
    internal static class Program
    {
        public static ActorSystem MyActorSystem;

        private static async Task Main()
        {
            MyActorSystem = ActorSystem.Create("MyActorSystem");

            var consoleWriterActor = MyActorSystem.ActorOf(ConsoleWriterActor.Props(), "consoleWriterActor");
            var tailCoordinatorActor = MyActorSystem.ActorOf(TailCoordinatorActor.Props(), "tailCoordinatorActor");
            var fileValidatorActor = MyActorSystem.ActorOf(FileValidatorActor.Props(consoleWriterActor), "validationActor");
            var consoleReaderActor = MyActorSystem.ActorOf(ConsoleReaderActor.Props(), "consoleReaderActor");

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            await MyActorSystem.WhenTerminated.ConfigureAwait(false);
        }
    }
    #endregion
}
