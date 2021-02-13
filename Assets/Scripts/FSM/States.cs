using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States<T>
{
    public virtual void Awake() { }
	public virtual void Execute() { }
	public virtual void Sleep() { }

	Dictionary<T, States<T>> _dictionary = new Dictionary<T, States<T>>();

	public void AddTransition(T key, States<T> state)
	{
		if (!_dictionary.ContainsKey(key))
			_dictionary.Add(key, state);
	}

	public void RemoveTransition(T key)
	{
		if (_dictionary.ContainsKey(key))
			_dictionary.Remove(key);
	}

	public States<T> GetStates(T key)
	{
		if (_dictionary.ContainsKey(key))
			return _dictionary[key];
		return null;
	}
}
