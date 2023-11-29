using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StateMachine;

public abstract class StateMachine : MonoBehaviour
{

    public delegate void Event();

    public Event StateChanged;

    public class State : IEquatable<State>
    {
        public Event EnterEvent;
        public Event LeaveEvent;
        int id;
        private readonly string name;
        public State(string name = "")
        {
            var rand = new System.Random();
            id = rand.Next();
            this.name = name;
        }
        public override string ToString()
        {
            return name;
        }
        public override int GetHashCode()
        {
            return id;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as State);
        }
        public bool Equals(State obj)
        {
            return obj != null && obj.id == this.id;
        }
    }

    public class Command : IEquatable<Command>
    {
        int id;
        private readonly string name;
        public Command(string name = "")
        {
            var rand = new System.Random();
            id = rand.Next();
            this.name = name;
        }
        public override string ToString()
        {
            return name;
        }
        public override int GetHashCode()
        {
            return id;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Command);
        }
        public bool Equals(Command obj)
        {
            return obj != null && obj.id == this.id;
        }
    }

    public class StateTransition
    {
        readonly State CurrentState;
        readonly Command Command;
        public StateTransition(State currentState, Command command)
        {
            CurrentState = currentState;
            Command = command;
        }
        public override int GetHashCode()
        {
            return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            StateTransition other = obj as StateTransition;
            return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
        }
    }

    public Dictionary<StateTransition, State> transitions;
    public State CurrentState { get; private set; }
    public State PreviousState { get; private set; }

    public void SetInitialState(State state)
    {
        CurrentState = state;
        try
        {
            CurrentState.EnterEvent();
        }
        catch (NullReferenceException) { };
    }

    public State GetNext(Command command)
    {
        StateTransition transition = new StateTransition(CurrentState, command);
        State nextState;
        if (!transitions.TryGetValue(transition, out nextState))
            throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
        Debug.Log("Transitioning: " + CurrentState + "+" + command + " -> " + nextState);
        return nextState;
    }

    public State MoveNext(object cmdOrState)
    {
        State nextState;
        if (cmdOrState.GetType() == typeof(Command))  {
            nextState = GetNext(cmdOrState as Command);
        } else
        {
            nextState = cmdOrState as State;
        }
        try
        {
            CurrentState.LeaveEvent();
        } 
        catch (NullReferenceException) { };

        try
        {
            StateChanged();
        }
        catch (NullReferenceException) { };

        PreviousState = CurrentState;

        try
        {
            nextState.EnterEvent();
        }
        catch (NullReferenceException) { };
        
        CurrentState = nextState;
        return CurrentState;
    }
}