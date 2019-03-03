using System;
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

            var consoleWriterActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
            var consoleReaderActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(consoleWriterActor)));

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            await MyActorSystem.WhenTerminated.ConfigureAwait(false);
        }
    }
    #endregion
}
