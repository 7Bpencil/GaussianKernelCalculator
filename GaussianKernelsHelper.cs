using System;
using System.Runtime.CompilerServices;

namespace SevenBoldPencil.MathTools
{
    public static class GaussianKernelsHelper
    {
        private struct Sample
        {
            public double X, Y;
        }

        public static void GetKernelBilinearSameSize(
            double sigma, int kernelSize, out double[] weightsBilinear, out double[] samplePositionsBilinear, int sampleCount = 1000)
        {
            GetKernelBilinear(sigma, kernelSize * 2 - 1, out weightsBilinear, out samplePositionsBilinear, sampleCount);
        }

        public static void GetKernelBilinear(
            double sigma, int kernelSize, out double[] weightsBilinear, out double[] samplePositionsBilinear, int sampleCount = 1000)
        {
            // Steve M comment at:
            // https://dev.theomader.com/gaussian-kernel-calculator/

            if ((kernelSize - 1) % 4 != 0) throw new ArgumentException("(kernelSize - 1) % 4 != 0");
            GetKernel(sigma, kernelSize, out var weights, out var samplePositions, sampleCount);

            var newSize = (kernelSize + 1) / 2;
            var pairsPerSideAmount = (kernelSize - 1) / 4;

            weightsBilinear = new double[newSize];
            samplePositionsBilinear = new double[newSize];

            weightsBilinear[pairsPerSideAmount] = weights[pairsPerSideAmount * 2];
            for (var i = 0; i < pairsPerSideAmount; ++i)
            {
                var i_o = i * 2;

                var newWeight = weights[i_o] + weights[i_o + 1];
                weightsBilinear[i] = newWeight;
                weightsBilinear[newSize - 1 - i] = newWeight;

                var newPosition = samplePositions[i_o + 1] - weights[i_o] / newWeight;
                samplePositionsBilinear[i] = newPosition;
                samplePositionsBilinear[newSize - 1 - i] = -newPosition;
            }
        }

        public static void GetKernel(
            double sigma, int kernelSize, out double[] weights, out double[] samplePositions, int sampleCount = 1000)
        {
            // https://dev.theomader.com/gaussian-kernel-calculator/

            if (kernelSize % 2 != 1) throw new ArgumentException("kernelSize % 2 != 1");

            Func<double, double> GD = x => GaussianDistribution(x, 0, sigma);

            weights = new double[kernelSize];

            // need an even number of intervals for simpson integration => odd number of samples
            var samplesPerBin = (int) Math.Ceiling((double) sampleCount / kernelSize);
            if (samplesPerBin % 2 == 0) ++samplesPerBin;

            // now sample kernel taps and calculate tap weights
            var weightSum = 0d;
            var kernelLeft = -Math.Floor(kernelSize / 2d);
            var samplesCache = new Sample[samplesPerBin];
            for (var tap = 0; tap < kernelSize; ++tap)
            {
                var left = kernelLeft - 0.5 + tap;

                SampleInterval(GD, left, left + 1, samplesCache);
                var tapWeight = IntegrateSimpson(samplesCache);

                weights[tap] = tapWeight;
                weightSum += tapWeight;
            }

            // normalize kernel
            for (var i = 0; i < kernelSize; ++i) {
                weights[i] /= weightSum;
            }

            samplePositions = GetSamplePositions(kernelSize);
        }

        private static readonly double Sqrt2PI = Math.Sqrt(2 * Math.PI);
        private static double GaussianDistribution(double x, double mu, double sigma)
        {
            var d = x - mu;
            return Math.Exp(-d * d / (2 * sigma * sigma)) / (Sqrt2PI * sigma);
        }

        private static void SampleInterval(Func<double, double> f, double minInclusive, double maxInclusive, Sample[] samplesCache)
        {
            var sampleCount = samplesCache.Length;
            var stepSize = (maxInclusive - minInclusive) / (sampleCount - 1);

            for (var s = 0; s < sampleCount; ++s)
            {
                var x = minInclusive + s * stepSize;
                samplesCache[s].X = x;
                samplesCache[s].Y = f(x);
            }
        }

        private static double IntegrateSimpson(Sample[] samples)
        {
            var result = samples.First().Y + samples.Last().Y;

            for (var s = 1; s < samples.Length - 1; ++s)
            {
                var sampleWeight = (s % 2 + 1) * 2;
                result += sampleWeight * samples[s].Y;
            }

            var h = (samples.Last().X - samples.First().X) / (samples.Length - 1);
            return result * h / 3.0;
        }

        private static double[] GetSamplePositions(int kernelSize)
        {
            var samplePositions = new double[kernelSize];
            var kernelLeft = -(kernelSize / 2);
            for (var i = 0; i < kernelSize; ++i) {
                samplePositions[i] = kernelLeft + i;
            }

            return samplePositions;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref T First<T>(this T[] array)
        {
            return ref array[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref T Last<T>(this T[] array)
        {
            return ref array[^1];
        }
    }
}
