using System;
using System.IO;
using System.Linq;

namespace SpadApp
{
    public class HistogramBuffer
    {
        public const int ChannelCount = 65536;

        private readonly uint[] _counts;

        public HistogramBuffer()
        {
            _counts = new uint[ChannelCount];
        }

        public HistogramBuffer(uint[] source)
        {
            if (source.Length != ChannelCount)
                throw new ArgumentException($"Histogram size must be {ChannelCount}");

            _counts = new uint[ChannelCount];

            Array.Copy(source, _counts, ChannelCount);
        }

        public uint[] RawData => _counts;

        public uint this[int index]
        {
            get => _counts[index];
            set => _counts[index] = value;
        }

        public void Clear()
        {
            Array.Clear(_counts, 0, ChannelCount);
        }

        public void CopyFrom(uint[] source)
        {
            if (source.Length != ChannelCount)
                throw new ArgumentException($"Histogram size must be {ChannelCount}");

            Array.Copy(source, _counts, ChannelCount);
        }

        public double IntegralCounts()
        {
            double sum = 0;

            for (int i = 0; i < ChannelCount; i++)
                sum += _counts[i];

            return sum;
        }

        public uint MaxCount()
        {
            return _counts.Max();
        }

        public int MaxIndex()
        {
            uint max = 0;
            int index = 0;

            for (int i = 0; i < ChannelCount; i++)
            {
                if (_counts[i] > max)
                {
                    max = _counts[i];
                    index = i;
                }
            }

            return index;
        }

        public double[] ToDoubleArray()
        {
            double[] result = new double[ChannelCount];

            for (int i = 0; i < ChannelCount; i++)
                result[i] = _counts[i];

            return result;
        }

        public double[] ToTimeAxis(double resolutionPs)
        {
            double[] axis = new double[ChannelCount];

            for (int i = 0; i < ChannelCount; i++)
                axis[i] = i * resolutionPs;

            return axis;
        }

        public void SaveAsText(string filePath)
        {
            using StreamWriter sw = new StreamWriter(filePath);

            for (int i = 0; i < ChannelCount; i++)
            {
                sw.WriteLine(_counts[i]);
            }
        }

        public void SaveAsCsv(string filePath, double resolutionPs)
        {
            using StreamWriter sw = new StreamWriter(filePath);

            sw.WriteLine("Channel,Time_ps,Counts");

            for (int i = 0; i < ChannelCount; i++)
            {
                double time = i * resolutionPs;

                sw.WriteLine($"{i},{time},{_counts[i]}");
            }
        }

        public HistogramBuffer Clone()
        {
            return new HistogramBuffer(_counts);
        }
    }
}