using UnityEngine;

public class EndlessCity : MonoBehaviour
{

    [SerializeField] Transform playerCarTransform;
    [SerializeField] Transform otherCityTransform;
    [SerializeField] float halfLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerCarTransform.position.z>transform.position.z+halfLength+40f)
        {
            transform.position = new Vector3(0,0,otherCityTransform.position.z+halfLength*2); // new position assign
        }
    }
}
