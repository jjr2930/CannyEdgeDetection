using System.Diagnostics;
using System.Windows.Forms.VisualStyles;

namespace CannyEdgeDetection
{
    public enum SlopeType
    {
        /// <summary>
        /// ����
        /// </summary>
        Horizontal,
        /// <summary>
        /// ����
        /// </summary>
        Vertical,
        /// <summary>
        /// ���밢��
        /// </summary>
        RightUp,
        /// <summary>
        /// ���ϴ밢��
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
        PictureBox gradientMagnitudeImageBox;
        PictureBox nonMaximumSupressionImageBox;
        PictureBox histeresisedImageBox;

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

        DurationChecker durationChecker = new DurationChecker();

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

        //�������� ����� ����Ǹ� imageContianer�� ũ�⵵ �����Ѵ�.
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

            //�̹����� �׷��̽����Ϸ� ��ȯ�Ѵ�.
            Console.WriteLine("Grayscale");
            durationChecker.Start();
            grayscaleImage = new GrayImage(originBitmap);
            durationChecker.StopAndPrint();
            grayscaleImageBox.Image = grayscaleImage.ToBitmap();

            //����þ� ���� �̹����� ���´�.
            Console.WriteLine("blur using gaussian blur");
            durationChecker.Start();
            bluredImage = GrayImage.Convolute(grayscaleImage, gaussianKernel.kernelMatrix);
            durationChecker.StopAndPrint();
            gaussianBluredImageBox.Image = bluredImage.ToBitmap();

            //���� �Һ� ����ŷ
            Console.WriteLine("vertical sobel");
            durationChecker.Start();
            sobelVerticalMaksedImage = GrayImage.Convolute(bluredImage, sobelMasks[SOBEL_VERTICAL]);
            durationChecker.StopAndPrint();
            sobelVerticalMaskedImageBox.Image = sobelVerticalMaksedImage.ToBitmap();


            //���� �Һ� ����ŷ
            Console.WriteLine("horizontal sobel");
            durationChecker.Start();
            sobelHorizontalMaksedImage = GrayImage.Convolute(bluredImage, sobelMasks[SOBEL_HORIZONTAL]);
            durationChecker.StopAndPrint();
            sobelHorizontalMaskedImageBox.Image = sobelHorizontalMaksedImage.ToBitmap();

            //�׷����Ʈ ���ϱ� ����
            Console.WriteLine("gradient");
            
            int width = sobelVerticalMaksedImage.Width;
            int height = sobelVerticalMaksedImage.Height;
            gradientMagnitudeImage = new GrayImage(width, height);
            gradientSlopes = new SlopeType[height, width];

            durationChecker.Start();
            DoGradient(sobelVerticalMaksedImage, sobelHorizontalMaksedImage, gradientMagnitudeImage, gradientSlopes);
            durationChecker.StopAndPrint();
            gradientMagnitudeImageBox.Image = gradientMagnitudeImage.ToBitmap();

            //�ִ밪 ���� ����
            Console.WriteLine("NonMaximumSuppression");
            durationChecker.Start();
            nonMaximumSuppressionImage = DoNonMaxiumSuppression(gradientMagnitudeImage, gradientSlopes);
            durationChecker.StopAndPrint();
            nonMaximumSupressionImageBox.Image = nonMaximumSuppressionImage.ToBitmap();

            //�����׸��ý��� �����Ѵ�.
            Console.WriteLine("Histeresis");
            durationChecker.Start();
            histeresisedImage = DoHisteresis(nonMaximumSuppressionImage, (float)LowThresholdUpDown.Value, (float)HighThresholdUpDown.Value);
            durationChecker.StopAndPrint();
            histeresisedImageBox.Image = histeresisedImage.ToBitmap();

            //picturebox���� ��ġ�� ����
            RepositionControls();
        }

