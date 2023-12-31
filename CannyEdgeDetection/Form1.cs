using System.Diagnostics;
using System.Windows.Forms.VisualStyles;

namespace CannyEdgeDetection
{
    public enum SlopeType
    {
        /// <summary>
        /// 수평
        /// </summary>
        Horizontal,
        /// <summary>
        /// 수직
        /// </summary>
        Vertical,
        /// <summary>
        /// 우상대각선
        /// </summary>
        RightUp,
        /// <summary>
        /// 우하대각선
        /// </summary>
        RightDown
    }

    public partial class Form1 : Form
    {
        const int SOBEL_VERTICAL = 0;
        const int SOBEL_HORIZONTAL = 1;
        const float RADIAN_TO_DEGREE = 180f / MathF.PI;

        PictureBox originImageBox;
        PictureBox grayscaleImageBox;
        PictureBox gaussianBluredImageBox;
        PictureBox sobelHorizontalMaskedImageBox;
        PictureBox sobelVerticalMaskedImageBox;
        PictureBox gradientImageBox;
        PictureBox nonMaximumSupressionImageBox;
        PictureBox histeresisedImageBox;

        Label originImageLabel;
        Label grayscaleImageLabel;
        Label gaussianBluredImageLabel;
        Label sobelHorizontalMaskedImageLabel;
        Label sobelVerticalMaskedImageLabel;
        Label gradientImageLabel;
        Label nmpImageLabel;
        Label histeresisImageLabel;

        GrayImage grayscaleImage;
        GrayImage bluredImage;
        GrayImage sobelHorizontalMaksedImage;
        GrayImage sobelVerticalMaksedImage;
        GrayImage gradientMagnitudeImage;
        GrayImage nonMaximumSuppressionImage;
        GrayImage histeresisedImage;

        GaussianKernel gaussianKernel;
        SlopeType[,] gradientSlopes;
        OpenFileDialog dialog;

        float[][,] sobelMasks = new float[2][,]
        {
            new float[3,3]
            {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            },
            new float[3,3]
            {
                { 1, 2, 1 },
                { 0, 0, 0 },
                {-1, -2, -1 }
            }
        };


        public Form1()
        {
            InitializeComponent();

            this.ResizeEnd += OnResizeEnded;
        }

        //윈도우의 사이즈가 변경되면 imageContianer의 크기도 변경한다.
        private void OnResizeEnded(object? sender, EventArgs e)
        {
            ImageContainer.Left = 10;
            ImageContainer.Width = this.Width - 50;
            ImageContainer.Height = this.Height - 200;
        }

        private void DoCannyEdgeDetection()
        {
            Bitmap originBitmap = new Bitmap(dialog.FileName);
            originImageBox.Image = originBitmap;

            //이미지를 그레이스케일로 변환한다.
            Console.WriteLine("Grayscale");
            grayscaleImage = new GrayImage(originBitmap);
            grayscaleImageBox.Image = grayscaleImage.ToBitmap();

            //가우시안 블러된 이미지를 얻어온다.
            Console.WriteLine("blur using gaussian blur");
            bluredImage = GrayImage.Convolute(grayscaleImage, gaussianKernel.kernelMatrix);
            gaussianBluredImageBox.Image = bluredImage.ToBitmap();

            //세로 소벨 마스킹
            Console.WriteLine("vertical sobel");
            sobelVerticalMaksedImage = GrayImage.Convolute(bluredImage, sobelMasks[SOBEL_VERTICAL]);
            sobelVerticalMaskedImageBox.Image = sobelVerticalMaksedImage.ToBitmap();


            //가로 소벨 마스킹
            Console.WriteLine("horizontal sobel");
            sobelHorizontalMaksedImage = GrayImage.Convolute(bluredImage, sobelMasks[SOBEL_HORIZONTAL]);
            sobelHorizontalMaskedImageBox.Image = sobelHorizontalMaksedImage.ToBitmap();

            //그레디언트 구하기 수행
            Console.WriteLine("gradient");

            int width = sobelVerticalMaksedImage.Width;
            int height = sobelVerticalMaksedImage.Height;
            gradientMagnitudeImage = new GrayImage(width, height);
            gradientSlopes = new SlopeType[height, width];

            DoGradient(sobelVerticalMaksedImage, sobelHorizontalMaksedImage, gradientMagnitudeImage, gradientSlopes);
            gradientImageBox.Image = gradientMagnitudeImage.ToBitmap();

            //최대값 억제 수행
            Console.WriteLine("NonMaximumSuppression");
            nonMaximumSuppressionImage = DoNonMaxiumSuppression(gradientMagnitudeImage, gradientSlopes);
            nonMaximumSupressionImageBox.Image = nonMaximumSuppressionImage.ToBitmap();

            //히스테리시스를 수행한다.
            Console.WriteLine("Histeresis");
            histeresisedImage = DoHisteresis(nonMaximumSuppressionImage, (float)LowThresholdUpDown.Value, (float)HighThresholdUpDown.Value);
            histeresisedImageBox.Image = histeresisedImage.ToBitmap();

            //picturebox들의 위치를 갱신
            RepositionControls();
        }

