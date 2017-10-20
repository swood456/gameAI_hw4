using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSwapButton : MonoBehaviour {

    public void swap_level(string level_name)
    {
        SceneManager.LoadScene(level_name);
    }
}
