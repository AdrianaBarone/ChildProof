using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject PopUpCanvas;
    public GameObject PauseCanvas;
    public GameObject CursorCanvas;
    public GameObject InfoArea;
    public GameObject InventoryCanvas;

    public static UIManager Instance;

    private Animator animator;

    [Header("Suoni UI")]
    public AudioClip InfoItem;
    public AudioClip SbloccoAchivement;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowUnlockAchievementPopup(Achievement achievement)
    {
        var titleText = PopUpCanvas.transform.Find("PanelPopUp/titleText").GetComponent<TMP_Text>();
        var descriptionText = PopUpCanvas.transform.Find("PanelPopUp/descriptionText").GetComponent<TMP_Text>();

        descriptionText.text = "Nuovo Achievement Sbloccato!";
        titleText.text = achievement.data.name;

        animator.SetTrigger("PopUp");
        AudioManager.Instance.PlaySound(SbloccoAchivement);
        //AudioManager.Instance.ChangeToSnapshot(1,3f)
    }


    public void ShowCompleteAchievementPopup(Achievement achievement)
    {
        var titleText = PopUpCanvas.transform.Find("PanelPopUp/titleText").GetComponent<TMP_Text>();
        var descriptionText = PopUpCanvas.transform.Find("PanelPopUp/descriptionText").GetComponent<TMP_Text>();

        descriptionText.text = "Nuovo Achievement Completato!";
        titleText.text = achievement.data.name;

        animator.SetTrigger("PopUp");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (InfoArea.activeSelf)
            {
                PlayerManager.Instance.TransitionToExploration();
                animator.SetTrigger("HideInfo");
                Time.timeScale = 1;
            }
        }

        if (InventoryCanvas.activeSelf)
        {
            // NOTE: probabilmente esiste un modo più efficiente per fare questa cosa
            for (int i = 0; i < 10; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    Item item = InventoryManager.Instance.GetItem(i);
                    if (item != null)
                    {
                        ShowInfo(item);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !PauseCanvas.activeSelf)
        {

            if (PlayerManager.Instance.InStatePhoneUp())
            {
                // TODO: close phone, transition to exploration


            }

            Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            InventoryCanvas.SetActive(false);
            CursorCanvas.SetActive(false);
            PauseCanvas.SetActive(true);

            Time.timeScale = 0;
            PlayerManager.Instance.PrepareTransition();
        }
    }

    public void ShowInventory(bool show)
    {
        InventoryCanvas.SetActive(show);
    }

    public void ExitGame()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ShowInfo(Item item)
    {
        PlayerManager.Instance.PrepareTransition();
        var itemNameText = InfoArea.transform.Find("InfoPanel/NamePanel/Name").GetComponent<TMP_Text>();
        var itemDescriptionText = InfoArea.transform.Find("InfoPanel/DescriptionPanel/Description").GetComponent<TMP_Text>();
        var itemImage = InfoArea.transform.Find("InfoPanel/NamePanel/Image").GetComponent<Image>();

        itemNameText.text = item.data.name;
        itemDescriptionText.text = item.data.description;
        itemImage.sprite = item.data.icon;

        animator.SetTrigger("ShowInfo");
        Time.timeScale = 0;
        AudioManager.Instance.PlaySound(InfoItem);
        //AudioManager.Instance.ChangeToSnapshot(1,3f)
    }
}
