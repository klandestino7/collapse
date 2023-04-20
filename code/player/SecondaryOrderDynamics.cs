using Sandbox;
using System;

namespace NxtStudio.Collapse;

public class SecondOrderDynamics
{
    public float PI = 3.1415926535897931f;

    private Vector3 xp;
    private Vector3 y, yd;

    private float k1, k2, k3;

    public SecondOrderDynamics(float f, float z, float r, Vector3 x0)
    {
        // compute constants 
        k1 = z / (PI * f);
        k2 = 1 / ((2 * PI * f) * (2 * PI * f));
        k3 = r * z / (2 * PI * f);

        xp = x0;
        y = x0;
        yd = 0;
    }

    public Vector3 Update(float T, Vector3 x, Vector3 xd)
    {
        if (xd == null)
        { // estimate veolcity
            xd = (x - xp) / T;
            xp = x;
        }
        float k2_stable = Math.Max(k2, 1.1f * (T*T/4 + T*k1/2)); // clamp k2 to guarantee stability
        y = y + T * yd;
        yd = yd + T * (x + k3*xd - y - k1*yd) / k2_stable;
        return y;
    }
}