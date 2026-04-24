using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Script;
public class ButtonHighlighter : MonoBehaviour
{
    private Button previousButton;
    [SerializeField] private float scaleAmount = 1.1f;
    public GameObject defaultButton;
    public GameObject singleplaybtn;

    [SerializeField] Button[] levels;
    [SerializeField] bool levelSelection;

    private void Awake()
    {
        if (!AndroidTV.IsAndroidOrFireTv())
        {
            //this.gameObject.GetComponent<ButtonHighlighter>().enabled = false;
        }

        if (!PlayerPrefs.HasKey("currentHighlighter"))
        {
            PlayerPrefs.SetInt("currentHighlighter", 0);
        }
    }
    void Start()
    {
        if (defaultButton != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton);
        }

        if (levelSelection)
        {
            EventSystem.current.SetSelectedGameObject(levels[PlayerPrefs.GetInt("currentHighlighter")].gameObject);
        }
    }
    private void OnEnable()
    {
        if (defaultButton != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton);
        }
    }
    void Update()
    {
        var selectedObj = EventSystem.current.currentSelectedGameObject;
        //print(selectedObj);
        if (selectedObj == null) return;
        var selectedAsButton = selectedObj.GetComponent<Button>();
        if (selectedAsButton != null && selectedAsButton != previousButton)
        {
            if (selectedAsButton.transform.name != "PauseButton")
                HighlightButton(selectedAsButton);
        }
        if (previousButton != null && previousButton != selectedAsButton)
        {
            UnHighlightButton(previousButton);
        }
        previousButton = selectedAsButton;


        //MenuScript.instance.SelectedObj = selectedAsButton.gameObject;
        //if (levelSelection)
        //{
        //    MenuScript.instance.Content_move();
        //}
    }

    public static GameObject FindGameObjectInChildWithTag(GameObject parent, string tag)
    {
        Transform t = parent.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).gameObject.tag == tag)
            {
                return t.GetChild(i).gameObject;
            }
        }
        return null;
    }
    void HighlightButton(Button butt)
    {
        butt.transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
        if (butt.transform.tag == "Giveborder")
        {
            FindGameObjectInChildWithTag(butt.gameObject, "border").SetActive(true);
        }
        if (butt.GetComponent<Outline>() != null)
        {
            butt.GetComponent<Outline>().enabled = true;
        }
    }
    void UnHighlightButton(Button butt)
    {
        butt.transform.localScale = new Vector3(1, 1, 1);
        if (butt.transform.tag == "Giveborder")
        {
            FindGameObjectInChildWithTag(butt.gameObject, "border").SetActive(false);
        }
        if (butt.GetComponent<Outline>() != null)
        {
            butt.GetComponent<Outline>().enabled = false;
        }
    }
    public void setHigleter(GameObject defaultButton)
    {
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }
    public void SetDefaultButton(GameObject _defaultButton)
    {
        defaultButton = _defaultButton;
    }

    //default highlight for levels selection
    public void DefaultHighlighter_Levels()
    {
        EventSystem.current.SetSelectedGameObject(levels[PlayerPrefs.GetInt("currentHighlighter")].gameObject);
    }
}