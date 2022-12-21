using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasButton : MonoBehaviour
{
    public Sprite musicOn, musicOff;

    private void Start() {
        if (PlayerPrefs.GetString("music") == "No" && gameObject.name == "Volume"){
            GetComponent<Image>().sprite = musicOff;
        }
    }

    public void RestartGame() {
        playSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadTwitch(){
        playSound();
        //Application.OpenURL("https://www.twitch.tv/lul1ch");
    }

    public void SoundSwitcher(){
        if (PlayerPrefs.GetString("music") == "No"){
            PlayerPrefs.SetString("music", "Yes");
            GetComponent<AudioSource>().Play();
            GetComponent<Image>().sprite = musicOn;
        } else {
            PlayerPrefs.SetString("music", "No");
            GetComponent<Image>().sprite = musicOff;
        }
    }

    public void OpenShop(){
        playSound();

        SceneManager.LoadScene("Shop");
    }

    public void CloseShop(){
        playSound();

        SceneManager.LoadScene("Game");
    }

    private void playSound(){
        if (PlayerPrefs.GetString("music") != "No") {
            GetComponent<AudioSource>().Play();
        }
    }
}
