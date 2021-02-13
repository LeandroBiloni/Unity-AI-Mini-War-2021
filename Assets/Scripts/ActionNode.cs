using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode<T> : INode
{
    FSM<T> _fsm;
    T _key;

	public ActionNode(FSM<T> myFSM, T key)
    {
        _fsm = myFSM;
        _key = key;
    }
    //LLama a la transicion de estado del state machine
	public void Execute()
    {
        _fsm.Transition(_key);
    }
}
