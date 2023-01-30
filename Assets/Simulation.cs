using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using list = System.Collections.Generic.List<Particle>;
using vector2 = UnityEngine.Vector2;

using static Config;

public class Simulation : MonoBehaviour
{
    public list particles = new list();

    // Import simulation variables from Config.cs
    public static int N = Config.N;
    public static float SIM_W = Config.SIM_W;
    public static float BOTTOM = Config.BOTTOM;
    public static float DAM = Config.DAM;
    public static int DAM_BREAK = Config.DAM_BREAK;
    public static float G = Config.G;
    public static float SPACING = Config.SPACING;
    public static float K = Config.K;
    public static float K_NEAR = Config.K_NEAR;
    public static float REST_DENSITY = Config.REST_DENSITY;
    public static float R = Config.R;
    public static float SIGMA = Config.SIGMA;
    public static float MAX_VEL = Config.MAX_VEL;
    public static float WALL_DAMP = Config.WALL_DAMP;
    public static float VEL_DAMP = Config.VEL_DAMP;
    public static float DT = Config.DT;
    public static float WALL_POS = Config.WALL_POS;

    // Get Base_Particle object from Scene
    public GameObject Base_Particle;
    
    void Start() {
        Base_Particle = GameObject.Find("Base_Particle");


        /* 
        for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++) {
                // Create a new particle
                GameObject new_particle = Instantiate(Base_Particle, new Vector3(i * SPACING, j * SPACING, 0), Quaternion.identity);
                // Set as child of the Simulation object
                new_particle.transform.parent = gameObject.transform;
                
            }
        }
        */
    }

    private float density;
    private float density_near;
    private float dist;
    private float distance;
    private float normal_distance;
    private float relative_distance;
    private float total_pressure;
    private float velocity_difference;
    private vector2 pressure_force;
    private vector2 particule_to_neighbor;
    private vector2 pressure_vector;
    private vector2 normal_p_to_n;
    private vector2 viscosity_force;

    public void calculate_density(list particles) {
        /*
            Calculates density of particles
            Density is calculated by summing the relative distance of neighboring particles
            We distinguish density and near density to avoid particles to collide with each other
            which creates instability

        Args:
            particles (list[Particle]): list of particles
        */

        // For each particle
        foreach (Particle p in particles) {
            density = 0.0f;
            density_near = 0.0f;

            // For each neighbor
            foreach (Particle n in particles) {
                // Calculate distance between particles
                dist = Vector2.Distance(p.pos, n.pos);

                if (dist < R) {
                    normal_distance = 1 - dist / R;
                    p.rho += normal_distance * normal_distance;
                    p.rho_near += normal_distance * normal_distance * normal_distance;
                    n.rho += normal_distance * normal_distance;
                    n.rho_near += normal_distance * normal_distance * normal_distance;
                    // Add n to p's neighbors
                    p.neighbours.Add(n);
                }
            }
            p.rho += density;
            p.rho_near += density_near;
        }
    }

    public void create_pressure(list particles) {
        /*
            Calculates pressure force of particles
            Neighbors list and pressure have already been calculated by calculate_density
            We calculate the pressure force by summing the pressure force of each neighbor
            and apply it in the direction of the neighbor

        Args:
            particles (list[Particle]): list of particles
        */

        foreach (Particle p in particles) {
            pressure_force = vector2.zero;

            foreach (Particle n in p.neighbours) {
                particule_to_neighbor = n.pos - p.pos;
                distance = Vector2.Distance(p.pos, n.pos);

                normal_distance = 1 - distance / R;
                total_pressure = (p.press + n.press) * normal_distance * normal_distance + (p.press_near + n.press_near) * normal_distance * normal_distance * normal_distance;
                pressure_vector = total_pressure * particule_to_neighbor.normalized;
                n.force += pressure_vector;
                pressure_force += pressure_vector;
                }
            p.force -= pressure_force;
            }
        }

    public void calculate_viscosity(list particles) {
        /*
        Calculates the viscosity force of particles
        Force = (relative distance of particles)*(viscosity weight)*(velocity difference of particles)
        Velocity difference is calculated on the vector between the particles

        Args:
            particles (list[Particle]): list of particles
        */
        foreach (Particle p in particles) {
            foreach (Particle n in p.neighbours) {
                particule_to_neighbor = n.pos - p.pos;
                distance = Vector2.Distance(p.pos, n.pos);
                normal_p_to_n = particule_to_neighbor.normalized;
                relative_distance = distance / R;
                velocity_difference  = Vector2.Dot(p.vel - n.vel, normal_p_to_n);
                if (velocity_difference > 0) {
                    viscosity_force = (1 - relative_distance) * velocity_difference * SIGMA * normal_p_to_n;
                    p.vel -= viscosity_force*0.5f;
                    n.vel += viscosity_force*0.5f;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Add children GameObjects to particles list
        particles.Clear();
        foreach (Transform child in transform)
        {
            particles.Add(child.GetComponent<Particle>());
        }

        foreach (Particle p in particles)
        {
            p.UpdateState();
        }

        calculate_density(particles);

        // For each particle
        foreach (Particle p in particles)
        {
            p.CalculatePressure();
        }

        create_pressure(particles);

        calculate_viscosity(particles);
    }
}
