using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mangment : MonoBehaviour
{
    public  bool jumpbutton = false;
        public void jump(){
            jumpbutton = true;
         
        }

    public static Mangment returnManger (){
        return new Mangment();
    }


     public void RestartGame()
    {
      

        // Restart the scene (Assuming this script is attached to a GameObject in the scene)
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
     void Start()
    {
        // Make the game run as fast as possible
        Application.targetFrameRate = 120;
  
    }
}
