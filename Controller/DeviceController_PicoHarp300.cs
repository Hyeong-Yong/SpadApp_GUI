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
        private System.Windows.Forms.Timer? _monitorTimer;

        public bool IsConnected => _device.IsConnected;

        /// <summary>
        /// 장치 연결 및 모니터링 타이머 설정
        /// </summary>
        public bool ConnectDevice(System.Windows.Forms.Timer timer, Action<string> logAction)
        {
            try
            {
                // 1. Library Version Check (정적 메서드로 즉시 호출)
                PicoHarp_DeviceInfo.LibVer.Clear();
                string libVersion = PicoHarpDevice.GetLibraryVersion();
                PicoHarp_DeviceInfo.LibVer.Append(libVersion);

                // 2. Search Devices
                List<int> devices = PicoHarpDevice.FindDevices();
                PicoHarp_DeviceInfo.AvailableDevices.Clear();
                PicoHarp_DeviceInfo.AvailableDevices.AddRange(devices);

                if (PicoHarp_DeviceInfo.AvailableDevices.Count == 0)
                {
                    logAction("No PicoHarp devices found.");
                    return false;
                }

                // 3. Connect & Initialize (첫 번째 장비 자동 연결)
                int targetIndex = PicoHarp_DeviceInfo.AvailableDevices[0];

                // 2층 API를 통해 연결 및 초기화 한 번에 수행
                string serial = _device.Connect(targetIndex, PicoHarp_Native.MODE_HIST);

                // 전역 데이터 모델 업데이트
                PicoHarp_DeviceInfo.DeviceIndex = targetIndex;
                PicoHarp_DeviceInfo.Serial.Clear();
                PicoHarp_DeviceInfo.Serial.Append(serial);
                PicoHarp_DeviceInfo.IsConnected = true;

                logAction($"Connected to Device {targetIndex} (S/N: {serial}). Monitoring started.");

                // 4. 모니터링 타이머 구동
                _monitorTimer = timer;
                _monitorTimer.Interval = 100; // 매뉴얼 권장 게이트 타임 100ms 설정
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

        /// <summary>
        /// 안전하게 장치 연결 해제
        /// </summary>
        public void DisconnectDevice(Action<string> logAction)
        {
            try
            {
                _monitorTimer?.Stop();

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


    }
}