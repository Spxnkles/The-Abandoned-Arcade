using System.Collections;
using UnityEngine;

public class AIDoors : MonoBehaviour
{
    public float closeDelay = 2f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("door collided");
        Door door = other.GetComponent<Door>();
        if (door == null) return;

        if (!door.isOpen)
        {
            Debug.Log("Door is not null");
            door.isOpen = true;
            if (door.openAudio != null) door.audioSource.PlayOneShot(door.openAudio);
        }

        StopAllCoroutines();
        StartCoroutine(CloseAfterDelay(door));
    }

    private IEnumerator CloseAfterDelay(Door door)
    {
        yield return new WaitForSeconds(closeDelay);
        door.isOpen = false;
        if (door.closeAudio != null) door.audioSource.PlayOneShot(door.closeAudio);
    }
}
