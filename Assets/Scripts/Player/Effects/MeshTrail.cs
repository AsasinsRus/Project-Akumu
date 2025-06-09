using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public Transform positionToSpawn;
    public Material material;

    public float activeTime = 2f;
    private float activeTimer = 0;

    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 2f;
    public bool isTrailActive = false;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateTrail()
    {
        isTrailActive = true;
        StartCoroutine(ActivateTrail(activeTime));
    }

    private IEnumerator ActivateTrail(float activeTime)
    {
        activeTimer = activeTime;

        while(activeTimer > 0)
        {
            activeTimer -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.layer = 13;
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer meshRenderer = gObj.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);
                
                meshFilter.mesh = mesh;
                meshRenderer.material = material;

                Destroy(gObj, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
    }
}
