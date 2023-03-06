using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] AudioClip musicaMenu;
    [SerializeField] AudioClip musicaBase;
    [SerializeField] AudioClip musicaExplorar;
    [SerializeField] AudioClip musicaFin;

    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ChangeMusic()
	{
        if (GameManager.instance.gameState == GameManager.GameState.BASE)
		{
            AudioClip newClip = musicaBase;
            if (audioSource.clip != newClip)
            {
                audioSource.Stop();
                audioSource.clip = musicaBase;
                audioSource.Play();
            }
		}
        else if (GameManager.instance.gameState == GameManager.GameState.EXPLORE)
		{
            AudioClip newClip = musicaExplorar;

            if (audioSource.clip != newClip)
			{
                audioSource.Stop();
                audioSource.clip = musicaExplorar;
                audioSource.Play();
            }
           
        }
        else if (GameManager.instance.gameState == GameManager.GameState.MENU_INICIO)
        {
            AudioClip newClip = musicaMenu;
            if (audioSource.clip != newClip)
            {
                audioSource.Stop();
                audioSource.clip = musicaMenu;
                audioSource.Play();
            }
        }
        else if (GameManager.instance.gameState == GameManager.GameState.END)
        {
            AudioClip newClip = musicaFin;
            if (audioSource.clip != newClip)
            {
                audioSource.Stop();
                audioSource.clip = musicaFin;
                audioSource.Play();
            }
        }

    }

}
