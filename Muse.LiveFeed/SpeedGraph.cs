using Harthoorn.MuseClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        bool update = true;

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
            //this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            graphics = this.CreateGraphics();
            timer = new Timer();
            timer.Enabled = true;
            timer.Interval = 200;
            timer.Tick += Timer_Tick;

            this.Paint += SpeedGraph_Paint;
            
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (update) this.Invalidate();
            

        }

        private void SpeedGraph_Paint(object sender, PaintEventArgs e)
        {
            lock (data)
            {
                bitmap = new Bitmap(this.Width, this.Height);
                var graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                Draw(graphics, data[Channel.EEG_AF7], Color.LightGreen, 20, 100);
                Draw(graphics, data[Channel.EEG_AF8], Color.LightSkyBlue, 140, 100);
                Draw(graphics, data[Channel.EEG_TP9], Color.OrangeRed, 260, 100);
                Draw(graphics, data[Channel.EEG_TP10], Color.LightYellow, 380, 100);

                e.Graphics.DrawImage(bitmap, 1, 1);
                update = false;
            }
        }

        public void Append(Channel channel, float[] values)
        {
            lock (data)
            {
                var set = data[channel];

                
                set.AddRange(values);
                LimitFromStart(set, this.Width);
                update = true;
            }
            
        }

        public void LimitFromStart<T>(List<T> list, int limit)
        {
            int cut = Math.Max(0, list.Count - limit);
            if (cut > 0) list.RemoveRange(0, cut);
        }

        const float AMPLITUDE = 0x800; 

        public void Draw(Graphics graphics, IList<float> data, Color color, int offset, int height)
        {
            
            var axispen = new Pen(Color.Gray, 1);

            graphics.DrawLine(axispen, 10, offset, 10, offset + height);
            Pen pen = new Pen(color);

            int ymax = height / 2;
            int y0 = offset + (int)ymax;
            graphics.DrawLine(axispen, 0, y0, this.Width, y0);

            int count = data.Count;

            float factor = (float)ymax / AMPLITUDE;

            int xa = 0, ya = 0;
            bool first = true;

            for (int x = 0; x < data.Count; x++)
            {
                float actual = data[x] - AMPLITUDE;
                int v = (int)(factor * actual); 
                int y = y0 - v;

                if (x > 0)
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
