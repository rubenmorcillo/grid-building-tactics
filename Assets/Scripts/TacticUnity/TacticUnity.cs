using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticUnity : MonoBehaviour
{
    public CombatData combatData;
    public bool myTurn = false;
   
    void Start()
    {
        combatData = new CombatData();
    }

    void Update()
    {
        if (combatData.movementDone && combatData.actionDone)
		{
            CombateManager.instance.EndTurn();
		}
    }

    

    public void BeginTurn()
    {
        //if (GameManager.instance.mostrarDebug) Debug.Log("empieza mi turno:" + datosUnidad.ToString());
        combatData.RestorePoints();
        myTurn = true;
    }
    public void EndTurn()
    {
        myTurn = false;
    }


}
