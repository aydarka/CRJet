using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace CRJet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public enum Injector
        {
            Os,
            N,
            NN
        }
        class FixOils
        {
            public double t = 0;
            public Rectangle obl;
            int tolsh;
            public int num_wal;
            Rectangle[] rect;
            byte detal;
            Brush brush; 
            public FixOils(int x, int y, int tolsh, Rectangle[] rect, int num_wal, int d, Brush b)//, double vxO, double vyO, double w1, double med)
            {
                this.detal = 1;
                this.rect = rect;
                this.num_wal = num_wal;
                this.tolsh = tolsh;
                obl = new Rectangle(x, y, d, d);
                this.brush = b;
            }
            public FixOils(int x, int y, int tolsh, int d, Brush b)
            {
                this.detal = 2;
                this.tolsh = tolsh;
                obl = new Rectangle(x, y, d, d);
                this.brush = b;
            }
            public void Draw(Graphics g, int palec_y)
            {
                if (detal == 1)
                {
                    obl.Y = rect[num_wal].Top + tolsh;
                }
                else if (detal == 2)
                {
                    obl.Y = palec_y + tolsh;
                }
                g.FillEllipse(this.brush, obl);
            }
        }
        public class Oils
        {
            public double t = 0;
            public double d, d_;
            public double p;
            public double ro, m_, V_;
            public Rectangle obl;
            public int xB, yB;
            double BAngle;
            double Angle;
            double k_, mu;
            //int yg;
            double vx0, vy0, v0;
            internal double vx, vy, v;
            public double x, y;
            double Sin(double Angle)
            {
                return Math.Sin(Angle * Math.PI / 180);
            }
            double Cos(double Angle)
            {
                return Math.Cos(Angle * Math.PI / 180);
            }
            double Tg(double Angle)
            {
                return Math.Tan(Angle * Math.PI / 180);
            }
            public Oils(double Angle1, int Gamma, int DrillAngle, double w1, double d1,
                double p1, double temperature1, int med, int l, Injector nozzle)
            {
                this.Angle = Angle1;
                BAngle = Math.Asin(Sin(Angle) * crankshaftLenght / rockerLenght) * 180 / Math.PI;
                int xA = (int)(Sin(Angle) * crankshaftLenght);
                int yA = (int)(Cos(Angle) * crankshaftLenght);
                double w = w1 * 2 * Math.PI / 60;//частота вращения КВ [об/мин] → переведено в [рад/с]
                ro = 0.000000001 * (903.6 - 5.65 * temperature1 * 0.1); //плотность моторного масла [кг/м3] → переведено в [кг/мм3]
                p = p1 * 100; //давление подачи масла [бар] → переведено в [МПа] или [(кг*мм/с2)/мм2]
                this.d = d1;
                double v_tau = 0;
                double v_p = Math.Sqrt(2 * p / ro) * scale;
                double v_p_x = 0;
                double v_p_y = 0;
                double v_c_x = 0;
                double v_c_y = 0;
                int r_r = 19 + 240 / l;
                if ((l == 22 * scale && (nozzle == Injector.NN || nozzle == Injector.N)) || nozzle == Injector.Os)
                {
                    xB = (int)(xA - ShShDiam / 2 * Cos(90 - BAngle + DrillAngle));
                    yB = (int)(yA + ShShDiam / 2 * Sin(90 - BAngle + DrillAngle));
                    v_tau = w * (crankshaftLenght + ShShDiam / 2 * Cos(Angle - (DrillAngle - BAngle)));
                    double v_k = (w * Math.Sqrt(d * d / 2) + Math.Sqrt(w * w * d * d / 2 + 2 * p / ro)) * scale -
                        Math.Sqrt(2 * p / ro) * scale;
                    double v_k_x = v_k * Sin(Angle);
                    double v_k_y = v_k * Cos(Angle);
                    double v_tau_x = v_tau * Cos(Angle);
                    double v_tau_y = v_tau * Sin(Angle);
                    v_c_x = v_k_x + v_tau_x;
                    v_c_y = v_k_y + v_tau_y;
                }
                else if (nozzle == Injector.NN)
                {
                    int r_s;
                    if (l > 240)
                    {
                        r_s=(int)(PalecDiam+14)/ 2;
                        xB = (int)(r_s * Sin(Gamma - BAngle));
                        yB = (int)(crankshaftLenght * Cos(Angle) + rockerLenght * Cos(BAngle) + r_s * Cos(Gamma - BAngle));
                    }
                    else
                    {
                        r_s= (int)(r_r / Cos(90 - Gamma)) + 4;
                        xB = (int)((rockerLenght - l) * Sin(BAngle) + r_s * Sin(Gamma - BAngle));
                        yB = (int)(crankshaftLenght * Cos(Angle) + l * Cos(BAngle) + r_s * Cos(Gamma - BAngle));
                    }
                    v_c_x = -crankshaftLenght * Cos(Angle) * (l / rockerLenght - 1) * w;
                    v_c_y = -(crankshaftLenght * (-Sin(Angle) - l * Tg(BAngle) * Cos(Angle) / rockerLenght)) * w;
                }
                else if (nozzle == Injector.N)
                {
                    int r_s;
                    if (l > 240)
                    {
                        r_s = (int)(PalecDiam + 14) / 2;
                        xB = -(int)(r_s * Sin(-Gamma + BAngle));
                        yB = (int)(crankshaftLenght * Cos(Angle) + rockerLenght * Cos(BAngle) + r_s * Cos(-Gamma + BAngle));
                    }
                    else
                    {
                        r_s = (int)(r_r / Cos(90 + Gamma)) + 4;
                        xB = (int)((rockerLenght - l) * Sin(BAngle) - r_s * Sin(-Gamma + BAngle));
                        yB = (int)(crankshaftLenght * Cos(Angle) + l * Cos(BAngle) + r_s * Cos(-Gamma + BAngle));
                    }
                    v_c_x = -crankshaftLenght * Cos(Angle) * (l / rockerLenght - 1) * w;
                    v_c_y = -(crankshaftLenght * (-Sin(Angle) - l * Tg(BAngle) * Cos(Angle) / rockerLenght)) * w;
                }
                v_p_x = v_p * Sin(Gamma - BAngle);
                v_p_y = v_p * Cos(Gamma - BAngle);
                vx0 = v_p_x + v_c_x;
                vy0 = v_p_y + v_c_y;
                v0 = Math.Sqrt(vx0 * vx0 + vy0 * vy0);
                d_ = 0.06 / med * v0;
                if (d_ < 2)
                    d_ = 2;
                else if (d_ > 10)
                    d_ = 10;
                V_ = d / 2 * Math.PI * d / 2 * d / 2 / 4; //объем элементарного цилиндрика струи масла [мм3]
                m_ = ro * V_; //масса элементарного цилиндрика струи масла [кг]
                mu = 18.1 / 1000000; //динамическая вязкость воздуха [Н/мм2] или [кг*1000мм/(с*мм)^2]
                k_ = 6 * Math.PI * d / 2 * mu;// *1000; //коэффициент сопротивления Стокса [кг/с^2]
                vx = vx0;
                vy = vy0;
            }
            public void Fly(Brush b, Graphics g, double med, double interval)
            {
                double vxt, vyt;
                //учет сопротивления воздуха
                if (v > 30000)
                {
                    vxt = ((-m_ / k_) * vx0 * Math.Exp(-k_ / m_ * t) + m_ / k_ * vx0);
                    //vyt = (-m_ / k_) * ((vy0 + scale * (9810 - a_piston/10) * m_ / k_) * Math.Exp(-k_ / m_ * t) +
                    //    scale * 9810 * t - (vy0 + scale * (9810 - a_piston/10) * m_ / k_));
                    vyt = (-m_ / k_) * ((vy0 + scale * 9810 * m_ / k_) * Math.Exp(-k_ / m_ * t) +
                        scale * 9810 * t - (vy0 + scale * 9810 * m_ / k_));
                    vx = vxt / t;
                    vy = vyt / t;
                }
                else
                {
                    vxt = vx * t;
                    //vyt = vy * t - (9810 - a_piston/10) * t * t * scale / 2;
                    vyt = vy * t - 9810 * t * t * scale / 2;
                }
                v = Math.Sqrt(vxt / t * vxt / t + vyt / t * vyt / t);
                x = (int)(cx + xB + vxt);
                y = (int)(cy - yB - vyt);

                obl = new Rectangle((int)(x - d_ / 2), (int)(y - d_ / 2), (int)d_, (int)d_);
                //obl = new Rectangle((int)(x - d/ 2), (int)(y - d/ 2), (int)d, (int)d);
                g.FillEllipse(b, obl);
                double dt;
                dt = (double)interval * 0.001 / (double)med;//1000 / ((double)w1 * 6) / (double)(med);
                t += dt;
            }
        }
        //переменные и объекты
        Graphics g;
        Bitmap bitmap;
        Pen pen, pen2, penB, PenPurple, PenViolet, pa, pb, PenOrange, PenRed, PenOrangeT, PenRedT, PenPurpleT, PenVioletT;
        Brush brushL, brushG, brushD, BrushRed, BrushOrange, BrushOrangeRed, brushPen, 
            brushPen2, brushProc;
        int ugl2 = 31, ugl = 31, drill = 90, faza = 20, gamma = 31, gamma2 = 31,
            fi2 = 90, psi2 = 70, fi = 70, psi = 90, med_max = 2048, med_min = 256,
            seconds = 0, faz = 90, domed = 1024, palec_Y, x1, y1, x2, y2, x3, y3, x4, y4,
            psiMax = 180, psi2Max = 180, l = 22 * scale, l2 = 22 * scale, 
            gamma2Min=5,gammaMin=5, gamma2Max = 90, gammaMax = 90;
        double d = 1.5 * scale, a = 0, med = 1024, da, ds, p = 4, temperature = 20,
            w = 3000; //частота вращения КВ 3000 [об/мин]
        bool bey = false, beyN = false, beyNN = false, beyOs = false, N = false, NN = false,
            prot = false, avto = false, Os = true, coorChange = true;
        const int cx = 93 * scale, cy = 250 * scale, scale = 2, crankshaftLenght = 40 * scale,
            rockerLenght = 136 * scale, KShDiam = 66 * scale, ShShDiam = 48 * scale,
            PalecDiam = 30 * scale, PistonDiam = 85 * scale, wall_n = 5;
        Rectangle[] wall = new Rectangle[wall_n];
        List<Oils> oN = new List<Oils>();
        List<Oils> oNN = new List<Oils>();
        List<FixOils> fix_o = new List<FixOils>();
        //double a_pist, a_piston,aA,aB;
        Rectangle prot_rect, prot2_rect, prot_rect2, prot2_rect2, faza_rect, dopfaza_rect;
        //Rectangle airBlock;
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Угол поворота КВ [град.]:\n\n               " + "0";
            //label6.Text = "Направление струи:\nна НН стенку";
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bitmap);
            pictureBox1.Image = bitmap;
            pen = new Pen(Color.DarkCyan, 1);
            pen2 = new Pen(Color.Black, 1);
            penB = new Pen(Color.Gray, 4);
            PenPurple = new Pen(Color.Purple, 3);
            PenViolet = new Pen(Color.BlueViolet, 3);
            pa = new Pen(Color.OrangeRed, 1);
            PenOrange = new Pen(Color.Orange, 3);
            PenRed = new Pen(Color.Red, 3);
            pb = new Pen(Color.Purple, 1);
            brushL = Brushes.LightGray;
            brushG = Brushes.Gray;
            BrushRed = Brushes.Red;
            BrushOrange = Brushes.Orange;
            BrushOrangeRed = Brushes.OrangeRed;
            brushD = Brushes.DarkGray;
            brushPen = Brushes.DarkCyan;
            brushPen2 = Brushes.Black;
            brushProc = Brushes.LightBlue;
            PenRedT=new Pen(Color.Red, 2);
            PenOrangeT = new Pen(Color.Orange, 2);
            PenPurpleT = new Pen(Color.Purple, 2);
            PenVioletT = new Pen(Color.BlueViolet, 2);
            Draw(0);
            //aA = crankshaftLenght * (w / 30 * Math.PI) * (w / 30 * Math.PI);
            //aB = crankshaftLenght / rockerLenght / 2;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //var begin = DateTime.Now;
            try
            {
                ds += 1;
                if (ds * timer1.Interval >= 1000)
                {
                    ds = 0;
                    seconds++;
                }
                if (a >= 720)
                    a = 0;
                Draw(a);
                if (avto && (bey || beyN || beyNN || beyOs || oN.Count > 1 || oNN.Count > 1))
                    med = 1024;
                else
                    med = domed;
                da = w * 6 * (double)timer1.Interval * 0.001 / med;
                a += da;
                //a_piston = aA * (Cos(a) + aB * Cos(2 * a));
                //"da = " + Convert.ToString(da) + "\ndt = " + Convert.ToString(da/(w*6));
                label1.Text = "Угол поворота КВ [град.]:\n\n               " + (int)a;
                label_med.Text = med.ToString();
                //if (oN.Count>1)
                //    label4.Text = Convert.ToString(oN[1].v);
                if ((oN.Count > 1 && oN[1].v < 5000) || (oNN.Count > 1 && oNN[1].v < 5000))
                    med_min = 64;
                else
                {
                    med_min = 256;
                    if (med < med_min)
                    {
                        med = med_min;
                        label_med.Text = med.ToString();
                    }
                }
                if (bey)
                {
                    if (checkBox_N.Checked && (faza_rect.IntersectsWith(prot_rect) || dopfaza_rect.IntersectsWith(prot_rect)) ||
                        (checkBox_NN.Checked && checkBox_N.Checked &&
                        (faza_rect.IntersectsWith(prot2_rect) || dopfaza_rect.IntersectsWith(prot2_rect) ||
                        faza_rect.IntersectsWith(prot_rect) || dopfaza_rect.IntersectsWith(prot_rect)) &&
                        (psi + psi2 >= 359) || (fi == 0 && fi2 == 0)))
                        oN.Add(new Oils(a, -gamma, -ugl, w, d, p, temperature, (int)med, l, Injector.N));
                    if (checkBox_NN.Checked && (faza_rect.IntersectsWith(prot2_rect) || dopfaza_rect.IntersectsWith(prot2_rect)) ||
                        (checkBox_NN.Checked && checkBox_N.Checked &&
                        (faza_rect.IntersectsWith(prot2_rect) || dopfaza_rect.IntersectsWith(prot2_rect) ||
                        faza_rect.IntersectsWith(prot_rect) || dopfaza_rect.IntersectsWith(prot_rect)) &&
                        (psi + psi2 >= 359) || (fi == 0 && fi2 == 0)))
                        oNN.Add(new Oils(a, gamma2, ugl2, w, d, p, temperature, (int)med, l2, Injector.NN));
                }
                if (beyN)
                    oN.Add(new Oils(a, -gamma, -ugl, w, d, p, temperature, (int)med, l, Injector.N));
                if (beyNN)
                    oNN.Add(new Oils(a, gamma2, ugl2, w, d, p, temperature, (int)med, l2, Injector.NN));
                if (beyOs)
                    oN.Add(new Oils(a, 0, 0, w, d, p, temperature, (int)med, l, Injector.Os));

                for (int i = 1; i < oN.Count; i++)
                {
                    oN[i].Fly(BrushRed, g, med, timer1.Interval);
                    if (oN[i].obl.X < 0 || oN[i].obl.X > pictureBox1.Width
                        || oN[i].obl.Y > pictureBox1.Height || oN[i].obl.Y < 0)
                    {
                        oN.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (Os)
                    {
                        //Проверка на пересечение "капли" и "пальца" при направлении по оси шатуна
                        int y_palec = (int)(Math.Sqrt(PalecDiam * PalecDiam / 4 -
                            (oN[i].x - cx) * (oN[i].x - cx)) + palec_Y);
                        if (oN[i].y <= y_palec && oN[i].y >= -y_palec)
                        {
                            fix_o.Add(new FixOils(oN[i].obl.X, oN[i].obl.Y, oN[i].obl.Y - palec_Y, (int)d, BrushRed));
                            oN.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                    else
                    {
                        //Проверка на пересечение "капли Н" и ("верхн.г.ш." или "стерж.ш.") при направлении на Н стенку
                        //int y_verh_g = (int)(Math.Sqrt((PalecDiam + 10) * (PalecDiam + 10) / 4
                        //    - (oN[i].x - cx) * (oN[i].x - cx)) + palec_Y);
                        int shatun_left = (int)(-(x2 - x1) * oN[i].y - (x1 * y2 - x2 * y1)) / (y1 - y2);
                        int x = (int)(-Math.Sqrt((PalecDiam + 10) * (PalecDiam + 10) / 4 -
                        (oN[i].y - palec_Y) * (oN[i].y - palec_Y)) + cx);
                        //g.DrawEllipse(pen,x-3,(float)oN[i].y,6,6);
                        if (N && ((oN[i].x > x && oN[i].y>palec_Y- PalecDiam/2-5 && oN[i].y<palec_Y+PalecDiam/2+5) 
                            || oN[i].x >= shatun_left - 2))
                        {
                            oN.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                    for (int j = 0; j < wall_n; j++)
                    {
                        if (oN[i].obl.IntersectsWith(wall[j]))
                        {
                            fix_o.Add(new FixOils(oN[i].obl.X, oN[i].obl.Y, oN[i].obl.Y - wall[j].Top, wall, j, (int)d, BrushRed));//, oN[i].vx, oN[i].vy,w,med));
                            oN.RemoveAt(i);
                            i--;
                        }
                    }
                }
                for (int i = 1; i < oNN.Count; i++)
                {
                    oNN[i].Fly(BrushOrange, g, med, timer1.Interval);
                    if (oNN[i].x < 0 || oNN[i].x > pictureBox1.Width
                        || oNN[i].y > pictureBox1.Height || oNN[i].y < 0)
                    {
                        oNN.RemoveAt(i);
                        i--;
                        continue;
                    }
                    //Проверка на пересечение "капли НН" и ("верхн.г.ш." или "стерж.ш.") при направлении на НН стенку
                    int x = (int)(Math.Sqrt((PalecDiam + 10) * (PalecDiam + 10) / 4 -
                        (oNN[i].y - palec_Y) * (oNN[i].y - palec_Y)) + cx);
                    int shatun_right = (int)(-(x4 - x3) * oNN[i].y - (x3 * y4 - x4 * y3)) / (y3 - y4);
                    if (NN && (oNN[i].x < x || oNN[i].x <= shatun_right - 2))
                    {
                        oNN.RemoveAt(i);
                        i--;
                        continue;
                    }
                    for (int j = 0; j < wall_n; j++)
                        if (oNN[i].obl.IntersectsWith(wall[j]))
                        {
                            fix_o.Add(new FixOils(oNN[i].obl.X, oNN[i].obl.Y, oNN[i].obl.Y - wall[j].Top, wall, j, (int)d, BrushOrange));//, oN[i].vx, oN[i].vy,w,med));
                            oNN.RemoveAt(i);
                            i--;
                        }
                }
                for (int i = 0; i < fix_o.Count; i++)
                    fix_o[i].Draw(g, palec_Y);
            }
            catch (Exception e1)
            {
                timer1.Enabled = false;
                MessageBox.Show("Зафиксируйте момент возникновения данной ошибки и обратитесь к автору работы (sevenay@mail.ru)\n"
                    + e1, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            //var end = DateTime.Now;
            //label3.Text = (end - begin).Milliseconds.ToString();
        }

        //функции
        void Picture()
        {
            if (!NN && N && l == 44)
            {
                pictureBox2.BackgroundImage = Properties.Resources.прот1;
                ugl = 31;
            }
            else if (!NN && N && l != 44)
            {
                pictureBox2.BackgroundImage = Properties.Resources.prot_N;
                ugl = 0;
            }
            else if (!N && NN && l2 == 44)
            {
                pictureBox2.BackgroundImage = Properties.Resources.прот2;
                ugl2 = 31;
            }
            else if (!N && NN && l2 != 44)
            {
                pictureBox2.BackgroundImage = Properties.Resources.prot_NN;
                ugl2 = 0;
            }
            else if (N && l == 44 && NN && l2 == 44)
            {
                pictureBox2.BackgroundImage = Properties.Resources.прот21;
                ugl = 31;
                ugl2 = 31;
            }
            else if (N && l != 44 && NN && l2 != 44)
            {
                pictureBox2.BackgroundImage = Properties.Resources.prot_;
                ugl = 0;
                ugl2 = 0;
            }
            else if (N && l == 44 && NN && l2 != 44)
            {
                pictureBox2.BackgroundImage = Properties.Resources.pr_NN;
                ugl = 31;
                ugl2 = 0;
            }
            else if (N && l != 44 && NN && l2 == 44)
            {
                pictureBox2.BackgroundImage = Properties.Resources.pr_N;
                ugl = 0;
                ugl2 = 31;
            }
            else
            {
                ugl = 31;
                ugl2 = 31;
            }

        }
        void crankshaft(double Angle)
        {

            int x = (int)(Sin(Angle) * crankshaftLenght);
            int y = (int)(Cos(Angle) * crankshaftLenght);
            g.FillEllipse(brushL, cx - KShDiam / 2, cy - KShDiam / 2, KShDiam, KShDiam);
            g.FillEllipse(brushL, cx - (ShShDiam + 6 * scale) / 2 + x, cy - (ShShDiam + 6 * scale) / 2 - y, (ShShDiam + 6 * scale), (ShShDiam + 6 * scale));
            g.FillPie(brushL, cx - (int)(KShDiam * 2.1 / 2), cy - (int)(KShDiam * 2.1 / 2), (int)(KShDiam * 2.1), (int)(KShDiam * 2.1), (int)(Angle) + 18, 140);
        }
        void rocker(double Angle)
        {
            int x = (int)(Sin(Angle) * crankshaftLenght);
            int y = (int)(Cos(Angle) * crankshaftLenght);
            g.FillEllipse(brushD, cx - (ShShDiam + 10) / 2 + x, cy - (ShShDiam + 10) / 2 - y, (ShShDiam + 10), (ShShDiam + 10));

            double BAngle = Math.Asin(Sin(Angle) * crankshaftLenght / rockerLenght) * 180 / Math.PI;
            int yg = (int)(Cos(Angle) * crankshaftLenght + Cos(BAngle) * rockerLenght);
            palec_Y = cy - yg;
            g.FillEllipse(brushD, cx - (PalecDiam + 10) / 2, palec_Y - (PalecDiam + 10) / 2, (PalecDiam + 10), (PalecDiam + 10));

            Point[] ps = new Point[4];
            ps[0] = new Point((int)(cx + Sin(BAngle) * 10 * scale + Cos(BAngle) * 10 * scale),
                (int)(cy - yg + Cos(BAngle) * 10 * scale - Sin(BAngle) * 10 * scale));
            ps[1] = new Point((int)(cx + Sin(BAngle) * (rockerLenght - 20 * scale) + Cos(BAngle) * 11 * scale),
                (int)(cy - yg + Cos(BAngle) * (rockerLenght - 20 * scale) - Sin(BAngle) * 11 * scale));
            ps[3] = new Point((int)(cx + Sin(BAngle) * 10 * scale - Cos(BAngle) * 10 * scale),
                (int)(cy - yg + Cos(BAngle) * 10 * scale + Sin(BAngle) * 10 * scale));
            ps[2] = new Point((int)(cx + Sin(BAngle) * (rockerLenght - 20 * scale) - Cos(BAngle) * 11 * scale),
                (int)(cy - yg + Cos(BAngle) * (rockerLenght - 20 * scale) + Sin(BAngle) * 11 * scale));
            g.FillPolygon(brushD, ps);
            x1 = ps[3].X + 4;
            y1 = ps[3].Y;
            x2 = ps[2].X + 4;
            y2 = ps[2].Y;
            x3 = ps[0].X + 2;
            y3 = ps[0].Y;
            x4 = ps[1].X + 2;
            y4 = ps[1].Y;
            Rectangle rSh2 = new Rectangle(cx - ShShDiam / 2 + x, cy - ShShDiam / 2 - y, ShShDiam, ShShDiam);
            Rectangle rSh = new Rectangle(cx - ShShDiam / 2 + x - 3, cy - ShShDiam / 2 - y - 3, ShShDiam + 6, ShShDiam + 6);
            g.FillEllipse(brushG, rSh2);
            g.FillEllipse(brushG, cx - PalecDiam / 2, cy - PalecDiam / 2 - yg, PalecDiam, PalecDiam);
            if (prot)
            {
                if (checkBox_N.Checked)
                {
                    if (beyN)
                        g.DrawArc(PenRed, rSh, (int)(-90 - BAngle - ugl - fi), psi + fi);
                    else
                        g.DrawArc(PenPurple, rSh, (int)(-90 - BAngle - ugl - fi), psi + fi);
                }
                if (checkBox_NN.Checked)
                {
                    if (beyNN)
                        g.DrawArc(PenOrange, rSh2, (int)(-90 - BAngle + ugl2 - fi2), psi2 + fi2);
                    else
                        g.DrawArc(PenViolet, rSh2, (int)(-90 - BAngle + ugl2 - fi2), psi2 + fi2);
                }
            }
        }
        void piston(double Angle)
        {
            double BAngle = Math.Asin(Sin(Angle) * crankshaftLenght / rockerLenght) * 180 / Math.PI;
            int yg = (int)(Cos(Angle) * crankshaftLenght + Cos(BAngle) * rockerLenght);
            wall[0] = new Rectangle((int)(cx - PistonDiam / 2 - 1), cy - yg - 47 * scale, PistonDiam, 22 * scale);
            g.FillRectangle(brushL, wall[0]);
            wall[1] = new Rectangle((int)(cx - PistonDiam / 2 - 1), cy - yg - 47 * scale, 5, 84 * scale);
            g.FillRectangle(brushL, wall[1]);
            wall[2] = new Rectangle((int)(cx + PistonDiam / 2 - 4), cy - yg - 47 * scale, 5, 84 * scale);
            g.FillRectangle(brushL, wall[2]);
            //airBlock= new Rectangle((int)(cx - PistonDiam / 2 - 1), cy - yg - 25 * scale, PistonDiam, 80 * scale);
            //g.DrawRectangle(pen,airBlock);
        }
        void gilza()
        {
            g.DrawString("Н", new Font("Arial", 20), brushG,
               (int)(cx - (PistonDiam) / 2 - 50 * scale - 1 + 34 * scale), cy - 240 * scale);
            wall[3] = new Rectangle((int)(cx - (PistonDiam) / 2 - 50 * scale - 1), cy - 240 * scale, 49 * scale, 158 * scale);
            g.DrawRectangle(penB, wall[3]);
            g.DrawString("НН", new Font("Arial", 20), brushG,
               (int)(cx + (PistonDiam) / 2 + 2), cy - 240 * scale);
            wall[4] = new Rectangle((int)(cx + (PistonDiam) / 2 + 2), cy - 240 * scale, 49 * scale, 158 * scale);
            g.DrawRectangle(penB, wall[4]);
        }
        void oil(double Angle)
        {
            //oil
            double BAngle = Math.Asin(Sin(Angle) * crankshaftLenght / rockerLenght) * 180 / Math.PI;
            int xA = (int)(Sin(Angle) * crankshaftLenght);
            int yA = (int)(Cos(Angle) * crankshaftLenght);
            Point p1, p2;
            int shift = (int)(scale * Cos(BAngle));
            if (N)
            {
                int xB, yB, xBB, yBB;
                xB = (int)(xA - ShShDiam / 2 * Cos(90 - BAngle - ugl));
                yB = (int)(yA + ShShDiam / 2 * Sin(90 - BAngle - ugl));
                xBB = (int)(xB - 130 * scale * Cos(90 - BAngle - gamma));
                yBB = (int)(yB + 130 * scale * Sin(90 - BAngle - gamma));
                if (trackBar_Coor.Value == 22)
                {
                    label_lInfo.Visible = false;
                    label_l.Visible = false;
                    xB = (int)(xA - ShShDiam / 2 * Cos(90 - BAngle - ugl));
                    yB = (int)(yA + ShShDiam / 2 * Sin(90 - BAngle - ugl));
                    xBB = (int)(xB - 130 * scale * Cos(90 - BAngle - gamma));
                    yBB = (int)(yB + 130 * scale * Sin(90 - BAngle - gamma));
                    gammaMin = 5;
                    trackBar_gamma.Minimum = gammaMin;
                    gammaMax = 90;
                    trackBar_gamma2.Maximum = gammaMax;
                }
                else if (trackBar_Coor.Value > 22 && trackBar_Coor.Value <= 120)
                {
                    if (trackBar_Coor.Value > 112)
                        gammaMin = 50 + (trackBar_Coor.Value - 112) * 5;
                    else
                        gammaMin = 50;
                    gammaMax = 90;
                    trackBar_gamma.Maximum = gammaMax;
                    label_lInfo.Visible = true;
                    label_l.Visible = true;
                    label_l.BringToFront();
                    trackBar_gamma.Minimum = gammaMin;
                    if (gamma < gammaMin)
                    {
                        gamma = trackBar_gamma.Minimum;
                        label_gamma.Text = gamma.ToString();
                    }
                    if (gamma > gammaMax)
                    {
                        gamma = trackBar_gamma.Maximum;
                        label_gamma.Text = gamma.ToString();
                    }
                    xB = (int)(crankshaftLenght * Sin(Angle) - l * Sin(BAngle));
                    yB = (int)(crankshaftLenght * Cos(Angle) + l * Cos(BAngle));
                    int xB1 = (int)(crankshaftLenght * Sin(Angle) - ShShDiam / 2 * Sin(BAngle));
                    int yB1 = (int)(crankshaftLenght * Cos(Angle) + ShShDiam / 2 * Cos(BAngle));
                    xBB = xB - (int)(70 * scale * Sin(BAngle + gamma));
                    yBB = yB + (int)(70 * scale * Cos(BAngle + gamma));
                    int r_r = 16 + 240 / l;
                    int r_s = (int)(r_r / Cos(90 - gamma)) + 4;
                    int xC = xB - (int)(r_s * Sin(gamma + BAngle));
                    int yC = yB + (int)(r_s * Cos(gamma + BAngle));
                    if (beyN)
                    {
                        g.DrawLine(PenRed, new Point(cx + xB - shift, cy - yB + shift),
                            new Point(cx + xB1 - shift, cy - yB1 + shift));
                        g.DrawLine(PenRed, new Point(cx + xB - shift, cy - yB + shift),
                             new Point(cx + xC, cy - yC));
                    }
                    else
                    {
                        g.DrawLine(PenPurple, new Point(cx + xB - shift, cy - yB + shift),
                            new Point(cx + xB1 - shift, cy - yB1 + shift));
                        g.DrawLine(PenPurple, new Point(cx + xB - shift, cy - yB + shift),
                            new Point(cx + xC, cy - yC));
                    }
                }
                else
                {
                    gammaMin = 35;
                    trackBar_gamma.Minimum = gammaMin;
                    gammaMax = 140;
                    trackBar_gamma.Maximum = gammaMax;
                    xB = 0;
                    yB = (int)(crankshaftLenght * Cos(Angle) + rockerLenght * Cos(BAngle));
                    int xB1 = (int)((rockerLenght - ShShDiam / 2) * Sin(BAngle));
                    int yB1 = (int)((rockerLenght * Cos(BAngle) + crankshaftLenght * Cos(a)) - (rockerLenght - ShShDiam / 2) * Cos(BAngle));
                    xBB = xB - (int)(70 * scale * Sin(BAngle + gamma));
                    yBB = yB + (int)(70 * scale * Cos(BAngle + gamma));
                    int xC = (int)((PalecDiam + 14) / 2 * Sin(gamma + BAngle));
                    int yC = (int)(crankshaftLenght * Cos(Angle) + rockerLenght * Cos(BAngle) + (PalecDiam + 14) / 2 * Cos(gamma + BAngle));
                    if (beyN)
                    {
                        g.DrawLine(PenRedT, new Point(cx + xB + shift, cy - yB - shift),
                            new Point(cx + xB1 + shift, cy - yB1 - shift));
                        g.DrawLine(PenRedT, new Point(cx + xB + shift, cy - yB - shift),
                            new Point(cx - xC, cy - yC));
                        g.DrawEllipse(PenRedT, cx - (PalecDiam+4) / 2, palec_Y - (PalecDiam + 4) / 2, (PalecDiam + 4), (PalecDiam + 4));
                    }
                    else
                    {
                        g.DrawLine(PenPurpleT, new Point(cx + xB + shift, cy - yB - shift),
                            new Point(cx + xB1 + shift, cy - yB1 - shift));
                        g.DrawLine(PenPurpleT, new Point(cx + xB + shift, cy - yB - shift),
                             new Point(cx - xC, cy - yC));
                        g.DrawEllipse(PenPurpleT, cx - (PalecDiam + 4) / 2, palec_Y - (PalecDiam + 4) / 2, (PalecDiam + 4), (PalecDiam + 4));
                    }
                    g.FillEllipse(brushG, cx - PalecDiam / 2, palec_Y - PalecDiam / 2, PalecDiam, PalecDiam);
                }
                p1 = new Point(cx + xB, cy - yB);
                p2 = new Point(cx + xBB, cy - yBB);

                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                g.DrawLine(pen, p1, p2);
                //угол γ
                Rectangle duga_gamma = new Rectangle(cx + xB - ShShDiam, cy - yB - ShShDiam, ShShDiam * 2, ShShDiam * 2);
                g.DrawString("γ", new Font("Arial", 10), brushPen,
                    cx + (int)(xB - 60 * scale * Cos(90 - BAngle - gamma / 2)),
                    cy - (int)(yB + 60 * scale * Sin(90 - BAngle - gamma / 2)));
                g.DrawArc(pen, duga_gamma, (int)(-BAngle - 90), -gamma);
                if (!prot)
                {
                    double alfa = Angle;
                    if (alfa >= 360)
                        alfa -= 360;

                    double b1 = -ugl - BAngle;
                    if (trackBar_Coor.Value != 22)
                        b1 = -BAngle;
                    double
                        a1 = alfa + drill + faza / 2 - b1,
                        a2 = alfa + drill - faza / 2 - b1;
                    if (a1 >= 360)
                    {
                        a1 -= 360;
                        a2 -= 360;
                    }
                    if ((a1 >= 0 && a2 <= 0) || (a1 >= 360 && a2 <= 360))
                        beyN = true;
                    else
                        beyN = false;
                }
            }
            else
            {
                label_lInfo.Visible = false;
                label_l.Visible = false;
            }
            if (NN)
            {
                int xB, yB, xBB, yBB;
                if (trackBar_Coor2.Value == 22)
                {
                    label_l2Info.Visible = false;
                    label_l2.Visible = false;
                    xB = (int)(xA - ShShDiam / 2 * Cos(90 - BAngle + ugl2));
                    yB = (int)(yA + ShShDiam / 2 * Sin(90 - BAngle + ugl2));
                    xBB = (int)(xB - 130 * scale * Cos(90 - BAngle + gamma2));
                    yBB = (int)(yB + 130 * scale * Sin(90 - BAngle + gamma2));
                    gamma2Min = 5;
                    trackBar_gamma2.Minimum = gamma2Min;
                    gamma2Max = 90;
                    trackBar_gamma2.Maximum = gamma2Max;
                }
                else if (trackBar_Coor2.Value > 22 && trackBar_Coor2.Value <=120)
                {
                    if (trackBar_Coor2.Value > 112)
                        gamma2Min = 50 + (trackBar_Coor2.Value - 112) * 5;
                    else
                        gamma2Min = 50;
                    gamma2Max = 90;
                    trackBar_gamma2.Maximum=gamma2Max;
                    label_l2Info.Visible = true;
                    label_l2.Visible = true;
                    label_l2.BringToFront();
                    trackBar_gamma2.Minimum = gamma2Min;
                    if (gamma2 < gamma2Min)
                    {
                        gamma2 = trackBar_gamma2.Minimum;
                        label_gamma2.Text = gamma2.ToString();
                    }
                    if (gamma2 > gamma2Max)
                    {
                        gamma2 = trackBar_gamma2.Maximum;
                        label_gamma2.Text = gamma2.ToString();
                    }
                    xB = (int)((rockerLenght - l2) * Sin(BAngle));
                    yB = (int)(crankshaftLenght * Cos(Angle) + l2 * Cos(BAngle));
                    int xB1 = (int)((rockerLenght - ShShDiam / 2) * Sin(BAngle));
                    int yB1 = (int)((rockerLenght * Cos(BAngle) + crankshaftLenght * Cos(a)) - (rockerLenght - ShShDiam / 2) * Cos(BAngle));
                    xBB = xB + (int)(70 * scale * Sin(-BAngle + gamma2));
                    yBB = yB + (int)(70 * scale * Cos(-BAngle + gamma2));
                    int r_r = 16 + 240 / l2;
                    int r_s = (int)(r_r / Cos(90 - gamma2)) + 4;
                    int xC = (int)((rockerLenght - l2) * Sin(BAngle) + r_s * Sin(gamma2 - BAngle));
                    int yC = (int)(crankshaftLenght * Cos(Angle) + l2 * Cos(BAngle) + r_s * Cos(gamma2 - BAngle));
                    if (beyNN)
                    {
                        g.DrawLine(PenOrange, new Point(cx + xB + shift, cy - yB - shift),
                            new Point(cx + xB1 + shift, cy - yB1 - shift));
                        g.DrawLine(PenOrange, new Point(cx + xB + shift, cy - yB - shift),
                            new Point(cx + xC, cy - yC));
                    }
                    else
                    {
                        g.DrawLine(PenViolet, new Point(cx + xB + shift, cy - yB - shift),
                            new Point(cx + xB1 + shift, cy - yB1 - shift));
                        g.DrawLine(PenViolet, new Point(cx + xB + shift, cy - yB - shift),
                             new Point(cx + xC, cy - yC));
                    }
                }
                else
                {
                    gamma2Min = 35;
                    trackBar_gamma2.Minimum = gamma2Min;
                    gamma2Max = 140;
                    trackBar_gamma2.Maximum = gamma2Max;
                    xB = 0;
                    yB = (int)(crankshaftLenght * Cos(Angle) + rockerLenght * Cos(BAngle));
                    int xB1 = (int)((rockerLenght - ShShDiam / 2) * Sin(BAngle));
                    int yB1 = (int)((rockerLenght * Cos(BAngle) + crankshaftLenght * Cos(a)) - (rockerLenght - ShShDiam / 2) * Cos(BAngle));
                    xBB = xB + (int)(70 * scale * Sin(-BAngle + gamma2));
                    yBB = yB + (int)(70 * scale * Cos(-BAngle + gamma2));
                    int xC = (int)((PalecDiam+14) / 2 * Sin(gamma2 - BAngle));
                    int yC = (int)(crankshaftLenght * Cos(Angle) + rockerLenght * Cos(BAngle) + (PalecDiam+14) / 2 * Cos(gamma2 - BAngle));
                    if (beyNN)
                    {
                        g.DrawLine(PenOrangeT, new Point(cx + xB + shift, cy - yB - shift),
                            new Point(cx + xB1 + shift, cy - yB1 - shift));
                        g.DrawLine(PenOrangeT, new Point(cx + xB + shift, cy - yB - shift),
                            new Point(cx + xC, cy - yC));
                        g.DrawEllipse(PenOrangeT, cx - PalecDiam / 2, palec_Y - PalecDiam / 2, PalecDiam, PalecDiam);
                    }
                    else
                    {
                        g.DrawLine(PenVioletT, new Point(cx + xB + shift, cy - yB - shift),
                            new Point(cx + xB1 + shift, cy - yB1 - shift));
                        g.DrawLine(PenVioletT, new Point(cx + xB + shift, cy - yB - shift),
                             new Point(cx + xC, cy - yC));
                        g.DrawEllipse(PenVioletT, cx - PalecDiam / 2, palec_Y - PalecDiam / 2, PalecDiam, PalecDiam);
                    }
                    g.FillEllipse(brushG, cx - PalecDiam / 2, palec_Y - PalecDiam / 2, PalecDiam, PalecDiam);
                }
                p1 = new Point(cx + xB, cy - yB);
                p2 = new Point(cx + xBB, cy - yBB);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                g.DrawLine(pen, p1, p2);
                //угол γ2
                Rectangle duga_gamma = new Rectangle(cx + xB - ShShDiam, cy - yB - ShShDiam, ShShDiam * 2, ShShDiam * 2);
                g.DrawString("γ2", new Font("Arial", 10), brushPen,
                    cx + (int)(xB - 60 * scale * Cos(90 - BAngle + gamma2 / 2)),
                    cy - (int)(yB + 60 * scale * Sin(90 - BAngle + gamma2 / 2)));
                g.DrawArc(pen, duga_gamma, (int)(-BAngle - 90), gamma2);
                if (!prot)
                {
                    double alfa = Angle;
                    if (alfa >= 360)
                        alfa -= 360;
                    double b1 = ugl2 - BAngle;
                    if (trackBar_Coor2.Value != 22)
                        b1 = -BAngle;
                    double
                        a1 = alfa + drill + faza / 2 - b1,
                        a2 = alfa + drill - faza / 2 - b1;
                    if (a1 >= 360)
                    {
                        a1 -= 360;
                        a2 -= 360;
                    }
                    if ((a1 >= 0 && a2 <= 0) || (a1 >= 360 && a2 <= 360))
                        beyNN = true;
                    else
                        beyNN = false;
                }
            }
            else
            {
                label_l2Info.Visible = false;
                label_l2.Visible = false;
            }
            if (Os)
            {
                int xB = (int)(xA - ShShDiam / 2 * Cos(90 - BAngle));
                int yB = (int)(yA + ShShDiam / 2 * Sin(90 - BAngle));
                int xBB = (int)(xB - 130 * scale * Cos(90 - BAngle));
                int yBB = (int)(yB + 130 * scale * Sin(90 - BAngle));
                p1 = new Point(cx + xB, cy - yB);
                p2 = new Point(cx + xBB, cy - yBB);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                g.DrawLine(pen, p1, p2);
                double alfa = Angle;
                if (alfa >= 360)
                {
                    alfa -= 360;
                }
                double b1 = -BAngle,
                    a1 = alfa + drill + faza / 2 - b1,
                    a2 = alfa + drill - faza / 2 - b1;
                if (a1 >= 360)
                {
                    a1 -= 360;
                    a2 -= 360;
                }
                if ((a1 >= 0 && a2 <= 0) || (a1 >= 360 && a2 <= 360))
                {
                    beyOs = true;
                }
                else
                    beyOs = false;
            }

            //drill
            int xO = cx - 8 * scale + (int)((crankshaftLenght + 6 * scale) * Sin(Angle));
            int yO = cy - 8 * scale - (int)((crankshaftLenght + 6 * scale) * Cos(Angle));
            int xD = cx - ShShDiam / 2 + (int)((crankshaftLenght) * Sin(Angle));
            int yD = cy - ShShDiam / 2 - (int)((crankshaftLenght) * Cos(Angle));
            g.FillEllipse(BrushOrangeRed, xO, yO, 16 * scale, 16 * scale);
            g.FillPie(BrushOrangeRed, xD, yD, ShShDiam, ShShDiam, (int)(Angle - 90 + drill - faza / 2), faza);
            //угол θ
            g.DrawLine(pen2, new Point(cx + (int)(crankshaftLenght * Sin(Angle)), cy - (int)(crankshaftLenght * Cos(Angle))),
                new Point(cx + (int)((crankshaftLenght + ShShDiam / 2) * Sin(Angle)), cy - (int)((crankshaftLenght + ShShDiam / 2) * Cos(Angle))));
            g.DrawPie(pen2, xD + ShShDiam / 4, yD + ShShDiam / 4, ShShDiam / 2, ShShDiam / 2, (int)(Angle - 90), drill);
            g.DrawString("θ", new Font("Arial", 10), brushPen2, cx + (int)(crankshaftLenght * Sin(Angle)), cy - (int)(crankshaftLenght * Cos(Angle)));
        }
        double Sin(double Angle)
        {
            return Math.Sin(Angle * Math.PI / 180);
        }
        double Cos(double Angle)
        {
            return Math.Cos(Angle * Math.PI / 180);
        }
        void process(double Angle)
        {
            string s_pr;
            if (Angle >= 1 && Angle < 180)
            {
                brushProc = Brushes.LightBlue;
                s_pr = "ВПУСК";
            }
            else if (Angle >= 180 && Angle < 360)
            {
                brushProc = Brushes.Wheat;
                s_pr = "СЖАТИЕ";
            }
            else if (Angle >= 360 && Angle < 540)
            {
                brushProc = Brushes.LightPink;
                s_pr = "РАБОЧИЙ ХОД";
            }
            else
            {
                brushProc = Brushes.BurlyWood;
                s_pr = "ВЫПУСК";
            }
            double BAngle = Math.Asin(Sin(Angle) * crankshaftLenght / rockerLenght) * 180 / Math.PI;
            int yg = (int)(Cos(Angle) * crankshaftLenght + Cos(BAngle) * rockerLenght);
            Rectangle pr = new Rectangle((int)(cx - (PistonDiam) / 2 - 1), cy - 240 * scale, PistonDiam + 1, cy - yg - 47 * scale);
            g.FillRectangle(brushProc, pr);
            g.DrawString(s_pr, new Font("Arial", 15), new SolidBrush(Color.White),
               (int)(cx - (PistonDiam) / 2 - 1), cy - 240 * scale);
        }
        void CoorOfDrills(string s)
        {
            g.DrawString(s, new Font("Arial", 10), brushPen, 50, 630);
        }
        void Draw(double a)
        {
            g.Clear(Color.White);
            crankshaft(a);
            rocker(a);
            process(a);
            piston(a);
            gilza();
            oil(a);
            Rects(a);
            if (coorChange && checkBox_NN.Checked)
            {
                CoorOfDrills("   Для изменения положения отверстий на\nшатуне перемещайте ползунок (вверх/вниз)");
                trackBar_Coor2.Visible = true;
            }
            else
                trackBar_Coor2.Visible = false;
            if (coorChange && checkBox_N.Checked)
            {
                CoorOfDrills("   Для изменения положения отверстий на\nшатуне перемещайте ползунок (вверх/вниз)");
                trackBar_Coor.Visible = true;
            }
            else
                trackBar_Coor.Visible = false;
            pictureBox1.Invalidate();
        }
        public void Rects(double a)
        {
            if (prot)
            {
                if (a > 359)
                    a = a - 360;
                double BAngle = Math.Asin(Sin(a) * crankshaftLenght / rockerLenght) * 180 / Math.PI;
                if ((int)(BAngle + a + drill + faza / 2) > 359)
                {
                    dopfaza_rect = new Rectangle(0, 0, faza / 2 - 359 + (int)(BAngle + a) + drill, 15);
                    if ((int)(BAngle + a + drill - faza / 2) > 359)
                        dopfaza_rect = new Rectangle((int)(BAngle + a + drill - faza / 2) - 359, 0, faza, 15);
                }
                else
                    dopfaza_rect = new Rectangle(370, 0, 0, 15);
                faza_rect = new Rectangle((int)(BAngle + a + drill - faza / 2), 0, faza, 15);
                if (dopfaza_rect.X + dopfaza_rect.Width > 359 && dopfaza_rect.Width > 1)
                    faza_rect = new Rectangle(dopfaza_rect.X - 359, 0, faza, 15);
                prot2_rect = new Rectangle(ugl2 - fi2, 0, psi2 + fi2, 5);
                prot_rect = new Rectangle(0, 7, -ugl + psi, 5);
                if (ugl2 - fi2 < 0)
                    prot2_rect2 = new Rectangle(360 + ugl2 - fi2, 0, -ugl2 + fi2, 5);
                else
                    prot2_rect2 = new Rectangle(1000, 0, 0, 5);
                if (psi <= ugl)
                    prot_rect2 = new Rectangle(360 - fi - ugl, 7, fi + psi, 5);
                else
                    prot_rect2 = new Rectangle(360 - fi - ugl, 7, ugl + fi, 5);
                //g.FillRectangle(new SolidBrush(Color.Gray), 0, 0, 359, 3);
                //g.FillRectangle(new SolidBrush(Color.BlueViolet), prot2_rect);
                //g.FillRectangle(new SolidBrush(Color.BlueViolet), prot2_rect2);
                //g.FillRectangle(new SolidBrush(Color.Purple), prot_rect);
                //g.FillRectangle(new SolidBrush(Color.Purple), prot_rect2);
                //g.FillRectangle(new SolidBrush(Color.OrangeRed), faza_rect);
                //g.FillRectangle(new SolidBrush(Color.OrangeRed), dopfaza_rect);
                //g.DrawString("dop", new Font("Arial", 10), new SolidBrush(Color.Black), dopfaza_rect.X, dopfaza_rect.Y);
                if ((faza_rect.IntersectsWith(prot2_rect) || dopfaza_rect.IntersectsWith(prot2_rect)
                    || faza_rect.IntersectsWith(prot2_rect2) || dopfaza_rect.IntersectsWith(prot2_rect2))
                    && checkBox_NN.Checked)
                    beyNN = true;
                else
                    beyNN = false;
                if ((faza_rect.IntersectsWith(prot_rect) || dopfaza_rect.IntersectsWith(prot_rect)
                    || faza_rect.IntersectsWith(prot_rect2) || dopfaza_rect.IntersectsWith(prot_rect2))
                    && checkBox_N.Checked)
                    beyN = true;
                else
                    beyN = false;
                //if (faza_rect.IntersectsWith(prot_rect) || faza_rect.IntersectsWith(prot2_rect)
                //    || dopfaza_rect.IntersectsWith(prot_rect) || dopfaza_rect.IntersectsWith(prot2_rect))
                //    bey = true;
                //else
                //    bey = false;
            }
        }
        public void GammaShow()
        {
            trackBar_gamma.Visible = true;
            label_gammaInfo.Visible = true;
            label_gamma.Visible = true;
            button_gammaMinus.Visible = true;
            button_gammaPlus.Visible = true;
        }
        public void Gamma2Show()
        {
            trackBar_gamma2.Visible = true;
            label_gamma2Info.Visible = true;
            label_gamma2.Visible = true;
            button_gamma2Minus.Visible = true;
            button_gamma2Plus.Visible = true;
        }
        public void FiPsiShow()
        {
            trackBar_fi.Visible = true;
            label_fiInfo.Visible = true;
            label_fi.Visible = true;
            button_fiMinus.Visible = true;
            button_fiPlus.Visible = true;
            trackBar_psi.Visible = true;
            label_psiInfo.Visible = true;
            label_psi.Visible = true;
            button_psiMinus.Visible = true;
            button_psiPlus.Visible = true;
        }
        public void Fi2Psi2Show()
        {
            trackBar_fi2.Visible = true;
            label_fi2Info.Visible = true;
            label_fi2.Visible = true;
            button_fi2Minus.Visible = true;
            button_fi2Plus.Visible = true;
            trackBar_psi2.Visible = true;
            label_psi2Info.Visible = true;
            label_psi2.Visible = true;
            button_psi2Minus.Visible = true;
            button_psi2Plus.Visible = true;
        }
        public void GammaHide()
        {
            trackBar_gamma.Visible = false;
            label_gammaInfo.Visible = false;
            label_gamma.Visible = false;
            button_gammaMinus.Visible = false;
            button_gammaPlus.Visible = false;
        }
        public void Gamma2Hide()
        {
            trackBar_gamma2.Visible = false;
            label_gamma2Info.Visible = false;
            label_gamma2.Visible = false;
            button_gamma2Minus.Visible = false;
            button_gamma2Plus.Visible = false;
        }
        public void FiPsiHide()
        {
            trackBar_fi.Visible = false;
            label_fiInfo.Visible = false;
            label_fi.Visible = false;
            button_fiMinus.Visible = false;
            button_fiPlus.Visible = false;
            trackBar_psi.Visible = false;
            label_psiInfo.Visible = false;
            label_psi.Visible = false;
            button_psiMinus.Visible = false;
            button_psiPlus.Visible = false;
        }
        public void Fi2Psi2Hide()
        {
            trackBar_fi2.Visible = false;
            label_fi2Info.Visible = false;
            label_fi2.Visible = false;
            button_fi2Minus.Visible = false;
            button_fi2Plus.Visible = false;
            trackBar_psi2.Visible = false;
            label_psi2Info.Visible = false;
            label_psi2.Visible = false;
            button_psi2Minus.Visible = false;
            button_psi2Plus.Visible = false;
        }

        //кнопки и корректоры текстов
        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                button8.Enabled = true;
                button7.Enabled = true;
                timer1.Enabled = false;
                button1.Text = "Пуск";
                //button1.Text = "Продолжить";
            }
            else
            {
                coorChange = false;
                button8.Enabled = false;
                button7.Enabled = false;
                timer1.Enabled = true;
                button1.Text = "Пауза";
                //button1.Text = "Пауза";
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            a--;
            if (a < 0)
                a = 720 - da;
            label1.Text = "Угол поворота КВ [град.]:\n\n               " + ((int)a).ToString();
            Draw(a);
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                drill = int.Parse(label_tetta.Text) - 360 * (int.Parse(label_tetta.Text) / 360);
                label_tetta.Text = drill.ToString();
            }
            catch
            {
                label_tetta.Text = "0";
                //MessageBox.Show("Число должно быть целым и попадать в диапазон от 0 до 359 град.","Ошибка ввода данных");
            }
            Draw(a);
        }
        private void button7_Click_1(object sender, EventArgs e)
        {
            a++;
            if (a > 720)
                a = da;
            label1.Text = "Угол поворота КВ [град.]:\n\n               " + ((int)a).ToString();
            Draw(a);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (!N)
            {
                N = true;
                label6.Text = "Направление струи:\nна Н стенку";
                //dif = -1;
            }
            else
            {
                N = false;
                label6.Text = "Направление струи:\nна НН стенку";
                //dif = 1;
            }
            oN.Clear();
            oNN.Clear();
            Draw(a);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (med > med_min)
            {
                med /= 2;
                domed = (int)med;
                label_med.Text = med.ToString();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (med < med_max)
            {
                med *= 2;
                domed = (int)med;
                label_med.Text = med.ToString();
            }
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(label_fi2.Text) > 0 && int.Parse(label_fi2.Text) <= gamma)
                {
                    fi2 = int.Parse(label_fi2.Text);
                }
                label_fi2.Text = fi2.ToString();
            }
            catch
            {
                fi2 = 10;
                label_fi2.Text = "10";
            }
            Draw(a);
        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                oN.Clear();
                oNN.Clear();
                if (int.Parse(textBox5.Text) >= 0 && int.Parse(textBox5.Text) < 3601)
                    w = int.Parse(textBox5.Text);
                textBox5.Text = w.ToString();
            }
            catch
            {
                textBox5.Text = "3000";
                //MessageBox.Show("Число должно быть целым и попадать в диапазон от 0 до 359 град.","Ошибка ввода данных");
            }
        }
        private void button18_Click(object sender, EventArgs e)
        {
            fix_o.Clear();
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                oN.Clear();
                oNN.Clear();
                if (double.Parse(textBox6.Text) >= 0 && double.Parse(textBox6.Text) <= 100)
                    p = double.Parse(textBox6.Text);
                textBox6.Text = p.ToString();
            }
            catch
            {
                textBox6.Text = "4";
            }
        }
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (double.Parse(textBox7.Text) >= 0 && double.Parse(textBox7.Text) <= 150)
                    temperature = double.Parse(textBox7.Text);
                textBox7.Text = temperature.ToString();
            }
            catch
            {
                textBox7.Text = "20";
            }
        }
        private void button19_Click(object sender, EventArgs e)
        {
            if (!avto)
            {
                label16.Text = "Автозамедление вкл.";
                avto = true;
                button2.Enabled = false;
                button3.Enabled = false;
            }
            else
            {
                label16.Text = "Автозамедление выкл.";
                avto = false;
                button2.Enabled = true;
                button3.Enabled = true;
            }
        }
        private void button20_Click(object sender, EventArgs e)
        {
            coorChange = true;
            seconds = 0;
            button1.Text = "Пуск";
            timer1.Enabled = false;
            a = 0;
            label1.Text = "Угол поворота КВ [град.]:\n\n               " + (int)a;
            Draw(a);
            fix_o.Clear();
            oN.Clear();
            oNN.Clear();
        }
        public void Save()
        {
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.Filter = "Формат RTF|*.bik";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(saveFileDialog1.FileName);
                writer.WriteLine(label_tetta.Text);
                writer.WriteLine(label_UslConus.Text);
                writer.WriteLine(textBox5.Text);
                writer.WriteLine(textBox6.Text);
                writer.WriteLine(textBox7.Text);
                writer.WriteLine(label_gamma2.Text);
                writer.WriteLine(label_gamma.Text);
                if (radioButton_onWall.Checked)
                    writer.WriteLine("1");
                else
                    writer.WriteLine("0");
                if (radioButton_poOsi.Checked)
                    writer.WriteLine("1");
                else
                    writer.WriteLine("0");
                if (checkBox_NN.Checked)
                    writer.WriteLine("1");
                else
                    writer.WriteLine("0");
                if (checkBox_N.Checked)
                    writer.WriteLine("1");
                else
                    writer.WriteLine("0");
                if (prot)
                {
                    writer.WriteLine("1");
                    writer.WriteLine("Убрать проточку");
                }
                else
                {
                    writer.WriteLine("0");
                    writer.WriteLine("Добавить проточку");
                }
                writer.WriteLine(trackBar_fi.Value);
                writer.WriteLine(trackBar_psi.Minimum);
                writer.WriteLine(trackBar_psi.Maximum);
                writer.WriteLine(trackBar_psi.Value);
                writer.WriteLine(trackBar_fi2.Value);
                writer.WriteLine(trackBar_psi2.Minimum);
                writer.WriteLine(trackBar_psi2.Maximum);
                writer.WriteLine(trackBar_psi2.Value);
                writer.WriteLine(ugl);
                writer.WriteLine(ugl2);
                writer.WriteLine(trackBar_Coor.Value);
                writer.WriteLine(trackBar_Coor2.Value);
                writer.Close();
            }
        }
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Текстовые файлы |*.bik";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = new StreamReader(openFileDialog1.FileName);
                label_tetta.Text = reader.ReadLine();
                drill = int.Parse(label_tetta.Text);
                trackBar_tetta.Value = drill;
                label_UslConus.Text = reader.ReadLine();
                faza = int.Parse(label_UslConus.Text);
                trackBar_UslConus.Value = faza;
                textBox5.Text = reader.ReadLine();
                w = double.Parse(textBox5.Text);
                textBox6.Text = reader.ReadLine();
                p = double.Parse(textBox6.Text);
                textBox7.Text = reader.ReadLine();
                temperature = double.Parse(textBox7.Text);
                label_gamma2.Text = reader.ReadLine();
                gamma2 = int.Parse(label_gamma2.Text);
                trackBar_gamma2.Value = gamma2;
                label_gamma.Text = reader.ReadLine();
                gamma = int.Parse(label_gamma.Text);
                trackBar_gamma.Value = gamma;
                string s = reader.ReadLine();
                if (s == "1")
                {
                    radioButton_onWall.Checked = true;
                }
                else
                {
                    radioButton_onWall.Checked = false;
                }
                s = reader.ReadLine();
                if (s == "1")
                {
                    radioButton_poOsi.Checked = true;
                }
                else
                {
                    radioButton_poOsi.Checked = false;
                }
                s = reader.ReadLine();
                if (s == "1")
                {
                    checkBox_NN.Checked = true;
                }
                else
                {
                    checkBox_NN.Checked = false;
                }
                s = reader.ReadLine();
                if (s == "1")
                {
                    checkBox_N.Checked = true;
                }
                else
                {
                    checkBox_N.Checked = false;
                }
                s = reader.ReadLine();
                if (s == "1")
                    prot = true;
                else
                    prot = false;
                pictureBox2.Visible = prot;
                panel1.Visible = !prot;
                button_Prot.Text = reader.ReadLine();
                label_fi.Text = reader.ReadLine();
                fi = int.Parse(label_fi.Text);
                trackBar_fi.Value = fi;
                trackBar_psi.Minimum = int.Parse(reader.ReadLine());
                psiMax = int.Parse(reader.ReadLine());
                trackBar_psi.Maximum = psiMax;
                label_psi.Text = reader.ReadLine();
                psi = int.Parse(label_psi.Text);
                trackBar_psi.Value = psi;
                label_fi2.Text = reader.ReadLine();
                fi2 = int.Parse(label_fi2.Text);
                trackBar_fi2.Value = fi2;
                trackBar_psi2.Minimum = int.Parse(reader.ReadLine());
                psi2Max = int.Parse(reader.ReadLine());
                trackBar_psi2.Maximum = psi2Max;
                label_psi2.Text = reader.ReadLine();
                psi2 = int.Parse(label_psi2.Text);
                trackBar_psi2.Value = psi2;
                ugl = int.Parse(reader.ReadLine());
                ugl2 = int.Parse(reader.ReadLine());
                trackBar_Coor.Value = int.Parse(reader.ReadLine());
                l = trackBar_Coor.Value * scale;
                label_l.Text = trackBar_Coor.Value.ToString();
                trackBar_Coor2.Value = int.Parse(reader.ReadLine());
                l2 = trackBar_Coor2.Value * scale;
                label_l2.Text = trackBar_Coor2.Value.ToString();
                reader.Close();
            }
            Picture();
            Draw(a);
        }
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }
        private void информацияToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            string s = "            Версия 2.0"
                + "\nДоработки в программе:"
                + "\n1.При возникновении ошибки программа не закрывается, а только выдает сообщение об ошибке."
                + "\n2.Упростилась регулировка основных параметров - теперь они изменяются при помощи ползунков (для точности регулировки используются кнопки)."
                + "\n3.Появилась возможность создавать проточку для каждого отверстия-форсунки в шатуне, а также для обеих отверстий одновременно."
                + "\n4.При добавлении проточки показываются уточняющие эскизы нижней головки шатуна с основными угловыми координатами."
                + "\nНедоработки в программе "
                + "\n1.Перестала работать кнопка автоматического замедления «Изменить»."
                + "\n2.Шатун и палец становятся проницаемыми для масла, если создавать сразу два отверстия-форсунки в нижней головке шатуна."
                + "\n"
                + "\n            Версия 2.1"
                 + "\nДоработки в программе:"
                 + "\n1.Исправлена недоработка п.1 версии 2.0."
                 + "\n2.Исправлена недоработка п.2 версии 2.0."
                 + "\n3.Скорректированы параметры сохранения и открытия внешних файлов."
                 + "\n4.Добавлена функция о предложении сохранить данные перед закрытием программы."
                 + "\n5.Добавленны пункты в меню Файл: Информация и Выход - с соответствующим функционалом."
                 + "\n"
                 + "\n            Версия 2.2"
                 + "\nДоработки в программе:"
                 + "\n1.Смоделировано изменение площади сечения масляной струи в зависимости от скорости порции с данным сечением."
                 + "\n2.Оптимизирован алгоритм программы - выявлены и устранены незначительные ошибки."
                 + "\n3.Скорректированы параметры сохранения и открытия внешних файлов."
                 + "\n"
                 + "\n            Версия 2.3"
                 + "\nДоработки в программе:"
                 + "\n1.Добавлена возможность изменения координат форсунок (масляных отверстий на шатуне) по стержню шатуна."
                 + "\n2.Изменилась реализация проточек - в данной версии для каждой форсунки предназначена собственная проточка (нет общей проточки)."
                 + "\n3.Разработаны новые, скорректировны старые и добавлены уточняющие эскизы для всех возможных комбинаций маслоподачи."
                 + "\n4.Скорректированы параметры сохранения и открытия внешних файлов."
                 + "\n"
                 + "\n            Версия 2.4"
                 + "\nДоработки в программе:"
                 + "\n1.Исправлена ошибка, возникающая при уменьшении углов сверления маслоподводящих отверстий на стержне шатуна."
                 + "\n2.Исправлена недоработка, связанная с маслоподающими сверлениями в стержне шатуна при 112 мм < l < 121 мм и 112 мм < l2 < 121 мм."
                 + "\n3.Добавлена возможность сверления маслоподводящего канала в шатуне до втулки верхней головки шатуна (l2 = 121 мм)."
                 + "\n4.Добавлены авторы программы."
                 + "\n"
                 + "\nАвторы программы:\n1. Магистр техники и технологии Бикташев Айдар Флёрович (sevenay@mail.ru);\n2. Доктор технических наук, "
                 + "профессор кафедры \"Поршневые двигатели\" МГТУ им Н.Э.Баумана Путинцев Сергей Викторович (putintsev50@yandex.ru)."
                 ;
           // Form infoMassage = new InfoMessageBox();
           // infoMassage.ShowDialog();
            MessageBox.Show(s, "Информация о программе");
        }
        private void выходToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void trackBar_fi2_Scroll(object sender, EventArgs e)
        {
            fi2 = trackBar_fi2.Value;
            label_fi2.Text = fi2.ToString();
            trackBar_psi2.Maximum = 360 - fi2;
            Draw(a);
        }
        private void button_fi2Minus_Click(object sender, EventArgs e)
        {
            if (fi2 > 0)
                fi2--;
            label_fi2.Text = fi2.ToString();
            trackBar_fi2.Value = fi2;
            trackBar_psi2.Maximum = 360 - fi2;
            Draw(a);
        }
        private void button_fi2Plus_Click(object sender, EventArgs e)
        {
            if (fi2 < trackBar_fi2.Maximum)
                fi2++;
            label_fi2.Text = fi2.ToString();
            trackBar_fi2.Value = fi2;
            trackBar_psi2.Maximum = 360 - fi2;
            Draw(a);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Сохранить текущие данные?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                Save();
            }
        }

        private void trackBar_Coor_Scroll(object sender, EventArgs e)
        {
            l = trackBar_Coor.Value * scale;
            label_l.Text = trackBar_Coor.Value.ToString();
            Picture();
            Draw(a);
        }

        private void trackBar_Coor2_Scroll(object sender, EventArgs e)
        {
            l2 = trackBar_Coor2.Value * scale;
            label_l2.Text = trackBar_Coor2.Value.ToString();
            Picture();
            Draw(a);
        }

        private void trackBar_psi2_Scroll(object sender, EventArgs e)
        {
            psi2 = trackBar_psi2.Value;
            label_psi2.Text = psi2.ToString();
            trackBar_fi2.Maximum = 360 - psi2;
            Draw(a);
        }
        private void button_psi2Minus_Click(object sender, EventArgs e)
        {
            if (psi2 > 0)
                psi2--;
            label_psi2.Text = psi2.ToString();
            trackBar_psi2.Value = psi2;
            trackBar_fi2.Maximum = 360 - psi2;
            Draw(a);
        }
        private void button_psi2Plus_Click(object sender, EventArgs e)
        {
            if (psi2 < trackBar_psi2.Maximum)
                psi2++;
            label_psi2.Text = psi2.ToString();
            trackBar_psi2.Value = psi2;
            trackBar_fi2.Maximum = 360 - psi2;
            Draw(a);
        }

        private void trackBar_fi_Scroll(object sender, EventArgs e)
        {
            fi = trackBar_fi.Value;
            label_fi.Text = fi.ToString();
            trackBar_psi.Maximum = 360 - fi;
            Draw(a);
        }
        private void button_fiMinus_Click(object sender, EventArgs e)
        {
            if (fi > 0)
                fi--;
            label_fi.Text = fi.ToString();
            trackBar_fi.Value = fi;
            trackBar_psi.Maximum = 360 - fi;
            Draw(a);
        }
        private void button_fiPlus_Click(object sender, EventArgs e)
        {
            if (fi < trackBar_fi.Maximum)
                fi++;
            label_fi.Text = fi.ToString();
            trackBar_fi.Value = fi;
            trackBar_psi.Maximum = 360 - fi;
            Draw(a);
        }

        private void trackBar_psi_Scroll(object sender, EventArgs e)
        {
            psi = trackBar_psi.Value;
            label_psi.Text = psi.ToString();
            trackBar_fi.Maximum = 360 - psi;
            Draw(a);
        }
        private void button_psiMinus_Click(object sender, EventArgs e)
        {
            if (psi > 0)
                psi--;
            label_psi.Text = psi.ToString();
            trackBar_psi.Value = psi;
            trackBar_fi.Maximum = 360 - psi;
            Draw(a);
        }
        private void button_psiPlus_Click(object sender, EventArgs e)
        {
            if (psi < trackBar_psi.Maximum)
                psi++;
            label_psi.Text = psi.ToString();
            trackBar_psi.Value = psi;
            trackBar_fi.Maximum = 360 - psi;
            Draw(a);
        }

        private void trackBar_gamma2_Scroll(object sender, EventArgs e)
        {
            gamma2 = trackBar_gamma2.Value;
            label_gamma2.Text = gamma2.ToString();
            Draw(a);
        }
        private void button_gamma2Minus_Click(object sender, EventArgs e)
        {
            if (gamma2 > gamma2Min)
            {
                gamma2--;
                label_gamma2.Text = gamma2.ToString();
                trackBar_gamma2.Value = gamma2;
            }
            Draw(a);
        }
        private void button_gamma2Plus_Click(object sender, EventArgs e)
        {
            if (gamma2 < gamma2Max)
            {
                gamma2++;
                label_gamma2.Text = gamma2.ToString();
                trackBar_gamma2.Value = gamma2;
            }
            Draw(a);
        }

        private void trackBar_gamma_Scroll(object sender, EventArgs e)
        {
            gamma = trackBar_gamma.Value;
            label_gamma.Text = gamma.ToString();
            Draw(a);
        }
        private void button_gammaMinus_Click(object sender, EventArgs e)
        {
            if (gamma > gammaMin)
            {
                gamma--;
                label_gamma.Text = gamma.ToString();
                trackBar_gamma.Value = gamma;
            }
            Draw(a);
        }
        private void button_gammaPlus_Click(object sender, EventArgs e)
        {
            if (gamma < gammaMax)
            {
                gamma++;
                label_gamma.Text = gamma.ToString();
                trackBar_gamma.Value = gamma;
            }
            Draw(a);
        }

        private void radioButton_poOsi_CheckedChanged(object sender, EventArgs e)
        {
            button_Prot.Visible = false;
            checkBox_NN.Visible = false;
            checkBox_N.Visible = false;
            checkBox_NN.Checked = false;
            checkBox_N.Checked = false;
            Os = true;
            N = false;
            NN = false;
            fix_o.Clear();
            oN.Clear();
            oNN.Clear();
            Draw(a);
            label_gamma2.Visible = false;
            label_gamma2Info.Visible = false;
            button_gamma2Plus.Visible = false;
            button_gamma2Minus.Visible = false;
            Picture();
        }
        private void radioButton_onWall_CheckedChanged(object sender, EventArgs e)
        {
            Os = false;
            beyOs = false;
            checkBox_NN.Visible = true;
            checkBox_NN.Checked = true;
            checkBox_N.Visible = true;
            checkBox_N.Checked = false;
            button_Prot.Visible = true;
            Picture();
        }

        private void checkBox_NN_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_NN.Checked)
            {
                button_Prot.Visible = true;
                NN = true;
                Os = false;
                //if (checkBox_N.Checked)
                //    pictureBox2.BackgroundImage = Properties.Resources.прот21;
                //else
                //    pictureBox2.BackgroundImage = Properties.Resources.прот2;
                Gamma2Show();
                Fi2Psi2Show();
            }
            else
            {
                NN = false;
                if (!checkBox_N.Checked)
                {
                    bey = false;
                    button_Prot.Visible = false;
                    panel1.Visible = true;
                    faz = 90;
                    prot = false;
                    GammaHide();
                    FiPsiHide();
                    pictureBox2.Visible = false;
                    button_Prot.Text = "Добавить проточку";
                }
                //else
                //    pictureBox2.BackgroundImage = Properties.Resources.прот1;
                Gamma2Hide();
                Fi2Psi2Hide();
            }
            fix_o.Clear();
            oN.Clear();
            oNN.Clear();
            Picture();
            Draw(a);
        }
        private void checkBox_N_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_N.Checked)
            {
                button_Prot.Visible = true;
                N = true;
                Os = false;
                //if (checkBox_NN.Checked)
                //    pictureBox2.BackgroundImage = Properties.Resources.прот21;
                //else
                //    pictureBox2.BackgroundImage = Properties.Resources.прот1;
                GammaShow();
                FiPsiShow();
            }
            else
            {
                N = false;
                if (!checkBox_NN.Checked)
                {
                    bey = false;
                    button_Prot.Visible = false;
                    panel1.Visible = true;
                    faz = 90;
                    prot = false;
                    Gamma2Hide();
                    Fi2Psi2Hide();
                    pictureBox2.Visible = false;
                    button_Prot.Text = "Добавить проточку";
                }
                //else
                //    pictureBox2.BackgroundImage = Properties.Resources.прот2;
                GammaHide();
                FiPsiHide();
            }
            fix_o.Clear();
            oN.Clear();
            oNN.Clear();
            Picture();
            Draw(a);
        }

        private void button_Prot_Click(object sender, EventArgs e)
        {
            if (!prot)
            {
                prot = true;
                button_Prot.Text = "Убрать проточку";
            }
            else
            {
                prot = false;
                bey = false;
                button_Prot.Text = "Добавить проточку";
            }
            pictureBox2.Visible = prot;
            panel1.Visible = !prot;
            Picture();
            Draw(a);
        }

        private void button_UslConusMinus_Click(object sender, EventArgs e)
        {
            faza--;
            if (faza < 0)
                faza = 90;
            label_UslConus.Text = faza.ToString();
            trackBar_UslConus.Value = faza;
            Draw(a);
        }

        private void button_UslConusPlus_Click(object sender, EventArgs e)
        {
            faza++;
            if (faza > 90)
                faza = 1;
            label_UslConus.Text = faza.ToString();
            trackBar_UslConus.Value = faza;
            Draw(a);
        }

        private void trackBar_UslConus_Scroll(object sender, EventArgs e)
        {
            faza = trackBar_UslConus.Value;
            label_UslConus.Text = faza.ToString();
            Draw(a);
        }

        private void button_tettaMinus_Click(object sender, EventArgs e)
        {
            drill--;
            if (drill < 0)
                drill = 359;
            label_tetta.Text = drill.ToString();
            trackBar_tetta.Value = drill;
            Draw(a);
        }

        private void button_tettaPlus_Click(object sender, EventArgs e)
        {
            drill++;
            if (drill > 359)
                drill = 0;
            label_tetta.Text = drill.ToString();
            trackBar_tetta.Value = drill;
            Draw(a);
        }

        private void trackBar_tetta_Scroll(object sender, EventArgs e)
        {
            drill = trackBar_tetta.Value;
            label_tetta.Text = drill.ToString();
            Draw(a);
        }
    }
}