        /// <summary>
        /// 만약 낮은 임계값이 높은 임계값보다 커진다면 둘을 교한한다.
        /// </summary>
        private void SwapHighAndLowIfNeed()
        {
            if (LowThresholdUpDown.Value > HighThresholdUpDown.Value)
            {
                var temp = LowThresholdUpDown.Value;
                LowThresholdUpDown.Value = HighThresholdUpDown.Value;
                HighThresholdUpDown.Value = temp;
            }
        }

        /// <summary>
        /// 모든 control들의 위치를 정한다.
        /// </summary>
        void RepositionControls()
        {
            Point point = new Point(10, 100);
            SetPictureBoxPosition(point.X, point.Y, originImageBox);
            SetLabelPositionByPictureBox(originImageLabel, originImageBox);
            
            point.X += originImageBox.Width + 10;
            SetPictureBoxPosition(point.X, point.Y, grayscaleImageBox);
            SetLabelPositionByPictureBox(grayscaleImageLabel, grayscaleImageBox);

            point.X += grayscaleImageBox.Width + 10;
            SetPictureBoxPosition(point.X, point.Y, gaussianBluredImageBox);
            SetLabelPositionByPictureBox(gaussianBluredImageLabel, gaussianBluredImageBox);

            point.X += gaussianBluredImageBox.Width + 10;
            SetPictureBoxPosition(point.X, point.Y, sobelHorizontalMaskedImageBox);
            SetLabelPositionByPictureBox(sobelHorizontalMaskedImageLabel, sobelHorizontalMaskedImageBox);

            point.X += sobelHorizontalMaskedImageBox.Width + 10;
            SetPictureBoxPosition(point.X, point.Y, sobelVerticalMaskedImageBox); 
            SetLabelPositionByPictureBox(sobelVerticalMaskedImageLabel, sobelVerticalMaskedImageBox);

            point.X += sobelVerticalMaskedImageBox.Width + 10;
            SetPictureBoxPosition(point.X, point.Y, gradientImageBox);
            SetLabelPositionByPictureBox(gradientImageLabel, gradientImageBox);

            point.X += gradientImageBox.Width + 10;
            SetPictureBoxPosition(point.X, point.Y, nonMaximumSupressionImageBox);
            SetLabelPositionByPictureBox(nmpImageLabel, nonMaximumSupressionImageBox);

            point.X += nonMaximumSupressionImageBox.Width + 10;
            SetPictureBoxPosition(point.X, point.Y, histeresisedImageBox);
            SetLabelPositionByPictureBox(histeresisImageLabel, histeresisedImageBox);
        }

        /// <summary>
        /// 주어진 픽처 박스를 통해 라벨의 위치를 정한다.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="pictureBox"></param>
        void SetLabelPositionByPictureBox(Label label, PictureBox pictureBox)
        {
            label.Refresh();
            label.Left = pictureBox.Left;
            label.Top = pictureBox.Bottom + 10;
        }

