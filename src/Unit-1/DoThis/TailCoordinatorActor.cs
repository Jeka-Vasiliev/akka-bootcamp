using Akka.Actor;
using System;

namespace WinTail
{
    public class TailCoordinatorActor : UntypedActor
    {
        #region Message types

        /// <summary>
        /// Start tailing the file at user-specified path.
        /// </summary>
        public class StartTail
        {
            public StartTail(string filePath, IActorRef reporterActor)
            {
                FilePath = filePath;
                ReporterActor = reporterActor;
            }

            public string FilePath { get; }

            public IActorRef ReporterActor { get; }
        }

        /// <summary>
        /// Stop tailing the file at user-specified path.
        /// </summary>
        public class StopTail
        {
            public StopTail(string filePath)
            {
                FilePath = filePath;
            }

            public string FilePath { get; }
        }

        #endregion

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case StartTail msg:
                    // here we are creating our first parent/child relationship!
                    // the TailActor instance created here is a child
                    // of this instance of TailCoordinatorActor
                    Context.ActorOf(TailActor.Props(msg.ReporterActor, msg.FilePath));
                    break;
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30), x =>
            {
                switch (x)
                {
                    case ArithmeticException _:
                        return Directive.Resume;
                    case NotSupportedException _:
                        return Directive.Stop;
                    default:
                        return Directive.Restart;
                }
            });
        }

        public static Props Props() => Akka.Actor.Props.Create(() => new TailCoordinatorActor());
    }
}
