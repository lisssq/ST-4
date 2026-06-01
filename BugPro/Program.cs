using System;
using Stateless;

namespace BugPro
{
    public enum State
    {
        Analysis, // разбор дефектов
        Resolution, // исправление дефекта
        NeedMoreInfo, // требуется дополнительная информация
        Rejected, // не дфект / не исправлять
        NotReproducible, // не воспроизводится
        Closed, // закрыто

    }

    public enum Trigger
    {
        ToResolution, // перейти к исправлению
        Reject, // отклонить (не дефект)
        AskInfo, // запросить дополнительную информацию
        VerifySuccess, // подтвердить исправление (проблема решена? да)
        VerifyFailure, // подтвердить не исправление (проблема решена? нет)
        CannotReproduce, // сообщить о невозможности воспроизвести
        ConfirmNotRepro, // подтвердить невозможность воспроизвести (ок? да)
        ConfirmBugExists, // подтвердить существование дефекта (ок? нет)
        Reopen, // переоткрыть дефект

    }

    public class Bug
    {
        public StateMachine<State, Trigger> machine { get; }

        public Bug()
        {
            machine = new StateMachine<State, Trigger>(State.Analysis);



            machine.Configure(State.Analysis)
                .Permit(Trigger.ToResolution, State.Resolution)
                .Permit(Trigger.Reject, State.Rejected)
                .Permit(Trigger.AskInfo, State.NeedMoreInfo);

            machine.Configure(State.Resolution)
                .Permit(Trigger.CannotReproduce, State.NotReproducible)
                .Permit(Trigger.VerifySuccess, State.Closed)
                .Permit(Trigger.VerifyFailure, State.Analysis)
                .Permit(Trigger.AskInfo, State.NeedMoreInfo);

            machine.Configure(State.NeedMoreInfo)
                .Permit(Trigger.ToResolution, State.Analysis);

            machine.Configure(State.NotReproducible)
                .Permit(Trigger.ConfirmNotRepro, State.Closed)
                .Permit(Trigger.ConfirmBugExists, State.Analysis);

            machine.Configure(State.Rejected)
                .Permit(Trigger.ToResolution, State.Analysis);

            machine.Configure(State.Closed)
                .Permit(Trigger.Reopen, State.Analysis);
        }


        public State CurrentState => machine.State;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Тест прохода по схеме:\n");
            var bug = new Bug();
            Console.WriteLine($"Старт: {bug.machine.State} (Analysis)");

            bug.machine.Fire(Trigger.ToResolution);
            Console.WriteLine($"Действие: ToResolution — Статус: {bug.machine.State}");

            bug.machine.Fire(Trigger.AskInfo);
            Console.WriteLine($"Действие: AskInfo — Статус: {bug.machine.State}");

            bug.machine.Fire(Trigger.ToResolution);
            Console.WriteLine($"Действие: ToResolution — Статус: {bug.machine.State}");

            bug.machine.Fire(Trigger.ToResolution);
            bug.machine.Fire(Trigger.VerifySuccess);
            Console.WriteLine($"Действие: VerifySuccess — Статус: {bug.machine.State}\n");
            Console.WriteLine("\n Дефект исправлен");
        }
    }
}