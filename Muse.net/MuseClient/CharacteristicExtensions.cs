using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Harthoorn.MuseClient
{
    public static class CharacteristicExtensions
    {
        public async static Task<bool> WriteCommand(this GattCharacteristic control, string command)
        {
            var buffer = Language.EncodeCommand(command);
            var status = await control.WriteValueAsync(buffer, GattWriteOption.WriteWithoutResponse).AsTask();
            return (status == GattCommunicationStatus.Success);
        }


        public async static Task<bool> Start(this GattCharacteristic control, GattCharacteristic c)
        {
            //var ok = await control.WriteCommand(Command.PAUSE);

            //if (ok)
            var ok = await control.WriteCommand(Preset.ACELEROMETER);

            if (ok)
                ok = await control.WriteCommand(Command.START);

            if (ok)
                ok = await control.WriteCommand(Command.RESUME);

            return ok;
        }

        

        public static GattCharacteristic GetCharacteristic(this GattDeviceService service, Guid UUID)
        {
            return service.GetCharacteristics(UUID).FirstOrDefault();
        }
    }
}
