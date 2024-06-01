using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using JetBrains.Annotations;
using System.Threading.Tasks;
using UnityEngine.TextCore.Text;
using Unity.Burst;

public class Prediction : MonoBehaviour
{
    public FloatingVelocity floatingVelocity;
    public PlanetGravity planetGravity;
    public GameObject WorldSaveManager;
    public MasterManager MasterManager;
    public GameObject potentialBody;
    public GameObject orbitMarker;
    public LineRenderer line;
    public Rigidbody2D rb;
    public GameObject Moon;
    public GameObject Earth;
    public GameObject Sun;
    public float G;
    public float rocketMass;
    public float gravityParam = 0;

    public KeplerParams KeplerParams = new KeplerParams();

    public bool updated;
    public TimeManager MyTime;


    public List<GameObject> SubLevel;

    public bool subPrediction = false;

    public Vector2 newInitialVelocity;
    public float newInitialTime;
    public Vector2 newInitialPosition;
    public Vector2 moonPosition;
    public float newGravityParam;
    public GameObject indicatorPrefab;
    public GameObject interceptIndicator = null;                                               

    public List<Vector3> poss = new List<Vector3>();

    //For Hyperbolic
    double Ho;
    double H;
    double Mo;
    double n;
    double a;
    double e;
    double i;
    public float startTime;

    public bool DO = true;

    private interceptDetector interceptDetector;

