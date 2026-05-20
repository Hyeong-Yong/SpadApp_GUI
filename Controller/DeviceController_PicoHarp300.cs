using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SpadApp.DLLWrapper;
using SpadApp.Parameters;

namespace SpadApp.Controller
{
    public class DeviceController_PicoHarp300
    {
        // 2층 API 객체를 내부에 캡슐화합니다.
        private readonly PicoHarpDevice _device = new();
        private readonly System.Windows.Forms.Timer _monitorTimer;

        public bool IsConnected => _device.IsConnected;
        // ============================================================
        // Events
        // ============================================================

        /// <summary>
        /// CountRate 업데이트 이벤트
        /// </summary>
        public event Action<int, int>? CountRateUpdated;

        // ============================================================
        // Constructor
        // ============================================================

        public DeviceController_PicoHarp300()
        {
            _monitorTimer = new System.Windows.Forms.Timer();
            _monitorTimer.Interval = 100;
            _monitorTimer.Tick += MonitorTimer_Tick;
        }
        // ============================================================
        // Monitor Timer
        // ============================================================

        private void MonitorTimer_Tick(object? sender, EventArgs e)
        {
            if (!_device.IsConnected)
                return;

            int rate0 = 0;
            int rate1 = 0;

            int ret0 = PicoHarp_Native.PH_GetCountRate(
                PicoHarp_DeviceInfo.DeviceIndex,
                0,
                ref rate0);

            int ret1 = PicoHarp_Native.PH_GetCountRate(
                PicoHarp_DeviceInfo.DeviceIndex,
                1,
                ref rate1);

            if (ret0 >= 0)
                PicoHarp_MeasurementStatus.CountRate0 = rate0;

            if (ret1 >= 0)
                PicoHarp_MeasurementStatus.CountRate1 = rate1;

            // UI에 이벤트 전달
            CountRateUpdated?.Invoke(rate0, rate1);
        }



        // ============================================================
        // Connect
        // ============================================================

