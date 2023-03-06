using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Interactuable : MonoBehaviour, IClickable
{
    // Start is called before the first frame update

    public enum InteractionType { PICK, SEARCH }
    public InteractionType type;
    public List<Resource> loot;
    public float interactTime = 2f;
	public bool looted = false;
	AudioSource audio;

	private void Start()
	{
		audio = GetComponent<AudioSource>();
	}

	private void CreateDefaultLoot()
	{
        int v = Mathf.RoundToInt(Random.Range(1, GameManager.instance.resourceManager.CountResources()));
        for (int i = 0; i < v; i++)
        {
			Resource r = new Resource(100, GameManager.instance.resourceManager.GetResourceByIndex(Mathf.RoundToInt(Random.Range(0, GameManager.instance.resourceManager.CountResources())))._name, 0);
			//Debug.Log("añadiendo " + r._name);
            loot.Add(r);
        }
    }

    public void GiveLoot()
	{
		if (!looted)
		{
			if (loot == null || loot.Count < 1)
			{
				loot = new List<Resource>();
				CreateDefaultLoot();
			}
			foreach (Resource r in loot)
			{
				//Debug.Log(gameObject.name + " entregando " + r._amount + " de  " + r._name);
				UtilsClass.CreateWorldTextPopup("+"+r._amount+" "+r._name, transform.position);
				GameManager.instance.resourceManager.cambiarRecursos(new ResourceManager.OnResourceValueChangeArgs(r._amount, r._name));
			}
			looted = true;
		}
      
		//podría esperar un poco y después eliminarlo
        //GridBuildingSystem3D.Instance.Demolish(GetComponent<PlacedObject_Done>());
	}


	public void OnHoverEnter()
	{
		if (!looted)
		{
			if (audio != null && !audio.isPlaying)
			{
				audio.Play();
			}
			foreach (Renderer r in GetComponentsInChildren<Renderer>())
			{
				CursorManager.instance.SetCursor(CursorManager.CursorType.LOOK);
				r.material.SetColor("_OutlineColor", new Color32(60, 237, 74, 190));
				r.material.SetFloat("_Outline", 0.2f);
			}
		}
		
	}

	public void OnHoverExit()
	{
		foreach (Renderer r in GetComponentsInChildren<Renderer>())
		{
			CursorManager.instance.SetCursor(CursorManager.CursorType.DEFAULT);
			r.material.SetColor("_OutlineColor", new Color32(0, 0, 0, 190));
			r.material.SetFloat("_Outline", 0f);
		}
	}
}
