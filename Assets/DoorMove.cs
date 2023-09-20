using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMove : MonoBehaviour
{
    public Transform m_Target;
    public Animator m_animation;
    public ObjectiveKillEnemies enemies = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colidiu: " + m_animation);
        Debug.Log("Enemies: " + enemies.notificationEnemiesRemainingThreshold);

        m_animation.SetBool("openDoor", true);
    }*/
}
