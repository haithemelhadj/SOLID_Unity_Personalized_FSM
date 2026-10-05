# Unity All Purpose State Machine (SOLID Friendly)

A flexible, extensible, SOLID-oriented state machine system for Unity.
Designed to keep state logic organized, share common behavior, and make gameplay values easy to tweak in the Inspector.

This README explains what the system is, how it works under the hood, and how to use it step by step.
The included player setup is the starting example. The shared core can also be adapted to other actors with their own states and context.

---

## Overview

This system uses:

* A **StateMachine** MonoBehaviour that initializes and runs your states.
* A **Context** component that holds the player's values, references, and shared gameplay behaviors.
* A **StatesList ScriptableObject**, which maps state enum values to state assets.
* A **StateFactory** that creates a separate set of runtime states for each machine.
* **State scripts** that inherit from `State`. States can inherit from each other to share behavior.

Responsibilities are separated so you can work on one part without putting everything into one player script:

* The machine runs the active state's lifecycle methods.
* Each state handles its own behavior and decides when to request a transition.
* The context gives states access to the player's data and components.
* The state assets store the values you configure in the Inspector.
* The factory turns those assets into runtime instances owned by one machine.

**Unity version:** 2022.3.62f2.

---

## System Architecture

### StateMachine (MonoBehaviour)

This component lives on your GameObject and handles:

* Preparing the context before the first state enters
* Creating the states through the factory
* Tracking the current state
* Forwarding Update, FixedUpdate, LateUpdate, and 2D collision events
* Cleaning up runtime state instances when the machine is destroyed

![Player Inspector showing the StateMachine, Context, state list, and initial state assignments](Assets/Readme_Recources/StateMachineComponent.PNG)

*The original Inspector screenshot shows the main assignments: Current Context, Player States, and Inital State. The updated component also includes debugging options.*

### Context

A separate `Context` component holds values that multiple player states need, such as:

* Rigidbody2D and collider references
* Animator controller and input handler
* Ground and wall checks
* Movement, jump, and combat values
* Shared flags such as whether the player can move

For example, Grounded and Jump can both read the same Rigidbody2D through `currentContext.Rb`, while applying different movement values.

The included `Context` is a player implementation. A different type of actor should provide its own context when adapting the generic core.

### StateFactory

The factory looks up a state by its enum value:

```csharp
factory.GetState(_States.Jump);
```

Each machine **clones the configured state assets** when it initializes. Two players can therefore use the same configuration without sharing their state timers or context references.

The assets are templates. Runtime clones belong to the machine and are destroyed with it.

---

## StatesList (ScriptableObject)

A ScriptableObject used to map a **State Enum** to a **State Asset**.

For example:

* `Grounded` -> a GroundedState asset
* `Jump` -> a JumpState asset
* `Fall` -> a FallState asset

Create one through **Create > States Config > Player States**.

![Unity Create menu showing States Config and Player States](Assets/Readme_Recources/StatesList.PNG)

Use one entry per enum key. Assign an asset to every state that your transitions can request, including the initial state.

---

## States

Each player state is a class inheriting from `State`, directly or through another state class.

Create the included state assets through **Create > States List > Player**.

![Unity Create menu showing the available player state assets](Assets/Readme_Recources/states.PNG)

Select an asset to edit its values. For example, GroundedState exposes separate walking and running speeds, acceleration, and deceleration.

![StatesList assignments and a GroundedState asset with editable movement values](Assets/Readme_Recources/StatesInstpector.PNG)

*The list chooses which asset represents a state. The selected state asset controls that state's settings.*

Common lifecycle methods:

* `OnEnter()` — set up the state when it becomes active
* `OnUpdate()` — update frame-based behavior
* `OnFixedUpdate()` — update physics-related behavior
* `OnLateUpdate()` — run late-frame behavior; the base implementation checks transitions
* `CheckSwitchState()` — decide whether another state should become active
* `OnExit()` — clean up when leaving the state

### Sharing Behavior Between States

The player uses inheritance to share common behavior:

```text
State
└── RootState
    ├── LocomotionState
    │   ├── GroundedState
    │   ├── JumpState
    │   └── FallState
    └── ActionState
```

For example, Grounded and Jump both inherit locomotion behavior instead of duplicating it. Their own overrides add the behavior that differs.

There is **one active state instance**. Parent behavior runs through `base` method calls. This is an inheritance-based state machine, not a behavior tree or a system with independently active parent and child states.

### Under the Hood

![Original State implementation showing machine, factory, context, timing, and animation fields](Assets/Readme_Recources/StateCode.PNG)

