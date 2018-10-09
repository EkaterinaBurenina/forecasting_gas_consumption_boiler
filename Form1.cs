using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
//using System.Reflection;
using System.Web.UI.DataVisualization.Charting;
using Microsoft.VisualBasic;

namespace ВКР_1
{
    public partial class Form1 : Form
    {
        const int Q_Col_Orig = 6; //изначальное кол-во столбцов
        int Q_Col = 6;  //кол-во столбцов
        int Nstr;

        string[] month;
        double[,] data;

        public Form1()
        {

            InitializeComponent();
        }

        //загрузка формы
        private void Form1_Load(object sender, EventArgs e)
        {
            string file;
            bool[] check = { true, true, true, true, true, true };

            ReadFile("Входные данные.txt", out file);
            TransformFile(file, check, out month, out data);

            bool flag = false;
            string _Nstr = "";
            while (!flag)
            {
                _Nstr = Interaction.InputBox("В файле " + month.Length + " строк. Сколько возьмем для проверки?", "");

                _Nstr = _Nstr.Replace(" ", "");
                while (_Nstr.Contains(" ")) _Nstr = _Nstr.Replace(" ", "");  //удаляем пробелы

                if (Int32.TryParse(_Nstr, out Nstr) == false) { _Nstr = Interaction.InputBox("В файле " + month.Length + " строк. Сколько возьмем для проверки?", ""); }
                else
                {
                    if (Int32.Parse(_Nstr) > month.Length) { _Nstr = Interaction.InputBox("В файле " + month.Length + " строк. Сколько возьмем для проверки?", ""); }
                    if (Int32.Parse(_Nstr) < 0) { _Nstr = Interaction.InputBox("В файле " + month.Length + " строк. Сколько возьмем для проверки?", ""); }
                    if (Int32.TryParse(_Nstr, out Nstr) && Int32.Parse(_Nstr) < month.Length && Int32.Parse(_Nstr) > 0) { flag = true; }
                }

            }

            Nstr = Int32.Parse(_Nstr);
            
        }

        //закрытие формы
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            File.Delete("PromFile.txt");
        }

