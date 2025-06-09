using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject _camera;
    [SerializeField] public float damping;
    [SerializeField] public Vector2 offset;

    private BoxCollider _collider;
    private bool playerOut = false;

    private Vector3 endPosition;

    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        endPosition = _camera.transform.position;
        _collider = GetComponent<BoxCollider>();
        PosOnPlayer();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(playerOut)
        { 
            endPosition = new Vector3(player.transform.position.x, player.transform.position.y, _camera.transform.position.z);
        }

        SmoothDamp();

    }

    private void PosOnPlayer()
    {
        transform.position = player.transform.position;
    }

    private void SmoothDamp()
    {
        _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position,
            endPosition + (Vector3)offset, 
            ref velocity, damping);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            playerOut = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerOut = false;
        }
    }
}
