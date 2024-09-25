
using System.Diagnostics;
using UnityEngine;

public class AComp : MonoBehaviour
{
    public void OnGUI()
    {
        GUILayout.Button("AComp onGUI .............", null);
    }

    public void Update()
    {
        Debug.Log("AComp update ..");
    }

    //
    void Main()
    {
        Debug.Log("AComp Main test done");
    }
}
