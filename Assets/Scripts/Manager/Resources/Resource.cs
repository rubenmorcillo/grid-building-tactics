using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class Resource
{
    [SerializeField]
    public int _amount;
    [SerializeField]
    public string _name;
    [SerializeField]
    public int _maxAmount;


    
    public Resource(int _amount, string _name, int _maxAmount)
	{
        this._amount = _amount;
        this._name = _name;
        this._maxAmount = _maxAmount;
	}

}
