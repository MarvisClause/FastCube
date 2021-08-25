using UnityEngine; 
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasButtons : MonoBehaviour
{
    public Sprite musicOn, musicOff;

    private void Start()
    {
        if (PlayerPrefs.GetString("music") == "No" && gameObject.name=="Music")
            GetComponent<Image>().sprite = musicOff;
    }
    public void RestartGame()
    {
        if (PlayerPrefs.GetString("music") != "No")
            GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    } 
    public void LoadInstagram()
    {
        if (PlayerPrefs.GetString("music") != "No")
            GetComponent<AudioSource>().Play();
        Application.OpenURL("https://www.instagram.com/marvisclause/");
    } 
    public void MusicWork()
    {//now the music is off,you need to turn it on
        if (PlayerPrefs.GetString("music") == "No"){
            GetComponent<AudioSource>().Play();
            PlayerPrefs.GetString("music", "Yes");
            GetComponent<Image>().sprite = musicOn;
       }else{
            PlayerPrefs.GetString("music", "No");
            GetComponent<Image>().sprite = musicOff;
        }
        
    }
}
