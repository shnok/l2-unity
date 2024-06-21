using UnityEngine;
using System.Collections;

public class BoatSway : MonoBehaviour {
    public float swayRangeDegreesX = 1f;
    public float swaySpeedX = 0.3f;
    public float swayAngleX = 0;
    public float pauseSwayXDuration = 0.3f;
    private bool swayDirectionRightX = true;
    private bool pauseSwayX = false;

    public float swayRangeDegreesZ = 2f;
    public float swaySpeedZ = 0.7f;
    public float swayAngleZ = 0;
    public float pauseSwayZDuration = 0.5f;
    private bool swayDirectionRightZ = true;
    private bool pauseSwayZ = false;

    private void Start() {
        swayDirectionRightX = Random.Range(0f, 1f) > 0.5f;
        swayDirectionRightZ = Random.Range(0f, 1f) > 0.5f;
        swayAngleX = Random.Range(-swayRangeDegreesX, swayRangeDegreesX);
        swayAngleZ = Random.Range(-swayRangeDegreesZ, swayRangeDegreesZ);
        transform.eulerAngles = new Vector3(swayAngleX, transform.eulerAngles.y, swayAngleZ);
    }

    void Update() {
        if (!pauseSwayX) {
            if (swayAngleX > swayRangeDegreesX) {
                swayDirectionRightX = false;
                StartCoroutine(PauseSwayX());
            } else if (swayAngleX < -swayRangeDegreesX) {
                swayDirectionRightX = true;
                StartCoroutine(PauseSwayX());
            }

            if (swayDirectionRightX) {
                swayAngleX += Time.deltaTime * swaySpeedX;
            } else {
                swayAngleX -= Time.deltaTime * swaySpeedX;
            }
        }

        if (!pauseSwayZ) {
            if (swayAngleZ > swayRangeDegreesZ) {
                swayDirectionRightZ = false;
                StartCoroutine(PauseSwayZ());
            } else if (swayAngleZ < -swayRangeDegreesZ) {
                swayDirectionRightZ = true;
                StartCoroutine(PauseSwayZ());
            }

            if (swayDirectionRightZ) {
                swayAngleZ += Time.deltaTime * swaySpeedZ;
            } else {
                swayAngleZ -= Time.deltaTime * swaySpeedZ;
            }
        }

        transform.eulerAngles = new Vector3(swayAngleX, transform.eulerAngles.y, swayAngleZ);
    }

    IEnumerator PauseSwayX() {
        pauseSwayX = true;
        yield return new WaitForSeconds(pauseSwayXDuration);
        pauseSwayX = false;
    }

    IEnumerator PauseSwayZ() {
        pauseSwayZ = true;
        yield return new WaitForSeconds(pauseSwayZDuration);
        pauseSwayZ = false;
    }
}
