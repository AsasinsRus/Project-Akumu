using EzySlice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicing : MonoBehaviour
{
    public GameObject slicePlane;
    public Material crossSectionMaterial = null;
    public LayerMask whatToSlice;
    public void Slice(Collider[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            SlicedHull hull = SliceObject(hits[i].gameObject, crossSectionMaterial);
            if (hull != null)
            {
                GameObject bottom = hull.CreateLowerHull(hits[i].gameObject, crossSectionMaterial);
                GameObject top = hull.CreateUpperHull(hits[i].gameObject, crossSectionMaterial);

                AddHullComponents(bottom);
                AddHullComponents(top);

                Destroy(hits[i].gameObject);
            }
        }
    }

    private SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial)
    {
        return obj.Slice(slicePlane.transform.position, slicePlane.transform.up, crossSectionMaterial);
    }

    private void AddHullComponents(GameObject obj)
    {
        obj.layer = 11;

        Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        MeshCollider collider = obj.AddComponent<MeshCollider>();
        collider.convex = true;

        rb.AddExplosionForce(100, obj.transform.position, 10);
    }
}