        /// <summary>
        /// Picture box의 위치와 크기를 주어진 값을 이용해 변경한다.
        /// </summary>
        /// <param name="x">x위치</param>
        /// <param name="y">y위치</param>
        /// <param name="pictureBox">변경할 picturebox</param>
        void SetPictureBoxPosition(int x, int y, PictureBox pictureBox)
        {
            pictureBox.Width = pictureBox.Image.Width;
            pictureBox.Height = pictureBox.Image.Height;

            pictureBox.Left = x;
            pictureBox.Top = y;
        }

        /// <summary>
        /// 비최대 억제 알고리즘을 수행한다.
        /// </summary>
        /// <param name="gradientImage">그레디언트 이미지</param>
        /// <param name="gradientSlopes">화소별 그레디언트의 각도가 저장된 배열</param>
        /// <returns></returns>
        private GrayImage DoNonMaxiumSuppression(GrayImage gradientImage, SlopeType[,] gradientSlopes)
        {
            GrayImage result = new GrayImage(gradientImage.Width, gradientImage.Height);
            DurationChecker timer = new DurationChecker();
            timer.Start();
            Parallel.For(0, gradientImage.Height, (y) =>
            {
                for (int x = 0; x < gradientImage.Width; ++x)
                {
                    SlopeType slope = gradientSlopes[y, x];
                    int[] pixel = new int[3];
                    Point[] positions = new Point[3];
                    //방향별로 비교할 위치를 얻어온다.
                    switch (slope)
                    {
                        case SlopeType.Horizontal:
                            {
                                positions[0] = new Point(x - 1, y);
                                positions[1] = new Point(x, y);
                                positions[2] = new Point(x + 1, y);
                            }
                            break;
                        case SlopeType.Vertical:
                            {
                                positions[0] = new Point(x, y - 1);
                                positions[1] = new Point(x, y);
                                positions[2] = new Point(x, y + 1);
                            }
                            break;
                        case SlopeType.RightUp:
                            {
                                positions[0] = new Point(x - 1, y - 1);
                                positions[1] = new Point(x, y);
                                positions[2] = new Point(x + 1, y + 1);
                            }
                            break;
                        case SlopeType.RightDown:
                            {

                                positions[0] = new Point(x - 1, y + 1);
                                positions[1] = new Point(x, y);
                                positions[2] = new Point(x + 1, y - 1);
                            }
                            break;
                        default:
                            break;
                    }

                    //얻은 위치의 픽셀들을 얻어온다.
                    pixel[0] = gradientImage[positions[0].Y, positions[0].X];
                    pixel[1] = gradientImage[positions[1].Y, positions[1].X];
                    pixel[2] = gradientImage[positions[2].Y, positions[2].X];

                    //pixel[1]가 최대값이면 살리고 나머지는 0으로 보냄
                    if (pixel[1] >= pixel[0] && pixel[1] >= pixel[2])
                    {
                        result[positions[0].Y, positions[0].X] = 0;
                        result[positions[1].Y, positions[1].X] = pixel[1];
                        result[positions[2].Y, positions[2].X] = 0;
                    }
                    //pixel[1]가 최대값이 아니면 0으로 만듬
                    else
                    {
                        result[positions[1].Y, positions[1].X] = 0;
                    }
                }
            });
            timer.StopAndPrint();

            return result;
        }

