using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRubble : MonoBehaviour
{

    [SerializeField]
    private int rubblesAmount = 10;

    [SerializeField]
    private float startAngle = 90f, endAngle = 270f;

    private Vector2 rubbleMoveDirection;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Fire", 0f, 6f);
    }

    private void Fire()
    {
        float angleStep = (endAngle - startAngle) / rubblesAmount;
        float angle = startAngle;

        for (int i = 0; i<rubblesAmount + 1; i++)
        {
            float rubDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float rubDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 rubMoveVector = new Vector3(rubDirX, rubDirY, 0f);
            Vector2 rubDir = (rubMoveVector - transform.position).normalized;

            GameObject rub = RubblePool.rubblePoolInstance.GetRubble();
            rub.transform.position = transform.position;
            rub.transform.rotation = transform.rotation;
            rub.SetActive(true);
            rub.GetComponent<Rubble>().SetMoveDirection(rubDir);

            angle += angleStep;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
