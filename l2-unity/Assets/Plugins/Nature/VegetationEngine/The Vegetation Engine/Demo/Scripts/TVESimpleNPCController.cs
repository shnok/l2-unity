using UnityEngine;

namespace TheVegetationEngine
{
    public class TVESimpleNPCController : MonoBehaviour
    {
        private float timeToChangeDirection;
        Vector3 direction;

        public void Start()
        {
            ChangeDirection();
        }

        public void Update()
        {
            timeToChangeDirection -= Time.deltaTime;

            if (timeToChangeDirection <= 0)
            {
                ChangeDirection();
            }

            transform.Translate(direction, Space.World);
        }

        private void ChangeDirection()
        {
            var speed = Random.Range(0.005f, 0.01f);
            direction = new Vector3(Random.Range(-1f, 1f) * speed, 0, Random.Range(-1f, 1f) * speed);
            timeToChangeDirection = Random.Range(0.5f, 2f);
        }
    }
}