*This screenshot shows the original implementation. The shared references, timers, and lifecycle methods now live in the generic `StateBase`. The concrete player `State` keeps the animation behavior. The screenshot explains the original structure; use the current scripts and example below when writing new code.*

---

## How to Use

### 1. Open the Example

Open the project in Unity **2022.3.62f2**, then open the sample scene under `Assets/_Scenes`.

Inspect the existing player first. It shows how the components, animation references, and state assets fit together.

### 2. Add the StateMachine and Context

For a new player, add the `StateMachine` and `Context` components to the same GameObject.

The sample player also needs its Rigidbody2D, collider, sprite renderer, animator setup, `AnimatorController`, and `InputsHandler` components. Match the example player's setup and assign the references exposed by Context.

The machine prepares Context before entering the initial state, so state entry code can use its initialized component references.

### 3. Create a StatesList ScriptableObject

Right click in your Project window:

**Create > States Config > Player States**

Add entries for the states your player will use. Each entry needs its enum value and corresponding state asset.

### 4. Create Your States

Use the existing state assets, or create a new script inheriting from `State`.

This example waits for a configurable duration before switching to the existing Grounded state:

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "Timed Idle State", menuName = "States List/Player/Timed Idle")]
public class TimedIdleState : State
{
    public override void OnEnter()
    {
        base.OnEnter();
        // Set up anything this state needs.
    }

    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (!IsCurrentState) return;

        if (Time.time - enterTime >= duration)
        {
            SwitchState(factory.GetState(_States.Grounded));
            return;
        }
    }

    public override void OnExit()
    {
        // Clean up anything this state started.
        base.OnExit();
    }
}
```

Save it as `TimedIdleState.cs`, create an asset through its menu, and register that asset under the existing `_States.Idle` key. Ensure Grounded is also registered. Set a positive Duration and a valid Animation Name on the asset.

You can add a new enum member when needed; append it without changing existing enum values so saved configurations keep their meaning.

### 5. Assign Everything

In the StateMachine Inspector:

* Assign your **Current Context** component
* Assign the **Player States** configuration asset
* Choose **Inital State** from the registered states

The spelling `initalState` is retained in the code to preserve existing serialized assignments.

### 6. Add Values and Tweaks

Set shared player values on Context and state-specific values on the state assets.

For example:

* Change walking and running acceleration on GroundedState
* Change air movement values on JumpState
* Assign the animation name used when a state enters
* Tune jump and ground-check settings on Context

Configure template assets before entering Play Mode. Runtime clones are created during initialization; editing a template does not automatically update an already-running clone.

### 7. Check Your Transitions

Put higher-priority conditions first and return after choosing a transition.

When inheriting transition checks, call `base.CheckSwitchState()` and then check `IsCurrentState`. A parent may already have switched away, so the child should stop checking.

The base transition method:

1. Rejects requests from an inactive state, a null target, or the same state instance
2. Calls the old state's `OnExit()`
3. Assigns the new active state
4. Calls the new state's `OnEnter()`

That means entry code sees itself as the active state, and an exited state cannot overwrite the next transition.

---

## Reusing the Core in Another Project

The generic code is in `Assets/_State Machine Core/StateMachineCore.cs`.

For a different actor, provide:

* Its state enum and context
* A state type based on `StateBase`
* A configuration implementing `IStateListConfig` and entries implementing `IStateEntry`
* A factory based on `StateFactoryBase` that clones and initializes its state assets
* A machine based on `StateMachineBase` that initializes the actor and disposes its factory when destroyed

Use the included `State`, `StatesList`, `StateFactory`, and `StateMachine` as the concrete example. Keep actor-specific animation, movement, and input logic in the actor's adapter and context.

The generic architecture and per-machine state cloning were brought over from the later **Parrying_Game** implementation. The original player assets remain the example here.

---

## Benefits

* Shared behavior through state inheritance
* Inspector-editable state configuration
* Separate runtime state instances for each machine
* A generic core that can be adapted to other actors
* Explicit entry, update, transition, and exit hooks
* Player animation behavior kept outside the generic core

---

## Validation

The Editor regression check `FsmPortValidation.Run` covers:

* Runtime instance isolation between machines
* State entry and exit ordering
* Rejection of stale and self-transition requests
* Missing-state error reporting
* Runtime clone cleanup

Run it with the Unity Editor's `-executeMethod FsmPortValidation.Run` command-line option, together with `-batchmode`, `-quit`, and `-projectPath` pointing to this project.

Compilation and these checks passed in Unity 2022.3.62f2 after the migration. Movement, combat, and visual behavior still need an interactive playthrough of the sample scene.

---

## License
Copyright (c) 2025 Haithem Elhadj

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

