using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorZombie : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> preZombies;
    private bool generar = true;
    [SerializeField] float delaySpawn = 5;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        if (StreetManager.instance.activado)
		{
            if (generar)
			{
                if (StreetManager.instance.listaEnemigos.Count < StreetManager.instance.maxEnemigos)
				{
                    StartCoroutine(SpawnDelay());
					GameObject zombieInstanciado = Instantiate(preZombies[0], transform.position, Quaternion.identity);
                    StreetManager.instance.listaEnemigos.Add(zombieInstanciado.GetComponent<Zombie>());
                }
            }
		}
    }

    IEnumerator SpawnDelay()
	{
        generar = false;
        yield return new WaitForSeconds(delaySpawn);
        generar = true;
	}

}
