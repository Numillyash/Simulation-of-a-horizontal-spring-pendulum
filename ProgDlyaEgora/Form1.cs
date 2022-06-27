using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Universal_Library_2;
using System.Threading;

namespace ProgDlyaEgora
{
    public partial class Form1 : Form
    {
        Universal un = new Universal();

        float g = 9.8f;

        static int centerX = 300;
        int centerY = 100;
        int graphY = 200;

        List<double> X = new List<double> { };
        List<double> Fc_m = new List<double> { };
        List<double> Fpr_m = new List<double> { };

        List<double> V = new List<double> { };

        double dxer = 0;

        float dt = 0.1f;

        bool first = true;
        static double x_pred = centerX + 100;
        double x_now = 0;

        Body body = new Body(x_pred, 1, 10, 0.9);

        float koeff = 0.5f;
        float koeff_t = 1f;

        private Bitmap _bitmap;
        private readonly Graphics _graphics;
        public Form1()
        {
            InitializeComponent();
            _bitmap = new Bitmap(1024, 768);
            _graphics = Graphics.FromImage(_bitmap);
            pictureBox1.Image = _bitmap;
            _graphics.Clear(Color.White);
            paintka();
        }
        private void paintka()
        {
            _graphics.Clear(Color.White);
            for (int i = 1; i < X.Count; i++)
            {
                _graphics.DrawLine(new Pen(Color.Black, 1), new PointF((i - 1) * koeff_t, (float)(X[i - 1] * koeff) + graphY), new PointF(i * koeff_t, (float)(X[i] * koeff) + graphY));

                //_graphics.DrawLine(new Pen(Color.Blue, 3), new PointF((i - 1) * koeff_t, (float)(Fc_m[i - 1] * koeff) + graphY), new PointF(i * koeff_t, (float)(Fc_m[i] * koeff) + graphY));

                //_graphics.DrawLine(new Pen(Color.Green, 1), new PointF((i - 1) * koeff_t, (float)(V[i - 1] * koeff) + graphY), new PointF(i * koeff_t, (float)(V[i] * koeff) + graphY));

                //_graphics.DrawLine(new Pen(Color.Red, 1), new PointF((i - 1) * koeff_t, (float)(Fpr_m[i - 1] * koeff) + graphY), new PointF(i * koeff_t, (float)(Fpr_m[i] * koeff) + graphY));
            }
            _graphics.DrawLine(new Pen(Color.Green, 1), new PointF(0, graphY), new PointF(800, graphY));
            _graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, 800, 100));
            _graphics.DrawLine(new Pen(Color.Green, 1), new PointF(0, centerY), new PointF(800, centerY));

            _graphics.DrawLine(new Pen(Color.Green, 1), new PointF(centerX, 0), new PointF(centerX, centerY));
            _graphics.DrawLine(new Pen(Color.Green, 1), new PointF(2 * centerX, 0), new PointF(2 * centerX, centerY));
            _graphics.DrawLine(new Pen(Color.Blue, 1), new PointF((float)dxer + centerX, centerY - 10), new PointF(2 * centerX, centerY - 10));


            _graphics.FillRectangle(new SolidBrush(Color.Red), new RectangleF((float)dxer - 10 + centerX, centerY - 20, 20, 20));

            Refresh();
        }

        bool keyed = false;

        private void frmSendSMS_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode.Equals(Keys.Return))
            if (e.KeyCode == Keys.Space)
            {
                if (keyed)
                {
                    dt = 0.1f;
                }
                else
                    dt = 0;
            }
        }

        private void schet()
        {
            X = new List<double> { };
            Fc_m = new List<double> { };
            Fpr_m = new List<double> { };
            dxer = 0;
            first = true;
            x_pred = centerX + 100;
            x_now = 0;
            body = new Body(x_pred, 1, 5, 0.9);
            double dx_max = 1;
            double dx_min = 0;

            bool isWorking = true;
            bool lol = true;
            while (isWorking)
            {
                Thread.Sleep(24);

                phisic(out dxer);
                if (dxer > dx_max)
                {
                    dx_max = dxer;
                }
                else if (dxer < dx_max && lol)
                {
                    lol = false;

                    //un.println(dx_max);
                    if (dx_max < 0.9)
                    { isWorking = false; }
                    //_graphics.DrawLine(new Pen(Color.Blue, 1), new PointF((i) * koeff, 0), new PointF((i) * koeff, 400));
                    dx_min = dx_max;
                }
                if (dxer < dx_min)
                {
                    dx_min = dxer;
                }
                else
                if (dxer > dx_min && !lol)
                {
                    lol = true;
                    //_graphics.DrawLine(new Pen(Color.Blue, 1), new PointF((i) * koeff, 0), new PointF((i) * koeff, 400));
                    dx_max = dx_min;
                }
                paintka();

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void phisic(out double dx)
        {

            V.Add(body.vel);
            dx = centerX - body.x;
            double Fpr = (body.k * dx);
            double Fc = -un.sign((int)body.vel) * body.mass * body.mu * g;
            double F = Fc + Fpr;
            if (first)
            {
                body.acc = F / body.mass;
                body.x += body.acc * dt * dt / 2;
                x_now = body.x;
                first = !first;
                body.vel += body.acc * dt;
            }
            else
            {
                body.acc = F / body.mass;
                body.x = 2 * x_now - x_pred + body.acc * dt * dt;

                x_pred = x_now;
                x_now = body.x;
                body.vel = (x_now - x_pred) / dt;
            }
            X.Add(body.x - centerX);
            Fc_m.Add(Fc);
            Fpr_m.Add(Fpr);
            //un.println(body.x);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            schet();
        }

    }
    public class Body
    {
        /// <summary>
        /// Координата по Х в м
        /// </summary>
        public double x { get; set; }

        /// <summary>
        /// Масса в кг
        /// </summary>
        public double mass { get; set; }

        /// <summary>
        /// Коеффицент сопротивления пружины
        /// </summary>
        public double k { get; set; }

        /// <summary>
        /// Коеффицент трения поверхности
        /// </summary>
        public double mu { get; set; }

        /// <summary>
        /// Скорость
        /// </summary>
        public double vel { get; set; } = 0;

        /// <summary>
        /// Ускорение
        /// </summary>
        public double acc { get; set; }

        public Body()
        {
        }

        public Body(double x, double mass, double k, double mu)
        {
            this.x = x;
            this.mass = mass;
            this.k = k;
            this.mu = mu;
        }
    }
}
