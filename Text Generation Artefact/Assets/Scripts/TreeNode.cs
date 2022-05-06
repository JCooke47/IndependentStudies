using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode : MonoBehaviour
{
    string _text;
    List<TreeNode> _childNodes;
    List<float> _odds;

    public TreeNode(string text)
    {
        _text = text;
    }

    public TreeNode(string text, List<TreeNode> childNodes)
    {
        _text = text;
        _childNodes = childNodes;
    }

      public TreeNode(string text, List<TreeNode> childNodes, List<float> odds)
    {
        _text = text;
        _childNodes = childNodes;
        _odds = odds;
    }

    public string Text
    {
        get => _text;
    }

    public List<float> Odds
    {
        get => _odds;
    }

    public List<TreeNode> ChildNodes
    {
        get => _childNodes;
        set => _childNodes = value;
    }
}
