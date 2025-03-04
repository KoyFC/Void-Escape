using UnityEngine;
using System.Collections;
using System;

public class PlayerMovementScript : MonoBehaviour
{
    private PlayerController m_PlayerController = null;

    private bool m_IsAlive = true;
    private int m_CurrentIndex = 0;
    private int m_PreviousIndex = 0;
    [HideInInspector] public bool m_IsMoving = false;

    #region Main Methods
    private void Start()
    {
        m_PlayerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        PlayerInputScript.Instance.OnMovementPressed += HandleMovement;
#elif UNITY_ANDROID
        PlayerInputScript.Instance.OnMovementPressed += HandleMovementAccelerometer;
#else
        PlayerInputScript.Instance.OnMovementPressed += HandleMovement;
#endif
        InGameManager.Instance.OnPerspectiveChanged += ResetPosition;
        InGameUIManager.OnConfidenceDepleted += HandleGameOver;
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        PlayerInputScript.Instance.OnMovementPressed -= HandleMovement;
#elif UNITY_ANDROID
        PlayerInputScript.Instance.OnMovementPressed -= HandleMovementAccelerometer;
#else
        PlayerInputScript.Instance.OnMovementPressed -= HandleMovement;
#endif
        InGameManager.Instance.OnPerspectiveChanged -= ResetPosition;
        InGameUIManager.OnConfidenceDepleted -= HandleGameOver;
    }
#endregion

    #region Handling Methods
    private void HandleMovement()
    {
        if (m_IsMoving) return;

        m_PreviousIndex = m_CurrentIndex;

        if (PlayerInputScript.Instance.m_NextPressed && m_CurrentIndex < 1)
        {
            m_CurrentIndex++;
        }
        if (PlayerInputScript.Instance.m_PreviousPressed && m_CurrentIndex > -1)
        {
            m_CurrentIndex--;
        }

        // If we try to move to the same position (if we're already at the extremes) we don't continue.
        // If we did continue, player movement would get disabled for the duration of the movement,
        // even if we didn't physically move. Anyway, just leave this line here.
        if (m_PreviousIndex == m_CurrentIndex) return; 

        StopAllCoroutines();

        Vector3 newPosition;
        Quaternion newRotation;
        int indexDifference = m_PreviousIndex - m_CurrentIndex; // Determining the multiplier that we apply to the rotation

        if (InGameManager.Instance.m_IsHorizontal)
        {
            newPosition = UpdatePlayerPositionHorizontal();
            newRotation = Quaternion.Euler(0, 0, indexDifference * 45);
        }
        else
        {
            newPosition = UpdatePlayerPositionVertical();
            newRotation = Quaternion.Euler(-indexDifference * 30, 0, 0);
        }

        StartCoroutine(LerpToPosition(newPosition, true));
        StartCoroutine(LerpRotation(newRotation));
    }

    private void HandleMovementAccelerometer()
    {
        if (m_IsMoving) return;

        m_PreviousIndex = m_CurrentIndex;

        Vector3 accelerometer = PlayerInputScript.Instance.m_Accelerometer;
        float sensitivity = 0.25f;

        // Determining the new index based on the accelerometer input
        if (accelerometer.x > sensitivity)
        {
            m_CurrentIndex = 1;
        }
        else if (accelerometer.x < -sensitivity)
        {
            m_CurrentIndex = -1;
        }
        else
        {
            m_CurrentIndex = 0;
        }

        // If we try to move to the same position (if we're already at the extremes) we don't continue.
        if (m_PreviousIndex == m_CurrentIndex) return;

        StopAllCoroutines();

        Vector3 newPosition;
        Quaternion newRotation;
        int indexDifference = m_PreviousIndex - m_CurrentIndex;

        if (InGameManager.Instance.m_IsHorizontal)
        {
            newPosition = UpdatePlayerPositionHorizontal();
            newRotation = Quaternion.Euler(0, 0, indexDifference * 45);
        }
        else
        {
            newPosition = UpdatePlayerPositionVertical();
            newRotation = Quaternion.Euler(-indexDifference * 30, 0, 0);
        }

        StartCoroutine(LerpToPosition(newPosition, true));
        StartCoroutine(LerpRotation(newRotation));
    }


    private void HandleGameOver()
    {
        m_IsAlive = false;
        StartCoroutine(LerpToPosition(PointManager.Instance.m_GameOverPoint.position, false, 3f));
        StartCoroutine(LerpScale(Vector3.zero));
    }
    #endregion

    #region Helper Methods
    private void ResetPosition()
    {
        m_CurrentIndex = 0;
        Vector3 centerPoint = PointManager.Instance.m_PlayerPoints.CenterPoint.position;
        StartCoroutine(LerpToPosition(centerPoint, false));
    }

    private Vector3 UpdatePlayerPositionHorizontal()
    {
        Vector3 newPosition = new Vector3(0, 0, 10);

        switch (m_CurrentIndex)
        {
            case -1:
                newPosition.x = PointManager.Instance.m_PlayerPoints.LeftPoint.position.x;
                break;
            case 0:
                newPosition.x = PointManager.Instance.m_PlayerPoints.CenterPoint.position.x;
                break;
            case 1:
                newPosition.x = PointManager.Instance.m_PlayerPoints.RightPoint.position.x;
                break;
        }

        return newPosition;
    }

    private Vector3 UpdatePlayerPositionVertical()
    {
        Vector3 newPosition = new Vector3(0, 0, 10);

        switch (m_CurrentIndex)
        {
            case 1:
                newPosition.y = PointManager.Instance.m_PlayerPoints.BottomPoint.position.y;
                break;
            case 0:
                newPosition.y = PointManager.Instance.m_PlayerPoints.CenterPoint.position.y;
                break;
            case -1:
                newPosition.y = PointManager.Instance.m_PlayerPoints.TopPoint.position.y;
                break;
        }

        return newPosition;
    }

    private IEnumerator LerpToPosition(Vector3 targetPosition, bool blockMovement, float duration = 0)
    {
        if (blockMovement) m_IsMoving = true;

        float elapsedTime = 0f;
        // If the duration is the default value, we use the lerp duration from the InGameManager.
        duration = (duration == 0) ? InGameManager.Instance.m_MovementLerpDuration : duration;
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            // If the player gets hit while moving, we stop the movement and return to the previous position when done
            if (m_PlayerController.m_PlayerHealth.m_Hit && m_IsAlive)
            {
                while (m_PlayerController.m_PlayerHealth.m_Hit) yield return null;

                m_CurrentIndex = m_PreviousIndex;
                targetPosition = startPosition;
                startPosition = transform.position;

                elapsedTime = 0f;
                duration /= 3f;
            }

            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            elapsedTime /= Time.timeScale;
            yield return null;
        }

        transform.position = targetPosition;

        if (blockMovement) m_IsMoving = false;
    }

    private IEnumerator LerpRotation(Quaternion targetRotation)
    {
        float elapsedTime = 0;
        float duration = InGameManager.Instance.m_RotationLerpDuration;
        Quaternion originalRotation = transform.rotation;

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            elapsedTime /= Time.timeScale;
            yield return null;
        }
        transform.rotation = targetRotation;

        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(targetRotation, Quaternion.identity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            elapsedTime /= Time.timeScale;
            yield return null;
        }
        transform.rotation = Quaternion.identity;
    }

    private IEnumerator LerpScale(Vector3 targetScale, float duration = 3f)
    {
        float elapsedTime = 0;
        Vector3 originalScale = transform.localScale;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            elapsedTime /= Time.timeScale;
            yield return null;
        }
    }
    #endregion
}

