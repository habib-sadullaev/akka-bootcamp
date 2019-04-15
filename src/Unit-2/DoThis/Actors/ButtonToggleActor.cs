using Akka.Actor;
using System;
using System.Windows.Forms;

namespace ChartApp.Actors
{
    public class ButtonToggleActor : UntypedActor
    {
        public class Toggle { }

        private readonly IActorRef _coordinatorActor;
        private readonly Button _myButton;
        private bool _toggledOn;
        private CounterType _myCounterType;

        public ButtonToggleActor(IActorRef coordinatorActor, Button myButton, CounterType myCounterType, bool toggledOn = false)
        {
            _coordinatorActor = coordinatorActor;
            _myButton = myButton;
            _toggledOn = toggledOn;
            _myCounterType = myCounterType;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Toggle _ when _toggledOn:
                    _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Unwatch(_myCounterType));
                    FlipToggle();
                    break;
                case Toggle _ when !_toggledOn:
                    _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Watch(_myCounterType));
                    FlipToggle();
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }

        private void FlipToggle()
        {
            _toggledOn = !_toggledOn;

            _myButton.Text = $"{_myCounterType} ({(_toggledOn ? "ON" : "OFF")})";
        }
    }
}
