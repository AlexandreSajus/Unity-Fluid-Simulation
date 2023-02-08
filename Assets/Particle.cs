using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using vector2 = UnityEngine.Vector2;
using list = System.Collections.Generic.List<Particle>;

using static Config;

public class Particle : MonoBehaviour
{
    /*
    A single particle of the simulated fluid
    Attributes:
        x_pos: x position of the particle
        y_pos: y position of the particle
        previous_x_pos: x position of the particle in the previous frame
        previous_y_pos: y position of the particle in the previous frame
        visual_x_pos: x position of the particle that is shown on the screen
        visual_y_pos: y position of the particle that is shown on the screen
        rho: density of the particle
        rho_near: near density of the particle, used to avoid collisions between particles
        press: pressure of the particle
        press_near: near pressure of the particle, used to avoid collisions between particles
        neighbors: list of the particle's neighbors
        x_vel: x velocity of the particle
        y_vel: y velocity of the particle
        x_force: x force applied to the particle
        y_force: y force applied to the particle
    */

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

    // Physics variables
    public vector2 pos;
    public vector2 previous_pos;
    public vector2 visual_pos;
    public float rho = 0.0f;
    public float rho_near = 0.0f;
    public float press = 0.0f;
    public float press_near = 0.0f;
    public list neighbours = new list();
    public vector2 vel = vector2.zero;
    public vector2 force = new vector2(0f, -G);
    public float velocity = 0.0f;

    // Spatial partitioning position in grid
    public int grid_x;
    public int grid_y;

    void Start()
    {
        // Set initial position
        pos = transform.position;
        previous_pos = pos;
        visual_pos = pos;
    }

    // Update is called once per frame
    public void UpdateState()
    {
        // Reset previous position
        previous_pos = pos;

        // Apply force using Newton's second law and Euler integration with mass = 1
        vel += force * Time.deltaTime * DT;

        // Move particle according to its velocity using Euler integration
        pos += vel * Time.deltaTime * DT;

        // Update visual position
        visual_pos = pos;
        transform.position = visual_pos;

        // Reset force
        force = new vector2(0, -G);

        // Define velocity using Euler integration
        vel = (pos - previous_pos) / Time.deltaTime / DT;

        // Calculate velocity
        velocity = vel.magnitude;

        // Set to MAX_VEL if velocity is greater than MAX_VEL
        if (velocity > MAX_VEL)
        {
            vel = vel.normalized * MAX_VEL;
        }

        // Reset density
        rho = 0.0f;
        rho_near = 0.0f;

        // Reset neighbors
        neighbours = new list();

        // If pos under BOTTOM, delete particle
        if (pos.y < BOTTOM)
        {
            // If name not Base_Particle, delete particle
            if (name != "Base_Particle")
            {
                Destroy(gameObject);
            }
        }
    }

    public void CalculatePressure()
    {
        press = K * (rho - REST_DENSITY);
        press_near = K_NEAR * rho_near;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Calculate the normal vector of the collision
        vector2 normal = collision.contacts[0].normal;

        // Calculate the velocity of the particle in the normal direction
        float vel_normal = Vector2.Dot(vel, normal);

        // If the velocity is positive, the particle is moving away from the wall
        if (vel_normal > 0)
        {
            return;
        }

        // Calculate the velocity of the particle in the tangent direction
        vector2 vel_tangent = vel - normal * vel_normal;

        // Calculate the new velocity of the particle
        vel = vel_tangent - normal * vel_normal * WALL_DAMP;

        // Move the particle out of the wall
        pos = collision.contacts[0].point + normal * WALL_POS;
    }

}
