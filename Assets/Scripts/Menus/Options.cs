using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class Options : MonoBehaviourPun
{
    public float verticalSens, horizontalSens, aimSens, fov;
    public PlayerController playerController;
    public Camera cam;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider vSlider;
    public Slider hSlider;
    public Slider aSlider;
    public Slider fovSlider;
    public TMP_Text vText;
    public TMP_Text hText;
    public TMP_Text aText;
    public TMP_Text fovText;
    Resolution[] resolutions;
    
    void Awake() 
    {
        if(!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }  
    }
    void Start()
    {
        LoadSettings();
        
        fullscreenToggle.isOn = Screen.fullScreen;
        
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        vText.text = vSlider.value.ToString();
        hText.text = hSlider.value.ToString();
        aText.text = aSlider.value.ToString();
        fovText.text = fovSlider.value.ToString();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void vSens(float vValue)
    {
        //save
        PlayerPrefs.SetFloat("vSens", vValue);
        PlayerPrefs.Save();

        //send to controller
        verticalSens = PlayerPrefs.GetFloat("vSens", 3);
        playerController.vSensitivity = verticalSens;

        //set text
        vText.text = vSlider.value.ToString();
    }

    public void hSens(float hValue)
    {
        //save
        PlayerPrefs.SetFloat("hSens", hValue);
        PlayerPrefs.Save();

        //send to controller
        horizontalSens = PlayerPrefs.GetFloat("hSens", 100);
        playerController.hSensitivity = horizontalSens;

        //set text
        hText.text = hSlider.value.ToString();
    }

    public void aSens(float aValue)
    {
        //save
        PlayerPrefs.SetFloat("aimSens", aValue);
        PlayerPrefs.Save();

        //send to controller
        aimSens = PlayerPrefs.GetFloat("aimSens", 3);
        playerController.aimSensitivity = aimSens;

        //set text
        aText.text = aSlider.value.ToString();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void ChangeFOV(float value)
    {
        //save
        PlayerPrefs.SetFloat("FOV", value);
        PlayerPrefs.Save();
            
        //set fov
        playerController.fov = PlayerPrefs.GetFloat("FOV", 60);

        //set text
        fovText.text = fovSlider.value.ToString();
    }

    [PunRPC]
    public void FOV(float value)
    {
        if(photonView.IsMine)
        {
            Debug.LogError(photonView.Owner.NickName);
            
            Debug.LogError(value);
        
            //save
            PlayerPrefs.SetFloat("FOV", value);
            PlayerPrefs.Save();
            
            //set fov
            playerController.fov = PlayerPrefs.GetFloat("FOV", 60);

            //set text
            fovText.text = fovSlider.value.ToString();
        }
    }
    public void LoadSettings()
    {
        if(photonView.IsMine)
        {
            verticalSens = PlayerPrefs.GetFloat("vSens", 3);
            horizontalSens = PlayerPrefs.GetFloat("hSens", 100);
            aimSens = PlayerPrefs.GetFloat("aimSens", 3);
            fov = PlayerPrefs.GetFloat("FOV", 60);

            playerController = transform.parent.parent.GetComponent<PlayerController>();
            playerController.hSensitivity = horizontalSens;
            playerController.vSensitivity = verticalSens;
            playerController.aimSensitivity = aimSens;
            hSlider.value = horizontalSens;
            vSlider.value = verticalSens;
            aSlider.value = aimSens;
            fovSlider.value = fov;
        }
    }
}
