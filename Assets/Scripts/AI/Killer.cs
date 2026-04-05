using System.Collections;
using System.Xml.Linq;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Killer : MonoBehaviour
{
    public BehaviorGraphAgent agent;

    public float scareDuration = 0.5f;

    private bool playerDead = false;

    private void OnTriggerEnter(Collider other)
    {
        if (UIManager.Instance.gameEnded) return;

        if (other.CompareTag("Player"))
        {
            if (playerDead) return;
            playerDead = true;

            StartCoroutine(ScareAndDie());
        }
    }

    private IEnumerator ScareAndDie()
    {
        GameObject jumpscareImage = GameObject.Find("Player/Canvas/Jumpscare");
        AudioSource jumpscareAudio = GameObject.Find("scareAudio").GetComponent<AudioSource>();

        PlayerController.Instance.freeze = true;
        if (jumpscareImage != null) jumpscareImage.SetActive(true);

        if (jumpscareAudio != null && jumpscareAudio != null) jumpscareAudio.Play();

        yield return new WaitForSeconds(2f);

        if (jumpscareImage != null) jumpscareImage.SetActive(false);

        UIManager.Instance.ShowDeathScreen();
    }

    public IEnumerator NoticePlayer(float duration)
    {
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(0f, 321f, 0f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.rotation = targetRot;
    }
}