        //нажатие на ссылку на файл входных данных
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("Входные данные.txt");
        }

        //нажатие кнопки "Рассчитать"
        private void Сalculation_Click(object sender, EventArgs e)
        {
            string f;
            double[] k;
            double[] Ym;
            double[] X = new double[Q_Col - 1]; //значения факторов Х
            double result = new double { };
            bool[] Check = new bool[] { checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked };
            //очистим поля
            Znachimost.Text = "";
            Adekvatnost.Text = "";

                ReadFile("Входные данные.txt", out f);           //считали файл
                TransformFile(f, Check, out month, out data); //преобразовали его
            
            BaseFunc(data, out k, out Ym);       //функция построения регрессионной модели

            string[] TBox = new string[] { textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text };
            double x;

            for (int i = 0; i < TBox.Length; i++)
                if (Check[i])
                {
                    if (Double.TryParse(TBox[i], out x))
                        X[i] = Double.Parse(TBox[i]);     //считываем введенные данные
                    else
                        MessageBox.Show("Введите число", "Ошибка!");
                }

            //считаем результат
            for (int j = 0; j < (Q_Col-1); j++)
                result += k[j + 1] * X[j];
            result += k[0];

            Result.Text = result.ToString();        //выводим на экран
            //кнопка "сохранить в файл" активной
            SaveFile.Enabled = true;
           
        }

        //нажатие кнопки "Сохранить"
        private void SaveFile_Click(object sender, EventArgs e)
        {
            string file;
            string promF;
            ReadFile("Входные данные.txt", out file);
            ReadFile("PromFile.txt", out promF);

            string Str = "\n";
            Str += Interaction.InputBox("Введите месяц, для которого Вы хотите сохранить прогноз, \nнапример \n\nМарт2018", "Добавление строки в файл");

            Str = Str.Replace(" ", "");
            while (Str.Contains(" ")) Str = Str.Replace(" ", "");  //удаляем пробелы

            Str += "\t" + textBox1.Text + "\t" + textBox2.Text + "\t" + textBox3.Text + "\t" + textBox4.Text;

            File.AppendAllText("Отчет.txt", "Отчет сформирован " + DateTime.Now + "\r\n\r\nДанные, введенные пользователем:\r\n" + Str + "\r\n\r\nРасcчитанное прогнозное значение: " + Result.Text + "\r\n\r\nМодель построена на основе следующих данных: \r\n\r\n" + file + "\r\n" + promF, Encoding.GetEncoding(1251));
        }

        //нажатие кнопки "Перестроить модель"
        private void Rebuild_Click(object sender, EventArgs e)
        {
            Graph.Invalidate();
            Znachimost.Text = "";
            Adekvatnost.Text = "";
        }

        //обработка изменения состояния checkBox
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) textBox1.Enabled = true;
            else textBox1.Enabled = false;

            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked)
                MessageBox.Show("Выберите фактор", "Ошибка!");
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) textBox2.Enabled = true;
            else textBox2.Enabled = false;

            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked) 
                MessageBox.Show("Выберите фактор", "Ошибка!");
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked) textBox3.Enabled = true;
            else textBox3.Enabled = false;

            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked) 
                MessageBox.Show("Выберите фактор", "Ошибка!");
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked) textBox4.Enabled = true;
            else textBox4.Enabled = false;

            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked) 
                MessageBox.Show( "Выберите фактор", "Ошибка!");
        }

        //отрисовка графика
        private void Graph_Paint(object sender, PaintEventArgs E)
        {

            Pen blackpen = new Pen(Color.Black, 4);
            Pen bluepen = new Pen(Color.DodgerBlue, 4);
            Pen Greenpen = new Pen(Color.Green, 4);

            float W = Graph.Width, H = Graph.Height;  //факическая ширина и высота поля  
            float W1 = 20, H1 = H-15;   //ширина и высота, используемая под график (без полей)
            // оси координат
            E.Graphics.DrawLine(Pens.Black, W1, 0, W1, H);
            E.Graphics.DrawLine(Pens.Black, 0, H1, W, H1);

            bool[] check = new bool[] { checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked };
            string f;
            double[] k;
            double[] Ym;

            ReadFile("Входные данные.txt", out f);           //считали файл
            TransformFile(f, check, out month, out data); //преобразовали его

            BaseFunc(data, out k, out Ym);       //функция построения регрессионной модели

            double Ymax = 0;
            double Ymaxm = 0;

            // найдем максимальный У
            for (int i = 0; i < month.Length - Nstr; i++)
            {
                if (data[i, (data.GetLength(1) - 1)] > Ymax)
                    Ymax = data[i, (data.GetLength(1) - 1)];

                if (Ym[i] > Ymaxm)
                    Ymaxm = Ym[i];

            }
            if (Ymaxm > Ymax) Ymax = Ymaxm;

            //координаты первой точки в числах
            double X0 = 0;
            //координаты первой точки в пикселях
            double X0_Pix = W1;

            double Y0 = data[0, (data.GetLength(1) - 1)];
            double Y0_Pix = H1 - H1 * Y0 / Ymax;
            E.Graphics.DrawEllipse(Greenpen, (float)X0_Pix, (float)Y0_Pix, 2, 2);

            double Y0m = Ym[0];
            double Y0m_Pix = H1 - H1 * Y0m / Ymax;
            E.Graphics.DrawEllipse(Greenpen, (float)X0_Pix, (float)Y0m_Pix, 2, 2);

            //записываем числа по оси 0..+Y
            for (int i = 0; i <= 10; i++)
            {
                int P2 = (int)Math.Pow(10, (int)Math.Round(Ymax, 0).ToString().Length - 1); //10 в степени (5)
                int P = (int)Math.Round(Ymax / P2) * P2;    //округление в большую сторону до ближайшей степени 10

                E.Graphics.DrawString(("y*10^" + (Ymax.ToString().Length - 2)).ToString(), this.Font, Brushes.Blue, (float)W1, (float)0);
                E.Graphics.DrawString(((P / 10 * i) / (P2 / 10)).ToString(), this.Font, Brushes.Blue, (float)0, (float)H - (H1 / 10 * i));
                E.Graphics.DrawLine(Pens.Black, (float)W1-2, (float)H - (H1 / 10 * i), (float)W1+2, (float)H - (H1 / 10 * i));
            }

            //0 по оси Х
            E.Graphics.DrawString(X0.ToString(), this.Font, Brushes.Blue, (float)W1, (float)H1);
           
            // прорисовка через 2 точки
            for (int i = 1; i < month.Length - Nstr; i++)
            {
                //координаты второй точки в числах
                double X1 = i;
                //координаты второй точки в пикселях
                double X1_Pix = W1 + (W - W1) * (i) / (month.Length - Nstr);

                //ДЛЯ ОПЫТА
                double Y1 = data[i, data.GetLength(1) - 1];
                double Y1_Pix = H1 - H1 * Y1 / Ymax;

                ////ДЛЯ МОДЕЛИ
                double Y1m = Ym[i];
                double Y1m_Pix = H1 - H1 * Y1m / Ymax;

                //строим отрезок линии графика между точками (x0,y0    x1,y1)
                E.Graphics.DrawLine(blackpen, (float)X0_Pix, (float)Y0_Pix
                                            , (float)X1_Pix, (float)Y1_Pix);
                E.Graphics.DrawLine(bluepen, (float)X0_Pix, (float)Y0m_Pix
                                            , (float)X1_Pix, (float)Y1m_Pix);
                //жирная точка графика (x1_pix,y1_pix)
                E.Graphics.DrawEllipse(Greenpen, (float)X1_Pix, (float)Y1_Pix, 2, 2);
                E.Graphics.DrawEllipse(Greenpen, (float)X1_Pix, (float)Y1m_Pix, 2, 2);

                //записываем числа по оси 0..+X
                E.Graphics.DrawString(X1.ToString(), this.Font, Brushes.Blue, (float)X1_Pix, (float)H1);
                E.Graphics.DrawLine(Pens.Black, (float)X1_Pix, (float)H1+2, (float)X1_Pix, (float)H1-2);

                //сдвиг интервала на 1 шаг вправо
                X0_Pix = X1_Pix;
                Y0_Pix = Y1_Pix;
                Y0m_Pix = Y1m_Pix;

            }//for jx
        }

        //дополнительное поле
        private void _graph_Paint(object sender, PaintEventArgs e)
        {
            Pen blackpen = new Pen(Color.Black, 4);
            Pen bluepen = new Pen(Color.DodgerBlue, 4);
            float W = _graph.Width, H = _graph.Height;

            e.Graphics.DrawLine(blackpen, (float)5, (float)H / 3, (float)45, (float)H / 3);
            e.Graphics.DrawString(" - опыт", this.Font, Brushes.Black, (float)45, (float)H / 3 - 7);
            e.Graphics.DrawLine(bluepen, (float)5, (float)2 * H / 3, (float)45, (float)2 * H / 3);
            e.Graphics.DrawString(" - модель", this.Font, Brushes.Black, (float)45, (float)2 * H / 3 - 7);
        }

        //чтение файла
        public void ReadFile(string fname, out string file)
        {
            file = File.ReadAllText(fname, Encoding.Default);  //считываем файл построчно
        }

        //Преобразование файла
        public void TransformFile(string F, bool[] Check_Box, out string[] Month, out double[,] Data)
        {
            string F1;
            Edit(F, out F1);    //удаляем лишние пробелы и знаки табуляции

            string[] mass1 = F1.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);   //разделяем файл на символы, исключая пустые элементы
            double[] n = new double[(mass1.Length - mass1.Length / Q_Col_Orig)]; //сами данные
            Month = new string[mass1.Length / Q_Col_Orig];   //месяцы
            double N;
            int k = 0;
            int h = 0;
            for (int i = 0; i < (mass1.Length); i++)
            {
                if (Double.TryParse(mass1[i], out N))   //приводим к численому типу
                {
                    n[k] = Double.Parse(mass1[i]);
                    k++;
                }
                else         //месяцы оставляем как есть
                {
                    Month[h] = mass1[i];
                    h++;
                }
            }
            //разделим n в массивы по (Q_Col_Orig-1) (без месяцев)             
            double[,] Data_Beg = new double[(n.Length / (Q_Col_Orig - 1)), (Q_Col_Orig - 1)];
            int l = 0;
            for (int i = 0; i < (n.Length / (Q_Col_Orig - 1)); i++)
                for (int j = 0; j < (Q_Col_Orig - 1); j++)
                {
                    Data_Beg[i, j] = n[l];
                    l++;
                }
            //поиск исключенных эл-тов
            for (int i = 0; i < Month.Length; i++)
                for (int j = 0; j < (Q_Col_Orig - 2); j++)  //столбик с У не проверяем
                    if (!Check_Box[j]) Data_Beg[i, j] = 0.01;   //обнуляем исключенные эл-ты
            //узнаем кол-во удаленных столбцов     
            int DelitCol = 0;
            for (int i = 0; i < Check_Box.Length; i++)
                if (!Check_Box[i]) DelitCol += 1;
            Data = new double[Month.Length, Q_Col_Orig - 1 - DelitCol];
            //обновим матрицу Data_Beg  //всё сначала......
            //Data_Beg должна быть строкой
            k = 0;
            string Data_beg = "";   
            for (int i = 0; i < Month.Length; i++)
                for (int j = 0; j < (Q_Col_Orig - 1); j++)
                    Data_beg += Data_Beg[i, j] + " ";
            string Data_Medium;     //уже без исключенных элементов
            Edit(Data_beg.ToString(), out Data_Medium);
            string[] Data_End = Data_Medium.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);   //разделяем файл на символы, исключая пустые элементы
            l = 0;
            for (int i = 0; i < Month.Length; i++)
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    Data[i, j] = Double.Parse(Data_End[l]);
                    l++;
                }
            Q_Col = Data.GetLength(1);
        }

        //основная функция
        private void BaseFunc(double[,] Data, out double[] Koef, out double[] Ym)
        {


            int N = Data.Length / Q_Col;  //кол-во строк
            int M = Q_Col - 1;    //кол-во столбцов X
            double[,] x = new double[N - Nstr, M];    //независимые случайные величины
            double[] y = new double[N - Nstr];   //зависимая СВ
            int i = 0;
            int j = 0;

            //разделим основной массив на Х и У
            for (i = 0; i < N - Nstr; i++)
                for (j = 0; j < Q_Col; j++)
                    if (j != (Q_Col - 1)) x[i, j] = Data[i, j]; // TestBox.AppendText(x[i,j].ToString());}
                    else y[i] = Data[i, j];  //TestBox.AppendText(y[i].ToString());}

            //Оценка мат. ож. Х            
            double[] ChMx = new double[M];
            for (i = 0; i < M; i++) ChMx[i] = 0;   //начальные значение
            double[] Mx = new double[M]; ;
            for (j = 0; j < M; j++)
            {
                for (i = 0; i < N - Nstr; i++)
                    ChMx[j] += x[i, j];
                Mx[j] = ChMx[j] / N - Nstr;
            }

            //Оценка мат. ож. Y
            double ChMy = 0;
            double My;
            for (i = 0; i < N - Nstr; i++)
                ChMy += y[i];
            My = ChMy / N - Nstr;

            //Оценка дисперсии Х
            double[] ChDx = new double[M];
            double[] Dx = new double[M];
            for (j = 0; j < M; j++)
            {
                for (i = 0; i < N - Nstr; i++)
                    ChDx[j] += Math.Pow((x[i, j] - Mx[j]), 2);
                Dx[j] = ChDx[j] / (N - Nstr - 1);
            }

            //Оценка дисперсии Y
            double ChDy = 0;
            double Dy;
            for (i = 0; i < N - Nstr; i++)
                ChDy += Math.Pow((y[i] - My), 2);
            Dy = ChDy / (N - Nstr - 1);

            //Оценка коэф-тов корреляции между зависимыми и независимыми СВ
            double[] ChRxy = new double[M];
            double[] ZnRxy1 = new double[M];
            double[] ZnRxy2 = new double[M];
            double[] Rxy = new double[M];
            for (j = 0; j < M; j++)
            {
                for (i = 0; i < N - Nstr; i++)
                {
                    ChRxy[j] += (x[i, j] - Mx[j]) * (y[i] - My);
                    ZnRxy1[j] += Math.Pow((x[i, j] - Mx[j]), 2);
                    ZnRxy2[j] += Math.Pow((y[i] - My), 2);
                }
                Rxy[j] = ChRxy[j] / Math.Sqrt(ZnRxy1[j] * ZnRxy2[j]);
            }

            //Оценка коэф-тов корреляции между независимыми СВ
            double[] ChRxx = new double[M];
            double[] ZnRxx1 = new double[M];
            double[] ZnRxx2 = new double[M];
            double[,] Rxx = new double[M, M];
            for (j = 0; j < M; j++)
            {
                for (int k = 0; k < M; k++)
                {
                    for (i = 0; i < N - Nstr; i++)
                    {
                        ChRxx[j] += (x[i, j] - Mx[j]) * (x[i, k] - Mx[k]);
                        ZnRxx1[j] += Math.Pow((x[i, j] - Mx[j]), 2);
                        ZnRxx2[j] += Math.Pow((x[i, k] - Mx[k]), 2);
                    }
                    Rxx[j, k] = ChRxx[j] / Math.Sqrt(ZnRxx1[j] * ZnRxx2[j]);
                }
            }

            Koef = new double[M + 1];
            for (i = 0; i < M + 1; i++) Koef[i] = 0;

            //дополним матрицу Х единичным столбцом
            double[,] newX = new double[N - Nstr, M + 1];
            for (i = 0; i < N - Nstr; i++)
            {
                for (j = 0; j < M + 1; j++)
                {
                    if (j == 0) newX[i, j] = 1;
                    else newX[i, j] = x[i, j - 1];
                    //richTextBox1.AppendText(newX[i, j].ToString() + "\t");
                }
                //richTextBox1.AppendText("\n");
            }

            double[,] xT = new double[newX.GetLength(1), newX.GetLength(0)];
            Transpose(newX, out xT);
            double[,] xTx = new double[xT.GetLength(0), xT.GetLength(0)];
            Multiplication(xT, newX, out xTx);
            double[,] xTy = new double[xT.GetLength(0), xT.GetLength(0)];
            double[,] newY = new double[N - Nstr, 1];  //для корректного умножения
            for (i = 0; i < N - Nstr; i++) newY[i, 0] = y[i];
            Multiplication(xT, newY, out xTy);
            double[,] xTxConv = new double[xTx.GetLength(0), xTx.GetLength(1)];
            Converting(xTx, out xTxConv);
            double[,] K = new double[xTxConv.GetLength(0), xTxConv.GetLength(0)];
            Multiplication(xTxConv, xTy, out K);

            Ym = new double[N - Nstr];
            for (i = 0; i < M + 1; i++) Koef[i] = K[i, 0];

            for (i = 0; i < N - Nstr; i++)
            {
                for (j = 1; j < M; j++)
                    Ym[i] += Koef[j] * x[i, j - 1];
                Ym[i] += Koef[0];
                if (Ym[i] < 0) Ym[i] = 0;   //если Ym<0, обнуляем
            }
            
            
            //оценка остаточной дисперсии
            double ChDe = 0;
            double De;
            for (i = 0; i < N - Nstr; i++)
                ChDe += Math.Pow((Ym[i] - y[i]), 2);
            De = ChDe / (N - Nstr - M + 1 - 1);

            //Оценка коэффициента детерминации(коэффициент множественной корреляции)
            double R2 = Math.Abs(1 - De / Dy);

            //оценки дисперсий коэффициентов уравнения регрессии
            double[] Da = new double[M];
            double[,] Matrix = new double[N - Nstr, 1];
            double[,] MatrixT, MatrixT_Matrix, MatrixT_Matrix_Conv;

            for (j = 0; j < M; j++)
            {
                for (i = 0; i < N - Nstr; i++)
                {
                    Matrix[i, 0] = x[i, j];
                }
                Transpose(Matrix, out MatrixT);
                Multiplication(MatrixT, Matrix, out MatrixT_Matrix);
                Converting(MatrixT_Matrix, out MatrixT_Matrix_Conv);
                Da[j] = De * MatrixT_Matrix_Conv[0, 0];
            }

            //расчет статистики Z для проверки коэффициентов на  значимость
            double[] Z = new double[M];
            for (j = 0; j < M; j++)
                Z[j] = Koef[j] / Math.Sqrt(Da[j]);

            int q = N - Nstr - M - 1;  //количество степеней свобод
            double Tkr;
            Chart Chart1 = new Chart();
            Tkr = Chart1.DataManipulator.Statistics.InverseTDistribution(.05, q);   //критическое значение критерия Стьюдента, соответствующее вероятности 0,05
            double[] P = new double[M];
            i = 0;

            //вычислим достигнутые уровни значимости и выведем на экран
            foreach (double el in Z)
            {
                P[i] = Chart1.DataManipulator.Statistics.TDistribution(el, q, false);

                if (Math.Abs(el) > Tkr) Znachimost.AppendText(" - значимый. Р = " + Math.Round(P[i], 6) + "\n");
                else Znachimost.AppendText(" - незначимый. Р = " + Math.Round(P[i], 6) + "\n");
                i++;
            }

            //Оценка прогнозирующей способности
            double[,] x_new = new double[Nstr, M];
            double[] y_new = new double[Nstr];
            double[] Ym_new = new double[Nstr];
            int n_new = Nstr;

            for (i = N - n_new; i < N; i++)
                for (j = 0; j < Q_Col; j++)
                    if (j != (Q_Col - 1)) x_new[i - (N - n_new), j] = Data[i, j];
                    else y_new[i - (N - n_new)] = Data[i, j];

            for (i = 0; i < n_new; i++)
            {
                for (j = 1; j < M; j++)
                    Ym_new[i] += Koef[j] * x_new[i, j - 1];
                Ym_new[i] += Koef[0];
            }

            //Оценка дисперсии адекватности
            double Dad = 0;
            double ChDad = 0;
            for (i = 0; i < n_new; i++)
                ChDad += Math.Pow((Ym_new[i] - y_new[i]), 2);
            Dad = ChDad / n_new;

            Adekvatnost.AppendText("R2 = " + Math.Round(R2, 4) + "\nDe = " + Math.Round(De, 4) + "\nDad = " + Math.Round(Dad, 4) + "\n");
            double F = Dad / De;
            double Fkr = Chart1.DataManipulator.Statistics.InverseFDistribution(.05, n_new, N - n_new - M - 1);
            if (F < Fkr) Adekvatnost.AppendText("Модель адекватна\n"); else Adekvatnost.AppendText("Модель неадекватна\n");

            double p = 0;

            p = Chart1.DataManipulator.Statistics.FDistribution(F, n_new, q);
            Adekvatnost.AppendText("P = " + Math.Round(p, 4));

            //заполнение промежуточного файла данными о модели
            //полученное уравнение регрессии
            string str = "y = ";
            for (i = 0; i <= M; i++)
            {
                if (Koef[i] != null)
                {
                    if (i == 0) str += +Koef[i];
                    else str += " + " + Koef[i] + " * x" + i;
                }
            }

            File.WriteAllText("PromFile.txt", "\r\nПолученная модель: \r\n\r\n" + str + "\r\n\r\nФакторы:\r\n\r\nВключенные в модель:", Encoding.GetEncoding(1251));
            bool[] check = new bool[] { checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked };
            int k1 = 0;
            //включенные факторы
            for (i = 0; i < 4; i++)
            {
                if (check[i])
                {
                    File.AppendAllText("PromFile.txt", "\r\nx" + (i + 1), Encoding.GetEncoding(1251));

                    switch (i)
                    {
                        case 0: File.AppendAllText("PromFile.txt", " - средняя температура.\t", Encoding.GetEncoding(1251)); break;
                        case 1: File.AppendAllText("PromFile.txt", " - максимальная температура.\t", Encoding.GetEncoding(1251)); break;
                        case 2: File.AppendAllText("PromFile.txt", " - минимальная температура.\t", Encoding.GetEncoding(1251)); break;
                        case 3: File.AppendAllText("PromFile.txt", " - тепловая нагрузка.\t", Encoding.GetEncoding(1251)); break;
                    }


                    if (Math.Abs(Z[k1]) > Tkr) File.AppendAllText("PromFile.txt", " Значимый.\tДостигнутый уровень значимости Р = " + Math.Round(P[k1], 6), Encoding.GetEncoding(1251));
                    else File.AppendAllText("PromFile.txt", " Незначимый.\tДостигнутый уровень значимости Р = " + Math.Round(P[k1], 6), Encoding.GetEncoding(1251));
                    k1++;
                }
            }
            //исключенные факторы
            File.AppendAllText("PromFile.txt", "\r\n\r\nИсключенные из модели: \r\n", Encoding.GetEncoding(1251));
            for (i = 0; i < 4; i++)
            {
                if (!check[i])
                {
                    File.AppendAllText("PromFile.txt", "x" + (i + 1), Encoding.GetEncoding(1251));

                    switch (i)
                    {
                        case 0: File.AppendAllText("PromFile.txt", " - средняя температура.\t", Encoding.GetEncoding(1251)); break;
                        case 1: File.AppendAllText("PromFile.txt", " - максимальная температура.\t", Encoding.GetEncoding(1251)); break;
                        case 2: File.AppendAllText("PromFile.txt", " - минимальная температура.\t", Encoding.GetEncoding(1251)); break;
                        case 3: File.AppendAllText("PromFile.txt", " - тепловая нагрузка.\t", Encoding.GetEncoding(1251)); break;
                    }
                }
            }
            //характеристики модели
            File.AppendAllText("PromFile.txt", "\r\n\r\nХарактеристики модели: \r\n", Encoding.GetEncoding(1251));

            File.AppendAllText("PromFile.txt", "\r\nОценка коэффициентов корреляции между независимыми и зависимыми СВ: \r\nRxy[j] = ", Encoding.GetEncoding(1251));
            for (i = 0; i < M; i++) File.AppendAllText("PromFile.txt", Rxy[i].ToString() + "\r\n");

            File.AppendAllText("PromFile.txt", "\r\nОценка коэффициентов корреляции между независимыми СВ: \r\nRxx[i,j] = ", Encoding.GetEncoding(1251));
            for (i = 0; i < M; i++)
            {
                for (j = 0; j < M; j++)
                    File.AppendAllText("PromFile.txt", Rxx[i, j].ToString() + "\t ");
                File.AppendAllText("PromFile.txt", "\r\n");
            }

            File.AppendAllText("PromFile.txt", "\r\nОценка остаточной дисперсии: \r\nDe = ", Encoding.GetEncoding(1251));
            File.AppendAllText("PromFile.txt", De.ToString() + "\r\n");

            File.AppendAllText("PromFile.txt", "\r\nОценка коэффициента детерминации: \r\nR2 = ", Encoding.GetEncoding(1251));
            File.AppendAllText("PromFile.txt", R2.ToString() + "\r\n");

            File.AppendAllText("PromFile.txt", "\r\nОценка дисперсии коэффициентов уравнения регрессии: \r\nDa[j] = ", Encoding.GetEncoding(1251));
            for (i = 0; i < M; i++) File.AppendAllText("PromFile.txt", Da[i].ToString() + "\r\n");

            File.AppendAllText("PromFile.txt", "\r\nZ - статистика: \r\nZ[j] = ", Encoding.GetEncoding(1251));
            for (i = 0; i < M; i++) File.AppendAllText("PromFile.txt", Z[i].ToString() + "\r\n");

            File.AppendAllText("PromFile.txt", "\r\nКритическое значение критерия Стьюдента, соответствующее вероятности 0,05: \r\nTkr = ", Encoding.GetEncoding(1251));
            File.AppendAllText("PromFile.txt", Tkr.ToString() + "\r\n");

            File.AppendAllText("PromFile.txt", "\r\n\r\nМодель была построена на " + (N - n_new) + " данных. И проверена на " + n_new + " данных.\r\n\r\nОценка дисперсии адекватности: \r\nDad = ", Encoding.GetEncoding(1251));
            File.AppendAllText("PromFile.txt", Dad.ToString() + "\r\n");

            File.AppendAllText("PromFile.txt", "\r\nКритерий Фишера: \r\nF = ", Encoding.GetEncoding(1251));
            File.AppendAllText("PromFile.txt", F.ToString() + "\r\n");

            File.AppendAllText("PromFile.txt", "\r\nКритическое значение критерия Фишера: \r\nFkr = ", Encoding.GetEncoding(1251));
            File.AppendAllText("PromFile.txt", Fkr.ToString() + "\r\n");

            if (F < Fkr) File.AppendAllText("PromFile.txt", "\r\nМодель адекватна\r\n", Encoding.GetEncoding(1251)); else File.AppendAllText("PromFile.txt", "\r\nМодель неадекватна\r\n", Encoding.GetEncoding(1251));
            File.AppendAllText("PromFile.txt", "\r\nДостигнутый уровень значимости: \r\nP = ", Encoding.GetEncoding(1251));
            File.AppendAllText("PromFile.txt", p.ToString() + "\r\n");

            File.AppendAllText("PromFile.txt", "________________________________________________________________________________________\r\n\r\n");
        }

        //Удаление пробелов и знаков табуляции
        private void Edit(string f1, out string f2)
        {
            f2 = f1.Replace("0,01", "");
            while (f2.Contains("0,01")) f2 = f2.Replace("0,01", "");  //удаляем исключенные эл-ты

            f2 = f2.Replace("  ", " ");
            while (f2.Contains("  ")) f2 = f2.Replace("  ", " ");  //удаляем лишние пробелы

            f2 = f2.Replace("\t", " ");
            while (f2.Contains("\t")) f2 = f2.Replace("\t", " ");  //удаляем знаки табуляции

            f2 = f2.Replace("\r", "");
            while (f2.Contains("\r")) f2 = f2.Replace("\r", "");  //удаляем дичь
        }

        //функция транспонирования матрицы 
        private void Transpose(double[,] m, out double[,] mT)
        {

            mT = new double[m.GetLength(1), m.GetLength(0)];
            for (int i = 0; i < m.GetLength(1); i++)
                for (int j = 0; j < m.GetLength(0); j++)
                    mT[i, j] = m[j, i];
        }

        //функция умножения матриц 
        private void Multiplication(double[,] A, double[,] B, out double[,] C)
        {
            int m = A.GetLength(0); //колво строк А
            int n = A.GetLength(1); //колво столбцов А
            int h = B.GetLength(1); //колво столбцов B
            C = new double[m, h];

            for (int i = 0; i < m; i++)
                for (int j = 0; j < h; j++)
                    for (int k = 0; k < n; k++)
                        C[i, j] += A[i, k] * B[k, j];
        }

        //функция обращения матрицы
        private void Converting(double[,] m, out double[,] mConvert)
        {
            int n = m.GetLength(0);
            mConvert = new double[n, n];
            //______________________________________________________
            //обратная матрица СмОбратная   
            double[,] mCopy = new double[n, n];
            //задаем обратную матрицу как единичную           
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j) { mConvert[i, j] = 1; }
                    else { mConvert[i, j] = 0; }
                    mCopy[i, j] = m[i, j];    //создаем копию матрицы См
                }
            }
            //прямой ход
            for (int k = 0; k < n; k++)
            {
                double div = mCopy[k, k];
                for (int l = 0; l < n; ++l)
                {// делим строку на выбранный элемент === 1  ф  ф
                    mCopy[k, l] /= div;
                    mConvert[k, l] /= div;
                }
                for (int i = k + 1; i < n; ++i) //идем по столбeц ниже полученой единицы
                {
                    double multi = mCopy[i, k]; //элемент, который хотим занулить
                    for (int j = 0; j < n; ++j)// элемент по счету в строке i
                    {
                        mCopy[i, j] -= multi * mCopy[k, j];
                        mConvert[i, j] -= multi * mConvert[k, j];
                    }
                }
            }
            //обратный ход            
            for (int kk = n - 1; kk > 0; kk--)
            {
                mCopy[kk, n - 1] /= mCopy[kk, kk];
                mConvert[kk, n - 1] /= mCopy[kk, kk];

                for (int i = kk - 1; i + 1 > 0; i--)
                {
                    double multi2 = mCopy[i, kk];
                    for (int j = 0; j < n; j++)
                    {
                        mCopy[i, j] -= multi2 * mCopy[kk, j];
                        mConvert[i, j] -= multi2 * mConvert[kk, j];
                    }
                }
            }
            //_____________________________________________________________________
        }


    }
}
