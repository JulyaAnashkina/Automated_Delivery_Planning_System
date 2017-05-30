using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automated_Delivery_Planning_System
{
    public partial class Form1 : Form
    {
        public static CVRP.Client[][] mass_p_rout = null;
        public Form1()
        {
            InitializeComponent();
            int n = 12;
            int k = 6;
            CVRP.Client[] v = new CVRP.Client[n + 1];
            CVRP.Avto[] avto = new CVRP.Avto[k];

            CVRP.Reading_File(v, avto);

            double[][] c = new double[n + 1][]; //матрица расстояний между пунктами
            for (int i = 0; i < n + 1; i++)
                c[i] = new double[n + 1];

            CVRP.Get_Of_Distance_Matrix(c);

            double[][] s = new double[n + 1][]; //матрица километровых выйгрышей
            for (int h1 = 0; h1 < n + 1; h1++)
                s[h1] = new double[n + 1];

            CVRP.Get_Of_Kilometer_Wage_Matrix(s, c);

            CVRP.Client[][] ready_made_routes = new CVRP.Client[0][];
            CVRP.Client[] v_rest = new CVRP.Client[n + 1];
            v_rest = v;

            int[] used_capacity = new int[avto.Length];
            for (int i = 0; i < used_capacity.Length; i++)
                used_capacity[i] = 0;

            avto = CVRP.BubbleSort_Avto(avto);

            /**************************************************************/
            /*          ПОВТОР 1 ПРИ ИЗМЕнЕнИИ Грузоподъёмности           */
            /**************************************************************/
            Search_for_routes_for_the_remaining_vertices: //Поиск маршрутов для оставшихся вершин

            CVRP.Client[][] routes = new CVRP.Client[v_rest.Length - 1][];
            for (int i = 0; i < v_rest.Length - 1; i++)
                routes[i] = new CVRP.Client[] { v_rest[0], v_rest[i + 1], v_rest[0] };

            int[][] s_max_matr = new int[n + 1][]; //дополнительная матрица, какие ячейки уже рассматривались = 1
            for (int h1 = 0; h1 < n + 1; h1++)
                s_max_matr[h1] = new int[n + 1];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (i == j)
                        s_max_matr[i][j] = 1;
                    else
                        s_max_matr[i][j] = 0;

            int q = 0, q_count_cars = 0;
            for (int i = 0; i < avto.Length; i++)
                if ((avto[i].b >= q) & (used_capacity[i] != 1))
                {
                    q = avto[i].b;
                    q_count_cars++;
                }

            for (int i = 0; i < avto.Length; i++)
                if (avto[i].b == q)
                    used_capacity[i] = 1;
            int f = 1;

            /****************************************************************************/
            /*          ПОВТОР 2 для поиска двух новых вершин для объединения           */
            /****************************************************************************/
            Search_for_two_routes_to_merge: //Поиск двух маршрутов для объединения

            CVRP.Client[][] built_routes = new CVRP.Client[n][]; //готовые маршруты            
            for (int i = 0; i < routes.Length; i++)
            {
                built_routes[i] = new CVRP.Client[routes.Length];
                built_routes[i] = routes[i];
            }

            int[] coordinates = CVRP.Search_For_The_Maximum_Kilometer_Winnings(s_max_matr, s, routes, v_rest); //поиск двух маршрутов для объединения

            bool none_found_s_max = false;
            if ((coordinates[0] == 0) & (coordinates[1] == 0) & (coordinates[2] == 0) & (coordinates[3] == 0))
            {
                none_found_s_max = true;
                goto None_found_s_max;
            }

            built_routes = CVRP.Join_Routes(routes, coordinates, q, built_routes); //объединение маршрутов

            routes = new CVRP.Client[built_routes.Length][]; //готовые маршруты            
            for (int i = 0; i < built_routes.Length; i++)
            {
                routes[i] = new CVRP.Client[built_routes.Length];
                routes[i] = built_routes[i];
            }

            None_found_s_max:
            if (none_found_s_max)
            {
                CVRP.BubbleSort_Routes(routes);

                CVRP.Client[][] selected_routes = new CVRP.Client[ready_made_routes.Length + q_count_cars][];
                selected_routes = CVRP.Add_Finished_Routes(routes, ready_made_routes, q_count_cars);

                ready_made_routes = new CVRP.Client[selected_routes.Length][];
                for (int i = 0; i < (ready_made_routes.Length); i++)
                {
                    ready_made_routes[i] = new CVRP.Client[selected_routes.Length];
                    ready_made_routes[i] = selected_routes[i];
                }
                int a = 0;
                for (int i = 0; i < q_count_cars; i++)
                    a = routes[i].Length - 2;

                CVRP.Client[] v_cop2 = new CVRP.Client[v_rest.Length - a];
                v_cop2 = CVRP.Getting_Unused_Vertices(v_rest, routes, q_count_cars);

                v_rest = new CVRP.Client[v_cop2.Length];
                v_rest = v_cop2;

                if (v_rest.Length != 1)
                    goto Search_for_routes_for_the_remaining_vertices;
            }
            else
            {
                f++;
                goto Search_for_two_routes_to_merge; //поиск двух маршрутов для объединения
            }

            //оптимизация внутри каждого маршрута
            for (int i = 0; i < ready_made_routes.Length; i++)
            {
                CVRP.Client[] m_rout = new CVRP.Client[ready_made_routes[i].Length - 2];

                for (int i1 = 1; i1 < ready_made_routes[i].Length - 1; i1++)
                    m_rout[i1 - 1] = ready_made_routes[i][i1];

                int m = ready_made_routes[i].Length - 2;

                int m_factorial = 1;
                for (int f1 = 1; f1 <= m; f1++)
                    m_factorial = m_factorial * f1;

                int count = 0;
                CVRP.Client[] temp = new CVRP.Client[m_rout.Length];
                mass_p_rout = new CVRP.Client[m_factorial][];

                mass_p_rout = CVRP.Permutation_Generation(m_rout, count, mass_p_rout, temp, m);
                ready_made_routes = CVRP.Route_Optimization(ready_made_routes, mass_p_rout, c, i);
            }
            int p1 = 1;
            textBox1.Text += "Все маршруты построены";
            for (int i = 0; i < ready_made_routes.Length; i++)
            {
                textBox1.Text += Environment.NewLine + " ---------------МАРШРУТ " + p1.ToString() + "---------------";
                p1++;
                int all_q = 0;
                foreach (CVRP.Client l in ready_made_routes[i])
                {
                    all_q += l.q;
                    l.Show_Client();
                }
                textBox1.Text += Environment.NewLine + "Общий объём груза = " + all_q.ToString();
            }
            Console.ReadLine();
            /*Console.WriteLine("Все маршруты построены");
            for (int i = 0; i < ready_made_routes.Length; i++)
            {
                Console.WriteLine("\n ---------------МАРШРУТ {0}---------------", p1);
                p1++;
                int all_q = 0;
                foreach (CVRP.Client l in ready_made_routes[i])
                {
                    all_q += l.q;
                    l.Show_Client();
                }
                Console.WriteLine("Общий объём груза = {0}", all_q);
            }
            Console.ReadLine();*/
        }
    }
}
