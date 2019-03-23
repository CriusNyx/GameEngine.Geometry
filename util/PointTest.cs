using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PointTest : MonoBehaviour
{
    public void Start()
    {
        for(int i = 0; i < 15; i++)
        {
            int size = (int)Mathf.Pow(2, i);
            PrefTest(size);
        }
        //if(StressTest((int)Mathf.Pow(2, 15), 500))
        //{
        //    Debug.Log("Pass");
        //}
        //else
        //{
        //    Debug.Log("Fail");
        //}
    }

    private void PrefTest(int size)
    {
        PointDictionary<string> dic = new PointDictionary<string>();
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        for(int i = 0; i < size; i++)
        {
            dic.Add(Random.insideUnitSphere * Random.value * 100000f, "random");
        }
        stopwatch.Stop();
        long num = stopwatch.ElapsedMilliseconds;
        Debug.Log(size + ":" + stopwatch.ElapsedMilliseconds);

        stopwatch.Reset();
        stopwatch.Start();
        Vector3 a = Random.insideUnitSphere * Random.value * 100000f;
        Vector3 b = a + Random.insideUnitSphere * Random.value * 0.0000001f;
        dic.Add(a, "Fuck");
        if(dic.Get(b) != "Fuck")
        {
            Debug.Log("Error");
        }
        stopwatch.Stop();
        
        Debug.Log("Fetch Time = " + size + ":" + stopwatch.ElapsedMilliseconds);
    }

    private bool StressTest(int size, int repeatCount)
    {
        PointDictionary<string> dic = new PointDictionary<string>();
        for(int i = 0; i < size; i++)
        {
            dic.Add(Random.insideUnitSphere * Random.value * 100000f, "random");
        }

        for(int i = 0; i < repeatCount; i++)
        {
            Vector3 a = Random.insideUnitSphere * Random.value * 100000f;
            Vector3 b = Random.insideUnitSphere * Random.value * 100000f;
            while((b - a).sqrMagnitude < 1f)
            {
                b = Random.insideUnitSphere * Random.value * 100000f;
            }
            string val = "test: + i";
            dic.Add(a, val);
            if(dic.Get(b) == val)
            {
                return false;
            }
        }
        return true;
    }
}