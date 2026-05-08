using SpadApp.Parameters;
using System;

namespace SpadApp.Controller
{
    public class DeviceController
    {
        private System.Windows.Forms.Timer? _monitorTimer;

        /// <summary>
        /// 장치 연결 및 모니터링 타이머 설정
        /// </summary>
        public bool ConnectDevice(System.Windows.Forms.Timer timer, Action<string> logAction)
        {
            int ret;

           // 2. Library Version Check 
            ret = PicoHarpDevice.PH_GetLibraryVersion(PicoHarpInfo.LibVer);
            if (ret < 0)
            {
                logAction("Library version check failed.");
                return false;
            }

            // 3. Search & Open Device
            PicoHarpInfo.AvailableDevices.Clear();
            for (int i = 0; i < PicoHarpDevice.MAXDEVNUM; i++)
            {
                PicoHarpInfo.Serial.Clear();
                if (PicoHarpDevice.PH_OpenDevice(i, PicoHarpInfo.Serial) == 0)
                {
                    PicoHarpInfo.AvailableDevices.Add(i);
                }
            }

            if (PicoHarpInfo.AvailableDevices.Count == 0)
            {
                logAction("No PicoHarp devices found.");
                return false;
            }

            // 4. Initialize & Calibrate
            PicoHarpInfo.DeviceIndex = PicoHarpInfo.AvailableDevices[0];

            // Histogram 모드로 초기화 
            ret = PicoHarpDevice.PH_Initialize(PicoHarpInfo.DeviceIndex, PicoHarpDevice.MODE_HIST);
            if (ret < 0) return false;

            PicoHarpDevice.PH_Calibrate(PicoHarpInfo.DeviceIndex); // [cite: 640]

            PicoHarpInfo.IsConnected = true;
            logAction($"Connected to Device {PicoHarpInfo.DeviceIndex}. Monitoring started.");

            // 5. 모든 준비가 끝나면 타이머 시작
            _monitorTimer = timer;
            _monitorTimer.Interval = 100; // 매뉴얼 권장 게이트 타임 100ms 설정 [cite: 863]
            _monitorTimer.Start();

            return true;
        }

        public void DisconnectDevice(Action<string> logAction)
        {
            if (PicoHarpInfo.IsConnected)
            {
                PicoHarpDevice.PH_CloseDevice(PicoHarpInfo.DeviceIndex); // [cite: 574, 581]
                PicoHarpInfo.IsConnected = false;
                logAction("Device disconnected safely.");

                _monitorTimer?.Stop();
            }
        }

        /// <summary>
        /// 하드웨어에 명령을 내려 실제 측정을 수행하고 데이터를 채웁니다.
        /// </summary>
        /// <param name="targetBuffer">데이터를 담을 히스토그램 버퍼</param>
        /// <param name="isRepeating">루프 제어용 플래그 참조</param>
        public async Task ExecuteMeasurementAsync(uint[] targetBuffer, Func<bool> checkContinue)
        {
            await Task.Run(() =>
            {
                // 1. 히스토그램 메모리 초기화 
                PicoHarpDevice.PH_ClearHistMem(PicoHarpInfo.DeviceIndex, 0);

                // 2. 측정 시작 (설정된 수집 시간 동안) 
                PicoHarpDevice.PH_StartMeas(PicoHarpInfo.DeviceIndex, MeasurementStatus.AcquisitionTimeMs);

                int status = 0;
                // 3. CTC 상태 확인 루프 (측정 완료 대기) 
                // checkContinue()를 통해 외부에서 정지 버튼을 눌렀는지도 함께 체크합니다.
                while (status == 0 && checkContinue())
                {
                    PicoHarpDevice.PH_CTCStatus(PicoHarpInfo.DeviceIndex, ref status);
                    System.Threading.Thread.Sleep(10);
                }

                // 4. 측정 중지 
                PicoHarpDevice.PH_StopMeas(PicoHarpInfo.DeviceIndex);

                // 5. 히스토그램 데이터 읽기 
                PicoHarpDevice.PH_GetHistogram(PicoHarpInfo.DeviceIndex, targetBuffer, 0);
            });
        }

    }
}