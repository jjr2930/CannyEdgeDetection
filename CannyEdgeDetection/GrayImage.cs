using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CannyEdgeDetection
{
    /// <summary>
    /// 이미지를 읽고 그레이스케일로 전환하고,
    /// 각픽셀의 접근을 쉽게하기 위한 클래스
    /// </summary>
    public class GrayImage
    {
        /// <summary>
        /// 그레이스케일이라 float값 하나만 있으면 된다.
        /// </summary>
        int[,] color;

        /// <summary>
        /// 이미지의 높이 
        /// </summary>
        public int Height 
        {
            get => color.GetLength(0); 
        }
        /// <summary>
        /// 이미지의 너비
        /// </summary>
        public int Width
        {
            get => color.GetLength(1);
        }

        /// <summary>
        /// 화소를 얻어오기
        /// </summary>
        /// <param name="indexY"></param>
        /// <param name="indexX"></param>
        /// <returns></returns>
        public int this[int indexY, int indexX]
        {
            get
            {
                //배열 범위를 초과하면 0으로 리턴해준다.
                if (IsOutOfRange(indexX, indexY))
                {
                    return 0;
                }
                else
                {
                    return color[indexY, indexX];
                }
            }
            set
            {
                //배열 범위를 초과하면 아무일도 하지 않는다.
                if (IsOutOfRange(indexX, indexY))
                {
                    return;
                }
                else
                {
                    color[indexY, indexX] = value;
                }
            }
        }

        /// <summary>
        /// 비트맵을 이용해 color에 그레이스케일을 저장한다.
        /// </summary>
        /// <param name="bitmap"></param>
        public GrayImage(Bitmap bitmap)
        {
            color = new int[bitmap.Size.Height, bitmap.Size.Width];

            DurationChecker timer = new DurationChecker();
            timer.Start();
            for (int y = 0; y < bitmap.Size.Height; y++)
            {
                for (int x = 0; x < bitmap.Size.Width; x++)
                {
                    //단순하게 모든 색의 값을 더하고 3으로 나눈다.
                    var rgb = bitmap.GetPixel(x, y);
                    color[y, x] = (int)((rgb.R + rgb.G + rgb.B) / 3f);
                }
            }
            timer.StopAndPrint();
        }

        /// <summary>
        /// 주어진 크기로 color배열을 잡는다.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public GrayImage(int width, int height)
        {
            color = new int[height, width];
        }

        /// <summary>
        /// PictueBox에 넣기 위해 Bitmap으로 변환한다.
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {
            Bitmap newBitmap = new Bitmap(Width, Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int colorValue = color[y, x];

                    //값을 유요한 범위로 변환
                    if (colorValue < 0)
                        colorValue = 0;
                    if (colorValue > 255)
                        colorValue = 255;

                    Color c = Color.FromArgb(255, colorValue, colorValue, colorValue);
                    newBitmap.SetPixel(x, y, c);
                }
            }

            return newBitmap;
        }

        public bool IsOutOfRange(int x, int y)
        {
            if(0 <= x && x < Width
                && 0 <=y && y < Height)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 주어진 커널과 번볼루션 연산을 한다.
        /// </summary>
        /// <param name="image">이미지</param>
        /// <param name="kernel">커널</param>
        /// <returns>계산된 이미지 </returns>
        public static GrayImage Convolute(GrayImage image, float[,] kernel)
        {
            GrayImage result = new GrayImage(image.Width, image.Height);

            DurationChecker timer = new DurationChecker();
            timer.Start();
            //병렬 연산을 위해 parallel.for를 사용
            Parallel.For(0, image.Height, (y) =>
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int sum = 0;
                    int kernelHeight = kernel.GetLength(0);
                    int kernelWidth = kernel.GetLength(1);

                    for (int kx = 0; kx < kernelHeight; kx++)
                    {
                        for (int ky = 0; ky < kernelWidth; ky++)
                        {
                            int tx = x + kx;
                            int ty = y + ky;

                            bool invalidPosiiton = false;

                            //범위를 초과한 경우에는 흰색(255)으로 주고 계산하게 한다.                            
                            if (tx < 0)
                                invalidPosiiton = true;
                            if (ty < 0)
                                invalidPosiiton = true;
                            if (tx >= image.Width)
                                invalidPosiiton = true;
                            if (ty >= image.Height)
                                invalidPosiiton = true;

                            float color = 0f;
                            if (invalidPosiiton)
                            {
                                color = 255f;
                            }
                            else
                            {
                                color = image[ty, tx];
                            }

                            sum += (int)(color * kernel[ky, kx]);
                        }
                    }

                    result[y, x] = sum;
                }
            });
            timer.StopAndPrint();

            return result;
        }
    }
}
