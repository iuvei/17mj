using UnityEngine;
using System.Collections;

public class FruitDispenser : MonoBehaviour {

    public GameObject[] fruits;
    public GameObject bomb;

    public float z;

    public float powerScale;

    public bool pause = false;
    bool started = false;

    public float timer = 3f;

	void Start () {
	
	}
	
	void Update () {

        if (pause) return;

        timer -= Time.deltaTime;

        if (timer <= 0 && !started)
        {
            timer = 0f;
            started = true;
        }

        if (started)
        {
        	if (timer <= 0)
            {
            	FireUp();
                timer = 0.9f; //level
            }  
        }	
	}

    void FireUp()
    {
        if (pause) return;

        Spawn(false);


        if (Random.Range(0, 10) < 2)
        {
            Spawn(true);
        }
			
        if (Random.Range(0, 100) < 20)
        {
            Spawn(true);
        }

    }

    void Spawn(bool isBomb)
    {
        float x = Random.Range(-3.1f, 3.1f);

        z = Random.Range(14f, 19.8f);

        GameObject ins;

		if (!isBomb)
			ins = Instantiate (fruits [Random.Range (0, fruits.Length)], transform.position + new Vector3 (x, 0, z), Random.rotation) as GameObject;
		else {
			ins = Instantiate (bomb, transform.position + new Vector3 (x, 0, z), Random.rotation) as GameObject;
			MiniGames.Instance.PlayFruitBombSound ();
		}

        float power = Random.Range(1.5f, 1.8f) * -Physics.gravity.y * 1.5f * powerScale;
        Vector3 direction = new Vector3(-x * 0.05f * Random.Range(0.3f, 0.8f), 1, 0);
        direction.z = 0f;

        ins.GetComponent<Rigidbody>().velocity = direction * power;
        ins.GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * 0.1f, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
