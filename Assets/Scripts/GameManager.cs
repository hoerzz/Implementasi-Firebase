﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null; 

    public static GameManager Instance 

    { 

        get 

        { 

            if (_instance == null) 

            { 

                _instance = FindObjectOfType<GameManager> (); 

            } 

 

            return _instance; 

        } 

    } 
   // Fungsi [Range (min, max)] ialah menjaga value agar tetap berada di antara min dan max-nya 

    [Range (0f, 1f)] 

    public float AutoCollectPercentage = 0.1f; 
    public float SaveDelay = 5f;
    public ResourceConfig[] ResourcesConfigs; 
    public Sprite[] ResourcesSprites;

 

    public Transform ResourcesParent; 
    public ResourceController ResourcePrefab; 
    public TapText TapTextPrefab;

 

    public Transform CoinIcon;
    public Text GoldInfo; 
    public Text AutoCollectInfo; 

 

    private List<ResourceController> _activeResources = new List<ResourceController> (); 
    private List<TapText> _tapTextPool = new List<TapText> ();
    private float _collectSecond; 
    private float _saveDelayCounter;

 

    

 

    private void Start () 

    { 

        AddAllResources (); 
          GoldInfo.text = $"Gold: { UserDataManager.Progress.Gold.ToString ("0") }";
    } 
    private void Update () 

    { 
         float deltaTime = Time.unscaledDeltaTime;

        _saveDelayCounter -= deltaTime;

        // Fungsi untuk selalu mengeksekusi CollectPerSecond setiap detik 

        _collectSecond += Time.unscaledDeltaTime;
        _collectSecond += deltaTime; 

        if (_collectSecond >= 1f) 

        { 

            CollectPerSecond (); 

            _collectSecond = 0f; 

        } 

        CheckResourceCost ();

        CoinIcon.transform.localScale = Vector3.LerpUnclamped (CoinIcon.transform.localScale, Vector3.one * 1f, 0.15f);

        //CoinIcon.transform.Rotate (0f, 0f, Time.deltaTime * -100f);

    } 

 

    private void AddAllResources () 

    { 
        int index = 0;
         bool showResources = true;

        foreach (ResourceConfig config in ResourcesConfigs) 

        { 

            GameObject obj = Instantiate (ResourcePrefab.gameObject, ResourcesParent, false); 

            ResourceController resource = obj.GetComponent<ResourceController> (); 

 

            resource.SetConfig (index, config); 

             obj.gameObject.SetActive (showResources);

 

            if (showResources && !resource.IsUnlocked)

            {

                showResources = false;

            }

            _activeResources.Add (resource); 
            index++;

        } 

    } 

    public void ShowNextResource ()

    {

        foreach (ResourceController resource in _activeResources)

        {

            if (!resource.gameObject.activeSelf)

            {

                resource.gameObject.SetActive (true);

                break;

            }

        }

    }


     private void CheckResourceCost ()

    {

        foreach (ResourceController resource in _activeResources)

        {

            bool isBuyable = false;

            if (resource.IsUnlocked)

            {

                isBuyable = UserDataManager.Progress.Gold >= resource.GetUpgradeCost ();

            }

            else

            {

                isBuyable = UserDataManager.Progress.Gold >= resource.GetUnlockCost ();

            }

 

            resource.ResourceImage.sprite = ResourcesSprites[isBuyable ? 1 : 0];

        }

    }

 

    private void CollectPerSecond () 

    { 

        double output = 0; 

        foreach (ResourceController resource in _activeResources) 

        { 
             if (resource.IsUnlocked)

            {
            output += resource.GetOutput (); 
            }
        } 

 

        output *= AutoCollectPercentage; 

        // Fungsi ToString("F1") ialah membulatkan angka menjadi desimal yang memiliki 1 angka di belakang koma 

        AutoCollectInfo.text = $"Auto Collect: { output.ToString ("F1") } / second"; 

 

        AddGold (output); 

    } 

 

    public void AddGold (double value) 

    { 

        UserDataManager.Progress.Gold += value; 

        GoldInfo.text = $"Gold: { UserDataManager.Progress.Gold.ToString ("0") }"; 
        UserDataManager.Save ();

        UserDataManager.Save (_saveDelayCounter < 0f);

 

        if (_saveDelayCounter < 0f)

        {

            _saveDelayCounter = SaveDelay;

        }
    } 

     public void CollectByTap (Vector3 tapPosition, Transform parent)

    {

        double output = 0;

        foreach (ResourceController resource in _activeResources)

        {
             if (resource.IsUnlocked)

            {
            output += resource.GetOutput ();
            }
        }

 

        TapText tapText = GetOrCreateTapText ();

        tapText.transform.SetParent (parent, false);

        tapText.transform.position = tapPosition;

        SoundManager.smInstance.Audio.PlayOneShot(SoundManager.smInstance.ClickCoint);

        tapText.Text.text = $"+{ output.ToString ("0") }";

        tapText.gameObject.SetActive (true);

        CoinIcon.transform.localScale = Vector3.one * 1.75f;

 

        AddGold (output);

    }

 

    private TapText GetOrCreateTapText ()

    {

        TapText tapText = _tapTextPool.Find (t => !t.gameObject.activeSelf);

        if (tapText == null)

        {

            tapText = Instantiate (TapTextPrefab).GetComponent<TapText> ();

            _tapTextPool.Add (tapText);

        }

 

        return tapText;

    }
    public void home()
    {
        SceneManager.LoadScene (0);
    }

} 



// Fungsi System.Serializable adalah agar object bisa di-serialize dan

// value dapat di-set dari inspector

[System.Serializable]

public struct ResourceConfig

{

    public string Name;

    public double UnlockCost;

    public double UpgradeCost;

    public double Output;

}