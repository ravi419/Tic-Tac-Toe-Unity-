using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmptyCell : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    private void OnMouseDown()
    {
        Controller.Instance.Spawn(this.gameObject, int.Parse(this.gameObject.name));
    }

}
