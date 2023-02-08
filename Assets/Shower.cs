using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using vector2 = UnityEngine.Vector2;

public class Shower : MonoBehaviour
{
    // Get the Simulation object
    public GameObject Simulation;
    // Get the Base_Particle object from Scene
    public GameObject Base_Particle;
    public Vector2 init_speed = new Vector2(1.0f, 0.0f);
    public float spawn_rate = 1f;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        Simulation = GameObject.Find("Simulation");
        Base_Particle = GameObject.Find("Base_Particle");
    }

    // Update is called once per frame
    void Update()
    {
        // Limit the number of particles
        if (Simulation.transform.childCount < 1000)
        {
            // Spawn particles at a constant rate
            time += Time.deltaTime;
            if (time < 1.0f / spawn_rate)
            {
                return;
            }
            // Create a new particle at the current position of the object
            GameObject new_particle = Instantiate(Base_Particle, transform.position, Quaternion.identity);

            // update the particle's position
            new_particle.GetComponent<Particle>().pos = transform.position;
            new_particle.GetComponent<Particle>().previous_pos = transform.position;
            new_particle.GetComponent<Particle>().visual_pos = transform.position;
            new_particle.GetComponent<Particle>().vel = init_speed;

            // Set as child of the Simulation object
            new_particle.transform.parent = Simulation.transform;

            // Reset time
            time = 0.0f;
        }
    }
}
