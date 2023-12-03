using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CannyEdgeDetection
{
    public class GaussianKernel
    {
        public int size;
        public float[,] kernelMatrix;

        public GaussianKernel(int size, float sigma)
        {
            this.size = size;
            kernelMatrix = BuildGaussianKernel(size, sigma);
        }


        public static float[,] BuildGaussianKernel(int size, double sigma)
        {
            double[,] kernel = new double[size, size];
            double kernelSum = 0;
            int foff = (size - 1) / 2;
            double distance = 0;
            double constant = 1d / Math.Sqrt((2 * Math.PI * sigma * sigma));

            for (int y = -foff; y <= foff; y++)
            {
                for (int x = -foff; x <= foff; x++)
                {
                    distance = Math.Pow((x - y), 2) / (2 * sigma * sigma);
                    kernel[y + foff, x + foff] = constant * Math.Exp(-distance);
                    kernelSum += kernel[y + foff, x + foff];
                }
            }

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    kernel[y, x] = kernel[y, x] / kernelSum;
                }
            }

            float[,] floatKernel = new float[size, size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    floatKernel[y, x] = (float)kernel[y, x];
                }
            }
            return floatKernel;
        }
    }
}
