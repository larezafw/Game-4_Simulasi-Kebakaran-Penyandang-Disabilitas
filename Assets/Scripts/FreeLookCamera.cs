using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    const string cutoff = "_Cutoff";
    const string doorChild = "Cube_10";
    const string boardChild = "Board Content";
    [SerializeField] LayerMask layer;
    [SerializeField] Transform player;
    List<GameObject> lastObstacle;
    bool isFreeLookMode;

    private void Start()
    {
        lastObstacle = new List<GameObject>();
        int mode = PlayerPrefs.GetInt(Keyword.TOGGLE_FREELOOK, 0);
        SetMode(mode);        
    }

    private void FixedUpdate()
    {
        if (!isFreeLookMode) return;

        List<GameObject> currentObstacle = new List<GameObject>();
        Vector3 targetPos = player.position - transform.position;

        Ray ray = new Ray(transform.position, targetPos.normalized);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, targetPos.magnitude, layer);
        for (int i = 0; i < hits.Length; i++)
        {
            GameObject obstacle = hits[i].transform.gameObject;
            if (lastObstacle.Contains(obstacle))
            {
                GameObject childObstacle = null;
                if (obstacle.transform.Find(doorChild) != null) childObstacle = obstacle.transform.Find(doorChild).gameObject;
                else if (obstacle.transform.Find(boardChild) != null) childObstacle = obstacle.transform.Find(boardChild).gameObject;

                // UPDATE LIST
                lastObstacle.Remove(obstacle);
                currentObstacle.Add(obstacle);
                if (childObstacle != null)
                {
                    lastObstacle.Remove(childObstacle);
                    currentObstacle.Add(childObstacle);
                }
            }
            else
            {
                GameObject childObstacle = null;
                if (obstacle.transform.Find(doorChild) != null) childObstacle = obstacle.transform.Find(doorChild).gameObject;
                else if(obstacle.transform.Find(boardChild) != null) childObstacle = obstacle.transform.Find(boardChild).gameObject;

                Renderer obstacleRenderer = obstacle.GetComponent<Renderer>();
                Renderer childObstacleRenderer = childObstacle?.GetComponent<Renderer>();
                if (obstacleRenderer != null) obstacleRenderer.material?.SetFloat(cutoff, 0.2f);
                if (childObstacleRenderer != null) childObstacleRenderer.material?.SetFloat(cutoff, 0.2f);

                // RENEW LIST
                currentObstacle.Add(obstacle);
                if (childObstacle != null) currentObstacle.Add(childObstacle);
            }
        }

        ResetLastObstacleList(currentObstacle);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * targetPos.magnitude, Color.green);
    }

    void ResetLastObstacleList(List<GameObject> currentObstacle)
    {
        foreach (GameObject obstacle in lastObstacle)
        {
            Renderer obstacleRenderer = obstacle.GetComponent<Renderer>();
            if (obstacleRenderer != null) obstacleRenderer.material?.SetFloat(cutoff, 0f);
        }
        lastObstacle = currentObstacle;
    }

    public void SetMode(int mode)
    {
        if (mode == 0) isFreeLookMode = false;
        else isFreeLookMode = true;

        ResetLastObstacleList(new List<GameObject>());
    }
}