        /// <summary>
        /// ���� ���� �Ӱ谪�� ���� �Ӱ谪���� Ŀ���ٸ� ���� �����Ѵ�.
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
        /// ��� picture box�� ��ġ�� ���Ѵ�.
        /// </summary>
        void RepositionControls()
        {
            Point point = new Point(10, 100);
            SetImageBoxPosition(point.X, point.Y, originImageBox);

            point.X += originImageBox.Width + 10;
            SetImageBoxPosition(point.X, point.Y, grayscaleImageBox);

            point.X += grayscaleImageBox.Width + 10;
            SetImageBoxPosition(point.X, point.Y, gaussianBluredImageBox);

            point.X += gaussianBluredImageBox.Width + 10;
            SetImageBoxPosition(point.X, point.Y, sobelHorizontalMaskedImageBox);

            point.X += sobelHorizontalMaskedImageBox.Width + 10;
            SetImageBoxPosition(point.X, point.Y, sobelVerticalMaskedImageBox);

            point.X += sobelVerticalMaskedImageBox.Width + 10;
            SetImageBoxPosition(point.X, point.Y, gradientMagnitudeImageBox);

            point.X += gradientMagnitudeImageBox.Width + 10;
            SetImageBoxPosition(point.X, point.Y, nonMaximumSupressionImageBox);

            point.X += nonMaximumSupressionImageBox.Width + 10;
            SetImageBoxPosition(point.X, point.Y, histeresisedImageBox);
        }

        /// <summary>
        /// Picture box�� ��ġ�� ũ�⸦ �־��� ���� �̿��� �����Ѵ�.
        /// </summary>
        /// <param name="x">x��ġ</param>
        /// <param name="y">y��ġ</param>
        /// <param name="pictureBox">������ picturebox</param>
        void SetImageBoxPosition(int x, int y, PictureBox pictureBox)
        {
            pictureBox.Width = pictureBox.Image.Width;
            pictureBox.Height = pictureBox.Image.Height;

            pictureBox.Left = x;
            pictureBox.Top = y;
        }

