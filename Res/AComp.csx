
using System.Diagnostics;
using UnityEngine;

public class AComp : MonoBehaviour
{
    public void OnGUI()
    {
        GUILayout.Button("onGUI .............",null);
    }

    public void Update()
    {
        Debug.Log("update ..");
    }

    //
    void Main()
    {
        Debug.Log("Main test done");
    }
}
