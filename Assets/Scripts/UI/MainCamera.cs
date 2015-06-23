using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {
    private bool changing_team = false;
    private float rotation;

    void Update() {
        if (changing_team) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotation, 0), Time.deltaTime * 5f);
        }
    }

    public void changeTeam(int team) {
        rotation = (team == -1) ? 0 : 180;
        changing_team = true;
    }
}
