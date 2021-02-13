using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T>
{
	States<T> _currentState;

	public FSM(States<T> state)
	{
		if (state != null)
			SetState(state);
	}

	public void SetState(States<T> states)
	{
		_currentState = states;
		_currentState.Awake();
	}

	public void OnUpdate()
	{
		_currentState.Execute();
	} 

	public void Transition(T key)
	{
		States<T> newState = _currentState.GetStates(key);

		if (newState == null) return;

		_currentState.Sleep();
		newState.Awake();
		_currentState = newState;
	}
}