        /// <summary>
        /// 그래디언트를 구한다.
        /// </summary>
        /// <param name="verticalGradient">수직 소벨 마스크를 컨볼류션 곱을 한 이미지 </param>
        /// <param name="horizontalGradient">수평 소벨 마스크를 컨볼류션 곱을 한 이미지</param>
        /// <param name="gradientImage">그레디언트를 구한 이미지</param>
        /// <param name="slopeTypes">픽셀별 구해진 각도를 6방향으로 저장하는 곳</param>
        void DoGradient(GrayImage verticalGradient, GrayImage horizontalGradient, GrayImage gradientImage, SlopeType[,] slopeTypes)
        {
            int width = verticalGradient.Width;
            int height = verticalGradient.Height;

            DurationChecker timer = new DurationChecker();
            timer.Start();
            //병렬 연산을 위해 parallel.for를 사용
            Parallel.For(0, height, (y) =>
            {
                for (int x = 0; x < width; x++)
                {
                    float horizontalValue = (float)horizontalGradient[y, x];
                    float verticalValue = (float)verticalGradient[y, x];
                    //그레디언트를 구한다.
                    gradientImage[y, x] = (int)(Math.Sqrt(horizontalValue * horizontalValue + verticalValue * verticalValue));

                    float angle = MathF.Atan2(horizontalValue, verticalValue);
                    //호도법의 angle을 구한다.
                    angle *= RADIAN_TO_DEGREE;
                    //구해진 값을 8방향으로 쪼개고 이를 저장한다.
                    if (-22.5f < angle && angle <= 22.5f)
                    {
                        slopeTypes[y, x] = SlopeType.Horizontal;
                    }
                    else if (22.5f < angle && angle <= 67.5f)
                    {
                        slopeTypes[y, x] = SlopeType.RightUp;
                    }
                    else if (67.5f < angle && angle <= 112.5f)
                    {
                        slopeTypes[y, x] = SlopeType.Vertical;
                    }
                    else if (112.5f < angle && angle <= 157.5f)
                    {
                        slopeTypes[y, x] = SlopeType.RightDown;
                    }
                    else if (157.5f < angle && angle <= 180f
                        || -180f <= angle && angle < 157.5f)
                    {
                        slopeTypes[y, x] = SlopeType.Horizontal;
                    }
                    else if (-157.5f < angle && angle <= -112.5f)
                    {
                        slopeTypes[y, x] = SlopeType.RightUp;
                    }
                    else if (-112.5f < angle && angle <= -67.5f)
                    {
                        slopeTypes[y, x] = SlopeType.Vertical;
                    }
                    else if (-67.5f < angle && angle <= -22.5f)
                    {
                        slopeTypes[y, x] = SlopeType.RightDown;
                    }
                }
            });
            timer.StopAndPrint();
        }

        /// <summary>
        /// 히스테리시스를 수행한다.
        /// </summary>
        /// <param name="nonMaximumSuppressionImage">비최대억제 결과가 저장된 이미지</param>
        /// <param name="lowThreshold">낮은 임계값, 이 밑의 픽셀은 검은색으로 표현한다.</param>
        /// <param name="highThreshold">높은 임계값, 이 위의 픽셀 값은 흰색으로 표현한다.</param>
        /// <returns></returns>
        private GrayImage DoHisteresis(GrayImage nonMaximumSuppressionImage, float lowThreshold, float highThreshold)
        {
            GrayImage result = new GrayImage(nonMaximumSuppressionImage.Width, nonMaximumSuppressionImage.Height);

            DurationChecker timer = new DurationChecker();
            timer.Start();
            //병렬 연산을 위해 parallel.for를 사용
            Parallel.For(0, result.Height, (y) =>
            {
                //모든 픽셀을 검사한다.
                for (int x = 0; x < result.Width; x++)
                {
                    float pixel = nonMaximumSuppressionImage[y, x];
                    //낮은 임계 값 미만이면 검은 색으로 표시
                    if (pixel < lowThreshold)
                    {
                        result[y, x] = 0;
                    }
                    //높은 임계 값 이상이면 흰색으로 표시
                    else if (pixel >= highThreshold)
                    {
                        result[y, x] = 255;
                    }
                    else
                    {
                        //이 픽셀이 높은 임계값을 갖는 픽셀과 이어져 있는지 알기 위해서는 너비 우선 탐색을 이용한 길찾기를 하면 된다.
                        //너비 우선 탐색을 통해 높은 임계값을 갖는 필셀과 이어져 있다면, 이 화소를 흰색으로 표현한다.
                        if (BFS(nonMaximumSuppressionImage, x, y))
                        {
                            result[y, x] = 255;
                        }
                        //그렇지 않으면 검은색으로 표현해야하지만 이미지의 기본값은 검은색으로 표현되어있으니 아무일 안해도 된다.
                        //else
                        //{
                        //    result[y, x] = 0;
                        //}
                    }
                }
            });

            timer.StopAndPrint();
            return result;
        }

