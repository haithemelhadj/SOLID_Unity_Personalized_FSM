using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player States", menuName = "States Config/Player States")]

public class StatesList : ScriptableObject, IStateListConfig<_States, State, StatesList.StateEntry>
{
    [System.Serializable]
    public class StateEntry : IStateEntry<_States, State>
    {
        public _States StateKey => state;
        public State StateAsset => stateClass;
        public _States state;
        public State stateClass;

    }

    public List<StateEntry> Entries => states;
    public List<StateEntry> states = new List<StateEntry>();
}