        public bool ConnectDevice(Action<string> logAction)
        {
            try
            {
                // 1. Library Version Check
                PicoHarp_DeviceInfo.LibVer.Clear();

                string libVersion =
                    PicoHarpDevice.GetLibraryVersion();

                PicoHarp_DeviceInfo.LibVer.Append(libVersion);

                // 2. Search Devices
                List<int> devices =
                    PicoHarpDevice.FindDevices();

                PicoHarp_DeviceInfo.AvailableDevices.Clear();

                PicoHarp_DeviceInfo.AvailableDevices
                    .AddRange(devices);

                if (PicoHarp_DeviceInfo.AvailableDevices.Count == 0)
                {
                    logAction("No PicoHarp devices found.");
                    return false;
                }

                // 3. Connect
                int targetIndex =
                    PicoHarp_DeviceInfo.AvailableDevices[0];

                string serial =
                    _device.Connect(
                        targetIndex,
                        PicoHarpDevice.MODE_HIST);

                // Global Model Update
                PicoHarp_DeviceInfo.DeviceIndex = targetIndex;

                PicoHarp_DeviceInfo.Serial.Clear();

                PicoHarp_DeviceInfo.Serial.Append(serial);

                PicoHarp_DeviceInfo.IsConnected = true;

                logAction(
                    $"Connected to Device {targetIndex} (S/N: {serial})");

                // 4. Start Monitor
                _monitorTimer.Start();

                return true;
            }
            catch (PicoHarpException ex)
            {
                logAction($"PicoHarp Hardware Error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                logAction($"Connect General Error: {ex.Message}");
                return false;
            }
        }


        // ============================================================
        // Disconnect
        // ============================================================

        public void DisconnectDevice(Action<string> logAction)
        {
            try
            {
                _monitorTimer.Stop();

                if (_device.IsConnected)
                {
                    _device.Disconnect();

                    PicoHarp_DeviceInfo.IsConnected = false;

                    logAction("Device disconnected safely.");
                }
            }
            catch (Exception ex)
            {
                logAction($"Disconnect Error: {ex.Message}");
            }
        }
        public void DisconnectDevice()
        {
            try
            {
                _monitorTimer.Stop();

                if (_device.IsConnected)
                {
                    _device.Disconnect();

                    PicoHarp_DeviceInfo.IsConnected = false;

                    
                }
            }
            catch (Exception ex)
            {
                
            }
        }



        public async Task ExecuteMeasurementAsync(uint[] targetBuffer, Func<bool> checkContinue)
        {
            if (!_device.IsConnected) return;

            // ❌ 혹시 이 자리에 _monitorTimer?.Stop(); 이 있었다면 삭제하세요!

            await Task.Run(() =>
            {
                try
                {
                    _device.ClearHistogramMemory(0);
                    _device.StartMeasurement(PicoHarp_MeasurementStatus.AcquisitionTimeMs);

                    while (!_device.IsMeasurementFinished() && checkContinue())
                    {
                        Thread.Sleep(10);
                    }

                    _device.StopMeasurement();
                    _device.GetHistogram(targetBuffer, block: 0);
                }
                catch (PicoHarpException) { throw; }
                // ❌ finally 블록에 있던 _monitorTimer?.Start(); 도 삭제하세요!
            });
        }

        // ============================================================
        // Hardware Settings & Sync Control (컨트롤러 통합)
        // ============================================================

        /// <summary>
        /// 전역 설정 모델(PicoHarp_DeviceSettings)에 근거하여 하드웨어 파라미터를 일괄 주입하고 최신 해상도를 갱신합니다.
        /// </summary>
        /// <returns>성공 시 0, 실패 시 Native 에러 코드 반환</returns>
        public int UpdateDeviceSettings()
        {
            if (!IsConnected) return -1;
            int dev = PicoHarp_DeviceInfo.DeviceIndex;
            int ret;

            // 1. Sync Divider 설정 적용
            ret = PicoHarp_Native.PH_SetSyncDiv(dev, PicoHarp_DeviceSettings.SyncDivider);
            if (ret < 0) return ret;

            // 🌟 2. [핵심 추가] Sync Offset (케이블 지연 대체 보정치) 하드웨어 주입
            // (참고: PicoHarp_DeviceSettings 클래스에 public static int SyncOffset 필드가 미리 정의되어 있어야 합니다)
            ret = PicoHarp_Native.PH_SetSyncOffset(dev, PicoHarp_DeviceSettings.SyncOffset);
            if (ret < 0) return ret;

            // 3. CFD 입력 레벨 및 제로크로스 설정 (CH0 / CH1)
            ret = PicoHarp_Native.PH_SetInputCFD(dev, 0, PicoHarp_DeviceSettings.CFDLevel0, PicoHarp_DeviceSettings.CFDZeroCross0);
            if (ret < 0) return ret;

            ret = PicoHarp_Native.PH_SetInputCFD(dev, 1, PicoHarp_DeviceSettings.CFDLevel1, PicoHarp_DeviceSettings.CFDZeroCross1);
            if (ret < 0) return ret;

            // 4. Binning 및 글로벌 시간 오프셋 적용
            ret = PicoHarp_Native.PH_SetBinning(dev, PicoHarp_DeviceSettings.Binning);
            if (ret < 0) return ret;

            ret = PicoHarp_Native.PH_SetOffset(dev, PicoHarp_MeasurementStatus.AcqOffset);
            if (ret < 0) return ret;

            // 5. 설정을 먹인 뒤 변경된 물리 하드웨어 Resolution(ps) 동적 가로채기 및 전역 바인딩
            double resPs = 0;
            ret = PicoHarp_Native.PH_GetResolution(dev, ref resPs);
            if (ret >= 0)
            {
                PicoHarp_MeasurementStatus.ResolutionPs = resPs;
            }

            return ret;
        }


        // ============================================================
        // TTTR Mode & FIFO Control (추가된 부분)
        // ============================================================

        /// <summary>
        /// 장비의 동작 모드(HIST, T2, T3)를 초기화합니다.
        /// </summary>
        public bool InitializeMode(int mode)
        {
            // 이미 같은 모드면 아무것도 안함
            if (mode == PicoHarp_DeviceSettings.CurrentMode)
            {
                return false;
            }

            int ret = PicoHarp_Native.PH_Initialize(
                PicoHarp_DeviceInfo.DeviceIndex,
                mode);

            if (ret != 0)
            {
                throw new Exception($"PH_Initialize failed : {ret}");
            }

            // 현재 모드 저장
            PicoHarp_DeviceSettings.CurrentMode = mode;

            return true;
        }

        /// <summary>
        /// 지정된 타겟 시간(ms) 동안 TTTR 계측을 시작합니다.
        /// </summary>
        public int StartMeasurement(int tacqMs)
        {
            return PicoHarp_Native.PH_StartMeas(PicoHarp_DeviceInfo.DeviceIndex, tacqMs);
        }

        /// <summary>
        /// 계측을 강제 중지합니다.
        /// </summary>
        public int StopMeasurement()
        {
            return PicoHarp_Native.PH_StopMeas(PicoHarp_DeviceInfo.DeviceIndex);
        }

        /// <summary>
        /// 하드웨어 FIFO 버퍼로부터 실시간 레코드를 읽어옵니다.
        /// </summary>
        public int ReadFiFo(uint[] buffer, int count)
        {
            int nactual = 0; // 실제로 읽어온 갯수를 받아올 변수

            // ★ 4번째 파라미터로 out nactual 전달
            int ret = PicoHarp_Native.PH_ReadFiFo(PicoHarp_DeviceInfo.DeviceIndex, buffer, count, out nactual);

            // 통신 에러 발생 시 음수 에러 코드 반환
            if (ret < 0)
            {
                return ret;
            }

            // 정상 작동 시 실제로 버퍼에 채워진 레코드 개수를 반환
            return nactual;
        }

        /// <summary>
        /// 현재 장비의 글로벌 해상도(Resolution)를 ps 단위로 가져옵니다.
        /// </summary>
        public double GetResolutionPs()
        {
            double res = 0;
            PicoHarp_Native.PH_GetResolution(PicoHarp_DeviceInfo.DeviceIndex, ref res);
            return res;
        }

        // ============================================================
        // CountRate Getter
        // ============================================================

        public int GetCountRate(int channel)
        {
            int rate = 0;

            int ret = PicoHarp_Native.PH_GetCountRate(
                PicoHarp_DeviceInfo.DeviceIndex,
                channel,
                ref rate);

            return rate;
        }

        // ============================================================
        // Monitor Control
        // ============================================================

        public void StartMonitoring()
        {
            _monitorTimer.Start();
        }

        public void StopMonitoring()
        {
            _monitorTimer.Stop();
        }

    }
}