    // Start is called before the first frame update
    void Start()
    {
        DO = true;
        WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        MyTime = FindObjectOfType<TimeManager>();
        G = FindObjectOfType<SolarSystemManager>().G;
        interceptDetector = GetComponent<interceptDetector>();
        floatingVelocity = FindObjectOfType<FloatingVelocity>();

        TypeScript[] planets = FindObjectsOfType<TypeScript>();
        foreach (TypeScript planet in planets)
        {
            if (planet.GetComponent<TypeScript>().type == "moon")
            {
                Moon = planet.gameObject;
            }

            if (planet.GetComponent<TypeScript>().type == "earth")
            {
                Earth = planet.gameObject;
            }

            if (planet.GetComponent<TypeScript>().type == "sun")
            {
                Sun = planet.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (MasterManager == null)
        {
            GameObject MastRef = GameObject.FindGameObjectWithTag("MasterManager");
            if (MastRef)
            {
                MasterManager = MastRef.GetComponent<MasterManager>();
            }
        }

        if (planetGravity != null)
        {
            rb = planetGravity.rb;
            rocketMass = planetGravity.rb.mass;
            gravityParam = G * (planetGravity.getMass() + rocketMass);
        }

        if (planetGravity != null)
        {
            double time = MyTime.time;
            UnityEngine.Vector2 rocketPosition2D = rb.position;
            UnityEngine.Vector2 rocketVelocity2D = rb.velocity;
            if(planetGravity.GetComponent<RocketStateManager>().state == "simulate")
            {
                rocketVelocity2D -= new Vector2((float)floatingVelocity.velocity.Item1, (float)floatingVelocity.velocity.Item2);
            }
            (double, double) planetPosition2D = (planetGravity.getPlanet().GetComponent<DoubleTransform>().x_pos, planetGravity.getPlanet().GetComponent<DoubleTransform>().y_pos);
            if(DO == true)
            {
                DO = false;
                StartCoroutine(DrawLine(time, line, KeplerParams, rocketPosition2D, rocketVelocity2D, planetPosition2D, gravityParam));
            }   
        }

        if(planetGravity == null)
        {
            Destroy(this.gameObject);
        }

    }

    public IEnumerator DrawLine(double time, LineRenderer line, KeplerParams keplerParams, UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 rocketVelocity2D, (double, double) planetPos, float gravityParam)
    {
        int numPoints = 500;
        double[] times = new double[numPoints];
        Vector3[] positions = new Vector3[numPoints];

        if (planetGravity.gameObject.GetComponent<Rocket>().throttle > 0 || updated == false)
        {
            SetKeplerParams(keplerParams, rocketPosition2D, planetPos, rocketVelocity2D, gravityParam, time);
            if (rb.velocity.magnitude != 0 && keplerParams.eccentricity < 1)
            {
                line.loop = true;
                CalculatePoints(time, numPoints, gravityParam, planetPos, keplerParams, ref times, ref positions);
                line.positionCount = positions.Count();
                line.SetPositions(positions);
            }

            if (rb.velocity.magnitude != 0 && keplerParams.eccentricity >= 1)
            {
                line.loop = false;
                startTime = (float)MyTime.time;
                CalculateParametersHyperbolic(rocketPosition2D, rocketVelocity2D, planetPos, gravityParam, time, line);
                CalculateParameterHyperbolic(rocketPosition2D, rocketVelocity2D, planetPos, gravityParam, MyTime.time);
            }

            if(planetGravity.getPlanet() == Earth)
            {
                interceptDetector.DetectIntercept();
            }else if(interceptDetector.interceptIndicator != null)
            {
                Destroy(interceptDetector.interceptIndicator.gameObject);
            }
            
            updated = true;
        }

        yield return new WaitForSeconds(0.004f);
        DO = true;
    }

    //Should only be used for Intercept Detector
    public Vector3 GetPositionAtTime(double Time)
    {
        double x = 0;
        double y = 0;
        double VX;
        double VY;
        if(KeplerParams.eccentricity >= 1)
        {
            GetOrbitalPositionHyperbolic(Mo, Time, Ho, H, e, a, i, n, startTime, out x, out y);
        }

        if(KeplerParams.eccentricity < 1)
        {
            GetOrbitPositionKepler(gravityParam, Time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x, out y, out VX, out VY);
        }
        return new Vector3((float)x, (float)y, 0);
    }

    public static void GetOrbitPositionKepler(double gravityParam, double time, double semiMajorAxis, double eccentricity, double argPeriapsis, double LAN, double inclination, double trueAnomalyAtEpoch, out double X, out double Y, out double VX, out double VY)
    {
        // Compute MA (Mean Anomaly)
        // n = 2pi / T (T = time for one orbit)
        // M = n (t)
        double meanAngularMotion = Math.Sqrt(gravityParam / Math.Pow(semiMajorAxis, 3)); 
        double timeWithOffset = time + GetTimeOffsetFromTrueAnomaly(trueAnomalyAtEpoch, meanAngularMotion, eccentricity);
        double MA = timeWithOffset * meanAngularMotion;


        // Compute EA (Eccentric Anomaly)
        double EA = MA;


        for (int count = 0; count < 2000; count++)
        {
            double dE = (EA - eccentricity * Math.Sin(EA) - MA) / (1 - eccentricity * Math.Cos(EA));
            EA -= dE;
            if (Math.Abs(dE) < 1e-5)
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

        //Compute h and p for velocity
        double h = Math.Sqrt(gravityParam * semiMajorAxis * (1 - Math.Pow(eccentricity, 2)));
        double p = semiMajorAxis * (1 - Math.Pow(eccentricity, 2));

        VX = (X * h * eccentricity / (r * p)) * Math.Sin(TA) - (h / r) * (Math.Cos(LAN) * Math.Sin(argPeriapsis + TA) + Math.Sin(LAN) * Math.Cos(argPeriapsis + TA) * Math.Cos(inclination));
        VY = (Y * h * eccentricity / (r * p)) * Math.Sin(TA) + (h / r) * (Math.Cos(argPeriapsis + TA) * Math.Sin(inclination));
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
        return (Math.Sqrt(4 * Math.Pow(Math.PI, 2) * Math.Pow(semiMajorAxis, 3) / gravityParam));
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

        for (int count = 0; count < 2000; count++)
        {
            double dE = (EA - eccentricity * Math.Sin(EA) - MA) / (1 - eccentricity * Math.Cos(EA));
            EA -= dE;
            if (Math.Abs(dE) < 1e-50)
            {
                break;
            }
        }

        // Compute TA (True Anomaly)
        double TA = 2 * Math.Atan(Math.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Math.Tan(EA / 2));

        //Some corrections
        if (timeOffset > 0)
        {
            TA = 2 * Math.PI - TA;
        }

        TA = Modulo(TA, 2 * Math.PI);

        return TA;
    }

    public static double GetTimeOffsetFromTrueAnomaly(double trueAnomaly, double meanAngularMotion, double eccentricity)
    {
        // Offset by Math.Pi so 0 TA lines up with default start position from GetOrbitPositionKepler.
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

    public static void KtoCfromC(UnityEngine.Vector2 rocketPosition2D, (double, double) planetPosition2D, UnityEngine.Vector2 rocketVelocity2D, double gravityParam, double time, out double semiMajorAxis, out double eccentricity, out double argPeriapsis, out double LAN, out double inclination, out double timeToPeriapsis, out double trueAnomalyAtEpoch)
    {
        //Calculate rocket position in 3D and transform it for Kepler
        UnityEngine.Vector3 rocketPosition3D = new UnityEngine.Vector3(rocketPosition2D.x, 0, rocketPosition2D.y); //FLIP for Unity
        UnityEngine.Vector3 planetPosition3D = new UnityEngine.Vector3((float)planetPosition2D.Item1, 0, (float)planetPosition2D.Item2); //FLIP for Unity

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
        double E = (0.5f * Math.Pow(v, 2)) - gravityParam / r;

        //Compute semi-major axis
        double a = -gravityParam / (2 * E);

        //Compute eccentricity
        double e = Math.Sqrt(1 - Math.Pow(h, 2) / (a * gravityParam));

        //Compute inclination
        double i = Math.Acos(h_bar.z / h);

        //Compute right ascension of ascending node
        double omega_LAN = Math.Atan2(h_bar.x, -h_bar.y);

        //Compute argument of latitude v+w
        double lat = Math.Atan2((rocketPosition3D[2] / Math.Sin(i)), (rocketPosition3D[0] * Math.Cos(omega_LAN) + rocketPosition3D[1] * Math.Sin(omega_LAN)));

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
        argPeriapsis = Modulo(omega_AP, Math.PI * 2);
        LAN = omega_LAN;
        inclination = i;
        timeToPeriapsis = T;
        trueAnomalyAtEpoch = TA;
    }

    public void SetKeplerParams(KeplerParams keplerParams, UnityEngine.Vector2 rocketPosition2D, (double, double) planetPosition2D, UnityEngine.Vector2 rocketVelocity2D, double gravityParam, double time)
    {
        KtoCfromC(rocketPosition2D, planetPosition2D, rocketVelocity2D, gravityParam, time, out keplerParams.semiMajorAxis, out keplerParams.eccentricity, out keplerParams.argumentOfPeriapsis, out keplerParams.longitudeOfAscendingNode, out keplerParams.inclination, out keplerParams.timeToPeriapsis, out keplerParams.trueAnomalyAtEpoch);
    }

    public void CalculatePoints(double time, int numPoints, double gravityParam,(double, double) planetPos, KeplerParams keplerParams, ref double[] times, ref UnityEngine.Vector3[] positions)
    {
        double period = GetOrbitalPeriod(gravityParam, keplerParams.semiMajorAxis);
        double timeIncrement = period / numPoints;
        List<Vector3> newPos = new List<Vector3>();
        double SOI = Mathf.Infinity;
        if(planetGravity.getPlanet() == Earth)
        {
            SOI = planetGravity.SolarSystemManager.earthSOI;
        }

        if(planetGravity.getPlanet() == Moon)
        {
            SOI = planetGravity.SolarSystemManager.moonSOI;
        }
        for (int count = 0; count < numPoints; count++)
        {
            double X;
            double Y;
            double VX;
            double VY;
            GetOrbitPositionKepler(gravityParam, time + timeIncrement, keplerParams.semiMajorAxis, keplerParams.eccentricity, keplerParams.argumentOfPeriapsis, keplerParams.longitudeOfAscendingNode, keplerParams.inclination, keplerParams.trueAnomalyAtEpoch, out X, out Y, out VX, out VY);
            Vector3 pos = new Vector3((float)X, (float)Y, 0)/MapManager.scaledSpace + new Vector3((float)planetPos.Item1, (float)planetPos.Item2,  -100000000)/MapManager.scaledSpace;
            if((new Vector2(pos.x, pos.y)*MapManager.scaledSpace - new Vector2((float)planetPos.Item1, (float)planetPos.Item2)).magnitude < SOI)
            {
                newPos.Add(pos);
            }else if(planetGravity.getPlanet() == Sun){
                newPos.Add(pos);
            }else{
                positions = newPos.ToArray();
                line.loop = false;
                return;
            }
            times[count] = time;
            positions[count] = pos;

            time += timeIncrement;

        }
        positions = newPos.ToArray();
    }

    public void CalculateParametersHyperbolic(UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 rocketVelocity2D, (double, double) planetPosition2D, double gravityParam, double time, LineRenderer line)
    {
        //Calculate rocket position in 3D and transform it for Kepler
        UnityEngine.Vector3 rocketPosition3D = new UnityEngine.Vector3(rocketPosition2D.x, rocketPosition2D.y, 0);
        UnityEngine.Vector3 planetPosition3D = new UnityEngine.Vector3((float)planetPosition2D.Item1, (float)planetPosition2D.Item2, 0);

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
        UnityEngine.Vector3 eccentricity_bar = UnityEngine.Vector3.Cross(rocketVelocity3D, h_bar) / (float)gravityParam - rocketPosition3D / (float)r;
        double e = eccentricity_bar.magnitude;

        //Calculate inclination
        double i = Math.Atan2(-eccentricity_bar.y, -eccentricity_bar.x);

        //Calculate semi-major axis
        double a = 1 / (2 / r - Math.Pow(v, 2) / gravityParam);

        //Calculate raw position
        UnityEngine.Vector2 p = new UnityEngine.Vector2((float)(rocketPosition3D.x * Math.Cos(i) + rocketPosition3D.y * Math.Sin(i)), (float)(rocketPosition3D.y * Math.Cos(i) - rocketPosition3D.x * Math.Sin(i)));
        //Moon.transform.position = p;

        //Calculate Hyperbolic anomaly
        double Ho = Math.Atanh((p.y / (a * Math.Sqrt(Math.Pow(e, 2) - 1))) / (e - p.x / a));


        double Mo = Math.Sinh(Ho) * e - Ho;


        //Determine branch of hyperbola
        double dot = UnityEngine.Vector3.Dot(rocketPosition3D, rocketVelocity3D);
        double det = rocketPosition3D.x * rocketVelocity3D.y - rocketVelocity3D.x * rocketPosition3D.y;

        double angle = Math.Atan2(det, dot);

        //Calculate mean velocity
        double n = Math.Sqrt(gravityParam / Math.Abs(Math.Pow(a, 3))) * Math.Sign(angle);

        //Plot positions
        int timeStep = 1;
        int maxStep = 500;
        List<Vector3> positions = new List<Vector3>();
        double H = Ho;

        double SOI = Mathf.Infinity;
        if(planetGravity.getPlanet() == Earth)
        {
            SOI = planetGravity.SolarSystemManager.earthSOI;
        }

        if(planetGravity.getPlanet() == Moon)
        {
            SOI = planetGravity.SolarSystemManager.moonSOI;
        }

        bool enteredSOI = false;
        for (int ia = 0; ia < maxStep; ia++)
        {
            //Calculate mean anomaly
            double M = Mo + ((time + timeStep*ia) - startTime) * n;

            //Calculate current hyperbolic anomaly
            H = H + (M - e * Math.Sinh(H) + H) / (e * Math.Cosh(H) - 1);

            //Raw position vector
            Vector2 rawP = new UnityEngine.Vector2((float)(a * (e - Math.Cosh(H))), (float)(a * Math.Sqrt(Math.Pow(e, 2) - 1) * Math.Sinh(H)));
            Vector2 pos = new Vector2((float)(rawP.x * Math.Cos(i) - rawP.y * Math.Sin(i)), (float)(rawP.x * Math.Sin(i) + rawP.y * Math.Cos(i)));

            pos += new Vector2((float)planetPosition2D.Item1, (float)planetPosition2D.Item2);
            if(pos.magnitude != float.NaN)
            {   
                if((pos - new Vector2((float)planetPosition2D.Item1, (float)planetPosition2D.Item2)).magnitude < SOI)
                {
                    enteredSOI = true;
                    positions.Add(new Vector3(pos.x, pos.y, -100000000)/MapManager.scaledSpace);
                    timeStep += 10;
                }else if(enteredSOI == true)
                {
                    line.positionCount = positions.Count();
                    line.SetPositions(positions.ToArray());
                    return;
                }
            }
        }

        if(maxStep > 5000)
        {
            line.positionCount = positions.Count();
            line.SetPositions(positions.ToArray());
            return;
        }

        line.positionCount = positions.Count();
        line.SetPositions(positions.ToArray());
    }

    public void CalculateParameterHyperbolic(UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 rocketVelocity2D, (double, double) planetPosition2D, float gravityParam, double time)
    {
        //Calculate rocket position in 3D and transform it for Kepler
        UnityEngine.Vector3 rocketPosition3D = new UnityEngine.Vector3(rocketPosition2D.x, rocketPosition2D.y, 0);
        UnityEngine.Vector3 planetPosition3D = new UnityEngine.Vector3((float)planetPosition2D.Item1, (float)planetPosition2D.Item2, 0);

        rocketPosition3D = rocketPosition3D - planetPosition3D; //Assume planet at (0,0,0)

        //Calculate velocity
        UnityEngine.Vector3 rocketVelocity3D = new UnityEngine.Vector3(rocketVelocity2D.x, rocketVelocity2D.y, 0);

        //Find position and velocity magnitude
        float r = rocketPosition3D.magnitude;
        float v = rocketVelocity3D.magnitude;

        //Calculate specific angular momentum
        UnityEngine.Vector3 h_bar = UnityEngine.Vector3.Cross(rocketPosition3D, rocketVelocity3D);
        float h = rocketPosition3D.x * rocketVelocity3D.y - rocketPosition3D.y * rocketVelocity3D.x;

        //Calculate eccentricity vector
        UnityEngine.Vector3 eccentricity_bar = UnityEngine.Vector3.Cross(rocketVelocity3D, h_bar) / gravityParam - rocketPosition3D / r;
        e = eccentricity_bar.magnitude;

        //Calculate inclination
        i = Mathf.Atan2(-eccentricity_bar.y, -eccentricity_bar.x);

        //Calculate semi-major axis
        a = 1 / (2 / r - Mathf.Pow(v, 2) / gravityParam);


        //MOVING THAT TO GET POSITION MIGHT FIX
        //Calculate raw position
        Vector2 p = new Vector2((float)(rocketPosition3D.x * Math.Cos(i) + rocketPosition3D.y * Math.Sin(i)), (float)(rocketPosition3D.y * Math.Cos(i) - rocketPosition3D.x * Math.Sin(i)));
        //Moon.transform.position = p;

        //Calculate Hyperbolic anomaly
        Ho = Math.Atanh(p.y / (a * Math.Sqrt(Math.Pow(e, 2) - 1)) / (e - (p.x / a)));
        H = Ho;

        Mo = Math.Sinh(Ho) * e - Ho;


        //Determine branch of hyperbola
        double dot = UnityEngine.Vector3.Dot(rocketPosition3D, rocketVelocity3D);
        double det = rocketPosition3D.x * rocketVelocity3D.y - rocketVelocity3D.x * rocketPosition3D.y;

        double angle = Math.Atan2(det, dot);
        //Calculate mean velocity
        n = Math.Sqrt(gravityParam / Math.Abs(Math.Pow(a, 3))) * Math.Sign(h);

    }

    //ONLY FOR ROCKETPATHs
    public static void GetOrbitalPositionHyperbolic(double Mo, double time, double Ho, double H, double e, double a, double i, double n, double startTime, out double x, out double y)
    {
        //Calculate mean anomaly
        double M;
        H = Ho;
        double timeStep = (time-startTime)/500;
        double targetTime = time;
        time = startTime;
        int k = 0;

        while (time <= targetTime)
        {
            M = Mo + (time - startTime) * n;

            // Calculate current hyperbolic anomaly with initial guess H
            H = H + ((M - e * Math.Sinh(H) + H) / (e * Math.Cosh(H) - 1));

            time += timeStep;
            k++;
        }

        //print("This is H" + H + " this is Ho" + Ho);

        double rawX = a * (e - Math.Cosh(H));
        double rawY = a * Math.Sqrt(Math.Pow(e, 2) - 1) * Math.Sinh(H);

        x = rawX * Math.Cos(i) - rawY * Math.Sin(i);
        y = rawX * Math.Sin(i) + rawY * Math.Cos(i);
    }
}