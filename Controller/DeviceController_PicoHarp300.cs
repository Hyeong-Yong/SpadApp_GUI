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
                        PicoHarp_Native.MODE_HIST);

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