        /// <summary>
        /// ���ִ� ���� �˰����� �����Ѵ�.
        /// </summary>
        /// <param name="gradientImage">�׷����Ʈ �̹���</param>
        /// <param name="gradientSlopes">ȭ�Һ� �׷����Ʈ�� ������ ����� �迭</param>
        /// <returns></returns>
        private GrayImage DoNonMaxiumSuppression(GrayImage gradientImage, SlopeType[,] gradientSlopes)
        {
            GrayImage result = new GrayImage(gradientImage.Width, gradientImage.Height);

            Parallel.For(0, gradientImage.Height, (y) => 
            {
                for (int x = 0; x < gradientImage.Width; ++x)
                {
                    SlopeType slope = gradientSlopes[y, x];
                    int[] pixel = new int[3];
                    Point[] positions = new Point[3];
                    //���⺰�� ���� ��ġ�� ���´�.
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

                    //���� ��ġ�� �ȼ����� ���´�.
                    pixel[0] = gradientImage[positions[0].Y, positions[0].X];
                    pixel[1] = gradientImage[positions[1].Y, positions[1].X];
                    pixel[2] = gradientImage[positions[2].Y, positions[2].X];

                    //pixel[1]�� �ִ밪�̸� �츮�� �������� 0���� ����
                    if (pixel[1] >= pixel[0] && pixel[1] >= pixel[2])
                    {
                        result[positions[0].Y, positions[0].X] = 0;
                        result[positions[1].Y, positions[1].X] = pixel[1];
                        result[positions[2].Y, positions[2].X] = 0;
                    }
                    //pixel[1]�� �ִ밪�� �ƴϸ� 0���� ����
                    else
                    {
                        result[positions[1].Y, positions[1].X] = 0;
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// �׷����Ʈ�� ���Ѵ�.
        /// </summary>
        /// <param name="verticalGradient">���� �Һ� ����ũ�� �������� ���� �� �̹��� </param>
        /// <param name="horizontalGradient">���� �Һ� ����ũ�� �������� ���� �� �̹���</param>
        /// <param name="gradientImage">�׷����Ʈ�� ���� �̹���</param>
        /// <param name="slopeTypes">�ȼ��� ������ ������ 6�������� �����ϴ� ��</param>
        void DoGradient(GrayImage verticalGradient, GrayImage horizontalGradient, GrayImage gradientImage, SlopeType[,] slopeTypes)
        {
            int width = verticalGradient.Width;
            int height = verticalGradient.Height;

            Parallel.For(0, height, (y) =>
            {
                for (int x = 0; x < width; x++)
                {
                    float horizontalValue = (float)horizontalGradient[y, x];
                    float verticalValue = (float)verticalGradient[y, x];
                    //�׷����Ʈ�� ���Ѵ�.
                    gradientImage[y, x] = (int)(Math.Sqrt(horizontalValue * horizontalValue + verticalValue * verticalValue));

                    float angle = MathF.Atan2(horizontalValue, verticalValue);
                    //ȣ������ angle�� ���Ѵ�.
                    angle *= RADIAN_TO_DEGREE;
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
        }

        /// <summary>
        /// �����׸��ý��� �����Ѵ�.
        /// </summary>
        /// <param name="nonMaximumSuppressionImage">���ִ���� ����� ����� �̹���</param>
        /// <param name="lowThreshold">���� �Ӱ谪, �� ���� �ȼ��� ���������� ǥ���Ѵ�.</param>
        /// <param name="highThreshold">���� �Ӱ谪, �� ���� �ȼ� ���� ������� ǥ���Ѵ�.</param>
        /// <returns></returns>
        private GrayImage DoHisteresis(GrayImage nonMaximumSuppressionImage, float lowThreshold, float highThreshold)
        {
            GrayImage result = new GrayImage(nonMaximumSuppressionImage.Width, nonMaximumSuppressionImage.Height);

            Parallel.For(0, result.Height, (y) =>
            {
                //��� �ȼ��� �˻��Ѵ�.
                for (int x = 0; x < result.Width; x++)
                {
                    float pixel = nonMaximumSuppressionImage[y, x];
                    if (pixel < lowThreshold)
                    {
                        result[y, x] = 0;
                    }
                    else if (pixel >= highThreshold)
                    {
                        result[y, x] = 255;
                    }
                    else
                    {
                        //�� �ȼ��� ���� �Ӱ谪�� ���� �ȼ��� �̾��� �ִ��� �˱� ���ؼ��� �ʺ� �켱 Ž���� �̿��� ��ã�⸦ �ϸ� �ȴ�.
                        //�ʺ� �켱 Ž���� ���� ���� �Ӱ谪�� ���� �ʼ��� �̾��� �ִٸ�, �� ȭ�Ҹ� ������� ǥ���Ѵ�.
                        if (BFS(nonMaximumSuppressionImage, x, y))
                        {
                            result[y, x] = 255;
                        }
                        //�׷��� ������ ���������� ǥ���ؾ������� �̹����� �⺻���� ���������� ǥ���Ǿ������� �ƹ��� ���ص� �ȴ�.
                        //else
                        //{
                        //    result[y, x] = 0;
                        //}
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// �ʺ�켱 Ž���� ���Ͽ� ���� �Ӱ谪 �̻��� ȭ�ҿ� �̾��� �ִ��� �˻��Ѵ�.
        /// </summary>
        /// <param name="nmsImage">non maximum suprression �� ��ģ �̹���</param>
        /// <param name="x">�ʺ� �켱 Ž���� �� �ȼ��� x��ǥ</param>
        /// <param name="y">�ʺ� �켱 Ž���� �� �ȼ��� y��ǥ</param>
        /// <returns>���� �Ӱ谪�� �Ѵ� ���� ���� �Ǿ��ٸ� true, �ƴϸ� false </returns>
        bool BFS(GrayImage nmsImage, int x, int y)
        {
            var visited = new bool[nmsImage.Height, nmsImage.Width];
            Queue<Point> q = new Queue<Point>();
            //���� ������ ť�� �ִ´�.
            q.Enqueue(new Point(x, y));
            //ť�� �� ������ �����Ѵ�.
            while (q.Count > 0)
            {
                Point point = q.Dequeue();

                //out of range�� �Ѿ��.
                if (point.X < 0)
                    continue;
                if (point.Y < 0)
                    continue;
                if (point.X >= nmsImage.Width)
                    continue;
                if (point.Y >= nmsImage.Height)
                    continue;

                //�湮������ �Ѿ��.
                if (visited[point.Y, point.X])
                {
                    continue;
                }
                //�湮�ߴٴ� ǥ��
                visited[point.Y, point.X] = true;

                //�ش� �ȼ��� ���� �����´�.
                var nmsValue = nmsImage[point.Y, point.X];

                //���� �Ӱ谪�� ����ϴ� �ȼ��� ���� �Ǿ��ٸ� �̰��� ������.
                if (nmsValue >= (float)HighThresholdUpDown.Value)
                {
                    return true;
                }
                //���� �Ӱ谪���� ���� ���� ���� �ȼ��� �����ٸ� �� �������δ� ���� �ȵȴ�.
                else if (nmsValue < (float)LowThresholdUpDown.Value)
                {
                    continue;
                }
                //�� ���̿� �ִ� ���̸� ��� Ž���Ѵ�.
                else
                {
                    //�ֺ� �̿����� ť�� �ִ´�.
                    q.Enqueue(new Point(point.X - 1, point.Y - 1));
                    q.Enqueue(new Point(point.X,     point.Y - 1));
                    q.Enqueue(new Point(point.X + 1, point.Y - 1));
                    q.Enqueue(new Point(point.X - 1, point.Y));
                    q.Enqueue(new Point(point.X + 1, point.Y));
                    q.Enqueue(new Point(point.X - 1, point.Y + 1));
                    q.Enqueue(new Point(point.X,     point.Y + 1));
                    q.Enqueue(new Point(point.X + 1, point.Y + 1));
                }
            }

            //���� �Ӱ谪�� ����ϴ� �ȼ��� ������ ���Ѱ��̴�. �׷��Ƿ� �̰��� ������ �ƴϴ�
            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            originImageBox = new PictureBox();
            grayscaleImageBox = new PictureBox();
            gaussianBluredImageBox = new PictureBox();
            sobelHorizontalMaskedImageBox = new PictureBox();
            sobelVerticalMaskedImageBox = new PictureBox();
            gradientMagnitudeImageBox = new PictureBox();
            nonMaximumSupressionImageBox = new PictureBox();
            histeresisedImageBox = new PictureBox();

            ImageContainer.Controls.Add(originImageBox);
            ImageContainer.Controls.Add(grayscaleImageBox);
            ImageContainer.Controls.Add(gaussianBluredImageBox);
            ImageContainer.Controls.Add(sobelHorizontalMaskedImageBox);
            ImageContainer.Controls.Add(sobelVerticalMaskedImageBox);
            ImageContainer.Controls.Add(gradientMagnitudeImageBox);
            ImageContainer.Controls.Add(nonMaximumSupressionImageBox);
            ImageContainer.Controls.Add(histeresisedImageBox);

            gaussianKernel = new GaussianKernel(3, 5f);
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialog = new OpenFileDialog();
            dialog.Filter = "jpg(*.jpg)|*.jpg|jpeg(*.jpeg)|*.jpeg|png(*.png)|*.png";
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                //�̹����� ����.
                DoCannyEdgeDetection();
            }
        }

        private void BlurSizeUpDown_ValueChanged(object sender, EventArgs e)
        {
            gaussianKernel = new GaussianKernel((int)BlurSizeUpDown.Value, (float)BlurSigmaUpdown.Value);
            DoCannyEdgeDetection();
        }

        private void LowThresholdUpDown_ValueChanged(object sender, EventArgs e)
        {
            histeresisedImage = DoHisteresis(nonMaximumSuppressionImage, (float)LowThresholdUpDown.Value, (float)HighThresholdUpDown.Value);
            histeresisedImageBox.Image = histeresisedImage.ToBitmap();
            SwapHighAndLowIfNeed();
        }

        private void HighThresholdUpDown_ValueChanged(object sender, EventArgs e)
        {
            histeresisedImage = DoHisteresis(nonMaximumSuppressionImage, (float)LowThresholdUpDown.Value, (float)HighThresholdUpDown.Value);
            histeresisedImageBox.Image = histeresisedImage.ToBitmap();
            SwapHighAndLowIfNeed();
        }

        private void BlurSigmaUpdown_ValueChanged(object sender, EventArgs e)
        {
            gaussianKernel = new GaussianKernel((int)BlurSizeUpDown.Value, (float)BlurSigmaUpdown.Value);
            DoCannyEdgeDetection();
        }
    }
}
