using UnityEngine;
using TMPro;

public class ShowTextOnTrigger : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;  // Reference to the TMP text

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialText.gameObject.SetActive(false);
        }
    }
}
