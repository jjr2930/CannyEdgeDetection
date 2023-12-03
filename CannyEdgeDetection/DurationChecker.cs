using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CannyEdgeDetection
{
    public class DurationChecker
    {
        Stopwatch watch =new Stopwatch();
        public void Start()
        {
            watch.Restart();
        }

        public void Stop()
        {
            watch.Stop();
        }

        public void StopAndPrint() 
        {
            watch.Stop();
            Console.WriteLine($"duration : {watch.ElapsedMilliseconds}ms");
        }
    }
}
