using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;

namespace SpadApp.Model
{
    public class GaussianFitResult
    {
        public double Amplitude { get; set; }
        public double Mean { get; set; }
        public double Sigma { get; set; }

        // FWHM = 2 * sqrt(2 * ln(2)) * Sigma
        public double Fwhm => 2.354820045 * Sigma;

        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = "";
    }

    /// <summary>
    /// MathNet.Numerics를 활용하여 TCSPC 히스토그램의 System Jitter(FWHM)를 계산하는 클래스
    /// </summary>
    public static class GaussianCalculator
    {
        /// <summary>
        /// 히스토그램 배열을 받아 가우시안 피팅을 수행합니다.
        /// </summary>
        /// <param name="histogram">Y축 데이터 (카운트)</param>
        /// <param name="binWidth">X축 1칸의 시간 간격 (보통 ps 단위)</param>
        public static GaussianFitResult GaussianCalculateBin(double[] histogram, double binWidth)
        {
            if (histogram == null || histogram.Length < 3)
            {
                return new GaussianFitResult { IsSuccess = false, ErrorMessage = "데이터가 부족합니다." };
            }

            // 1. 최대 피크 찾기
            double maxCount = histogram.Max();
            if (maxCount < 20) // 데이터가 너무 적으면 노이즈로 간주
            {
                return new GaussianFitResult { IsSuccess = false, ErrorMessage = "유효한 피크 신호가 없습니다." };
            }
            int peakIndex = Array.IndexOf(histogram, maxCount);

            // 2. 가우시안 피팅을 위한 유효 데이터(ROI) 추출
            // 베이스라인 노이즈(DCR)가 피팅을 방해하지 않도록 피크의 20% 이상인 구간만 잘라냅니다.
            double threshold = maxCount * 0.2;

            List<double> xData = new List<double>();
            List<double> yDataLog = new List<double>();

            // 피크를 중심으로 좌우로 탐색
            int startIndex = peakIndex;
            while (startIndex > 0 && histogram[startIndex] > threshold) startIndex--;

            int endIndex = peakIndex;
            while (endIndex < histogram.Length - 1 && histogram[endIndex] > threshold) endIndex++;

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (histogram[i] > 0)
                {
                    xData.Add(i * binWidth);
                    yDataLog.Add(Math.Log(histogram[i])); // 자연 로그(ln) 변환
                }
            }

            if (xData.Count < 3) return new GaussianFitResult { IsSuccess = false, ErrorMessage = "피팅 구간(ROI)이 너무 좁습니다." };

            try
            {
                // =========================================================================
                // ★ MathNet.Numerics 핵심 알고리즘 (로그-파라볼라 선형 피팅)
                // 가우시안 식: y = A * exp(-(x - u)^2 / (2 * s^2))
                // 로그 변환식: ln(y) = ln(A) - (x^2 - 2xu + u^2) / (2 * s^2)
                // 이를 2차 다항식(C + Bx + Ax^2)으로 피팅하면 초기값 없이 완벽한 해를 얻습니다.
                // =========================================================================
                double[] p = Fit.Polynomial(xData.ToArray(), yDataLog.ToArray(), 2);

                double c = p[0];
                double b = p[1];
                double a = p[2];

                // a 계수가 양수면 아래로 볼록한 그래프이므로 가우시안 피크가 아님
                if (a >= 0) return new GaussianFitResult { IsSuccess = false, ErrorMessage = "정상적인 피크 형태가 아닙니다." };

                // 다항식 계수(a, b, c)를 가우시안 파라미터(Sigma, Mean, Amplitude)로 역산
                double sigma = Math.Sqrt(-1.0 / (2.0 * a));
                double mean = b * sigma * sigma;
                double amplitude = Math.Exp(c + Math.Pow(mean, 2) / (2.0 * Math.Pow(sigma, 2)));

                return new GaussianFitResult
                {
                    IsSuccess = true,
                    Amplitude = amplitude,
                    Mean = mean,
                    Sigma = sigma
                };
            }
            catch (Exception ex)
            {
                return new GaussianFitResult { IsSuccess = false, ErrorMessage = $"MathNet 피팅 에러: {ex.Message}" };
            }
        }
    }
}