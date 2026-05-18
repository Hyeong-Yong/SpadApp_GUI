using SpadApp.Controller;
using SpadApp.DLLWrapper;
using SpadApp.Model;
using SpadApp.Parameters;
using SpadApp.View;

namespace SpadApp
{
    public partial class ucMainView
    {

        // ------------------------------------------------------------
        // TCSPC
        // ------------------------------------------------------------
        private HistogramBuffer _histogram = new();
        private HistogramChartManager _chartManager;
        public DeviceController_PicoHarp300 TCSPCDeviceController = new();

        private void UpdateDeviceSettings_PicoHarp()
        {
            if (!PicoHarp_DeviceInfo.IsConnected) return;

            // 🌟 [핵심] 하드웨어 레지스터 설정을 바꾸는 동안 타이머와의 충돌을 원천 차단합니다.
            TCSPCDeviceController.StopMonitoring();

            try
            {
                // 1. UI에서 현재 설정값 읽어오기
                PicoHarp_MeasurementStatus.AcquisitionTimeMs = (int)numAcqTime.Value;
                PicoHarp_MeasurementSettings.SyncDivider = (int)numSyncDiv.Value;
                PicoHarp_MeasurementSettings.Binning = (int)numBinning.Value;
                PicoHarp_MeasurementSettings.CFDLevel0 = (int)numCFDLevel0.Value;
                PicoHarp_MeasurementSettings.CFDZeroCross0 = (int)numCFDZeroCross0.Value;
                PicoHarp_MeasurementSettings.CFDLevel1 = (int)numCFDLevel1.Value;
                PicoHarp_MeasurementSettings.CFDZeroCross1 = (int)numCFDZeroCross1.Value;

                // 2. 장비에 설정 적용 (Native API 순차 호출)
                PicoHarp_Native.PH_SetSyncDiv(PicoHarp_DeviceInfo.DeviceIndex, PicoHarp_MeasurementSettings.SyncDivider);
                PicoHarp_Native.PH_SetInputCFD(PicoHarp_DeviceInfo.DeviceIndex, 0, PicoHarp_MeasurementSettings.CFDLevel0, PicoHarp_MeasurementSettings.CFDZeroCross0);
                PicoHarp_Native.PH_SetInputCFD(PicoHarp_DeviceInfo.DeviceIndex, 1, PicoHarp_MeasurementSettings.CFDLevel1, PicoHarp_MeasurementSettings.CFDZeroCross1);
                PicoHarp_Native.PH_SetBinning(PicoHarp_DeviceInfo.DeviceIndex, PicoHarp_MeasurementSettings.Binning);

                // PicoHarp 설정 변경 => 새로운 Resolution 값 취득
                double resolution = 0;
                PicoHarp_Native.PH_GetResolution(PicoHarp_DeviceInfo.DeviceIndex, ref resolution);
                lblResolution.Text = resolution.ToString();

                // 차트 매니저에게도 변경된 해상도를 알림
                _chartManager.UpdateTimeAxis(resolution);
            }
            catch (Exception ex)
            {
                Log($"설정 업데이트 중 오류 발생: {ex.Message}");
            }
            finally
            {
                // 🌟 [핵심] 설정 반영이 성공하든, 중간에 예외가 발생하든 
                // 실시간 모니터링 타이머는 무조건 안전하게 다시 켭니다.
                TCSPCDeviceController.StartMonitoring();
            }
        }

        private void OnCountRateUpdated(int rate0, int rate1) =>
            (lblCountRate0.Text, lblCountRate1.Text) = ($"Count Rate 0 : {rate0}", $"Count Rate 1 : {rate1}");


        private decimal lastSyncValue = 1;
        private void numSyncDiv_ValueChanged(object sender, EventArgs e)
        {
            int current = (int)numSyncDiv.Value;

            if (current > lastSyncValue)
            { // 위로 버튼(▲)을 눌렀을 때\
                if (lastSyncValue == 1) numSyncDiv.Value = 2; else if (lastSyncValue == 2) numSyncDiv.Value = 4; else if (lastSyncValue == 4) numSyncDiv.Value = 8; else if (lastSyncValue == 8) numSyncDiv.Value = 8; // 8이면 고정
            }
            else if (current < lastSyncValue)
            { // 아래로 버튼(▼)을 눌렀을 때
                if (lastSyncValue == 8) numSyncDiv.Value = 4; else if (lastSyncValue == 4) numSyncDiv.Value = 2; else if (lastSyncValue == 2) numSyncDiv.Value = 1; else if (lastSyncValue == 1) numSyncDiv.Value = 1; // 1이면 고정
            }

            // 현재 값을 다시 저장
            lastSyncValue = numSyncDiv.Value;

            // 장비 설정값에 반영
            PicoHarp_MeasurementSettings.SyncDivider = (int)numSyncDiv.Value;
            UpdateDeviceSettings();
        }

        private void numBinning_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings_PicoHarp();
        private void numAcqTime_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings_PicoHarp();
        private void numCFDLevel0_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings_PicoHarp();
        private void numCFDZeroCross0_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings_PicoHarp();
        private void numCFDLevel1_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings_PicoHarp();
        private void numCFDZeroCross1_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings_PicoHarp();


    }
}