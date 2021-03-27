using System.Collections.Generic;
using UnityEngine;

public class Motion : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private bool rotate;
    [SerializeField] private Transform prefab;

    private List<GameObject> OriCirclePrefab = new List<GameObject>();
    private List<int> saveint = new List<int>();
    private List<int> falsesaveint = new List<int>();

    private GameObject[] ChangeCirclePrefab;

    private Vector3[] childForDirections;
    private Vector3[] direct;
    private Vector3 SelectDirect;

    private RaycastHit hit;

    private bool[] CollisionControl;

    private float[] distance;
    private float time;
    private float rotationleft = 90;
    private float rotationspeed = 100;

    private int save;
    private int a = -1;
    private int changeCirclePrefabCount;

    public void InstantiateInCircle(GameObject prefab, Vector3 location, bool yRot, int howMany, float radius, float yPosition)
    {
        float angleSection = Mathf.PI * 2f / howMany;
        for (int i = 0; i < howMany; i++)
        {
            float angle = i * angleSection;

            Vector3 rot;

            if (yRot)
                rot = new Vector3(0, Mathf.Cos(angle), -1 * Mathf.Sin(angle));
            else
                rot = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            Vector3 newPos = location + rot * radius;
            // newPos.z = 0;
            GameObject test = Instantiate(prefab, newPos, prefab.transform.rotation, parent.transform);
            if (yRot)
            {
                test.name = (i + 8).ToString();
            }
            else
                test.name = i.ToString();

            OriCirclePrefab.Add(test);
        }
    }
    private void CirclePrefabChange(int howMany)
    {
        ChangeCirclePrefab = new GameObject[changeCirclePrefabCount];
        for (int i = 0; i < howMany; i++)
        {
            if (i == 0 || i > 3 && i < 13)
            {
                a++;
                ChangeCirclePrefab[a] = OriCirclePrefab[i];
            }
        }
    }
    private void directanddistance()
    {
        for (int i = 0; i < changeCirclePrefabCount; i++)
        {
            distance[i] = (transform.position - ChangeCirclePrefab[i].transform.position).magnitude;
            direct[i] = (transform.position - ChangeCirclePrefab[i].transform.position).normalized;
        }
    }
    private void RayCast()
    {
        for (int i = 0; i < changeCirclePrefabCount; i++)
        {

            if (Physics.Raycast(transform.position, direct[i], out hit, distance[i]))
            {
                CollisionControl[i] = true;
                Debug.DrawRay(transform.position, direct[i] * distance[i], Color.yellow);
            }
            else
            {
                CollisionControl[i] = false;
                Debug.DrawRay(transform.position, direct[i] * distance[i], Color.red);
            }
        }
    }
    private void Movement()
    {
        time += Time.deltaTime;
        for (int i = 0; i < changeCirclePrefabCount; i++)
        {
            if (CollisionControl[i] == false)
            {
                saveint.Add(i);
            }

            if (CollisionControl[2])
                rotate = true;

            if (rotate == false)
            {
                if (i == changeCirclePrefabCount - 1 && time > 1 || CollisionControl[save] == true)
                {
                    int random = Random.Range(0, saveint.Count);
                    if (random < saveint.Count)
                    {
                        if (CollisionControl[saveint[random]] == false)
                        {
                            SelectDirect = direct[saveint[random]];
                            save = saveint[random];
                            saveint.Clear();
                            time = 0;
                            random = 0;
                        }
                        else
                            i--;

                    }
                    else
                        i--;
                }
            }
        }
        if (rotate == false)
        {
            transform.position += SelectDirect * Time.deltaTime * 1;
            Charachter();
        }

    }
    private void OnceRotate()
    {
        float rotation = rotationspeed * Time.deltaTime;
        if (rotationleft > rotation)
        {
            rotationleft -= rotation;
        }
        else
        {
            rotate = false;
            rotationleft = 90;
            rotationspeed = 100;
        }
        transform.Rotate(0, rotation, 0);
    }
    private void CharachterRotateControl()
    {
        if (rotate)
        {
            OnceRotate();
            ChildAssignedRotate();
            directanddistance();
        }
    }
    private void Charachter()
    {

        Vector3 relativePos = (direct[save] * 10) - transform.position;
        relativePos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        Quaternion rot = Quaternion.Lerp(transform.rotation, rotation, 5 * Time.deltaTime);
        transform.rotation = rot;

    }
    private void ChildAssignedRotate()
    {

        for (int i = 0; i < changeCirclePrefabCount; i++)
        {
            Quaternion rot = Quaternion.AngleAxis(180 - transform.eulerAngles.y, Vector3.up * -1);
            ChangeCirclePrefab[i].transform.position = transform.position + rot * childForDirections[i];
        }
    }
    private void DirectForChildAssignedRotate()
    {
        childForDirections = new Vector3[changeCirclePrefabCount];
        // determine direction only once, to avoid infinite rotation.
        for (int i = 0; i < changeCirclePrefabCount; i++)
        {
            childForDirections[i] = transform.position - ChangeCirclePrefab[i].transform.position;
        }
    }
    private void Awake()
    {
        changeCirclePrefabCount = 2 * ((8 / 2) + 1);

        CollisionControl = new bool[changeCirclePrefabCount];
        distance = new float[changeCirclePrefabCount];
        direct = new Vector3[changeCirclePrefabCount];

        InstantiateInCircle(prefab.gameObject, transform.position, false, 8, 1, transform.position.y);
        InstantiateInCircle(prefab.gameObject, transform.position, true, 8, 1, transform.position.y);
        CirclePrefabChange(16);
        directanddistance();
        DirectForChildAssignedRotate();
    }
    private void Update()
    {
        RayCast();
        Movement();
        CharachterRotateControl();
    }
}
