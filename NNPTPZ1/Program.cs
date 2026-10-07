using System;
using System.Collections.Generic;
using System.Drawing;
using NNPTPZ1.Mathematics;
using System.Globalization;

namespace NNPTPZ1
{
    /// <summary>
    /// This program should produce Newton fractals.
    /// See more at: https://en.wikipedia.org/wiki/Newton_fractal
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            int[] imageDimensions = GetImageDimensions(args);
            double[] coordinateBounds = GetCoordinateBounds(args);
            string output = args[6];
            Bitmap bitmap = new Bitmap(imageDimensions[0], imageDimensions[1]);
            double xmin = coordinateBounds[0];
            double xmax = coordinateBounds[1];
            double ymin = coordinateBounds[2];
            double ymax = coordinateBounds[3];

            double xstep = (xmax - xmin) / imageDimensions[0];
            double ystep = (ymax - ymin) / imageDimensions[1];

            List<ComplexNumber> knownRoots = new List<ComplexNumber>();
            Polynomial p = CreatePolynomial();
            Polynomial derivative = p.Derive();

            Console.WriteLine(p);
            Console.WriteLine(derivative);
            Color[] colors = GetDefaultColors();
            
            GenerateImage(imageDimensions, bitmap, xmin, ymin, xstep, ystep, knownRoots, p, derivative, colors);

            bitmap.Save(output ?? "../../../out.png");
        }

        private static void GenerateImage(int[] imageDimensions, Bitmap bitmap, double xmin, double ymin, double xstep, double ystep, List<ComplexNumber> knownRoots, Polynomial p, Polynomial derivative, Color[] colors)
        {
            // for every pixel in image...
            for (int i = 0; i < imageDimensions[0]; i++)
            {
                for (int j = 0; j < imageDimensions[1]; j++)
                {
                    // find "world" coordinates of pixel
                    double y = ymin + i * ystep;
                    double x = xmin + j * xstep;

                    ComplexNumber ox = new ComplexNumber()
                    {
                        Real = x,
                        Imaginary = y
                    };

                    if (ox.Real == 0)
                        ox.Real = 0.0001;
                    if (ox.Imaginary == 0)
                        ox.Imaginary = 0.0001;

                    // find solution of equation using newton's iteration
                    int iterationCount = FindNewtonRoot(p, derivative, ref ox);

                    // find solution root number
                    int id = GetKnownRootId(knownRoots, ox);

                    // colorize pixel according to root number
                    Color pixelColor = CalculatePixelColor(colors, iterationCount, id);
                    bitmap.SetPixel(j, i, pixelColor);
                }
            }
        }

        private static Color[] GetDefaultColors()
        {
            return new Color[]
                        {
                Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange, Color.Fuchsia, Color.Gold, Color.Cyan, Color.Magenta
                        };
        }

        private static double[] GetCoordinateBounds(string[] args)
        {
            double[] coordinateBounds = new double[4];
            for (int i = 0; i < coordinateBounds.Length; i++)
            {
                coordinateBounds[i] = double.Parse(args[i + 2], CultureInfo.InvariantCulture);
            }

            return coordinateBounds;
        }

        private static int[] GetImageDimensions(string[] args)
        {
            int[] imageDimensions = new int[2];
            for (int i = 0; i < imageDimensions.Length; i++)
            {
                imageDimensions[i] = int.Parse(args[i]);
            }

            return imageDimensions;
        }

        private static Color CalculatePixelColor(Color[] colors, int iterationCount, int id)
        {
            var pixelColor = colors[id % colors.Length];
            pixelColor = Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B);
            pixelColor = Color.FromArgb(Math.Min(Math.Max(0, pixelColor.R - (int)iterationCount * 2), 255), Math.Min(Math.Max(0, pixelColor.G - (int)iterationCount * 2), 255), Math.Min(Math.Max(0, pixelColor.B - (int)iterationCount * 2), 255));
            return pixelColor;
        }

        private static int GetKnownRootId(List<ComplexNumber> knownRoots, ComplexNumber ox)
        {
            var known = false;
            var id = 0;
            for (int w = 0; w < knownRoots.Count; w++)
            {
                if (Math.Pow(ox.Real - knownRoots[w].Real, 2) + Math.Pow(ox.Imaginary - knownRoots[w].Imaginary, 2) <= 0.01)
                {
                    known = true;
                    id = w;
                }
            }
            if (!known)
            {
                knownRoots.Add(ox);
                id = knownRoots.Count;
            }

            return id;
        }

        private static int FindNewtonRoot(Polynomial p, Polynomial derivative, ref ComplexNumber ox)
        {
            int iterationCount = 0;
            for (int q = 0; q < 30; q++)
            {
                var diff = p.Evaluate(ox).Divide(derivative.Evaluate(ox));
                ox = ox.Subtract(diff);

                if (Math.Pow(diff.Real, 2) + Math.Pow(diff.Imaginary, 2) >= 0.5)
                {
                    q--;
                }
                iterationCount++;
            }

            return iterationCount;
        }

        private static Polynomial CreatePolynomial()
        {
            Polynomial p = new Polynomial();
            p.Coefficients.Add(new ComplexNumber() { Real = 1 });
            p.Coefficients.Add(ComplexNumber.Zero);
            p.Coefficients.Add(ComplexNumber.Zero);
            p.Coefficients.Add(new ComplexNumber() { Real = 1 });
            return p;
        }
    }
}
