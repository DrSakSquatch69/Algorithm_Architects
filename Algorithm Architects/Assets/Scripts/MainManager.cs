using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    //Weapon Fields
    List<gunStats> gunList;
    int selectedGunPOS;

    //Setting Fields
    float sensitivity;


    //getters
    public List<gunStats> GetGunList() {return gunList; }
    public int GetSelectedGunPOS() { return selectedGunPOS; }
    public float GetSensitivity() { return sensitivity; }
    //setters
    public void SetSensitivity(float sensitivityPassedIn) 
    { 
        sensitivity = sensitivityPassedIn; 
    } 
    public void SetGunList(List<gunStats> gunListPassedIn)
    {
        if(gunList == null)
        {
            gunList = new List<gunStats>();
            gunList = gunListPassedIn;
        }
        else
        {
            gunList.Clear();
            gunList = gunListPassedIn;
        }
    }
    public  void SetSelectedGunPos(int selectedGunPOSPassedIn)
    {
        selectedGunPOS = selectedGunPOSPassedIn;
    }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
