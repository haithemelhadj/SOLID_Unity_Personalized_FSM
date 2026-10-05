using System;
using UnityEditor;
using UnityEngine;

public sealed class FsmPortTestState : State
{
    public int entered, exited;
    public bool sawSelfOnEnter;
    public override void OnEnter() { entered++; sawSelfOnEnter = ReferenceEquals(stateMachine.currentState, this); }
    public override void OnExit() { exited++; }
    public void Go(State next) => SwitchState(next);
}

public static class FsmPortValidation
{
    public static void Run()
    {
        var first = new GameObject("FSM validation A"); first.SetActive(false);
        var second = new GameObject("FSM validation B"); second.SetActive(false);
        var asset = ScriptableObject.CreateInstance<FsmPortTestState>();
        var other = ScriptableObject.CreateInstance<FsmPortTestState>();
        var config = ScriptableObject.CreateInstance<StatesList>();
        config.states.Add(new StatesList.StateEntry { state = _States.Grounded, stateClass = asset });
        config.states.Add(new StatesList.StateEntry { state = _States.Jump, stateClass = other });
        StateFactory fa = null, fb = null;
        try
        {
            var a = first.AddComponent<StateMachine>(); var b = second.AddComponent<StateMachine>();
            fa = new StateFactory(a, config); fb = new StateFactory(b, config);
            var a1 = (FsmPortTestState)fa.GetState(_States.Grounded);
            var b1 = (FsmPortTestState)fb.GetState(_States.Grounded);
            var a2 = (FsmPortTestState)fa.GetState(_States.Jump);
            Require(a1 != asset && a1 != b1, "State instances must be isolated");
            a.currentState = a1; b.currentState = b1;
            a1.Go(a2);
            Require(a.currentState == a2 && b.currentState == b1, "Transitions must be isolated");
            Require(a1.exited == 1 && a2.entered == 1 && a2.sawSelfOnEnter, "Lifecycle order");
            a1.Go(a1);
            Require(a.currentState == a2 && a1.exited == 1, "Stale state cannot transition");
            a2.Go(a2);
            Require(a2.entered == 1 && a2.exited == 0, "Self transition is ignored");
            Require(asset.entered == 0 && asset.exited == 0, "Asset must remain unchanged");
            bool missing = false;
            try { fa.GetState(_States.Death); } catch (InvalidOperationException) { missing = true; }
            Require(missing, "Missing state has an actionable error");
            a.currentState = null; b.currentState = null;
            fa.Dispose(); fb.Dispose(); fa = fb = null;
            Require(a1 == null && b1 == null && a2 == null, "Runtime clones cleaned up");
            Debug.Log("FSM_PORT_VALIDATION_PASSED: isolation, lifecycle, stale transitions, self transitions, missing config, cleanup");
        }
        finally
        {
            fa?.Dispose(); fb?.Dispose();
            UnityEngine.Object.DestroyImmediate(first); UnityEngine.Object.DestroyImmediate(second);
            UnityEngine.Object.DestroyImmediate(asset); UnityEngine.Object.DestroyImmediate(other); UnityEngine.Object.DestroyImmediate(config);
        }
    }
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
}
