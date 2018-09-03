using Harthoorn.MuseClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Muse.LiveFeed
{
    class SpeedGraph : Panel
    {
        Graphics graphics;
        Bitmap bitmap;
        Timer timer;

        Dictionary<Channel, List<float>> data = new Dictionary<Channel, List<float>>()
        {
            { Channel.EEG_AF7, new List<float>() },
            { Channel.EEG_AF8, new List<float>() },
            { Channel.EEG_TP9, new List<float>() },
            { Channel.EEG_TP10, new List<float>() },
            { Channel.EEG_AUX, new List<float>() },
        };

        public SpeedGraph()
        {
            this.DoubleBuffered = true;
            graphics = this.CreateGraphics();
            timer = new Timer();
            timer.Enabled = true;
            timer.Interval = 10;
            timer.Tick += Timer_Tick;

            this.Paint += SpeedGraph_Paint;
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //UpdateStyles();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (data)
            {
                this.Invalidate();
            }

        }

        private void SpeedGraph_Paint(object sender, PaintEventArgs e)
        {
            lock (data)
            {
                bitmap = new Bitmap(this.Width, this.Height);
                var graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;
                graphics.Clear(Color.Black);
                Draw(graphics, data[Channel.EEG_AF7], Color.Green, 20, 100);
                Draw(graphics, data[Channel.EEG_AF8], Color.Blue, 120, 100);
                Draw(graphics, data[Channel.EEG_TP9], Color.Red, 220, 100);
                Draw(graphics, data[Channel.EEG_TP10], Color.Yellow, 320, 100);

                e.Graphics.DrawImage(bitmap, 1, 1);
            }
        }

        public void Append(Channel channel, float[] values)
        {
            lock (data)
            {
                var set = data[channel];

                int max = this.Width;
                set.AddRange(values);
                int cut = Math.Max(0, set.Count - max);
                if (cut > 0) set.RemoveRange(0, cut);

            }
            
        }

        public void Draw(Graphics graphics, IList<float> data, Color color, int offset, int height)
        {
            graphics.DrawLine(new Pen(color, 4), 10, offset, 10, offset + height);

            int count = data.Count;
            float factor = (float)height / 0x400;
            Pen pen = new Pen(color);

            int xa = 0, ya = 0;
            bool first = true;
            for (int x = 0; x < count; x++)
            {
                float actual = data[x]-0x400;
                int y = offset + (int)(actual * factor);
                
                if (first)
                {
                    first = false;
                }
                else
                {
                    graphics.DrawLine(pen, xa, ya, x, y);
                }
                xa = x; ya = y;
            }
            var brush = new SolidBrush(Color.White);
            var font = new Font("Arial", 12);
        }
    }

    
}
