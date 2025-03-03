using UnityEngine;
using System.Collections;

public class ObstaclePortalScript : MonoBehaviour
{
    [SerializeField] private float m_ChangeScaleDuration = 0.75f;
    private Vector3 m_OriginalScale = Vector3.one;

    private void Awake()
    {
        m_OriginalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        StartCoroutine(HandleObstacle());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator HandleObstacle()
    {
        yield return new WaitUntil(() => !InGameManager.Instance.m_ChangingPerspective);

        StartCoroutine(ChangeScale(Vector3.zero, m_OriginalScale));

        yield return new WaitForSeconds(m_ChangeScaleDuration);
        SpawnObstacle();

        yield return new WaitForSeconds(0.25f);
        StartCoroutine(ChangeScale(m_OriginalScale, Vector3.zero));

        yield return new WaitForSeconds(m_ChangeScaleDuration);
        ObstaclePoolManager.Instance.ReturnToPool(gameObject);
    }

    private void SpawnObstacle()
    {
        // I have many dependencies ;-; Anyway, 2/3 chance for an asteroid
        //bool spawnAsteroid = Random.Range(0, 3) < 2;
        bool spawnAsteroid = true;

        GameObject spawnObject = null;
        if (spawnAsteroid)
        {
            int asteroidIndex = Random.Range(0, ObstaclePoolManager.Instance.m_AsteroidPrefabs.Count);
            spawnObject = ObstaclePoolManager.Instance.GetAsteroid(asteroidIndex);

        }
        else
        {
            int itemIndex = Random.Range(0, ObstaclePoolManager.Instance.m_ItemPrefabs.Count);
            spawnObject = ObstaclePoolManager.Instance.GetItem(itemIndex);
        }

        spawnObject.transform.position = transform.position;
    }

    private IEnumerator ChangeScale(Vector3 initialScale, Vector3 finalScale)
    {
        transform.localScale = initialScale;

        float elapsedTime = 0f;

        while (elapsedTime < m_ChangeScaleDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / m_ChangeScaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = finalScale;
    }
}
