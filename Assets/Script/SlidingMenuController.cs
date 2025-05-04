using UnityEngine;
using System.Collections;

public class SlidingMenuController : MonoBehaviour
{
    public RectTransform menuPanel;
    public GameObject menuToggleButton;

    public float slideDuration = 0.3f;
    public float hiddenY = -400f;
    public float visibleY = 0f;

    private bool isMenuVisible = false;
    private Coroutine currentCoroutine;

    private Vector2 startTouchPos;
    private bool swipeDetected = false;
    private float minSwipeDistance = 100f; // pixels

    private float screenHeight;
    private float bottomSwipeThreshold;

    private void Start()
    {
        if (isMenuVisible)
            HideMenu();
        screenHeight = Screen.height;
        bottomSwipeThreshold = screenHeight * 0.2f; // bottom 20%
        menuPanel.anchoredPosition = new Vector2(0, hiddenY);
        menuPanel.gameObject.SetActive(false);
        StartCoroutine (StartWait());
    }

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPos = touch.position;

                    // Restrict to bottom screen only
                    if (startTouchPos.y <= bottomSwipeThreshold)
                    {
                        swipeDetected = true;
                    }
                    else
                    {
                        swipeDetected = false;
                    }
                    break;

                case TouchPhase.Ended:
                    if (!swipeDetected) return;

                    Vector2 endTouchPos = touch.position;
                    float swipeDeltaY = endTouchPos.y - startTouchPos.y;

                    if (Mathf.Abs(swipeDeltaY) > minSwipeDistance)
                    {
                        if (swipeDeltaY > 0 && !isMenuVisible)
                        {
                            ShowMenu();
                        }
                        else if (swipeDeltaY < 0 && isMenuVisible)
                        {
                            HideMenu();
                        }
                    }

                    swipeDetected = false;
                    break;
            }
        }
    }

    private IEnumerator StartWait()
    {
        menuToggleButton.SetActive(false);
        yield return new WaitForSeconds(5);
        menuToggleButton.SetActive(true);
    }

    public void ToggleMenu()
    {
        if (isMenuVisible)
            HideMenu();
        else
            ShowMenu();
    }

    public void ShowMenu()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        menuPanel.gameObject.SetActive(true);
        currentCoroutine = StartCoroutine(SlideMenu(visibleY, true));
        menuToggleButton.SetActive(false);
        isMenuVisible = true;
    }

    public void HideMenu()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(SlideMenu(hiddenY, false));
        menuToggleButton.SetActive(true);
        isMenuVisible = false;
    }

    private IEnumerator SlideMenu(float targetY, bool keepActive)
    {
        Vector2 start = menuPanel.anchoredPosition;
        Vector2 end = new Vector2(start.x, targetY);
        float time = 0;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = time / slideDuration;
            menuPanel.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }

        menuPanel.anchoredPosition = end;

        if (!keepActive)
            menuPanel.gameObject.SetActive(false);
    }
}
