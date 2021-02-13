using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionNode : INode
{
    public delegate bool question();
    question _myQuestion;
    INode _nodeTrue;
    INode _nodeFalse;

    public QuestionNode(question myQuestion, INode nodeTrue, INode nodeFalse)
    {
        _myQuestion = myQuestion;
        _nodeTrue = nodeTrue;
        _nodeFalse = nodeFalse;
    }
    public void Execute()
    {
        if (_myQuestion())
        {
            _nodeTrue.Execute();
        }
        else
        {
            _nodeFalse.Execute();
        }
    }
}
