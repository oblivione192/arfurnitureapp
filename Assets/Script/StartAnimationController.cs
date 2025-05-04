using UnityEngine;

public class StartAnimationController : MonoBehaviour
{
    public GameObject animationPanel;

    public void DisablePanel()
    {
        animationPanel.SetActive(false); 

    }
}
