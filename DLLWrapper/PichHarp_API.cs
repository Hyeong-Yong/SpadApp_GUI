using System;
using System.Collections.Generic;
using System.Text;

namespace SpadApp.DLLWrapper
{
    public sealed class PicoHarpDevice : IDisposable
    {
        private int _deviceIndex = -1;
        private bool _connected;

        public bool IsConnected => _connected;
        public int DeviceIndex => _deviceIndex;

        // ------------------------------------------------------------
        // Library Info & Resource Search
        // ------------------------------------------------------------

        public static string GetLibraryVersion()
        {
            StringBuilder sb = new(128);
            CheckResult(PicoHarp_Native.PH_GetLibraryVersion(sb), -1);
            return sb.ToString();
        }

        public static List<int> FindDevices()
        {
            List<int> availableDevices = new();
            StringBuilder sb = new(128);

            for (int i = 0; i < PicoHarp_Native.MAXDEVNUM; i++)
            {
                sb.Clear();
                // OpenDevice 성공 시 0 반환
                if (PicoHarp_Native.PH_OpenDevice(i, sb) == 0)
                {
                    availableDevices.Add(i);
                    // 탐색용으로 열었으므로 일단 닫아줍니다.
                    PicoHarp_Native.PH_CloseDevice(i);
                }
            }
            return availableDevices;
        }

        // ------------------------------------------------------------
        // Connect / Disconnect
        // ------------------------------------------------------------

        public string Connect(int deviceIndex, int mode = PicoHarp_Native.MODE_HIST)
        {
            if (_connected) return string.Empty;

            StringBuilder serialSb = new(128);

            // 장치 열기
            CheckResult(PicoHarp_Native.PH_OpenDevice(deviceIndex, serialSb), deviceIndex);
            _deviceIndex = deviceIndex;

            // 초기화
            CheckResult(PicoHarp_Native.PH_Initialize(_deviceIndex, mode), _deviceIndex);

            // 캘리브레이션
            CheckResult(PicoHarp_Native.PH_Calibrate(_deviceIndex), _deviceIndex);

            _connected = true;
            return serialSb.ToString();
        }

        public void Disconnect()
        {
            if (!_connected) return;

            PicoHarp_Native.PH_CloseDevice(_deviceIndex);
            _connected = false;
            _deviceIndex = -1;
        }

        // ------------------------------------------------------------
        // Hardware Configuration
        // ------------------------------------------------------------

        public double GetResolution()
        {
            double resolution = 0;
            CheckResult(PicoHarp_Native.PH_GetResolution(_deviceIndex, ref resolution), _deviceIndex);
            return resolution;
        }

        public int GetCountRate(int channel)
        {
            int countRate = 0;
            CheckResult(PicoHarp_Native.PH_GetCountRate(_deviceIndex, channel, ref countRate), _deviceIndex);
            return countRate;
        }

        // ------------------------------------------------------------
        // Measurement Control
        // ------------------------------------------------------------

        public void ClearHistogramMemory(int block = 0)
        {
            CheckResult(PicoHarp_Native.PH_ClearHistMem(_deviceIndex, block), _deviceIndex);
        }

        public void StartMeasurement(int acquisitionTimeMs)
        {
            CheckResult(PicoHarp_Native.PH_StartMeas(_deviceIndex, acquisitionTimeMs), _deviceIndex);
        }

        public void StopMeasurement()
        {
            CheckResult(PicoHarp_Native.PH_StopMeas(_deviceIndex), _deviceIndex);
        }

        public bool IsMeasurementFinished()
        {
            int ctcStatus = 0;
            CheckResult(PicoHarp_Native.PH_CTCStatus(_deviceIndex, ref ctcStatus), _deviceIndex);
            return ctcStatus != 0; // 0이 아니면 수집 완료(CTC 만료) 상태
        }

        public void GetHistogram(uint[] chCount, int block = 0)
        {
            // 드라이버 함수 호출 (기본 단일 장비 계측 환경이라면 block 인자에는 0이 들어갑니다)
            CheckResult(PicoHarp_Native.PH_GetHistogram(_deviceIndex, chCount, block), _deviceIndex);
        }
        // ------------------------------------------------------------
        // Error Check Helper
        // ------------------------------------------------------------

        private static void CheckResult(int resultCode, int devIdx)
        {
            if (resultCode >= 0) return; // 0 이상은 성공

            StringBuilder errSb = new(256);
            PicoHarp_Native.PH_GetErrorString(errSb, resultCode);

            throw new PicoHarpException(resultCode, errSb.ToString().Trim());
        }

        // ------------------------------------------------------------
        // Dispose Pattern
        // ------------------------------------------------------------

        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }

        ~PicoHarpDevice()
        {
            Disconnect();
        }
    }

    public class PicoHarpException : Exception
    {
        public int ErrorCode { get; }
        public PicoHarpException(int errorCode, string message)
            : base($"PicoHarp Error {errorCode}: {message}")
        {
            ErrorCode = errorCode;
        }
    }
}