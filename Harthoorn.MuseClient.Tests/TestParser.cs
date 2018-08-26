using Harthoorn.MuseClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluetoothCore
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void TestTelemetry()
        {
            var array = new byte[] { 1, 74, 181, 184, 7, 64, 15, 127, 0, 27, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // bigendian:            [seq ] [battery] [volt] [?????], [temp]

            var t = Telemetry.Parse(array);
            Assert.AreEqual(90, (int)t.BatteryLevel);
            Assert.AreEqual(4083, (int)t.Voltage);
            Assert.AreEqual(330, t.SequenceId);
            Assert.AreEqual(27, t.Temperature);
            //Print.TelemetryModel(t);
        }
    }
}
