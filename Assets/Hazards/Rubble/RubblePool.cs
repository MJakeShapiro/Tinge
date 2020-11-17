using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubblePool : MonoBehaviour
{

    public static RubblePool rubblePoolInstance;

    [SerializeField]
    private GameObject pooledRubble;
    private bool notEnoughRubblesInPool = true;

    private List<GameObject> rubbles;
    // Start is called before the first frame update

    private void Awake()
    {
        rubblePoolInstance = this;
    }

    void Start()
    {
        rubbles = new List<GameObject>();
    }

    public GameObject GetRubble()
    {
        if(rubbles.Count > 0)
        {
            for (int i = 0; i<rubbles.Count; i++)
            {
                if (!rubbles[i].activeInHierarchy)
                {
                    return rubbles[i];
                }
            }
        }

        if (notEnoughRubblesInPool)
        {
            GameObject rub = Instantiate(pooledRubble);
            rub.SetActive(false);
            rubbles.Add(rub);
            return rub;
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
