using System.Collections;
using UnityEngine;
 
class DragAndDrop : MonoBehaviour {
    private bool dragging = false;
    private float distance;
 
    void OnMouseDown() {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }
 
    void OnMouseUp() {
        GetComponent<Rigidbody>().useGravity = true;
        dragging = false;
    }
 
    void Update() {
        if (dragging) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = new Vector3(rayPoint.x - 0.5f, 1f, rayPoint.z - 0.7f);
        }
    }
}