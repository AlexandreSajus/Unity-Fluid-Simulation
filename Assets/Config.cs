using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    // Simulation parameters
    public static int N = 20; // Number of particles
    public static float SIM_W = 0.5f;  // Simulation space width
    public static float BOTTOM = -2f;  // Simulation space ground
    public static float DAM = -0.3f;  // Position of the dam, simulation space is between -0.5 and 0.5
    public static int DAM_BREAK = 200; // Number of frames before the dam breaks
    public static float DT = 20f;  // Time step
    public static float WALL_POS = 0.08f; // Position adjustment for particles that are too close to the walls

    // Physics parameters
    public static float G = 0.02f * 0.25f;  // Acceleration of gravity
    public static float SPACING = 0.08f;  // Spacing between particles, used to calculate pressure
    public static float K = SPACING / 1000.0f;  // Pressure factor
    public static float K_NEAR = K * 10f;  // Near pressure factor, pressure when particles are close to each other
    // Default density, will be compared to local density to calculate pressure
    public static float REST_DENSITY = 3.0f;
    // Neighbour radius, if the distance between two particles is less than R, they are neighbours
    public static float R = SPACING * 1.25f;
    public static float SIGMA = 0.2f;  // Viscosity factor
    public static float MAX_VEL = 0.25f;  // Maximum velocity of particles, used to avoid instability
    // Wall constraints factor, how much the particle is pushed away from the simulation walls
    public static float WALL_DAMP = 0.2f;
    public static float VEL_DAMP = 0.5f;  // Velocity reduction factor when particles are going above MAX_VEL
}
