using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator  ButtonFlashing;
     public async void StartGameTask()
    {

        AudioManager.instance.Play("Start");
        ButtonFlashing.SetBool("Clicked", true);
        Debug.Log("set ButtonFlashing true");
        await Task.Delay(1000);
        ButtonFlashing.SetBool("Clicked", false);
        Debug.Log("LoadScene");
        SceneManager.LoadScene("MainGame");
    }

   
    public void QuitGame()
    {
        Application.Quit();
    }
}
