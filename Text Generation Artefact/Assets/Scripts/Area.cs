using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    private string _name;
    private string[] _connectedRooms;
    private string[] _weapons;
    private string[] _valuables;
    private string[] _clueItems;
    private bool _escapePossible;
    private string[] _escapeMethods;

    public Area(string name, string[] connectedRooms, string[] weapons, string[] valuables, 
                string[] clueItems, bool escapePossible, string[] escapeMethods)
    {
        _name = name;
        _connectedRooms = connectedRooms;
        _weapons = weapons;
        _valuables = valuables;
        _clueItems = clueItems;
        _escapePossible = escapePossible;
        _escapeMethods = escapeMethods;
    }

    public string Name
    {
        get => _name;
    }

    public string[] ConnectedRooms
    {
        get => _connectedRooms;
    }

    public string[] Weapons
    {
        get => _weapons;
    }

    public string[] Valuables
    {
        get => _valuables;
    }

    public string[] ClueItems
    {
        get => _clueItems;
    }

    public bool EscapePossible
    {
        get => _escapePossible;
    }

    public string[] EscapeMethods
    {
        get => _escapeMethods;
    }
}
