using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchMarkerInfo : MonoBehaviour
{

    public GameObject touchImage;
    public GameObject infoBox;
    public Text labelTextField;
    public Text UIDTextField;
    public Text coordinateTextField;
    public Text touchIDTextField;
    // Start is called before the first frame update
    public void UpdateInfoText(string labelText, string UIDText, string coordinateText, string touchIDText)
    {
        labelTextField.text = labelText;
        UIDTextField.text = UIDText;
        coordinateTextField.text = coordinateText;
        touchIDTextField.text = touchIDText;
    }

    public void ShowInfo(bool isShown)
    {
        touchImage.SetActive(isShown);
        infoBox.SetActive(isShown);
    }
}
