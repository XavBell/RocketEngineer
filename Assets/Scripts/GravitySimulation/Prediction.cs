using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using UnityEngine.Timeline;

public class Prediction : MonoBehaviour
{
    public PlanetGravity planetGravity;
    public GameObject WorldSaveManager;
    public MasterManager MasterManager;
    public GameObject potentialBody;
    public GameObject orbitMarker;
    public LineRenderer line;
    public Rigidbody2D rb;
    public GameObject Moon;
    public GameObject Earth;
    public float G;
    public float rocketMass;
    public float gravityParam = 0;

    public KeplerParams KeplerParams =  new KeplerParams();

    public bool updated;
    public TimeManager MyTime;





    
    // Start is called before the first frame update
    void Start()
    {
        WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        MyTime = FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(MasterManager == null)
        {
            GameObject MastRef = GameObject.FindGameObjectWithTag("MasterManager");
            if(MastRef)
            {
                MasterManager = MastRef.GetComponent<MasterManager>();
            }
        }

        if(MasterManager != null && MasterManager.ActiveRocket != null)
        {
            planetGravity = MasterManager.ActiveRocket.GetComponent<PlanetGravity>();
            rb = planetGravity.rb;
            G = planetGravity.G;
            rocketMass = planetGravity.rb.mass;
            gravityParam = G*(planetGravity.Mass + rocketMass);

        }

        if(planetGravity != null)
        {
            float time = Time.time;
            UnityEngine.Vector2 rocketPosition2D = rb.position;
            UnityEngine.Vector2 rocketVelocity2D = rb.velocity;
            UnityEngine.Vector2 planetPosition2D = planetGravity.planet.transform.position;
            if(planetGravity.planet.GetComponent<TypeScript>().type == "moon")
            {
                planetPosition2D = planetGravity.planet.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time);
                rocketVelocity2D = rb.velocity - new Vector2((planetGravity.planet.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time).x - planetGravity.planet.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time - MyTime.deltaTime).x)/MyTime.deltaTime, (planetGravity.planet.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time).y - planetGravity.planet.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time - MyTime.deltaTime).y)/MyTime.deltaTime);
            }
            DrawLine(time, line, KeplerParams, rocketPosition2D, rocketVelocity2D, planetPosition2D, gravityParam);
        }

    }

    void DrawLine(float time, LineRenderer line, KeplerParams keplerParams, UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 rocketVelocity2D, UnityEngine.Vector2 planetPosition2D, float gravityParam)
    {
        int numPoints = 1000;
        double[] times = new double[numPoints];
        UnityEngine.Vector3[] positions = new UnityEngine.Vector3[numPoints];

        if(Input.GetKey("z") || updated == false)
        {
            SetKeplerParams(keplerParams, rocketPosition2D, planetPosition2D, rocketVelocity2D, gravityParam, time);
            if(rb.velocity.magnitude != 0 && keplerParams.eccentricity < 1)
            {
                CalculatePoints(time, numPoints, gravityParam, planetPosition2D, keplerParams, ref times, ref positions);
                line.positionCount = numPoints;
                line.SetPositions(positions);
            }

            if(rb.velocity.magnitude != 0 && keplerParams.eccentricity > 1)
            {
                CalculateParametersHyperbolic(rocketPosition2D, rocketVelocity2D, planetPosition2D, gravityParam, time, line);
            }

            updated = true;
        }

        
    }

    public static UnityEngine.Vector3 GetOrbitPositionKepler(double gravityParam, double time, double semiMajorAxis, double eccentricity, double argPeriapsis, double LAN, double inclination, double trueAnomalyAtEpoch)
    {
        // Compute MA (Mean Anomaly)
        // n = 2pi / T (T = time for one orbit)
        // M = n (t)
        double meanAngularMotion = Math.Sqrt(gravityParam / Math.Pow(semiMajorAxis, 3)); // TODO (Mean Angular Motion can be computed at build/run time once)
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
        

        // Compute XYZ positions
        double X = r * (Math.Cos(LAN) * Math.Cos(argPeriapsis + TA) - Math.Sin(LAN) * Math.Sin(argPeriapsis + TA) * Math.Cos(inclination));
        double Y = r * (Math.Sin(LAN) * Math.Cos(argPeriapsis + TA) + Math.Cos(LAN) * Math.Sin(argPeriapsis + TA) * Math.Cos(inclination));
        double Z = r * (Math.Sin(inclination) * Math.Sin(argPeriapsis + TA));

        return new((float)X, (float)Z, 0); // FLIP Y-Z FOR UNITY
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
        return (Math.Sqrt(4 * Math.Pow(Mathf.PI, 2) * Math.Pow(semiMajorAxis, 3) / gravityParam));
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
        double TA_Clean = Modulo((trueAnomaly + Mathf.PI), (Mathf.PI * 2)) - Mathf.PI;
        double EA = Math.Acos((eccentricity + Math.Cos(TA_Clean)) / (1 + eccentricity * Math.Cos(TA_Clean)));
        if (TA_Clean < 0)
        {
            EA *= -1;
        }
        double MA = EA - eccentricity * Math.Sin(EA);
        double t = MA / meanAngularMotion;
        

        return t;
    }

    public static void KtoCfromC(UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 planetPosition2D, UnityEngine.Vector2 rocketVelocity2D, double gravityParam, double time, out double semiMajorAxis, out double eccentricity, out double argPeriapsis, out double LAN, out double inclination, out double timeToPeriapsis, out double trueAnomalyAtEpoch)
    {   
        //Calculate rocket position in 3D and transform it for Kepler
        UnityEngine.Vector3 rocketPosition3D = new UnityEngine.Vector3(rocketPosition2D.x, 0, rocketPosition2D.y); //FLIP for Unity
        UnityEngine.Vector3 planetPosition3D = new UnityEngine.Vector3(planetPosition2D.x, 0, planetPosition2D.y); //FLIP for Unity
        
        rocketPosition3D = rocketPosition3D - planetPosition3D; //Assume planet at (0,0,0)

        //Calculate velocity
        UnityEngine.Vector3 rocketVelocity3D = new UnityEngine.Vector3(rocketVelocity2D.x, 0, rocketVelocity2D.y); //FLIP for Unity

        //Find position and velocity magnitude
        double r = rocketPosition3D.magnitude;
        double v = rocketVelocity3D.magnitude;

        //Calculate specific angular momentum
        UnityEngine.Vector3 h_bar = UnityEngine.Vector3.Cross(rocketPosition3D, rocketVelocity3D);

        double h = h_bar.magnitude;

        //Compute specific energy
        double E = (0.5f * Math.Pow(v, 2)) - gravityParam/r;

        //Compute semi-major axis
        double a = -gravityParam/(2*E);

        //Compute eccentricity
        double e = Math.Sqrt(1 - Math.Pow(h,2)/(a*gravityParam));
      
        //Compute inclination
        double i = Math.Acos(h_bar.z/h);

        //Compute right ascension of ascending node
        double omega_LAN = Math.Atan2(h_bar.x, -h_bar.y);

        //Compute argument of latitude v+w
        double lat = Math.Atan2((rocketPosition3D[2]/Math.Sin(i)), (rocketPosition3D[0]*Math.Cos(omega_LAN) + rocketPosition3D[1] * Math.Sin(omega_LAN)));

        // Compute true anomaly, v, (not actual true anomaly)
        double p = a * (1 - Math.Pow(e, 2));
        double nu = Math.Atan2(Math.Sqrt(p / gravityParam) * UnityEngine.Vector3.Dot(rocketPosition3D, rocketVelocity3D), p - r);

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

    public void SetKeplerParams(KeplerParams keplerParams, UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 planetPosition2D, UnityEngine.Vector2 rocketVelocity2D, float gravityParam, float time)
    {
        KtoCfromC(rocketPosition2D, planetPosition2D,rocketVelocity2D, gravityParam, time, out keplerParams.semiMajorAxis, out keplerParams.eccentricity, out keplerParams.argumentOfPeriapsis, out keplerParams.longitudeOfAscendingNode, out keplerParams.inclination, out keplerParams.timeToPeriapsis, out keplerParams.trueAnomalyAtEpoch);
    }

    public void CalculatePoints(double time, int numPoints, double gravityParam, UnityEngine.Vector2 planetPosition2D, KeplerParams keplerParams, ref double[] times, ref UnityEngine.Vector3[] positions)
    {
        var period = GetOrbitalPeriod(gravityParam, keplerParams.semiMajorAxis);
        var timeIncrement = period / numPoints;

        for (int count = 0; count < numPoints; count++)
        {
            Vector3 pos = GetOrbitPositionKepler(gravityParam, time, keplerParams.semiMajorAxis, keplerParams.eccentricity, keplerParams.argumentOfPeriapsis, keplerParams.longitudeOfAscendingNode, keplerParams.inclination, keplerParams.trueAnomalyAtEpoch) + new Vector3(planetPosition2D.x, planetPosition2D.y, 0);
            times[count] = time;
            positions[count] = pos;

            time += timeIncrement;
        }

        DetectIntercept(positions, times, Earth, potentialBody, orbitMarker);

    }

    public void CalculateParametersHyperbolic(UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 rocketVelocity2D, UnityEngine.Vector2 planetPosition2D, double gravityParam, double time, LineRenderer line)
    {
        //Calculate rocket position in 3D and transform it for Kepler
        UnityEngine.Vector3 rocketPosition3D = new UnityEngine.Vector3(rocketPosition2D.x, rocketPosition2D.y, 0);
        UnityEngine.Vector3 planetPosition3D = new UnityEngine.Vector3(planetPosition2D.x, planetPosition2D.y, 0); 
        
        rocketPosition3D = rocketPosition3D - planetPosition3D; //Assume planet at (0,0,0)

        //Calculate velocity
        UnityEngine.Vector3 rocketVelocity3D = new UnityEngine.Vector3(rocketVelocity2D.x, rocketVelocity2D.y, 0); 

        //Find position and velocity magnitude
        double r = rocketPosition3D.magnitude;
        double v = rocketVelocity3D.magnitude;

        //Calculate specific angular momentum
        UnityEngine.Vector3 h_bar = UnityEngine.Vector3.Cross(rocketPosition3D, rocketVelocity3D);
        double h = h_bar.magnitude;

        //Calculate eccentricity vector
        UnityEngine.Vector3 eccentricity_bar = UnityEngine.Vector3.Cross(rocketVelocity3D, h_bar)/(float)gravityParam - rocketPosition3D/(float)r;
        double e = eccentricity_bar.magnitude;
        
        //Calculate inclination
        double i = Math.Atan2(-eccentricity_bar.y, -eccentricity_bar.x);

        //Calculate semi-major axis
        double a  = 1/(2/r - Math.Pow(v, 2)/gravityParam);
        
        //Calculate raw position
        UnityEngine.Vector2 p = new UnityEngine.Vector2((float)(rocketPosition3D.x*Math.Cos(i)+rocketPosition3D.y*Math.Sin(i)), (float)(rocketPosition3D.y*Math.Cos(i)-rocketPosition3D.x*Math.Sin(i)));
        //Moon.transform.position = p;

        //Calculate Hyperbolic anomaly
        double Ho = Math.Atanh((p.y/(a*Math.Sqrt(Math.Pow(e, 2)-1)))/(e-p.x/a));

        
        double Mo = Math.Sinh(Ho)*e-Ho;


        //Determine branch of hyperbola
        double dot = UnityEngine.Vector3.Dot(rocketPosition3D, rocketVelocity3D);
        double det = rocketPosition3D.x*rocketVelocity3D.y - rocketVelocity3D.x * rocketPosition3D.y;

        double angle = Math.Atan2(det, dot);

        //Calculate mean velocity
        double n = Math.Sqrt(gravityParam/Math.Abs(Math.Pow(a, 3)))*Math.Sign(angle);

        //Plot positions
        int timeStep = 10;
        int maxStep = 300;
        UnityEngine.Vector3[] positions = new UnityEngine.Vector3[maxStep];
        double H = Ho;
        double[]times = new double[maxStep];

        for(int ia = 0; ia<maxStep; ia++)
        {
            //Calculate mean anomaly
            double M = Mo + (((ia)*timeStep-time) + time)*n;

            //Calculate current hyperbolic anomaly
            H = H + (M - e*Math.Sinh(H) + H)/(e*Math.Cosh(H)-1);

            //Raw position vector
            UnityEngine.Vector2 rawP = new UnityEngine.Vector2((float)(a*(e - Math.Cosh(H))), (float)(a*Math.Sqrt(Math.Pow(e, 2)-1)*Math.Sinh(H)));
            if((new UnityEngine.Vector2((float)(rawP.x*Math.Cos(i)-rawP.y*Math.Sin(i)), (float)(rawP.x*Math.Sin(i)+rawP.y*Math.Cos(i))) + planetPosition2D).magnitude < 10000000)
            {
                positions[ia] = new UnityEngine.Vector2((float)(rawP.x*Math.Cos(i)-rawP.y*Math.Sin(i)), (float)(rawP.x*Math.Sin(i)+rawP.y*Math.Cos(i))) + planetPosition2D;
                times[ia] = (ia)*timeStep-time + time;
            }
                 
        }

        
        DetectIntercept(positions, times, Earth, potentialBody, orbitMarker);
        line.positionCount = maxStep;
        line.SetPositions(positions);
    }

    public void DetectIntercept(Vector3[] points, double[] times, GameObject orbitingBodyPos, GameObject potentialBody, GameObject orbitMarker)
    {
        //Minimum distance from moon is 391100 for gravity switch
        //That means that distance from planet must be at least 377,700
        List<Vector3> PotentialPos = new List<Vector3>();
        List<double> PotentialTimes = new List<double>();

        int i = 0;
        foreach(double time in PotentialTimes)
        {
            if((points[i] - Moon.GetComponent<BodyPath>().GetPositionAtTime((float)time)).magnitude >= 371800)
            {
                PotentialPos.Add(points[i]);
                PotentialTimes.Add(time);
            }
            i++;
        }

        i = 0;
        foreach(float time in PotentialTimes)
        {
            Vector3 potentialPosition = potentialBody.GetComponent<BodyPath>().GetPositionAtTime(time);
            //Assume minimum of 500m (?)
            if((potentialPosition - PotentialPos[i]).magnitude < 5000)
            {
                Debug.Log("Intercept Found");
                orbitMarker.transform.position = potentialPosition;
                return;
            }
            i++;
        }

    }

}