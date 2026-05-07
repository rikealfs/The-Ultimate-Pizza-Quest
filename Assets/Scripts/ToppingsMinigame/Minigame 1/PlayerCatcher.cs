using UnityEngine;
using System.Collections;

public class PlayerCatcher : MonoBehaviour
{
    public bool controlsInverted = false;
    public float speed = 10f;
    public float xLimit = 8f;
    float buttonMove = 0f;

    void Update()
{
    float move = Input.GetAxis("Horizontal") + buttonMove;

    if (controlsInverted)
    {
        move *= -1;
    }

    transform.Translate(Vector3.right * move * speed * Time.deltaTime);

    float clampedX = Mathf.Clamp(transform.position.x, -xLimit, xLimit);
    transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
}

    public void MoveLeftDown()
{
    buttonMove = -1f;
}

public void MoveRightDown()
{
    buttonMove = 1f;
}

public void StopMove()
{
    buttonMove = 0f;
}


    public void InvertControls(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(InvertRoutine(duration));
    }

    IEnumerator InvertRoutine(float duration)
    {
        controlsInverted = true;

        yield return new WaitForSeconds(duration);

        controlsInverted = false;
    }
}