        /// <summary>
        /// 너비우선 탐색을 통하여 높은 임계값 이상의 화소와 이어져 있는지 검사한다.
        /// </summary>
        /// <param name="nmsImage">non maximum suprression 를 거친 이미지</param>
        /// <param name="x">너비 우선 탐색을 할 픽셀의 x좌표</param>
        /// <param name="y">너비 우선 탐색을 할 픽셀의 y좌표</param>
        /// <returns>높은 임계값을 넘는 점과 연결 되었다면 true, 아니면 false </returns>
        bool BFS(GrayImage nmsImage, int x, int y)
        {
            var visited = new bool[nmsImage.Height, nmsImage.Width];
            Queue<Point> q = new Queue<Point>();
            //시작 지점을 큐에 넣는다.
            q.Enqueue(new Point(x, y));
            //큐가 빌 떄까지 수행한다.
            while (q.Count > 0)
            {
                Point point = q.Dequeue();

                //out of range면 넘어간다.
                if (point.X < 0)
                    continue;
                if (point.Y < 0)
                    continue;
                if (point.X >= nmsImage.Width)
                    continue;
                if (point.Y >= nmsImage.Height)
                    continue;

                //방문했으면 넘어간다.
                if (visited[point.Y, point.X])
                {
                    continue;
                }
                //방문했다는 표시
                visited[point.Y, point.X] = true;

                //해당 픽셀의 값을 가져온다.
                var nmsValue = nmsImage[point.Y, point.X];

                //높은 임계값을 통과하는 픽셀과 연결 되었다면 이것은 엣지다.
                if (nmsValue >= (float)HighThresholdUpDown.Value)
                {
                    return true;
                }
                //낮은 임계값보다 낮은 값을 갖는 픽셀을 만났다면 이 방향으로는 가면 안된다.
                else if (nmsValue < (float)LowThresholdUpDown.Value)
                {
                    continue;
                }
                //그 사이에 있는 값이면 계속 탐색한다.
                else
                {
                    //주변 이웃들을 큐에 넣는다.
                    q.Enqueue(new Point(point.X - 1, point.Y - 1));
                    q.Enqueue(new Point(point.X, point.Y - 1));
                    q.Enqueue(new Point(point.X + 1, point.Y - 1));
                    q.Enqueue(new Point(point.X - 1, point.Y));
                    q.Enqueue(new Point(point.X + 1, point.Y));
                    q.Enqueue(new Point(point.X - 1, point.Y + 1));
                    q.Enqueue(new Point(point.X, point.Y + 1));
                    q.Enqueue(new Point(point.X + 1, point.Y + 1));
                }
            }

            //높은 임계값을 통과하는 픽셀을 만나지 못한것이다. 그러므로 이것은 엣지가 아니다
            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            originImageBox = new PictureBox();
            grayscaleImageBox = new PictureBox();
            gaussianBluredImageBox = new PictureBox();
            sobelHorizontalMaskedImageBox = new PictureBox();
            sobelVerticalMaskedImageBox = new PictureBox();
            gradientImageBox = new PictureBox();
            nonMaximumSupressionImageBox = new PictureBox();
            histeresisedImageBox = new PictureBox();

            originImageLabel = new Label();
            grayscaleImageLabel = new Label();
            gaussianBluredImageLabel = new Label();
            sobelHorizontalMaskedImageLabel = new Label();
            sobelVerticalMaskedImageLabel = new Label();
            gradientImageLabel = new Label();
            nmpImageLabel = new Label();
            histeresisImageLabel = new Label();

            originImageLabel.Text = "Origin";
            grayscaleImageLabel.Text = "Gray";
            gaussianBluredImageLabel.Text = "Blured";
            sobelHorizontalMaskedImageLabel.Text = "Sobel H";
            sobelVerticalMaskedImageLabel.Text = "Sobel V";
            gradientImageLabel.Text = "Gradient";
            nmpImageLabel.Text = "NMP";
            histeresisImageLabel.Text = "histeresis";


            ImageContainer.Controls.Add(originImageBox);
            ImageContainer.Controls.Add(grayscaleImageBox);
            ImageContainer.Controls.Add(gaussianBluredImageBox);
            ImageContainer.Controls.Add(sobelHorizontalMaskedImageBox);
            ImageContainer.Controls.Add(sobelVerticalMaskedImageBox);
            ImageContainer.Controls.Add(gradientImageBox);
            ImageContainer.Controls.Add(nonMaximumSupressionImageBox);
            ImageContainer.Controls.Add(histeresisedImageBox);

            //add label into imagecontainer
            ImageContainer.Controls.Add(originImageLabel);
            ImageContainer.Controls.Add(grayscaleImageLabel);
            ImageContainer.Controls.Add(gaussianBluredImageLabel);
            ImageContainer.Controls.Add(sobelHorizontalMaskedImageLabel);
            ImageContainer.Controls.Add(sobelVerticalMaskedImageLabel);
            ImageContainer.Controls.Add(gradientImageLabel);
            ImageContainer.Controls.Add(nmpImageLabel);
            ImageContainer.Controls.Add(histeresisImageLabel);


            gaussianKernel = new GaussianKernel(3, 5f);
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialog = new OpenFileDialog();
            dialog.Filter = "jpg(*.jpg)|*.jpg|jpeg(*.jpeg)|*.jpeg|png(*.png)|*.png";
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                //이미지를 열고, 캐니엣지 탐지 알고리즘을 수행한다.
                DoCannyEdgeDetection();
            }
        }

