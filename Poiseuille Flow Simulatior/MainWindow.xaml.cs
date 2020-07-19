using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Text.RegularExpressions;

namespace Poiseuille_Flow_Simulator_Final
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //initialise and draw axis
        public MainWindow()
        {
            InitializeComponent();
            DrawAxis();
        }

        //declare permanent canvas coordinates (all valued will be scaled down/up to these)
        private double zmin = -5;
        private double zmax = 20;
        private double rmin = -10;
        private double rmax = 10;

        //booleans for checking if warning messagebox have already being displayed - to prevent constant 
        private bool displayed1 = false; //transition flow
        private bool displayed2 = false; //turbulent flow
        private bool displayed3 = false; //when R >> L (invalid circumstance)

        //object to store graphs
        Polyline pl;

        //main independant variables
        float R = 0;
        float L = 0;
        float Δp = 0;
        float μ = 0;
        float ρ = 0;

        //calculated/depedant variables
        float Re = 0;
        float V = 0;
        float τ = 0;
        float Q = 0;

        //scale factors
        double VSF = 0;
        double TSF = 0;
        double RaSF = 0;
        double LSF = 0;
        double ΔpSF = 0;
        double μSF = 0;

        //buttons to switch no slip conditon on and off
        public double noslip = 1;
        private void Button_Click_On(object sender, RoutedEventArgs e)
        {
            noslip = 1;
            //reset graph canvas
            chartCanvas.Children.Clear();
            //draw axis and graphs for Velocity and Shear
            DrawAxis();
            DrawGraphs(R, Δp, L, μ);
            if (Re <= 2900)
            {
                shear_stress.Background = Brushes.LightGray;
            }
        }
        private void Button_Click_Off(object sender, RoutedEventArgs e)
        {
            noslip = 0;
            shear_stress.Background = Brushes.Black;
            //reset graph canvas
            chartCanvas.Children.Clear();
            //draw axis and graphs for Velocity and Shear
            DrawAxis();
            DrawGraphs(R, Δp, L, μ);
        }

        //Calculating variables
        private void Calulate_Values(object sender, RoutedEventArgs e)
        {
            string transferR = r_value.Text;
            string transferL = l_value.Text;
            string transferΔp = Δp_value.Text;
            string transferμ = μ_value.Text;
            string transferρ = ρ_value.Text;

            if (String.IsNullOrEmpty(transferR) == false && String.IsNullOrEmpty(transferL) == false
                && String.IsNullOrEmpty(transferΔp) == false && String.IsNullOrEmpty(transferμ) == false
                && String.IsNullOrEmpty(transferρ) == false)
            {
                if (Check_Decimal_Points(transferR) == true && Check_Decimal_Points(transferL) == true &&
                Check_Decimal_Points(transferΔp) == true && Check_Decimal_Points(transferμ) == true &&
                Check_Decimal_Points(transferρ) == true)
                {
                    R = float.Parse(transferR);
                    L = float.Parse(transferL);
                    Δp = float.Parse(transferΔp);
                    μ = float.Parse(transferμ);
                    ρ = float.Parse(transferρ);

                    //unit
                    V = (1 / (4 * μ)) * (Δp / L) * (R * R);
                    Re = 2 * ρ * V * R / μ;
                    Q = (float)(3.1415926535 * Math.Pow(R, 4) * Δp / (8 * μ * L));
                    τ = (Δp / L) * (R / 2);

                    //display dependant variables
                    max_velocity.Text = V.ToString();
                    reynolds_number.Text = Re.ToString();
                    volumetric_flux.Text = Q.ToString();
                    shear_stress.Text = τ.ToString();

                    //Velocity and Shear Scale Factor corrections for logarithms
                    if (τ > 1)
                    {
                        τ *= 10;
                    }
                    if (V > 1)
                    {
                        V *= 10;
                    }

                    //determine scale factors (powers of ten)
                    RaSF = Math.Log10(R);
                    LSF = Math.Log10(L);
                    μSF = Math.Log10(μ);
                    ΔpSF = Math.Log10(Δp);

                    TSF = Math.Log10(τ);
                    VSF = Math.Log10(V);

                    //log function inaccuracy correction
                    if (RaSF > 0) { RaSF += 0.0001; }
                    else { RaSF -= 0.0001; }
                    if (LSF > 0) { LSF += 0.0001; }
                    else { LSF -= 0.0001; }
                    if (μSF > 0) { μSF += 0.0001; }
                    else { μSF -= 0.0001; }
                    if (ΔpSF > 0) { ΔpSF += 0.0001; }
                    else { ΔpSF -= 0.0001; }

                    if (VSF > 0) { VSF += 0.0001; }
                    else { VSF -= 0.0001; }
                    if (TSF > 0) { TSF += 0.0001; }
                    else { TSF -= 0.0001; }

                    //truncate to int form                    
                    RaSF = Math.Truncate(RaSF);
                    LSF = Math.Truncate(LSF);
                    μSF = Math.Truncate(μSF);
                    ΔpSF = Math.Truncate(ΔpSF);

                    TSF = Math.Truncate(TSF);
                    VSF = Math.Truncate(VSF);

                    //correction
                    VSF -= 1;
                    TSF -= 1;

                    //display scale factors                        
                    VTSF.Text = VSF.ToString() + "  " + TSF.ToString();

                    //check if laminar or turbulent and display warnings
                    Laminar_Turbulent(Re);

                    //check if L >> R (L 2.5 times greater)
                    Check_RL(R, L);

                    //reset graph canvas
                    chartCanvas.Children.Clear();

                    //draw axis and graphs for Velocity and Shear
                    DrawAxis();
                    DrawGraphs(R, Δp, L, μ);
                }
            }
            else
            {
                R = 0;
                L = 0;
                Δp = 0;
                μ = 0;
                ρ = 0;
                displayed1 = false;
                displayed2 = false;
                max_velocity.Text = "0";
                reynolds_number.Text = "0";
                volumetric_flux.Text = "0";
                shear_stress.Text = "0";
            }
        }

        //draw axis and origin on graph
        private void DrawAxis()
        {
            //add origin
            double sf = (chartCanvas.Width / chartCanvas.Height);
            pl = new Polyline();
            pl.Stroke = Brushes.Black;
            pl.Points.Add(CurvePoint(
            new Point(-0.25, 0.25 * sf)));
            pl.Points.Add(CurvePoint(
            new Point(0.25, -0.25 * sf)));
            chartCanvas.Children.Add(pl);
            //add origin
            pl = new Polyline();
            pl.Stroke = Brushes.Black;
            pl.Points.Add(CurvePoint(
            new Point(0.25, 0.25 * sf)));
            pl.Points.Add(CurvePoint(
            new Point(-0.25, -0.25 * sf)));
            chartCanvas.Children.Add(pl);


            // z axis:
            pl = new Polyline();
            pl.Stroke = Brushes.Gray;
            pl.StrokeDashArray = new DoubleCollection(new double[] { 4, 3 });
            pl.Points.Add(CurvePoint(new Point(0, rmax)));
            pl.Points.Add(CurvePoint(new Point(0, rmin)));
            chartCanvas.Children.Add(pl);

            // r axis:
            pl = new Polyline();
            pl.Stroke = Brushes.Gray;
            pl.StrokeDashArray = new DoubleCollection(new double[] { 4, 3 });
            pl.Points.Add(CurvePoint(new Point(zmax, 0)));
            pl.Points.Add(CurvePoint(new Point(zmin, 0)));
            chartCanvas.Children.Add(pl);
        }

        //draw graphs of velocity and shear profiles
        private void DrawGraphs(float R, float Δp, float L, float μ)
        {
            double RaSF_10 = Math.Pow(10, RaSF);
            double LSF_10 = Math.Pow(10, LSF);
            double μSF_10 = Math.Pow(10, μSF);
            double ΔpSF_10 = Math.Pow(10, ΔpSF);

            //Radius Scale factor corrections
            if (RaSF_10 <= 1) { RaSF_10 /= 10; }
            //adjustment for 1<R<10
            if (R > 1.0 && R < 10.0)
            {
                RaSF_10 *= 10;
            }
            //special cases: for R = negative powers of 10
            if (R <= 1)
            {
                if ((float)((-1) * Math.Log10(R)) % 1 == 0)
                {
                    RaSF_10 *= 10;
                }
            }

            LSF_10 = SF_corrections(L, LSF_10);
            μSF_10 = SF_corrections(μ, μSF_10);
            ΔpSF_10 = SF_corrections(Δp, ΔpSF_10);

            //adjust each factor accordingly
            R /= (float)RaSF_10;
            L /= (float)LSF_10;
            μ /= (float)μSF_10;
            Δp /= (float)ΔpSF_10;

            //scaling velocity for z direction
            double U_max_SF = (1 / (4 * μ)) * (Δp / L) * (R * R);
            if (U_max_SF > 100)
            {
                U_max_SF /= 100;
            }
            else if (U_max_SF >= 10 && U_max_SF < 100)
            {
                U_max_SF /= 10;
            }
            else if (U_max_SF >= 0.1 && U_max_SF < 1)
            {
                U_max_SF *= 10;
            }
            else if (U_max_SF >= 0.01 && U_max_SF < 0.1)
            {
                U_max_SF *= 10;
            }

            //scaling shear for z direction
            double τ_max_SF = (Δp / L) * (R / 2);
            if (τ_max_SF > 100)
            {
                τ_max_SF /= 100;
            }
            else if (τ_max_SF >= 10 && τ_max_SF < 100)
            {
                τ_max_SF /= 10;
            }
            else if (τ_max_SF >= 0.1 && τ_max_SF < 1)
            {
                τ_max_SF *= 10;
            }
            else if (τ_max_SF >= 0.01 && τ_max_SF < 0.1)
            {
                τ_max_SF *= 10;
            }

            // Draw poiseuille profile curve:
            pl = new Polyline();
            pl.Stroke = Brushes.Magenta;
            for (int i = -1000; i <= 1000; i++)
            {
                double r = i / 50.0;
                double z = (1 / (4 * μ)) * (Δp / L) * (R * R - noslip * (r * r));

                z /= U_max_SF;

                //check if points lie within graph canvas limits and do not exceed pipe diameter
                if (z >= 0 && z <= zmax && R <= rmax && R >= rmin && Math.Abs(r) <= R)
                {
                    pl.Points.Add(CurvePoint(new Point(z, r)));
                }
            }
            chartCanvas.Children.Add(pl);

            // Draw shear curve:
            pl = new Polyline();
            pl.Stroke = Brushes.Green;
            for (int i = -1000; i <= 1000; i++)
            {
                double r = i / 50.0;
                double z = (noslip) * Math.Abs((Δp / L) * (r / 2));

                //check if points lie within graph canvas limits and do not exceed pipe diameter
                if (z >= 0 && z <= zmax && R <= rmax && R >= rmin && Math.Abs(r) <= R)
                {
                    pl.Points.Add(CurvePoint(new Point(z, r)));
                }
            }
            chartCanvas.Children.Add(pl);

            //adjust back for drawing the pipe
            R *= (float)RaSF_10;

            //draw pipe limits
            pl = new Polyline();
            pl.Stroke = Brushes.Black;
            pl.Points.Add(CurvePoint(new Point(zmin, R / RaSF_10)));
            pl.Points.Add(CurvePoint(new Point(zmax, R / RaSF_10)));
            pl.Points.Add(CurvePoint(new Point(zmax, (-1) * R / RaSF_10)));
            pl.Points.Add(CurvePoint(new Point(zmin, (-1) * R / RaSF_10)));
            pl.Points.Add(CurvePoint(new Point(zmin, R / RaSF_10)));
            chartCanvas.Children.Add(pl);
        }

        private double SF_corrections(double x, double SF_10)
        {
            //Length Scale Factor corrections
            if (SF_10 > 1)
            {
                SF_10 *= 10;
            }
            if (x > 1.0 && x < 10.0)
            {
                SF_10 *= 10;
            }
            //special cases: for R = negative powers of 10
            if (x <= 1)
            {
                if ((float)((-1) * Math.Log10(x)) % 1 == 0)
                {
                    SF_10 *= 10;
                }
            }
            if ((float)((-1) * Math.Log10(x)) % 1 == 0)
            {
                SF_10 /= 10;
            }
            return SF_10;
        }

        //check conditions for poiseulle flow
        //check if fluid is laminar/turbulent
        private void Laminar_Turbulent(float Re)
        {
            //check to see if fluid is laminar, if not display warnings
            if (Re <= 2300)
            {
                Laminar.Background = Brushes.Green;
                Transition.Background = Brushes.White;
                Turbulent.Background = Brushes.White;

                //allow values to be displayed for laminar flow
                volumetric_flux.Background = Brushes.LightGray;
                max_velocity.Background = Brushes.LightGray;
                shear_stress.Background = Brushes.LightGray;
                chartCanvas.Background = Brushes.LightGray;

                displayed1 = false;
                displayed2 = false;
            }
            else if (2300 < Re && Re <= 2900)
            {
                Laminar.Background = Brushes.White;
                Transition.Background = Brushes.Orange;
                Turbulent.Background = Brushes.White;

                //allow values to be displayed for transition flow
                volumetric_flux.Background = Brushes.LightGray;
                max_velocity.Background = Brushes.LightGray;
                shear_stress.Background = Brushes.LightGray;
                chartCanvas.Background = Brushes.LightGray;

                if (displayed1 == false)
                {
                    if (R != 0 && L != 0 && Δp != 0 && μ != 0 && ρ != 0)
                    {
                        MessageBox.Show("Fluid is becoming turbulent - Model may lack accuracy", "WARNING");
                        displayed1 = true;
                    }
                }
                displayed2 = false;
            }
            else if (Re > 2900)
            {
                Laminar.Background = Brushes.White;
                Transition.Background = Brushes.White;
                Turbulent.Background = Brushes.Red;

                //blackout values when fluid is turbulent - Poiseulle flow no long holds true
                volumetric_flux.Background = Brushes.Black;
                max_velocity.Background = Brushes.Black;
                shear_stress.Background = Brushes.Black;
                chartCanvas.Background = Brushes.Black;

                if (displayed2 == false)
                {
                    if (R != 0 && L != 0 && Δp != 0 && μ != 0 && ρ != 0)
                    {
                        MessageBox.Show("Fluid has become turbulent - Fluid no longer follows a Poiseuille flow profile", "WARNING");
                        displayed2 = true;
                    }
                }
                displayed1 = false;
            }

        }
        //check if L >> R (2.5 times)
        private void Check_RL(float R, float L)
        {
            if (L <= 2.5 * R)
            {
                RL.Background = Brushes.Red;
                if (displayed3 == false)
                {
                    if (R != 0 && L != 0 && Δp != 0 && μ != 0 && ρ != 0)
                    {
                        displayed3 = true;
                        MessageBox.Show("L should be at least 2.5 times greater that R", "WARNING");
                    }

                }
            }
            else
            {
                displayed3 = false;
                RL.Background = Brushes.Green;
            }
        }

        private bool Check_Decimal_Points(string num)
        {
            if (num[0] == '.')
            {
                return false;
            }
            else if (num[num.Length - 1] == '.')
            {
                return false;
            }
            else
            {
                //count number of decimal points
                int count = 0;
                for (int n = 0; n < num.Length; n++)
                {
                    if (num[n] == '.')
                    {
                        count++;
                    }

                }

                //if more than one decimal point, number is invalid -> return false
                if (count > 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }

        //function to convert points to coordinates on canvas
        private Point CurvePoint(Point pt)
        {
            Point result = new Point();
            result.X = (pt.X - zmin) * chartCanvas.Width / (zmax - zmin);
            result.Y = chartCanvas.Height - (pt.Y - rmin) * chartCanvas.Height / (rmax - rmin);
            return result;
        }

        //allow only numbers and decimal point to be entered as value
        private void NumbersOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^.0-9]+").IsMatch(e.Text);
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Poiseuille Flow Simulation\n\n" +
            "1 - To use the simulation, please input a number for the 5 variables (R, L, Δp ,μ ,ρ) on the left.\n\n" +
            "2 - The app will then calculate and display 4 variables on the right and display two graphs traces to show the shear and velocity flow profile." +
            "Please note that the graph scaling may vary for the shear and velocity profiles and the graph is not intended for reading off accurate values " +
            "(these are given by the 4 variables on the right)\n\n" +
            "3 - A box on the bottom right will display the powers of 10 of the shear and velocity profiles as a reference for the graph.\n\n" +
            "4 - The application also determines whether the conditions for a poiseulle (laminar flow, L >> R) flow are met. The app will display warnings and/or black out calculated variables if these conditions are not met.\n\n" +
            "5 - The non slip condition can be toggled on and off using 2 buttons found below the graph. Turing the no-slip conditon off will blackout the shear display box and adjust the graph accordingly.\n\n" +
            "Extras:\n\n" +
            "Equations used to calculate variables:\n" +
            "Re = 2 * ρ * V * R / μ\n" +
            "V = (1 / (4 * μ)) * (Δp / L) * (R * R)\n" +
            "τ = (Δp / L) * (R / 2)\n" +
            "Q = (Pi * R^4 * Δp / (8 * μ * L))\n\n" +
            "About the app: 水 is a Chinese pictogram meaning water/liquid", "HELP");

        }
    }
}
