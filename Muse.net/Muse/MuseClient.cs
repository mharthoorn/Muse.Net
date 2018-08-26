using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Harthoorn.MuseClient
{
    public class MuseClient
    {
        public string Name { get; private set; }
        public ulong Address { get; private set; }

        public MuseClient(ulong address)
        {
            this.Address = address;
        }

        private BluetoothLEDevice device;
        private GattDeviceService service;

        private GattCharacteristic ch_control;
        private GattCharacteristic ch_accelerometer;
        private GattCharacteristic ch_gyroscope;
        private GattCharacteristic ch_telemetry;

        private GattCharacteristic ch_EEG_TP9;
        private GattCharacteristic ch_EEG_AF7;
        private GattCharacteristic ch_EEG_AF8;
        private GattCharacteristic ch_EEG_TP10;
        private GattCharacteristic ch_EEG_AUX;

        public event Action<Telemetry> NotifyTelemetry;
        public event Action<Accelerometer> NotifyAccelerometer;
        public event Action<Gyroscope> NotifyGyroscope;
        public event Action<Channel, Encefalogram> NotifyEeg;

        public IList<Channel> Subscriptions { get; private set; } = new List<Channel>();

        public async Task<bool> Connect()
        {
            device = await BluetoothLEDevice.FromBluetoothAddressAsync(this.Address);
            if (device is null) return false;

            service = device.GetGattService(MuseGuid.PRIMARY_SERVICE);
            if (service is null) return false;

            ch_control = service.GetCharacteristic(MuseGuid.CONTROL);
            ch_accelerometer = service.GetCharacteristic(MuseGuid.ACELEROMETER);
            ch_gyroscope = service.GetCharacteristic(MuseGuid.GYROSCOPE);
            ch_telemetry = service.GetCharacteristic(MuseGuid.TELEMETRY);

            ch_EEG_TP9 = service.GetCharacteristic(MuseGuid.EEG_TP9);
            ch_EEG_AF7 = service.GetCharacteristic(MuseGuid.EEG_AF7);
            ch_EEG_AF8 = service.GetCharacteristic(MuseGuid.EEG_AF8);
            ch_EEG_TP10 = service.GetCharacteristic(MuseGuid.EEG_TP10);
            ch_EEG_AUX = service.GetCharacteristic(MuseGuid.EEG_AUX);

            return true;
        }

        public async Task Subscribe(params Channel[] channels)
        {
            foreach(var channel in channels)
            {
                await SubscribeToChannel(channel);
            }
        }

        public async Task Resume()
        {
            await ch_control.WriteCommand(Command.RESUME); 
        }

        public async Task Start()
        {
            await ch_control.WriteCommand(Command.START); 
        }

        public async Task Pause()
        {
            await ch_control.WriteCommand(Command.PAUSE); 
        }

        public async Task UnsubscribeAll()
        {
            foreach(var channel in Subscriptions)
            {
                await UnsubscribeFromChannel(channel);
            }
        }

        public async Task<bool> SubscribeToChannel(Channel channel)
        {
            var characteristic = GetCharacteristic(channel);
            var status =  await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            var ok = (status == GattCommunicationStatus.Success);

            if (ok)
            {
                characteristic.ValueChanged += Notify;
                Subscriptions.Add(channel);
            }

            return ok;
        }

        public async Task<bool> UnsubscribeFromChannel(Channel channel)
        {
            var characteristic = GetCharacteristic(channel);
            var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
            var ok = (status == GattCommunicationStatus.Success);

            if (ok)
            {
                characteristic.ValueChanged -= Notify;
                Subscriptions.Remove(channel);
            }

            return ok;
        }


        private void Notify(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var bytes = args.CharacteristicValue.ToArray();
            var channel = GetChannel(sender);
            switch (channel)
            {
                case Channel.Telemetry: TriggerNotifyTelemetry(bytes); break;
                case Channel.Accelerometer: TriggerNotifyAccelerometer(bytes); break;
                case Channel.Gyroscope: TriggerNotifyGyroscope(bytes); break;
                case Channel.EEG_AF7:
                case Channel.EEG_AF8:
                case Channel.EEG_TP9:
                case Channel.EEG_TP10:
                case Channel.EEG_AUX: TriggerNotifyEeg(channel, bytes); break;
                
            }
        }

        private void TriggerNotifyEeg(Channel channel, ReadOnlySpan<byte> bytes)
        { 
            var eeg = Parse.Encefalogram(bytes);
            NotifyEeg?.Invoke(channel, eeg);
        }

        private void TriggerNotifyTelemetry(ReadOnlySpan<byte> bytes)
        {
            var telemetry = Parse.Telemetry(bytes);
            NotifyTelemetry?.Invoke(telemetry);
        }

        private void TriggerNotifyAccelerometer(ReadOnlySpan<byte> bytes)
        {
            var accelerometer = Parse.Accelerometer(bytes);
            NotifyAccelerometer?.Invoke(accelerometer);
        }

        private void TriggerNotifyGyroscope(ReadOnlySpan<byte> bytes)
        {
            var gyroscope = Parse.Gyroscope(bytes);
            NotifyGyroscope?.Invoke(gyroscope);
        }

        private GattCharacteristic GetCharacteristic(Channel channel)
        {
            switch (channel)
            {
                case Channel.Accelerometer: return ch_accelerometer;
                case Channel.Control: return ch_control;
                case Channel.Gyroscope: return ch_gyroscope;
                case Channel.Telemetry: return ch_telemetry;
                case Channel.EEG_AF7: return ch_EEG_AF7;
                case Channel.EEG_AF8: return ch_EEG_AF8;
                case Channel.EEG_TP9: return ch_EEG_TP9;
                case Channel.EEG_TP10: return ch_EEG_TP10;
                case Channel.EEG_AUX: return ch_EEG_AUX;
                default: return null;
            }
        }

        private Channel GetChannel(GattCharacteristic ch) 
        {
            if (ch == ch_control) return Channel.Control;
            if (ch == ch_accelerometer) return Channel.Accelerometer;
            if (ch == ch_telemetry) return Channel.Telemetry;
            if (ch == ch_gyroscope) return Channel.Gyroscope;
            if (ch == ch_EEG_AF7) return Channel.EEG_AF7;
            if (ch == ch_EEG_AF8) return Channel.EEG_AF8;
            if (ch == ch_EEG_TP9) return Channel.EEG_TP9;
            if (ch == ch_EEG_TP10) return Channel.EEG_TP10;
            if (ch == ch_EEG_AUX) return Channel.EEG_AUX;

            return Channel.None;
            
        }



    }

}