        /// <summary>
        /// 블러 사이즈가 변경된 경우 호출된다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlurSizeUpDown_ValueChanged(object sender, EventArgs e)
        {
            //가우시안 블러를 다시 만들고
            gaussianKernel = new GaussianKernel((int)BlurSizeUpDown.Value, (float)BlurSigmaUpdown.Value);
            //캐니 엣지 탐색을 수행한다.
            DoCannyEdgeDetection();
        }

        /// <summary>
        /// 낮은 임계값 변경된 경우 호출된다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LowThresholdUpDown_ValueChanged(object sender, EventArgs e)
        {
            //만약 low가 high보다 크다면 둘을 바꾼다.
            SwapHighAndLowIfNeed();
            //변경된 값을 이용하여 히스테리시스를 수행한다.
            histeresisedImage = DoHisteresis(nonMaximumSuppressionImage, (float)LowThresholdUpDown.Value, (float)HighThresholdUpDown.Value);
            histeresisedImageBox.Image = histeresisedImage.ToBitmap();
        }

        /// <summary>
        /// 높은 임계값이 변경된 경우 호출된다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HighThresholdUpDown_ValueChanged(object sender, EventArgs e)
        {
            //만약 low가 high보다 크다면 둘을 바꾼다.
            SwapHighAndLowIfNeed();
            //변경된 값을 이용하여 히스테리 시스를 수행한다.
            histeresisedImage = DoHisteresis(nonMaximumSuppressionImage, (float)LowThresholdUpDown.Value, (float)HighThresholdUpDown.Value);
            histeresisedImageBox.Image = histeresisedImage.ToBitmap();
        }

        /// <summary>
        /// 블러의 시그마(표준 편차)가 변경되었을 경우 호출됨
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlurSigmaUpdown_ValueChanged(object sender, EventArgs e)
        {
            //변경된 값으로 가우시안 커널을 다시 만들고,
            gaussianKernel = new GaussianKernel((int)BlurSizeUpDown.Value, (float)BlurSigmaUpdown.Value);

            //에지 탐색 수행
            DoCannyEdgeDetection();
        }
    }
}
