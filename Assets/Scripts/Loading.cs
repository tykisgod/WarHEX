using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour {

    public void LoadingLevel ( string LevelName ) {
        SceneManager . LoadScene ( LevelName );
    }

    public void ExitGame ( ) {
        Application . Quit ( );
    }

    // Use this for initialization
    void Start ( ) {

    }

    // Update is called once per frame
    void Update ( ) {

    }
}
