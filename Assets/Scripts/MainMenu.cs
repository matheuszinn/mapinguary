using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private Image spinner;
	public void StartGame()
	{
		StartCoroutine(LoadGameSceneAsync());
	}
	private IEnumerator LoadGameSceneAsync()
    {
        // Start the asynchronous load operation
        var loadOperation = SceneManager.LoadSceneAsync(gameObject.scene.buildIndex + 1);
    
        // Wait until the asynchronous load operation has completed
        while (!loadOperation.isDone)
        {
            // Update the fillAmount property of the Image component to reflect the loading progress
            spinner.fillAmount = Math.Min(loadOperation.progress + 0.3f, 1.0f);
    
            // Wait for the next frame
            yield return null;
        }
    }
}
