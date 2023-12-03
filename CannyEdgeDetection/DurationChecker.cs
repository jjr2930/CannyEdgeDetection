using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CannyEdgeDetection
{
    /// <summary>
    /// 시간을 측정하는데 쉽게 할 수 있도록 만든 클래스
    /// </summary>
    public class DurationChecker
    {
        Stopwatch watch =new Stopwatch();

        /// <summary>
        /// 스톱워치를 초기화하고 실행한다.
        /// </summary>
        public void Start()
        {
            watch.Restart();
        }

        /// <summary>
        /// 스톱워치를 멈춘다.
        /// </summary>
        public void Stop()
        {
            watch.Stop();
        }

        /// <summary>
        /// 스톱워치를 멈추고 걸린시간을 출력한다.
        /// </summary>
        public void StopAndPrint() 
        {
            watch.Stop();
            Console.WriteLine($"duration : {watch.ElapsedMilliseconds}ms");
        }
    }
}
