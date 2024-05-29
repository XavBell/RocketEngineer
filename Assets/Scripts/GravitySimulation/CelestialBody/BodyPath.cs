using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class BodyPath : MonoBehaviour
{
    public GameObject OrbitingBody;
    public GameObject WorldSaveManager;
    public MasterManager MasterManager;
    public FloatingOrigin FloatingOrigin;
    public LineRenderer line;
    public SolarSystemManager solarSystemManager;
    public double G;
    public double gravityParam = 0;
    public double orbitalPeriod;

    public KeplerParams KeplerParams =  new KeplerParams();

    public bool updated;
    public bool calculate  = false;
    public bool start  = false;

    public TimeManager MyTime;
    public Rigidbody2D rb;
    public bool bypass = false;


    
    // Start is called before the first frame update
    void Start()
    {
        WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        FloatingOrigin = FindObjectOfType<FloatingOrigin>();
        if(MyTime == null)
        {
            MyTime = FindObjectOfType<TimeManager>();
        }
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(MasterManager == null)
        {
            GameObject MastRef = GameObject.FindGameObjectWithTag("MasterManager");
            if(MastRef)
            {
                MasterManager = MastRef.GetComponent<MasterManager>();
            }
        }

        if(MasterManager != null)
        {
            G = solarSystemManager.G;
            gravityParam = G*(this.GetComponent<PhysicsStats>().mass + OrbitingBody.GetComponent<PhysicsStats>().mass);

        }

        if(calculate == true && MyTime != null)
        {
            double time = MyTime.time;
            (double, double) bodyPosition2D = (this.GetComponent<DoubleTransform>().x_pos, this.GetComponent<DoubleTransform>().y_pos);
            Vector2 bodyVelocity2D = new Vector2((float)this.GetComponent<PhysicsStats>().x_vel, (float)this.GetComponent<PhysicsStats>().y_vel);
            DoubleTransform dT = OrbitingBody.GetComponent<DoubleTransform>();
            (double, double) orbitingBodyPosition2D = (dT.x_pos, dT.y_pos);
            DrawLine(time, line, KeplerParams, bodyPosition2D, bodyVelocity2D, orbitingBodyPosition2D, gravityParam);
            orbitalPeriod = GetOrbitalPeriod(gravityParam, KeplerParams.semiMajorAxis);
            calculate = false;
            start = true;
        }

    }

    public void UpdatePos()
    {
        if(start == true)
        {
            if(updated == false)
            {
                double x;
                double y;
                GetOrbitPositionKepler(gravityParam, MyTime.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x, out y);
                (double, double) orbtingBodyPos = (OrbitingBody.GetComponent<DoubleTransform>().x_pos, OrbitingBody.GetComponent<DoubleTransform>().y_pos);
                double xpos = x + orbtingBodyPos.Item1;
                double ypos = y + orbtingBodyPos.Item2;
                Vector3 transform2 = new Vector3((float)xpos, (float)ypos, 0);
                this.gameObject.transform.position = transform2;
                this.GetComponent<DoubleTransform>().x_pos = xpos;
                this.GetComponent<DoubleTransform>().y_pos = ypos;
                updated = true;
            }
            
            
        }
    }

    void DrawLine(double time, LineRenderer line, KeplerParams keplerParams, (double, double) rocketPosition2D, Vector2 rocketVelocity2D, (double, double) planetPosition2D, double gravityParam)
    {
        int numPoints = 1000;
        double[] times = new double[numPoints];
        Vector3[] positions = new Vector3[numPoints];

        if(true == true)
        {
            if(bypass == false)
            {
                SetKeplerParams(keplerParams, rocketPosition2D, planetPosition2D, rocketVelocity2D, gravityParam, (float)time);
            }
            if(rocketVelocity2D.magnitude != 0)
            {
                CalculatePoints(time, numPoints, gravityParam, planetPosition2D, keplerParams, ref times, ref positions);
                line.positionCount = numPoints;
                line.SetPositions(positions);
            }
        }  
    }

    void UpdateLine((double, double) rocketVelocity2D, double time, (double, double)planetPosition2D, KeplerParams keplerParams)
    {
        int numPoints = 1000;
        double[] times = new double[numPoints];
        Vector3[] positions = new Vector3[numPoints];
        if(Math.Sqrt(Math.Pow(rocketVelocity2D.Item1, 2) + Math.Pow(rocketVelocity2D.Item2, 2)) != 0)
        {
            CalculatePoints(time, numPoints, gravityParam, planetPosition2D, keplerParams, ref times, ref positions);
            line.positionCount = numPoints;
            List<Vector3> newPos = new List<Vector3>();
            foreach(Vector3 pos in positions)
            {
                newPos.Add(pos + OrbitingBody.transform.position/1_000_00);
            }
            line.SetPositions(newPos.ToArray());
        }
    }

    public void ReDraw()
    {
        double time = MyTime.time;
        (double, double) bodyPosition2D = (this.GetComponent<DoubleTransform>().x_pos, this.GetComponent<DoubleTransform>().y_pos);
        (double, double) bodyVelocity2D =(this.GetComponent<PhysicsStats>().x_vel, this.GetComponent<PhysicsStats>().y_vel);
        DoubleTransform dT = OrbitingBody.GetComponent<DoubleTransform>();
        UpdateLine(bodyVelocity2D, time, bodyPosition2D, KeplerParams);
    }

    /// <summary>
    /// Gives the position at time of the body in reference to the body it orbits
    /// </summary>
    /// <param name="Time"></param>
    /// <returns>Position of body at time</returns>
    public Vector3 GetPositionAtTime(double Time)
    {
        double x;
        double y;
        GetOrbitPositionKepler(gravityParam, Time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x, out y);
        return new Vector3((float)x, (float)y, 0);
    }

    public (double x, double y) GetPositionAtTimeDouble(double Time)
    {
        double x;
        double y;
        GetOrbitPositionKepler(gravityParam, Time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x, out y);
        return (x, y);
    }

    public Vector2 GetVelocityAtTime(double Time)
    {
        double VX;
        double VY;
        GetOrbitVelocityKepler(gravityParam, Time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out VX, out VY);
        return new Vector2((float)VX, (float)VY);
    }

    public static void GetOrbitPositionKepler(double gravityParam, double time, double semiMajorAxis, double eccentricity, double argPeriapsis, double LAN, double inclination, double trueAnomalyAtEpoch, out double X, out double Y)
    {
        double meanAngularMotion = Math.Sqrt(gravityParam / Math.Pow(semiMajorAxis, 3)); // TODO (Mean Angular Motion can be computed once)
        double timeWithOffset = time + GetTimeOffsetFromTrueAnomaly(trueAnomalyAtEpoch, meanAngularMotion, eccentricity); //Same for timeoffset
        double MA = timeWithOffset * meanAngularMotion;
        

        // Compute EA (Eccentric Anomaly)
        double EA = MA;
        
        for (int count = 0; count < 3; count++)
        {
            double dE = (EA - eccentricity * Math.Sin(EA) - MA) / (1 - eccentricity * Math.Cos(EA));
            EA -= dE;
            if (Math.Abs(dE) < 1e-6)
            {
                break;
            } 
        }

        // Compute TA (True Anomaly)
        double TA = 2 * Math.Atan(Math.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Math.Tan(EA / 2));

        // Compute r (radius)
        double r = semiMajorAxis * (1 - eccentricity * Math.Cos(EA));

        // Compute XYZ positions
        X = r * (Math.Cos(LAN) * Math.Cos(argPeriapsis + TA) - Math.Sin(LAN) * Math.Sin(argPeriapsis + TA) * Math.Cos(inclination));
        //double Z = r * (Math.Sin(LAN) * Math.Cos(argPeriapsis + TA) + Math.Cos(LAN) * Math.Sin(argPeriapsis + TA) * Math.Cos(inclination));
        Y = r * (Math.Sin(inclination) * Math.Sin(argPeriapsis + TA));
    }

    public static void GetOrbitVelocityKepler(double gravityParam, double time, double semiMajorAxis, double eccentricity, double argPeriapsis, double LAN, double inclination, double trueAnomalyAtEpoch, out double VX, out double VY)
    {
        // Compute MA (Mean Anomaly)
        // n = 2pi / T (T = time for one orbit)
        // M = n (t)
        double meanAngularMotion = Math.Sqrt(gravityParam / Math.Pow(semiMajorAxis, 3)); //As for position, can be calculated at runtime once
        double timeWithOffset = time + GetTimeOffsetFromTrueAnomaly(trueAnomalyAtEpoch, meanAngularMotion, eccentricity);
        double MA = timeWithOffset * meanAngularMotion;
        

        // Compute EA (Eccentric Anomaly)
        double EA = MA;
        

        for (int count = 0; count < 3; count++)
        {
            double dE = (EA - eccentricity * Math.Sin(EA) - MA) / (1 - eccentricity * Math.Cos(EA));
            EA -= dE;
            if (Math.Abs(dE) < 1e-6)
            {
                break;
            } 
        }

        // Compute TA (True Anomaly)
        double TA = 2 * Math.Atan(Math.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Math.Tan(EA / 2));

        // Compute r (radius)
        double r = semiMajorAxis * (1 - eccentricity * Math.Cos(EA));
        
        //Compute h and p for velocity
        double h = Math.Sqrt(gravityParam * semiMajorAxis * (1- Math.Pow(eccentricity, 2)));
        double p = semiMajorAxis*(1-Math.Pow(eccentricity,2));

        // Compute XYZ positions
        double X = r * (Math.Cos(LAN) * Math.Cos(argPeriapsis + TA) - Math.Sin(LAN) * Math.Sin(argPeriapsis + TA) * Math.Cos(inclination));
        //double Z = r * (Math.Sin(LAN) * Math.Cos(argPeriapsis + TA) + Math.Cos(LAN) * Math.Sin(argPeriapsis + TA) * Math.Cos(inclination));
        double Y = r * (Math.Sin(inclination) * Math.Sin(argPeriapsis + TA));

        VX = (X*h*eccentricity/(r*p))*Math.Sin(TA) - (h/r)*(Math.Cos(LAN)* Math.Sin(argPeriapsis+TA) + Math.Sin(LAN)*Math.Cos(argPeriapsis+TA)*Math.Cos(inclination));
        VY = (Y*h*eccentricity/(r*p))*Math.Sin(TA) + (h/r)*(Math.Cos(argPeriapsis+TA)*Math.Sin(inclination));
    }

    public static float Modulo(float x, float m)
    {
        return (x % m + m) % m;
    }

    public static double Modulo(double x, double m)
    {
        return (x % m + m) % m;
    }

    public static double GetOrbitalPeriod(double gravityParam, double semiMajorAxis)
    {
        return Math.Sqrt(4 * Math.Pow(Math.PI, 2) * Math.Pow(semiMajorAxis, 3) / gravityParam);
    }

    public static double GetTrueAnomalyFromTimeOffset(double timeOffset, double gravityParam, double semiMajorAxis, double eccentricity)
    {
        if (timeOffset < 0)
        {
            timeOffset += GetOrbitalPeriod(gravityParam, semiMajorAxis);
        }

        double meanAngularMotion = Math.Sqrt(gravityParam / Math.Pow(semiMajorAxis, 3));
        double MA = timeOffset * meanAngularMotion;

        double EA = MA;

        for (int count = 0; count < 10; count++)
        {
            double dE = (EA - eccentricity * Math.Sin(EA) - MA) / (1 - eccentricity * Math.Cos(EA));
            EA -= dE;
            if (Math.Abs(dE) < 1e-12)
            {
                break;
            } 
        }

        // Compute TA (True Anomaly)
        double TA = 2 * Math.Atan(Math.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Math.Tan(EA / 2));

        //Some corrections
        if (timeOffset > 0)
        {
            TA = 2 * Mathf.PI - TA;
        }

        TA = Modulo(TA, 2 * Mathf.PI);

        return TA;
    }

    public static double GetTimeOffsetFromTrueAnomaly(double trueAnomaly, double meanAngularMotion, double eccentricity)
    {
        // Offset by Mathf.Pi so 0 TA lines up with default start position from GetOrbitPositionKepler.
        // Wrap into -pi to +pi range.
        double TA_Clean = Modulo((trueAnomaly + Math.PI), (Math.PI * 2)) - Math.PI;
        double EA = Math.Acos((eccentricity + Math.Cos(TA_Clean)) / (1 + eccentricity * Math.Cos(TA_Clean)));
        if (TA_Clean < 0)
        {
            EA *= -1;
        }
        double MA = EA - eccentricity * Math.Sin(EA);
        double t = MA / meanAngularMotion;
        

        return t;
    }

    public static void KtoCfromC((double, double) rocketPosition2D, (double, double)planetPosition2D, Vector2 rocketVelocity2D, double gravityParam, double time, out double semiMajorAxis, out double eccentricity, out double argPeriapsis, out double LAN, out double inclination, out double timeToPeriapsis, out double trueAnomalyAtEpoch)
    {   
        //Calculate rocket position in 3D and transform it for Kepler
        (double rocketPosition3D_x, double rocketPosition3D_y, double rocketPosition3D_z) = (rocketPosition2D.Item1, 0, rocketPosition2D.Item2);
        (double planetPosition3D_x, double planetPosition3D_y, double planetPosition3D_z) = (planetPosition2D.Item1, 0, planetPosition2D.Item2);
        (double, double, double) rocketPosition3D = (rocketPosition3D_x, rocketPosition3D_y, rocketPosition3D_z);
        (double, double, double) planetPosition3D = (planetPosition3D_x, planetPosition3D_y, planetPosition3D_z);

        rocketPosition3D = (rocketPosition3D.Item1 - planetPosition3D.Item1, 0, rocketPosition3D.Item3 - planetPosition3D.Item3); //Assume planet at (0,0,0)

        //Calculate velocity
        UnityEngine.Vector3 rocketVelocity3D = new UnityEngine.Vector3(rocketVelocity2D.x, 0, rocketVelocity2D.y); //FLIP for Unity

        //Find position and velocity magnitude
        double r = Math.Sqrt(Math.Pow(rocketPosition3D.Item1, 2) + Math.Pow(rocketPosition3D.Item3, 2));
        double v = rocketVelocity3D.magnitude;

        //Calculate specific angular momentum
        // Decompose velocity3D into the same format as rocketPosition3D
        double vX = rocketVelocity3D.x;
        double vY = rocketVelocity3D.y;
        double vZ = rocketVelocity3D.z;
        (double, double, double) h_bar = (rocketPosition3D.Item2 * vZ - rocketPosition3D.Item3 * vY, rocketPosition3D.Item3 * vX - rocketPosition3D.Item1 * vZ, rocketPosition3D.Item1 * vY - rocketPosition3D.Item2 * vX);

        double h = Math.Sqrt(Math.Pow(h_bar.Item1, 2) + Math.Pow(h_bar.Item2, 2) + Math.Pow(h_bar.Item3, 2));

        //Compute specific energy
        double E = (0.5f * Math.Pow(v, 2)) - gravityParam/r;

        //Compute semi-major axis
        double a = -gravityParam/(2*E);

        //Compute eccentricity
        double e = Math.Sqrt(1 - Math.Pow(h,2)/(a*gravityParam));
      
        //Compute inclination
        double i = Math.Acos(h_bar.Item3 / h);

        //Compute right ascension of ascending node
        double omega_LAN = Math.Atan2(h_bar.Item1, -h_bar.Item2);

        //Compute argument of latitude v+w
        double lat = Math.Atan2((rocketPosition3D.Item3 / Math.Sin(i)), (rocketPosition3D.Item1 * Math.Cos(omega_LAN) + rocketPosition3D.Item2 * Math.Sin(omega_LAN)));

        // Compute true anomaly, v, (not actual true anomaly)
        double p = a * (1 - Math.Pow(e, 2));
        double dot = rocketPosition3D.Item1 * vX + rocketPosition3D.Item3 * vZ;
        double nu = Math.Atan2(Math.Sqrt(p / gravityParam) * dot, p - r);

        // Compute argument of periapse, w (not actual argperi)
        double omega_AP = lat - nu;

        // Compute eccentric anomaly, EA
        double EA = 2 * Math.Atan(Math.Sqrt((1 - e) / (1 + e)) * Math.Tan(nu / 2));

        // Compute the time of periapse passage, T
        double n = Math.Sqrt(gravityParam / Math.Pow(a, 3));
        double T = time - (1 / n) * (EA - e * Math.Sin(EA));

        double TA = GetTrueAnomalyFromTimeOffset(T, gravityParam, a, e);
        

        semiMajorAxis = a;
        eccentricity = e;
        argPeriapsis = Modulo(omega_AP, Mathf.PI*2);
        LAN = omega_LAN;
        inclination = i;
        timeToPeriapsis = T;
        trueAnomalyAtEpoch = TA;
    }

    public void SetKeplerParams(KeplerParams keplerParams, (double, double) rocketPosition2D, (double, double) planetPosition2D, Vector2 rocketVelocity2D, double gravityParam, double time)
    {
        KtoCfromC(rocketPosition2D, planetPosition2D,rocketVelocity2D, gravityParam, time, out keplerParams.semiMajorAxis, out keplerParams.eccentricity, out keplerParams.argumentOfPeriapsis, out keplerParams.longitudeOfAscendingNode, out keplerParams.inclination, out keplerParams.timeToPeriapsis, out keplerParams.trueAnomalyAtEpoch);
        bypass = true;
    }

    public static void CalculatePoints(double time, int numPoints, double gravityParam, (double, double) planetPosition2D, KeplerParams keplerParams, ref double[] times, ref UnityEngine.Vector3[] positions)
    {
        var period = GetOrbitalPeriod(gravityParam, keplerParams.semiMajorAxis);
        var timeIncrement = period / numPoints;

        for (int count = 0; count < numPoints; count++)
        {
            double x;
            double y;
            GetOrbitPositionKepler(gravityParam, time, keplerParams.semiMajorAxis, keplerParams.eccentricity, keplerParams.argumentOfPeriapsis, keplerParams.longitudeOfAscendingNode, keplerParams.inclination, keplerParams.trueAnomalyAtEpoch, out x, out y);
            Vector3 pos = new Vector3((float)x, (float)y, 10000000);
            times[count] = time;
            positions[count] = pos/1_000_00;

            time += timeIncrement;
        }
    }

    public void CalculateParametersHyperbolic(UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 rocketVelocity2D, UnityEngine.Vector2 planetPosition2D, float gravityParam, float time, LineRenderer line)
    {
        //Calculate rocket position in 3D and transform it for Kepler
        UnityEngine.Vector3 rocketPosition3D = new UnityEngine.Vector3(rocketPosition2D.x, rocketPosition2D.y, 0);
        UnityEngine.Vector3 planetPosition3D = new UnityEngine.Vector3(planetPosition2D.x, planetPosition2D.y, 0); 
        
        rocketPosition3D = rocketPosition3D - planetPosition3D; //Assume planet at (0,0,0)

        //Calculate velocity
        UnityEngine.Vector3 rocketVelocity3D = new UnityEngine.Vector3(rocketVelocity2D.x, rocketVelocity2D.y, 0); 

        //Find position and velocity magnitude
        float r = rocketPosition3D.magnitude;
        float v = rocketVelocity3D.magnitude;

        //Calculate specific angular momentum
        UnityEngine.Vector3 h_bar = UnityEngine.Vector3.Cross(rocketPosition3D, rocketVelocity3D);
        float h = h_bar.magnitude;

        //Calculate eccentricity vector
        UnityEngine.Vector3 eccentricity_bar = UnityEngine.Vector3.Cross(rocketVelocity3D, h_bar)/gravityParam - rocketPosition3D/r;
        float e = eccentricity_bar.magnitude;
        
        //Calculate inclination
        float i = Mathf.Atan2(-eccentricity_bar.y, -eccentricity_bar.x);

        //Calculate semi-major axis
        float a  = 1/(2/r - Mathf.Pow(v, 2)/gravityParam);
        
        //Calculate raw position
        UnityEngine.Vector2 p = new UnityEngine.Vector2(rocketPosition3D.x*Mathf.Cos(i)+rocketPosition3D.y*Mathf.Sin(i), rocketPosition3D.y*Mathf.Cos(i)-rocketPosition3D.x*Mathf.Sin(i));
        //Moon.transform.position = p;

        //Calculate Hyperbolic anomaly
        float Ho = (float)Math.Atanh((p.y/(a*Mathf.Sqrt(Mathf.Pow(e, 2)-1)))/(e-p.x/a));

        
        float Mo = (float)(Math.Sinh(Ho)*e-Ho);


        //Determine branch of hyperbola
        float dot = UnityEngine.Vector3.Dot(rocketPosition3D, rocketVelocity3D);
        float det = rocketPosition3D.x*rocketVelocity3D.y - rocketVelocity3D.x * rocketPosition3D.y;

        float angle = Mathf.Atan2(det, dot);

        //Calculate mean velocity
        float n = Mathf.Sqrt(gravityParam/Mathf.Abs(Mathf.Pow(a, 3)))*Mathf.Sign(angle);

        //Plot positions
        float timeStep = 10f;
        int maxStep = 300;
        UnityEngine.Vector3[] positions = new UnityEngine.Vector3[maxStep];
        float H = Ho;

        for(int ia = 0; ia<maxStep; ia++)
        {
            //Calculate mean anomaly
            float M = Mo + (((ia)*timeStep-time) + time)*n;

            //Calculate current hyperbolic anomaly
            H = (float)(H + (M - e*Math.Sinh(H) + H)/(e*Math.Cosh(H)-1));

            //Raw position vector
            UnityEngine.Vector2 rawP = new UnityEngine.Vector2((float)(a*(e - Math.Cosh(H))), (float)(a*Mathf.Sqrt(Mathf.Pow(e, 2)-1)*Math.Sinh(H)));
            positions[ia] = new UnityEngine.Vector2(rawP.x*Mathf.Cos(i)-rawP.y*Mathf.Sin(i), rawP.x*Mathf.Sin(i)+rawP.y*Mathf.Cos(i)) + planetPosition2D;     
        }
 
        line.positionCount = maxStep;
        line.SetPositions(positions);
    }

}