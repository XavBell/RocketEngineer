using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class RocketPath : MonoBehaviour
{
    public PlanetGravity planetGravity;
    public GameObject WorldSaveManager;
    public MasterManager MasterManager;
    public Rigidbody2D rb;
    public float G;
    public float rocketMass;
    public float gravityParam = 0;

    public KeplerParams KeplerParams =  new KeplerParams();
    public bool updated;
    public TimeManager MyTime;

    double Ho;
    double H;
    double Mo;
    double n;
    double a;
    double e;
    double i;
    public double startTime;
    public double lastUpdatedTime;
    public bool bypass = false;
    
    // Start is called before the first frame update
    void Start()
    {
        WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        if(MyTime == null)
        {
            MyTime = FindObjectOfType<TimeManager>();
        }
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

        if(MasterManager != null)
        {
            planetGravity = this.GetComponent<PlanetGravity>();
            rb = planetGravity.rb;
            G = FindObjectOfType<SolarSystemManager>().G;
            rocketMass = planetGravity.rb.mass;
            gravityParam = G*(planetGravity.getMass() + rocketMass);

        }

        if(planetGravity != null)
        {
            double time = MyTime.time;
            UnityEngine.Vector2 rocketPosition2D = rb.position;
            UnityEngine.Vector2 rocketVelocity2D = rb.velocity;
            UnityEngine.Vector2 planetPosition2D = planetGravity.getPlanet().transform.position;
        }
    }

    public void CalculateParameters()
    {
        startTime = MyTime.time;
        SetKeplerParams(KeplerParams, rb.position, planetGravity.getPlanet().transform.position, rb.velocity, gravityParam, startTime);
        if(KeplerParams.eccentricity > 1)
        {
            CalculateParametersHyperbolic(rb.position, rb.velocity, planetGravity.getPlanet().transform.position, gravityParam, startTime);
        }
    }

    public Vector2 updatePosition()
    {
        if(MyTime != null)
        {
            if(KeplerParams.eccentricity < 1)
            {
                double x = 0;
                double y = 0;
                double vX;
                double vY;
                GetOrbitPositionKepler(gravityParam, MyTime.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x, out y, out vX, out vY);
                Vector2 transformV = new Vector3((float)x, (float)y, 0) + planetGravity.getPlanet().transform.position;
                return transformV;
            }

            if(KeplerParams.eccentricity > 1)
            {
                double x;
                double y;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, MyTime.time, Ho, e, a, i, n, startTime, out x, out y, out vX, out vY);
                Vector2 transformV = new Vector3((float)x, (float)y, 0) + planetGravity.getPlanet().transform.position;
                return transformV;
            }
        }

        if(MyTime == null)
        {
            MyTime = FindObjectOfType<TimeManager>();
            if(KeplerParams.eccentricity < 1)
            {
                double x = 0;
                double y = 0;
                double vX;
                double vY;
                GetOrbitPositionKepler(gravityParam, MyTime.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x, out y, out vX, out vY);
                Vector2 transformV = new Vector3((float)x, (float)y, 0) + planetGravity.getPlanet().transform.position;
                return transformV;
            }

            if(KeplerParams.eccentricity > 1)
            {
                double x = 0;
                double y = 0;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, MyTime.time, Ho,  e, a, i, n, startTime, out x, out y, out vX, out vY);
                Vector2 transformV = new Vector3((float)x, (float)y, 0) + planetGravity.getPlanet().transform.position;
                return transformV;
            }
        }
        return rb.position;
    }

    public Vector2 updateVelocity()
    {
        if(MyTime != null)
        {
            if(KeplerParams.eccentricity < 1)
            {
                double x1 = 0;
                double y1 = 0;
                double vX;
                double vY;
                GetOrbitPositionKepler(gravityParam, MyTime.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x1, out y1, out vX, out vY);
                return new Vector2((float)vX, (float)vY);
            }

            if(KeplerParams.eccentricity > 1)
            {
                double x1 = 0;
                double y1 = 0;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, MyTime.time, Ho, e, a, i, n, startTime, out x1, out y1, out vX, out vY);
                return new Vector2((float)vX, (float)vY);
            }
        }

        if(MyTime == null)
        {
            MyTime = FindObjectOfType<TimeManager>();
            if(KeplerParams.eccentricity < 1)
            {
                double x1 = 0;
                double y1 = 0;
                double vX;
                double vY;
                GetOrbitPositionKepler(gravityParam, MyTime.time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x1, out y1, out vX, out vY);
                return new Vector2((float)vX, (float)vY);
            }

            if(KeplerParams.eccentricity > 1)
            {
                double x1 = 0;
                double y1 = 0;
                double vX;
                double vY;
                GetOrbitalPositionHyperbolic(Mo, MyTime.time, Ho, e, a, i, n, startTime, out x1, out y1, out vX, out vY);
                return new Vector2((float)vX, (float)vY);
            }
        }
        return rb.velocity;
    }

    public Vector3 GetPositionAtTime(double Time)
    {
        double x;
        double y;
        double VX;
        double VY;
        GetOrbitPositionKepler(gravityParam, Time, KeplerParams.semiMajorAxis, KeplerParams.eccentricity, KeplerParams.argumentOfPeriapsis, KeplerParams.longitudeOfAscendingNode, KeplerParams.inclination, KeplerParams.trueAnomalyAtEpoch, out x, out y, out VX, out VY);
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
            if (Math.Abs(dE) < 1e-50)
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
        X = r * (Math.Cos(LAN) * Math.Cos(argPeriapsis + TA) - Math.Sin(LAN) * Math.Sin(argPeriapsis + TA) * Math.Cos(inclination));
        Y = r * (Math.Sin(inclination) * Math.Sin(argPeriapsis + TA));

        VX = (X*h*eccentricity/(r*p))*Math.Sin(TA) - (h/r)*(Math.Cos(LAN)* Math.Sin(argPeriapsis+TA) + Math.Sin(LAN)*Math.Cos(argPeriapsis+TA)*Math.Cos(inclination));
        VY = (Y*h*eccentricity/(r*p))*Math.Sin(TA) + (h/r)*(Math.Cos(argPeriapsis+TA)*Math.Sin(inclination));

        // FLIP Y-Z FOR UNITY
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

        for (int count = 0; count < 500; count++)
        {
            double dE = (EA - eccentricity * Math.Sin(EA) - MA) / (1 - eccentricity * Math.Cos(EA));
            EA -= dE;
            if (Math.Abs(dE) < 1e-10)
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

    public static void KtoCfromC(UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 planetPosition2D, UnityEngine.Vector2 rocketVelocity2D, double gravityParam, double time, out double semiMajorAxis, out double eccentricity, out double argPeriapsis, out double LAN, out double inclination, out double timeToPeriapsis, out double trueAnomalyAtEpoch, out double AP)
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
        argPeriapsis = Modulo(omega_AP, Math.PI*2);
        LAN = omega_LAN;
        inclination = i;
        timeToPeriapsis = T;
        trueAnomalyAtEpoch = TA;
        AP = omega_AP;
    }

    public void SetKeplerParams(KeplerParams keplerParams, UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 planetPosition2D, UnityEngine.Vector2 rocketVelocity2D, double gravityParam, double time)
    {
        KtoCfromC(rocketPosition2D, planetPosition2D,rocketVelocity2D, gravityParam, time, out keplerParams.semiMajorAxis, out keplerParams.eccentricity, out keplerParams.argumentOfPeriapsis, out keplerParams.longitudeOfAscendingNode, out keplerParams.inclination, out keplerParams.timeToPeriapsis, out keplerParams.trueAnomalyAtEpoch, out keplerParams.AP);
    }

    public void CalculateParametersHyperbolic(UnityEngine.Vector2 rocketPosition2D, UnityEngine.Vector2 rocketVelocity2D, UnityEngine.Vector2 planetPosition2D, float gravityParam, double time)
    {
        startTime = MyTime.time;
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
        Vector3 h_bar = UnityEngine.Vector3.Cross(rocketPosition3D, rocketVelocity3D);
        double h = rocketPosition3D.x * rocketVelocity3D.y - rocketPosition3D.y * rocketVelocity3D.x;

        //Calculate eccentricity vector
        UnityEngine.Vector3 eccentricity_bar = Vector3.Cross(rocketVelocity3D, h_bar)/gravityParam - rocketPosition3D/(float)r;
        e = eccentricity_bar.magnitude;
        

        //Calculate inclination
        i = Math.Atan2(-eccentricity_bar.y, -eccentricity_bar.x);

        //Calculate semi-major axis
        a  = 1/(2/r - Math.Pow(v, 2)/gravityParam);
        
        //Calculate raw position
        UnityEngine.Vector2 p = new UnityEngine.Vector2((float)(rocketPosition3D.x*Math.Cos(i)+rocketPosition3D.y*Math.Sin(i)), (float)(rocketPosition3D.y*Math.Cos(i)-rocketPosition3D.x*Math.Sin(i)));
        //Moon.transform.position = p;

        //Calculate Hyperbolic anomaly
        Ho = Math.Atanh(p.y/(a*Math.Sqrt(Math.Pow(e, 2)-1)) / (e-(p.x/a)));
        H = Ho;
        lastUpdatedTime = MyTime.time;
        
        Mo = Math.Sinh(Ho)*e-Ho;


        //Determine branch of hyperbola
        double dot = UnityEngine.Vector3.Dot(rocketPosition3D, rocketVelocity3D);
        double det = rocketPosition3D.x*rocketVelocity3D.y - rocketVelocity3D.x * rocketPosition3D.y;

        double angle = Math.Atan2(det, dot);

        //Calculate mean velocity
        n = Math.Sqrt(gravityParam/Math.Abs(Math.Pow(a, 3)))*Math.Sign(h);
    }

    public void GetOrbitalPositionHyperbolic(double Mo, double time, double Ho, double e, double a, double i, double n, double startTime, out double x, out double y, out double VX, out double VY)
    {
        //SEE COMMENT IN PREDICTION
        //Calculate mean anomaly
        double M = Mo + (time - startTime)*n;
        //Calculate current hyperbolic anomaly
        if(lastUpdatedTime != time)
        {
            H = H + ((M - e*Math.Sinh(H) + H) / (e*Math.Cosh(H)-1));
            lastUpdatedTime = time;
        }
        
        double rawX = a*(e - Math.Cosh(H));
        double rawY = a*Math.Sqrt(Math.Pow(e, 2)-1)*Math.Sinh(H);
        
        x = rawX*Math.Cos(i)-rawY*Math.Sin(i);
        y = rawX*Math.Sin(i)+rawY*Math.Cos(i);

        double t = (e * Math.Cosh(H)-1)/n;

        double rawVX = -a*Math.Sinh(H)/t;
        double rawVY = Math.Sqrt(Math.Pow(e, 2) - 1)* a * Math.Cosh(H)/t;
        VX = rawVX * Math.Cos(i) - rawVY*Math.Sin(i);
        VY = rawVX * Math.Sin(i) + rawVY*Math.Cos(i);
    }

}