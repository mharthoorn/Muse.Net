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

            var t = Parse.Telemetry(array);
            Assert.AreEqual(90, (int)t.BatteryLevel);
            Assert.AreEqual(4083, (int)t.Voltage);
            Assert.AreEqual(330, t.SequenceId);
            Assert.AreEqual(27, t.Temperature);
            //Print.TelemetryModel(t);
        }

        [TestMethod]
        public void TestGyroscope()
        {
            var array = new byte[] {
                82, 109,    // sequence
                13, 178, 13, 157, 60, 115, // sample 1
                18, 5, 13, 73, 60, 53, 17, // sample 2
                183, 17, 227, 60, 143   // sample 3
            }; 

            var a = Parse.Gyroscope(array);
            Assert.AreEqual(21101, a.SequenceId);

            Assert.AreEqual(0.2139894112f, a.Samples[0].X);
            Assert.AreEqual(0.21270767200000001f, a.Samples[0].Y);
            Assert.AreEqual(0.9445197200000001f, a.Samples[0].Z);

            Assert.AreEqual(0.2815553776f, a.Samples[1].X);
            Assert.AreEqual(0.20758071520000002f, a.Samples[1].Y);
            Assert.AreEqual(0.9407355376000001f, a.Samples[1].Z);

            Assert.AreEqual(0.276794632f, a.Samples[2].X);
            Assert.AreEqual(0.27948018080000003f, a.Samples[2].Y);
            Assert.AreEqual(0.9462287056f, a.Samples[2].Z);

        }

        [TestMethod]
        public void TestGram()
        {

            var array = new byte[] { 87, 33, 192, 82, 73, 6, 106, 242, 49, 64, 88, 153, 128, 66, 254, 44, 119, 157 };
            var correct = new int[] { 1394, 448, 1316, 2310, 1711, 561, 1029, 2201, 2052, 766, 711, 1949 };

            var g = Parse.EegSamples(array);

            for (int i = 0; i < 12; i++)
            {
                Assert.AreEqual(correct[i], g[i]);
            }
        }
    }
}
